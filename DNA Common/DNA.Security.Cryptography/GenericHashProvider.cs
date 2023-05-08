using System;
using System.IO;
using DNA.IO;
using DNA.Security.Cryptography.Crypto;

namespace DNA.Security.Cryptography
{
	public abstract class GenericHashProvider : IHashProvider
	{
		public int HashLength
		{
			get
			{
				return GetHashAlgorythim().GetDigestSize();
			}
		}

		protected abstract IDigest GetHashAlgorythim();

		public abstract Hash CreateHash(byte[] data);

		public abstract Hash Parse(string s);

		public abstract HashProcess BeginHash();

		public GenericHashProvider()
		{
		}

		public Hash Compute(byte[] data)
		{
			if (data.Length > int.MaxValue)
			{
				throw new Exception("Data over 4GB not supported yet");
			}
			IDigest hashAlgorythim = GetHashAlgorythim();
			hashAlgorythim.BlockUpdate(data, 0, data.Length);
			byte[] array = new byte[hashAlgorythim.GetDigestSize()];
			hashAlgorythim.DoFinal(array, 0);
			return CreateHash(array);
		}

		public Hash Compute(byte[] data, long length)
		{
			if (length > int.MaxValue)
			{
				throw new Exception("Data over 4GB not supported yet");
			}
			IDigest hashAlgorythim = GetHashAlgorythim();
			hashAlgorythim.BlockUpdate(data, 0, (int)length);
			byte[] array = new byte[hashAlgorythim.GetDigestSize()];
			hashAlgorythim.DoFinal(array, 0);
			return CreateHash(array);
		}

		public Hash Compute(byte[] data, long start, long length)
		{
			if (length > int.MaxValue)
			{
				throw new Exception("Data over 4GB not supported yet");
			}
			IDigest hashAlgorythim = GetHashAlgorythim();
			hashAlgorythim.BlockUpdate(data, (int)start, (int)length);
			byte[] array = new byte[hashAlgorythim.GetDigestSize()];
			hashAlgorythim.DoFinal(array, 0);
			return CreateHash(array);
		}

		public Hash Read(BinaryReader reader)
		{
			return CreateHash(reader.ReadBytes(HashLength));
		}

		public virtual Hash GetFileHash(string path)
		{
			MemoryStream memoryStream = new MemoryStream();
			FileInfo fileInfo = new FileInfo(path);
			using (FileStream source = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				memoryStream.CopyStream(source, fileInfo.Length);
			}
			return Compute(memoryStream.GetBuffer(), fileInfo.Length);
		}
	}
}
