using System;

namespace DNA.IO.Storage
{
	public struct FileActionCompletedEventArgs
	{
		public Exception Error { get; private set; }

		public object UserState { get; private set; }

		public FileActionCompletedEventArgs(Exception error, object userState)
		{
			this = default(FileActionCompletedEventArgs);
			Error = error;
			UserState = userState;
		}
	}
}
