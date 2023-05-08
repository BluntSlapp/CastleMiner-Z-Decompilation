using System;

namespace DNA.IO.Storage
{
	public sealed class SaveDevicePromptEventArgs : EventArgs
	{
		public bool ShowDeviceSelector { get; internal set; }
	}
}
