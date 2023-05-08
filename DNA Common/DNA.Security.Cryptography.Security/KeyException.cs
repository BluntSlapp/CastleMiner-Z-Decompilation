using System;

namespace DNA.Security.Cryptography.Security
{
	public class KeyException : GeneralSecurityException
	{
		public KeyException()
		{
		}

		public KeyException(string message)
			: base(message)
		{
		}

		public KeyException(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
