using System;
using System.Globalization;
using DNA.Security.Cryptography.Crypto;
using DNA.Security.Cryptography.Crypto.Digests;

namespace DNA.Security.Cryptography
{
	public class SHA256HashProvider : GenericHashProvider
	{
		private class SHA256HashProcess : HashProcess
		{
			public SHA256HashProcess()
				: base(new Sha256Digest())
			{
			}

			protected override Hash CreateHash(byte[] data)
			{
				return new SHA256Hash(data);
			}
		}

		private class SHA256Hash : Hash
		{
			public SHA256Hash(byte[] data)
				: base(data)
			{
			}
		}

		public override HashProcess BeginHash()
		{
			return new SHA256HashProcess();
		}

		public override Hash Parse(string s)
		{
			byte[] array = new byte[s.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			return new SHA256Hash(array);
		}

		protected override IDigest GetHashAlgorythim()
		{
			return new Sha256Digest();
		}

		public override Hash CreateHash(byte[] data)
		{
			return new SHA256Hash(data);
		}

		public override Hash GetFileHash(string path)
		{
			throw new NotImplementedException();
		}
	}
}
