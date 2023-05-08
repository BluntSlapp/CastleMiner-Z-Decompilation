using System.IO;

namespace DNA.Security.Cryptography
{
	public interface IHashProvider
	{
		int HashLength { get; }

		Hash Compute(byte[] data);

		Hash Compute(byte[] data, long length);

		Hash Compute(byte[] data, long start, long length);

		HashProcess BeginHash();

		Hash GetFileHash(string path);

		Hash Read(BinaryReader reader);

		Hash Parse(string s);
	}
}
