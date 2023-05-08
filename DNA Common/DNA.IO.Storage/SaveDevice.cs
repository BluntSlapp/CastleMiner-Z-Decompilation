using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DNA.IO.Compression;
using DNA.Security;
using DNA.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace DNA.IO.Storage
{
	public abstract class SaveDevice : IGameComponent, IUpdateable, IAsyncSaveDevice, ISaveDevice, IDisposable
	{
		private class FileOperationState
		{
			public string File;

			public string Pattern;

			public bool TamperProof;

			public bool Compressed;

			public FileAction Action;

			public object UserState;

			public void Reset()
			{
				TamperProof = false;
				Compressed = false;
				File = null;
				Pattern = null;
				Action = null;
				UserState = null;
			}
		}

		[Flags]
		private enum FileOptions : uint
		{
			None = 0u,
			Compressesd = 1u,
			Encrypted = 2u
		}

		private const int FileIdent = 1146311762;

		private const int FileVersion = 5;

		private static byte[] CommonKeyOld;

		private static byte[] CommonKey2;

		private static byte[] LocalKeyOld;

		private static byte[] LocalKey2;

		private static string promptForCancelledMessage;

		private static string forceCancelledReselectionMessage;

		private static string promptForDisconnectedMessage;

		private static string forceDisconnectedReselectionMessage;

		private static string deviceRequiredTitle;

		private static string deviceOptionalTitle;

		private static readonly string[] deviceOptionalOptions;

		private static readonly string[] deviceRequiredOptions;

		public bool PromptForReselect;

		private string _containerName;

		private int updateOrder;

		private bool enabled = true;

		private bool deviceWasConnected;

		private SaveDevicePromptState state;

		private readonly SaveDevicePromptEventArgs promptEventArgs = new SaveDevicePromptEventArgs();

		private readonly SaveDeviceEventArgs eventArgs = new SaveDeviceEventArgs();

		private StorageDevice storageDevice;

		private SuccessCallback _promptCallback;

		public static readonly int[] ProcessorAffinity;

		private Queue<FileOperationState> pendingStates = new Queue<FileOperationState>(100);

		private readonly object pendingOperationCountLock = new object();

		private int pendingOperations;

		private StorageContainer _currentContainer;

		private bool disposed;

		public static string PromptForCancelledMessage
		{
			get
			{
				return promptForCancelledMessage;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					promptForCancelledMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string ForceCancelledReselectionMessage
		{
			get
			{
				return forceCancelledReselectionMessage;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					forceCancelledReselectionMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string PromptForDisconnectedMessage
		{
			get
			{
				return promptForDisconnectedMessage;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					promptForDisconnectedMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string ForceDisconnectedReselectionMessage
		{
			get
			{
				return forceDisconnectedReselectionMessage;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					forceDisconnectedReselectionMessage = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string DeviceRequiredTitle
		{
			get
			{
				return deviceRequiredTitle;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					deviceRequiredTitle = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string DeviceOptionalTitle
		{
			get
			{
				return deviceOptionalTitle;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					deviceOptionalTitle = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string OkOption
		{
			get
			{
				return deviceRequiredOptions[0];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					deviceRequiredOptions[0] = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string YesOption
		{
			get
			{
				return deviceOptionalOptions[0];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					deviceOptionalOptions[0] = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public static string NoOption
		{
			get
			{
				return deviceOptionalOptions[1];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					deviceOptionalOptions[1] = ((value.Length < 256) ? value : value.Substring(0, 256));
				}
			}
		}

		public bool IsReady
		{
			get
			{
				if (storageDevice != null)
				{
					return storageDevice.IsConnected;
				}
				return false;
			}
		}

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				if (enabled != value)
				{
					enabled = value;
					if (this.EnabledChanged != null)
					{
						this.EnabledChanged(this, null);
					}
				}
			}
		}

		public int UpdateOrder
		{
			get
			{
				return updateOrder;
			}
			set
			{
				if (updateOrder != value)
				{
					updateOrder = value;
					if (this.UpdateOrderChanged != null)
					{
						this.UpdateOrderChanged(this, null);
					}
				}
			}
		}

		public bool IsBusy
		{
			get
			{
				lock (pendingOperationCountLock)
				{
					return pendingOperations > 0;
				}
			}
		}

		public event EventHandler<SaveDeviceEventArgs> DeviceDisconnected;

		public event EventHandler<EventArgs> EnabledChanged;

		public event EventHandler<EventArgs> UpdateOrderChanged;

		public event SaveCompletedEventHandler SaveCompleted;

		public event LoadCompletedEventHandler LoadCompleted;

		public event DeleteDirectoryCompletedEventHandler DeleteDirectoryCompleted;

		public event DeleteCompletedEventHandler DeleteCompleted;

		public event FileExistsCompletedEventHandler FileExistsCompleted;

		public event GetFilesCompletedEventHandler GetFilesCompleted;

		static SaveDevice()
		{
			CommonKeyOld = new byte[32]
			{
				236, 34, 252, 119, 2, 225, 246, 242, 214, 172,
				157, 191, 175, 246, 57, 246, 219, 180, 178, 196,
				212, 135, 153, 18, 146, 132, 30, 41, 238, 149,
				142, 228
			};
			CommonKey2 = new byte[16]
			{
				236, 34, 252, 119, 2, 225, 246, 242, 214, 172,
				157, 191, 175, 246, 57, 246
			};
			deviceOptionalOptions = new string[2];
			deviceRequiredOptions = new string[1];
			ProcessorAffinity = new int[1] { 5 };
			StorageSettings.ResetSaveDeviceStrings();
		}

		protected SaveDevice(string containerName, byte[] keyOld, byte[] key)
		{
			LocalKeyOld = keyOld;
			LocalKey2 = key;
			_containerName = containerName;
		}

		public virtual void Initialize()
		{
		}

		public void PromptForDevice(SuccessCallback callBack)
		{
			_promptCallback = callBack;
			if (state == SaveDevicePromptState.None)
			{
				state = SaveDevicePromptState.ShowSelector;
			}
		}

		public static void EnsureCreated(StorageContainer container, string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directoryName))
			{
				EnsureCreated(container, directoryName);
			}
			if (!container.DirectoryExists(path))
			{
				container.CreateDirectory(path);
			}
		}

		public void UnPack(string fileName, string dest)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				byte[] buffer;
				using (Stream stream = _currentContainer.OpenFile(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					int count = (int)stream.Length;
					BinaryReader binaryReader = new BinaryReader(stream);
					buffer = binaryReader.ReadBytes(count);
				}
				BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(buffer));
				int num = binaryReader2.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string text = binaryReader2.ReadString();
					string text2 = text.ToLower();
					text = text.Substring(text2.LastIndexOf("worlds\\"));
					int count2 = binaryReader2.ReadInt32();
					byte[] array = binaryReader2.ReadBytes(count2);
					EnsureCreated(_currentContainer, Path.GetDirectoryName(text));
					using (Stream stream2 = _currentContainer.OpenFile(text, FileMode.Create, FileAccess.Write, FileShare.None))
					{
						stream2.Write(array, 0, array.Length);
					}
				}
			}
			Flush();
		}

		protected abstract void GetStorageDevice(AsyncCallback callback, SuccessCallback resultCallback);

		protected virtual void PrepareEventArgs(SaveDeviceEventArgs args)
		{
			args.Response = SaveDeviceEventResponse.Prompt;
			args.PlayerToPrompt = null;
		}

		public void Update(GameTime gameTime)
		{
			if (!GamerServicesDispatcher.IsInitialized)
			{
				throw new InvalidOperationException(Strings.NeedGamerService);
			}
			bool flag = true;
			if (PromptForReselect || !deviceWasConnected)
			{
				flag = storageDevice != null && storageDevice.IsConnected;
			}
			if (!flag && deviceWasConnected)
			{
				PrepareEventArgs(eventArgs);
				if (this.DeviceDisconnected != null)
				{
					this.DeviceDisconnected(this, eventArgs);
				}
				if (PromptForReselect)
				{
					HandleEventArgResults();
				}
				else
				{
					state = SaveDevicePromptState.None;
				}
			}
			else if (!flag)
			{
				try
				{
					if (!Guide.IsVisible)
					{
						switch (state)
						{
						case SaveDevicePromptState.ShowSelector:
							state = SaveDevicePromptState.None;
							GetStorageDevice(StorageDeviceSelectorCallback, _promptCallback);
							break;
						case SaveDevicePromptState.PromptForCanceled:
							ShowMessageBox(eventArgs.PlayerToPrompt, deviceOptionalTitle, promptForCancelledMessage, deviceOptionalOptions, ReselectPromptCallback, _promptCallback);
							break;
						case SaveDevicePromptState.ForceCanceledReselection:
							ShowMessageBox(eventArgs.PlayerToPrompt, deviceRequiredTitle, forceCancelledReselectionMessage, deviceRequiredOptions, ForcePromptCallback, null);
							break;
						case SaveDevicePromptState.PromptForDisconnected:
							ShowMessageBox(eventArgs.PlayerToPrompt, deviceOptionalTitle, promptForDisconnectedMessage, deviceOptionalOptions, ReselectPromptCallback, null);
							break;
						case SaveDevicePromptState.ForceDisconnectedReselection:
							ShowMessageBox(eventArgs.PlayerToPrompt, deviceRequiredTitle, forceDisconnectedReselectionMessage, deviceRequiredOptions, ForcePromptCallback, null);
							break;
						}
					}
				}
				catch (GuideAlreadyVisibleException)
				{
				}
			}
			deviceWasConnected = flag;
		}

		private void StorageDeviceSelectorCallback(IAsyncResult result)
		{
			SuccessCallback successCallback = (SuccessCallback)result.AsyncState;
			storageDevice = StorageDevice.EndShowSelector(result);
			if (storageDevice != null && storageDevice.IsConnected)
			{
				try
				{
					_currentContainer = OpenContainer(_containerName);
					if (successCallback != null)
					{
						successCallback(true);
					}
					return;
				}
				catch
				{
					if (successCallback != null)
					{
						successCallback(false);
					}
					return;
				}
			}
			PrepareEventArgs(eventArgs);
			HandleEventArgResults();
		}

		private void ForcePromptCallback(IAsyncResult result)
		{
			Guide.EndShowMessageBox(result);
			state = SaveDevicePromptState.ShowSelector;
		}

		private void ReselectPromptCallback(IAsyncResult result)
		{
			int? num = Guide.EndShowMessageBox(result);
			state = ((num.HasValue && num.Value == 0) ? SaveDevicePromptState.ShowSelector : SaveDevicePromptState.None);
			promptEventArgs.ShowDeviceSelector = state == SaveDevicePromptState.ShowSelector;
			SuccessCallback successCallback = (SuccessCallback)result.AsyncState;
			if (state == SaveDevicePromptState.None && successCallback != null)
			{
				successCallback(false);
			}
		}

		private void HandleEventArgResults()
		{
			storageDevice = null;
			switch (eventArgs.Response)
			{
			case SaveDeviceEventResponse.Prompt:
				state = (deviceWasConnected ? SaveDevicePromptState.PromptForDisconnected : SaveDevicePromptState.PromptForCanceled);
				break;
			case SaveDeviceEventResponse.Force:
				state = (deviceWasConnected ? SaveDevicePromptState.ForceDisconnectedReselection : SaveDevicePromptState.ForceCanceledReselection);
				break;
			default:
				state = SaveDevicePromptState.None;
				break;
			}
		}

		private static void ShowMessageBox(PlayerIndex? player, string title, string text, IEnumerable<string> buttons, AsyncCallback callback, object state)
		{
			if (player.HasValue)
			{
				Guide.BeginShowMessageBox(player.Value, title, text, buttons, 0, MessageBoxIcon.None, callback, state);
			}
			else
			{
				Guide.BeginShowMessageBox(title, text, buttons, 0, MessageBoxIcon.None, callback, state);
			}
		}

		public void SaveAsync(string fileName, bool tamperProof, bool compressed, FileAction saveAction)
		{
			SaveAsync(fileName, tamperProof, compressed, saveAction, null);
		}

		public void SaveAsync(string fileName, bool tamperProof, bool compressed, FileAction saveAction, object userState)
		{
			PendingOperationsIncrement();
			FileOperationState fileOperationState = GetFileOperationState();
			fileOperationState.File = fileName;
			fileOperationState.TamperProof = tamperProof;
			fileOperationState.Compressed = compressed;
			fileOperationState.Action = saveAction;
			fileOperationState.UserState = userState;
			ThreadPool.QueueUserWorkItem(DoSaveAsync, fileOperationState);
		}

		public void LoadAsync(string fileName, FileAction loadAction)
		{
			LoadAsync(fileName, loadAction, null);
		}

		public void LoadAsync(string fileName, FileAction loadAction, object userState)
		{
			PendingOperationsIncrement();
			FileOperationState fileOperationState = GetFileOperationState();
			fileOperationState.File = fileName;
			fileOperationState.Action = loadAction;
			fileOperationState.UserState = userState;
			ThreadPool.QueueUserWorkItem(DoLoadAsync, fileOperationState);
		}

		public void DeleteAsync(string fileName)
		{
			DeleteAsync(fileName, null);
		}

		public void DeleteAsync(string fileName, object userState)
		{
			PendingOperationsIncrement();
			FileOperationState fileOperationState = GetFileOperationState();
			fileOperationState.File = fileName;
			fileOperationState.UserState = userState;
			ThreadPool.QueueUserWorkItem(DoDeleteAsync, fileOperationState);
		}

		public void FileExistsAsync(string fileName)
		{
			FileExistsAsync(fileName, null);
		}

		public void FileExistsAsync(string fileName, object userState)
		{
			PendingOperationsIncrement();
			FileOperationState fileOperationState = GetFileOperationState();
			fileOperationState.File = fileName;
			fileOperationState.UserState = userState;
			ThreadPool.QueueUserWorkItem(DoFileExistsAsync, fileOperationState);
		}

		public void GetFilesAsync()
		{
			GetFilesAsync(null);
		}

		public void GetFilesAsync(object userState)
		{
			GetFilesAsync("*", userState);
		}

		public void GetFilesAsync(string pattern)
		{
			GetFilesAsync(pattern, null);
		}

		public void GetFilesAsync(string pattern, object userState)
		{
			PendingOperationsIncrement();
			FileOperationState fileOperationState = GetFileOperationState();
			fileOperationState.Pattern = pattern;
			fileOperationState.UserState = userState;
			ThreadPool.QueueUserWorkItem(DoGetFilesAsync, fileOperationState);
		}

		private void SetProcessorAffinity()
		{
			Thread.CurrentThread.SetProcessorAffinity(ProcessorAffinity);
		}

		private void DoSaveAsync(object asyncState)
		{
			SetProcessorAffinity();
			FileOperationState fileOperationState = asyncState as FileOperationState;
			Exception error = null;
			try
			{
				Save(fileOperationState.File, fileOperationState.TamperProof, fileOperationState.Compressed, fileOperationState.Action);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, fileOperationState.UserState);
			if (this.SaveCompleted != null)
			{
				this.SaveCompleted(this, args);
			}
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoLoadAsync(object asyncState)
		{
			SetProcessorAffinity();
			FileOperationState fileOperationState = asyncState as FileOperationState;
			Exception error = null;
			try
			{
				Load(fileOperationState.File, fileOperationState.Action);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, fileOperationState.UserState);
			if (this.LoadCompleted != null)
			{
				this.LoadCompleted(this, args);
			}
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoDeleteDirectoryAsync(object asyncState)
		{
			SetProcessorAffinity();
			FileOperationState fileOperationState = asyncState as FileOperationState;
			Exception error = null;
			try
			{
				DeleteDirectory(fileOperationState.File);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, fileOperationState.UserState);
			if (this.DeleteDirectoryCompleted != null)
			{
				this.DeleteDirectoryCompleted(this, args);
			}
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoDeleteAsync(object asyncState)
		{
			SetProcessorAffinity();
			FileOperationState fileOperationState = asyncState as FileOperationState;
			Exception error = null;
			try
			{
				Delete(fileOperationState.File);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			FileActionCompletedEventArgs args = new FileActionCompletedEventArgs(error, fileOperationState.UserState);
			if (this.DeleteCompleted != null)
			{
				this.DeleteCompleted(this, args);
			}
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoFileExistsAsync(object asyncState)
		{
			SetProcessorAffinity();
			FileOperationState fileOperationState = asyncState as FileOperationState;
			Exception error = null;
			bool result = false;
			try
			{
				result = FileExists(fileOperationState.File);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			FileExistsCompletedEventArgs args = new FileExistsCompletedEventArgs(error, result, fileOperationState.UserState);
			if (this.FileExistsCompleted != null)
			{
				this.FileExistsCompleted(this, args);
			}
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoGetFilesAsync(object asyncState)
		{
			SetProcessorAffinity();
			FileOperationState fileOperationState = asyncState as FileOperationState;
			Exception error = null;
			string[] result = null;
			try
			{
				result = GetFiles(fileOperationState.Pattern);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			GetFilesCompletedEventArgs args = new GetFilesCompletedEventArgs(error, result, fileOperationState.UserState);
			if (this.GetFilesCompleted != null)
			{
				this.GetFilesCompleted(this, args);
			}
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void PendingOperationsIncrement()
		{
			lock (pendingOperationCountLock)
			{
				pendingOperations++;
			}
		}

		private void PendingOperationsDecrement()
		{
			lock (pendingOperationCountLock)
			{
				pendingOperations--;
			}
		}

		private FileOperationState GetFileOperationState()
		{
			lock (pendingStates)
			{
				if (pendingStates.Count > 0)
				{
					FileOperationState fileOperationState = pendingStates.Dequeue();
					fileOperationState.Reset();
					return fileOperationState;
				}
				return new FileOperationState();
			}
		}

		private void ReturnFileOperationState(FileOperationState state)
		{
			lock (pendingStates)
			{
				pendingStates.Enqueue(state);
			}
		}

		private StorageContainer OpenContainer(string containerName)
		{
			IAsyncResult asyncResult = storageDevice.BeginOpenContainer(containerName, (AsyncCallback)null, (object)null);
			asyncResult.AsyncWaitHandle.WaitOne();
			return storageDevice.EndOpenContainer(asyncResult);
		}

		private void VerifyIsReady()
		{
			if (!IsReady)
			{
				throw new InvalidOperationException(Strings.StorageDevice_is_not_valid);
			}
		}

		private void Save(string fileName, byte[] dataToSave, bool tamperProof, bool compressed)
		{
			FileOptions fileOptions = FileOptions.None;
			if (compressed)
			{
				fileOptions |= FileOptions.Compressesd;
				dataToSave = CompressionTools.Compress(dataToSave);
			}
			if (tamperProof)
			{
				fileOptions |= FileOptions.Encrypted;
				dataToSave = ((LocalKey2 != null) ? SecurityTools.EncryptData(LocalKey2, dataToSave) : SecurityTools.EncryptData(CommonKey2, dataToSave));
				MD5HashProvider mD5HashProvider = new MD5HashProvider();
				Hash hash = mD5HashProvider.Compute(dataToSave);
				MemoryStream memoryStream = new MemoryStream(dataToSave.Length + hash.Data.Length + 8);
				BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
				binaryWriter.Write(hash.Data.Length);
				binaryWriter.Write(hash.Data);
				binaryWriter.Write(dataToSave.Length);
				binaryWriter.Write(dataToSave);
				binaryWriter.Flush();
				dataToSave = memoryStream.ToArray();
			}
			using (Stream output = _currentContainer.CreateFile(fileName))
			{
				BinaryWriter binaryWriter2 = new BinaryWriter(output);
				binaryWriter2.Write(1146311762);
				binaryWriter2.Write(5);
				binaryWriter2.Write((uint)fileOptions);
				binaryWriter2.Write(dataToSave.Length);
				binaryWriter2.Write(dataToSave);
				binaryWriter2.Flush();
			}
		}

		public void Save(string fileName, bool tamperProof, bool compressed, FileAction saveAction)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				MemoryStream memoryStream = new MemoryStream(1024);
				saveAction(memoryStream);
				byte[] dataToSave = memoryStream.ToArray();
				Save(fileName, dataToSave, tamperProof, compressed);
			}
		}

		public void Load(string fileName, FileAction loadAction)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				int num2;
				FileOptions fileOptions;
				byte[] array;
				using (Stream input = _currentContainer.OpenFile(fileName, FileMode.Open))
				{
					BinaryReader binaryReader = new BinaryReader(input);
					int num = binaryReader.ReadInt32();
					if (num != 1146311762)
					{
						throw new Exception();
					}
					num2 = binaryReader.ReadInt32();
					if (num2 < 3 || num2 > 5)
					{
						throw new Exception();
					}
					fileOptions = (FileOptions)binaryReader.ReadUInt32();
					int count = binaryReader.ReadInt32();
					array = binaryReader.ReadBytes(count);
				}
				if ((FileOptions.Encrypted & fileOptions) != 0)
				{
					MemoryStream input2 = new MemoryStream(array);
					BinaryReader binaryReader2 = new BinaryReader(input2);
					MD5HashProvider mD5HashProvider = new MD5HashProvider();
					int count2 = binaryReader2.ReadInt32();
					Hash other = mD5HashProvider.CreateHash(binaryReader2.ReadBytes(count2));
					int count3 = binaryReader2.ReadInt32();
					array = binaryReader2.ReadBytes(count3);
					Hash hash = mD5HashProvider.Compute(array);
					if (!hash.Equals(other))
					{
						throw new Exception();
					}
					switch (num2)
					{
					case 3:
						array = SecurityTools.DecryptData(CommonKeyOld, array);
						break;
					case 4:
						array = ((LocalKeyOld != null) ? SecurityTools.DecryptData(LocalKeyOld, array) : SecurityTools.DecryptData(CommonKeyOld, array));
						break;
					default:
						array = ((LocalKey2 != null) ? SecurityTools.DecryptData(LocalKey2, array) : SecurityTools.DecryptData(CommonKey2, array));
						break;
					}
				}
				if ((FileOptions.Compressesd & fileOptions) != 0)
				{
					array = CompressionTools.Decompress(array);
				}
				if (num2 < 5)
				{
					Save(fileName, array, (FileOptions.Compressesd & fileOptions) != 0, (FileOptions.Encrypted & fileOptions) != 0);
				}
				MemoryStream stream = new MemoryStream(array);
				loadAction(stream);
			}
		}

		public void Delete(string fileName)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				if (_currentContainer.FileExists(fileName))
				{
					_currentContainer.DeleteFile(fileName);
				}
			}
		}

		public bool FileExists(string fileName)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				return _currentContainer.FileExists(fileName);
			}
		}

		public string[] GetFiles()
		{
			return GetFiles(null);
		}

		public string[] GetFiles(string pattern)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				return string.IsNullOrEmpty(pattern) ? _currentContainer.GetFileNames() : _currentContainer.GetFileNames(pattern);
			}
		}

		public void GetDirectoriesAsync(string path)
		{
			throw new NotImplementedException();
		}

		public void GetDirectoriesAsync(string path, string pattern)
		{
			throw new NotImplementedException();
		}

		public void CreateDirectoryAsync(string path)
		{
			throw new NotImplementedException();
		}

		public void DeleteDirectoryAsync(string path)
		{
			DeleteDirectoryAsync(path, null);
		}

		public void DeleteDirectoryAsync(string path, object userState)
		{
			PendingOperationsIncrement();
			FileOperationState fileOperationState = GetFileOperationState();
			fileOperationState.File = path;
			fileOperationState.UserState = userState;
			ThreadPool.QueueUserWorkItem(DoDeleteDirectoryAsync, fileOperationState);
		}

		public string[] GetDirectories(string path)
		{
			return GetDirectories(path, null);
		}

		public string[] GetDirectories(string path, string pattern)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				if (string.IsNullOrEmpty(pattern))
				{
					pattern = "*";
				}
				path = ((!string.IsNullOrEmpty(path)) ? Path.Combine(path, pattern) : pattern);
				return _currentContainer.GetDirectoryNames(path);
			}
		}

		public void CreateDirectory(string path)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				_currentContainer.CreateDirectory(path);
			}
		}

		public void DeleteDirectory(string path)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				DeleteDirectoryInternal(_currentContainer, path);
			}
		}

		private void DeleteDirectoryInternal(StorageContainer container, string path)
		{
			string[] directoryNames = container.GetDirectoryNames(Path.Combine(path, "*"));
			string[] array = directoryNames;
			foreach (string path2 in array)
			{
				DeleteDirectoryInternal(container, path2);
			}
			string[] fileNames = container.GetFileNames(Path.Combine(path, "*"));
			string[] array2 = fileNames;
			foreach (string file in array2)
			{
				container.DeleteFile(file);
			}
			container.DeleteDirectory(path);
		}

		public void DirectoryExistsAsync(string path)
		{
			throw new NotImplementedException();
		}

		public bool DirectoryExists(string path)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				return _currentContainer.DirectoryExists(path);
			}
		}

		public void Flush()
		{
			if (storageDevice == null)
			{
				return;
			}
			lock (storageDevice)
			{
				if (_currentContainer != null)
				{
					_currentContainer.Dispose();
					try
					{
						_currentContainer = OpenContainer(_containerName);
						return;
					}
					catch
					{
						_currentContainer = null;
						return;
					}
				}
			}
		}

		public void DeleteStorage()
		{
			if (storageDevice == null)
			{
				return;
			}
			lock (storageDevice)
			{
				if (_currentContainer != null)
				{
					_currentContainer.Dispose();
					try
					{
						storageDevice.DeleteContainer(_containerName);
						_currentContainer = OpenContainer(_containerName);
						return;
					}
					catch
					{
						_currentContainer = null;
						return;
					}
				}
			}
		}

		public void SaveRaw(string fileName, FileAction saveAction)
		{
			VerifyIsReady();
			lock (storageDevice)
			{
				using (Stream stream = _currentContainer.CreateFile(fileName))
				{
					saveAction(stream);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing && _currentContainer != null)
				{
					_currentContainer.Dispose();
					_currentContainer = null;
				}
				disposed = true;
			}
		}

		~SaveDevice()
		{
			Dispose(false);
		}
	}
}
