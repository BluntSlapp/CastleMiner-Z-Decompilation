using System;
using Microsoft.Xna.Framework.Net;

namespace DNA.Net
{
	public class InvalidMessageException : Exception
	{
		public NetworkGamer Sender;

		public InvalidMessageException(NetworkGamer sender, string message)
			: base(message)
		{
			Sender = sender;
		}

		public InvalidMessageException(NetworkGamer sender, Exception innerException)
			: base("Invalid Message", innerException)
		{
			Sender = sender;
		}
	}
}
