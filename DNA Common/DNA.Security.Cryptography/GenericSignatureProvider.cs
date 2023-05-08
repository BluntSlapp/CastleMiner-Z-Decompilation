using System.IO;
using DNA.IO;
using DNA.Security.Cryptography.Crypto;

namespace DNA.Security.Cryptography
{
	public abstract class GenericSignatureProvider : ISignatureProvider
	{
		private ISigner _signer;

		protected ISigner Signer
		{
			get
			{
				return _signer;
			}
		}

		public abstract Signature Parse(string s);

		public abstract Signature FromByteArray(byte[] data);

		public GenericSignatureProvider(ISigner signer)
		{
			_signer = signer;
		}

		public virtual Signature GetFileSignature(string path)
		{
			MemoryStream memoryStream = new MemoryStream();
			FileInfo fileInfo = new FileInfo(path);
			using (FileStream source = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				memoryStream.CopyStream(source, fileInfo.Length);
			}
			return Sign(memoryStream.GetBuffer(), fileInfo.Length);
		}

		public Signature Sign(byte[] data)
		{
			return Sign(data, 0L, data.Length);
		}

		public Signature Sign(byte[] data, long length)
		{
			return Sign(data, 0L, length);
		}

		public Signature Sign(byte[] data, long start, long length)
		{
			_signer.BlockUpdate(data, (int)start, (int)length);
			return FromByteArray(_signer.GenerateSignature());
		}
	}
}
