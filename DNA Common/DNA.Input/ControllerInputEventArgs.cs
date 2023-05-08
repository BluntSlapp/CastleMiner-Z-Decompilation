using System;
using Microsoft.Xna.Framework;

namespace DNA.Input
{
	public class ControllerInputEventArgs : EventArgs
	{
		public GameController Controller;

		public GameTime GameTime;

		public ControllerInputEventArgs()
		{
		}

		public ControllerInputEventArgs(GameController controller, GameTime time)
		{
			Controller = controller;
			GameTime = time;
		}
	}
}
