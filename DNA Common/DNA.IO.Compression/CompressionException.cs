using System;

namespace DNA.IO.Compression
{
	public class CompressionException : Exception
	{
		public CompressionException()
		{
		}

		public CompressionException(string msg)
			: base(msg)
		{
		}

		public CompressionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
