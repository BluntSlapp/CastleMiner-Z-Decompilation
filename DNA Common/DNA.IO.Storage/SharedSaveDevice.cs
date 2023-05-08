using System;
using Microsoft.Xna.Framework.Storage;

namespace DNA.IO.Storage
{
	public sealed class SharedSaveDevice : SaveDevice
	{
		public SharedSaveDevice(string containerName, byte[] oldKey, byte[] key)
			: base(containerName, oldKey, key)
		{
		}

		protected override void GetStorageDevice(AsyncCallback callback, SuccessCallback resultCallback)
		{
			StorageDevice.BeginShowSelector(callback, (object)resultCallback);
		}
	}
}
