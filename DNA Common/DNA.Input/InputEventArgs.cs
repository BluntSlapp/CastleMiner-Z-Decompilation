using System;
using Microsoft.Xna.Framework;

namespace DNA.Input
{
	public class InputEventArgs : EventArgs
	{
		public InputManager InputManager;

		public GameTime GameTime;

		public InputEventArgs(InputManager inputManger, GameTime time)
		{
			InputManager = inputManger;
			GameTime = time;
		}
	}
}
