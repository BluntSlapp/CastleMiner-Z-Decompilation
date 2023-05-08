using DNA.Security.Cryptography.Crypto;

namespace DNA.Security.Cryptography
{
	public abstract class HashProcess
	{
		private IDigest _hashAlg;

		public HashProcess(IDigest hashAlg)
		{
			_hashAlg = hashAlg;
		}

		public void ComputeBlock(byte[] buffer, int offset, int count)
		{
			_hashAlg.BlockUpdate(buffer, offset, count);
		}

		protected abstract Hash CreateHash(byte[] data);

		public Hash EndHash(byte[] buffer, int offset, int count)
		{
			_hashAlg.BlockUpdate(buffer, offset, count);
			return EndHash();
		}

		public Hash EndHash()
		{
			byte[] array = new byte[_hashAlg.GetDigestSize()];
			_hashAlg.DoFinal(array, 0);
			return CreateHash(array);
		}
	}
}
