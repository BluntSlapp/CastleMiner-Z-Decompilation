using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework.Storage;

namespace DNA.IO.Storage
{
	internal class SaveOperationAsyncResult : IAsyncResult
	{
		private readonly object accessLock = new object();

		private bool isCompleted;

		private readonly StorageDevice storageDevice;

		private readonly string containerName;

		private readonly string fileName;

		private readonly FileAction fileAction;

		private readonly FileMode fileMode;

		public object AsyncState { get; set; }

		public WaitHandle AsyncWaitHandle { get; private set; }

		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		public bool IsCompleted
		{
			get
			{
				lock (accessLock)
				{
					return isCompleted;
				}
			}
		}

		internal SaveOperationAsyncResult(StorageDevice device, string container, string file, FileAction action, FileMode mode)
		{
			storageDevice = device;
			containerName = container;
			fileName = file;
			fileAction = action;
			fileMode = mode;
		}

		private void EndOpenContainer(IAsyncResult result)
		{
			using (storageDevice.EndOpenContainer(result))
			{
				if (fileMode != FileMode.Create)
				{
					FileMode fileMode2 = fileMode;
					int num = 3;
				}
			}
			lock (accessLock)
			{
				isCompleted = true;
			}
		}
	}
}
