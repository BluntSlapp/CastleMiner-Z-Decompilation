namespace DNA.Security.Cryptography
{
	public interface ISignatureProvider
	{
		Signature Sign(byte[] data);

		Signature Sign(byte[] data, long length);

		Signature Sign(byte[] data, long start, long length);

		Signature GetFileSignature(string path);

		Signature Parse(string s);
	}
}
