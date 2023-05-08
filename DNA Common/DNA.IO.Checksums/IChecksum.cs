namespace DNA.IO.Checksums
{
	public interface IChecksum<T>
	{
		T Value { get; }

		void Reset();

		void Update(byte bval);

		void Update(byte[] buffer);

		void Update(byte[] buf, int off, int len);
	}
}
