using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class UpdateEventArgs : EventArgs
	{
		public GameTime GameTime;

		public UpdateEventArgs()
		{
		}

		public UpdateEventArgs(GameTime gameTime)
		{
			GameTime = gameTime;
		}
	}
}
