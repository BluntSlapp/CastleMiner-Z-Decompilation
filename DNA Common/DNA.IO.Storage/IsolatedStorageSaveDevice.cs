using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Threading;

namespace DNA.IO.Storage
{
	public class IsolatedStorageSaveDevice : IAsyncSaveDevice, ISaveDevice
	{
		private class FileOperationState
		{
			public string File;

			public bool TamperProof;

			public bool Compressed;

			public string Pattern;

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

		private Queue<FileOperationState> pendingStates = new Queue<FileOperationState>(100);

		private readonly object pendingOperationCountLock = new object();

		private int pendingOperations;

		public bool IsReady
		{
			get
			{
				return true;
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

		public event SaveCompletedEventHandler SaveCompleted;

		public event LoadCompletedEventHandler LoadCompleted;

		public event DeleteCompletedEventHandler DeleteCompleted;

		public event FileExistsCompletedEventHandler FileExistsCompleted;

		public event GetFilesCompletedEventHandler GetFilesCompleted;

		private IsolatedStorageFile GetIsolatedStorage()
		{
			return IsolatedStorageFile.GetUserStoreForApplication();
		}

		public void Save(string fileName, bool tamperProof, bool Compressed, FileAction saveAction)
		{
			throw new NotImplementedException();
		}

		public void Load(string fileName, FileAction loadAction)
		{
			throw new NotImplementedException();
		}

		public void Delete(string fileName)
		{
			using (IsolatedStorageFile isolatedStorageFile = GetIsolatedStorage())
			{
				if (isolatedStorageFile.FileExists(fileName))
				{
					isolatedStorageFile.DeleteFile(fileName);
				}
			}
		}

		public bool FileExists(string fileName)
		{
			using (IsolatedStorageFile isolatedStorageFile = GetIsolatedStorage())
			{
				return isolatedStorageFile.FileExists(fileName);
			}
		}

		public string[] GetFiles()
		{
			using (IsolatedStorageFile isolatedStorageFile = GetIsolatedStorage())
			{
				return isolatedStorageFile.GetFileNames();
			}
		}

		public string[] GetFiles(string pattern)
		{
			using (IsolatedStorageFile isolatedStorageFile = GetIsolatedStorage())
			{
				return isolatedStorageFile.GetFileNames(pattern);
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

		private void DoSaveAsync(object asyncState)
		{
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
			this.SaveCompleted(this, args);
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoLoadAsync(object asyncState)
		{
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
			this.LoadCompleted(this, args);
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoDeleteAsync(object asyncState)
		{
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
			this.DeleteCompleted(this, args);
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoFileExistsAsync(object asyncState)
		{
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
			this.FileExistsCompleted(this, args);
			ReturnFileOperationState(fileOperationState);
			PendingOperationsDecrement();
		}

		private void DoGetFilesAsync(object asyncState)
		{
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
			this.GetFilesCompleted(this, args);
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
			throw new NotImplementedException();
		}

		public string[] GetDirectories(string path)
		{
			throw new NotImplementedException();
		}

		public string[] GetDirectories(string path, string pattern)
		{
			throw new NotImplementedException();
		}

		public void CreateDirectory(string path)
		{
			throw new NotImplementedException();
		}

		public void DeleteDirectory(string path)
		{
			throw new NotImplementedException();
		}

		public void DirectoryExistsAsync(string path)
		{
			throw new NotImplementedException();
		}

		public bool DirectoryExists(string path)
		{
			throw new NotImplementedException();
		}
	}
}
