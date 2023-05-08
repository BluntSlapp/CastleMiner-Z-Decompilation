using System;
using System.Globalization;
using DNA.Security.Cryptography.Crypto;
using DNA.Security.Cryptography.Crypto.Digests;

namespace DNA.Security.Cryptography
{
	public class MD5HashProvider : GenericHashProvider
	{
		private class MD5HashProcess : HashProcess
		{
			public MD5HashProcess()
				: base(new MD5Digest())
			{
			}

			protected override Hash CreateHash(byte[] data)
			{
				return new MD5Hash(data);
			}
		}

		private class MD5Hash : Hash
		{
			public MD5Hash(byte[] data)
				: base(data)
			{
			}
		}

		public override HashProcess BeginHash()
		{
			return new MD5HashProcess();
		}

		public override Hash Parse(string s)
		{
			byte[] array = new byte[s.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			return new MD5Hash(array);
		}

		protected override IDigest GetHashAlgorythim()
		{
			return new MD5Digest();
		}

		public override Hash CreateHash(byte[] data)
		{
			return new MD5Hash(data);
		}

		public override Hash GetFileHash(string path)
		{
			throw new NotImplementedException();
		}
	}
}
