using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public class InputManager
	{
		private GameController[] _controllers = new GameController[4];

		private ControllerButtons _buttonsPressed;

		public GameController[] Controllers
		{
			get
			{
				return _controllers;
			}
		}

		public ControllerButtons ButtonsPressed
		{
			get
			{
				return _buttonsPressed;
			}
		}

		public InputManager()
		{
			_controllers[0] = new GameController(PlayerIndex.One);
			_controllers[1] = new GameController(PlayerIndex.Two);
			_controllers[2] = new GameController(PlayerIndex.Three);
			_controllers[3] = new GameController(PlayerIndex.Four);
		}

		public void Update()
		{
			Buttons buttons = (Buttons)0;
			for (int i = 0; i < _controllers.Length; i++)
			{
				GameController gameController = _controllers[i];
				gameController.Update();
				if (gameController.PressedButtons.A)
				{
					buttons |= Buttons.A;
				}
				if (gameController.PressedButtons.B)
				{
					buttons |= Buttons.B;
				}
				if (gameController.PressedButtons.Back)
				{
					buttons |= Buttons.Back;
				}
				if (gameController.PressedButtons.BigButton)
				{
					buttons |= Buttons.BigButton;
				}
				if (gameController.PressedButtons.LeftShoulder)
				{
					buttons |= Buttons.LeftShoulder;
				}
				if (gameController.PressedButtons.LeftStick)
				{
					buttons |= Buttons.LeftStick;
				}
				if (gameController.PressedButtons.RightShoulder)
				{
					buttons |= Buttons.RightShoulder;
				}
				if (gameController.PressedButtons.RightStick)
				{
					buttons |= Buttons.RightStick;
				}
				if (gameController.PressedButtons.Start)
				{
					buttons |= Buttons.Start;
				}
				if (gameController.PressedButtons.X)
				{
					buttons |= Buttons.X;
				}
				if (gameController.PressedButtons.Y)
				{
					buttons |= Buttons.Y;
				}
			}
			_buttonsPressed = new ControllerButtons(buttons);
		}
	}
}
