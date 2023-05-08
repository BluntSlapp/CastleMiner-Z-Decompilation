using System;

namespace DNA
{
	public class PrecisionException : Exception
	{
		public PrecisionException()
		{
		}

		public PrecisionException(string message)
			: base(message)
		{
		}
	}
}
