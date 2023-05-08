using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class DrawEventArgs : EventArgs
	{
		public GraphicsDevice Device;

		public GameTime GameTime;

		public DrawEventArgs()
		{
		}

		public DrawEventArgs(GraphicsDevice device, GameTime gameTime)
		{
			Device = device;
			GameTime = gameTime;
		}
	}
}
