namespace DNA.IO.Compression.Zip.Compression
{
	public class DeflaterPending : PendingBuffer
	{
		public DeflaterPending()
			: base(65536)
		{
		}
	}
}
