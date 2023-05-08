using System;
using Microsoft.Xna.Framework;

namespace DNA.IO.Storage
{
	public sealed class SaveDeviceEventArgs : EventArgs
	{
		public SaveDeviceEventResponse Response { get; set; }

		public PlayerIndex? PlayerToPrompt { get; set; }
	}
}
