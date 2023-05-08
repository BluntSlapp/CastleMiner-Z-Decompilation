using System.Globalization;
using DNA.Security.Cryptography.Crypto;
using DNA.Security.Cryptography.Crypto.Digests;

namespace DNA.Security.Cryptography
{
	public class SHA1HashProvider : GenericHashProvider
	{
		private class SHA1HashProcess : HashProcess
		{
			public SHA1HashProcess()
				: base(new Sha1Digest())
			{
			}

			protected override Hash CreateHash(byte[] data)
			{
				return new SHA1Hash(data);
			}
		}

		private class SHA1Hash : Hash
		{
			public SHA1Hash(byte[] data)
				: base(data)
			{
			}
		}

		public override HashProcess BeginHash()
		{
			return new SHA1HashProcess();
		}

		public override Hash Parse(string s)
		{
			byte[] array = new byte[s.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = byte.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			return new SHA1Hash(array);
		}

		protected override IDigest GetHashAlgorythim()
		{
			return new Sha1Digest();
		}

		public override Hash CreateHash(byte[] data)
		{
			return new SHA1Hash(data);
		}
	}
}
