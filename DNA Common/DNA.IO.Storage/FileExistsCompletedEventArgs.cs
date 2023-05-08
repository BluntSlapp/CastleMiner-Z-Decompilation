using System;

namespace DNA.IO.Storage
{
	public struct FileExistsCompletedEventArgs
	{
		public Exception Error { get; private set; }

		public object UserState { get; private set; }

		public bool Result { get; private set; }

		public FileExistsCompletedEventArgs(Exception error, bool result, object userState)
		{
			this = default(FileExistsCompletedEventArgs);
			Error = error;
			Result = result;
			UserState = userState;
		}
	}
}
