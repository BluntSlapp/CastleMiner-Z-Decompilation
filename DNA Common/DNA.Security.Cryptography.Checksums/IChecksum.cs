namespace DNA.Security.Cryptography.Checksums
{
	public interface IChecksum
	{
		uint Value { get; }

		void Reset();

		void Update(int bval);

		void Update(byte[] buffer);

		void Update(byte[] buf, int off, int len);
	}
}
