using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public class GameController
	{
		private PlayerIndex _playerIndex;

		private GamePadState _lastState;

		private GamePadState _currentState;

		private ControllerButtons _pressedButtons;

		private ControllerButtons _releasedButtons;

		private ControllerDPad _pressedDPad;

		private ControllerDPad _releasedDPad;

		private GamePadCapabilities _caps;

		public float LeftTriggerPullThreshhold = 0.25f;

		public float RightTriggerPullThreshhold = 0.25f;

		public bool ButtonPressed
		{
			get
			{
				if (!_pressedButtons.A && !_pressedButtons.B && !_pressedButtons.Back && !_pressedButtons.BigButton && !_pressedButtons.LeftShoulder && !_pressedButtons.LeftStick && !_pressedButtons.LeftTrigger && !_pressedButtons.RightShoulder && !_pressedButtons.RightStick && !_pressedButtons.RightTrigger && !_pressedButtons.Start && !_pressedButtons.X)
				{
					return _pressedButtons.Y;
				}
				return true;
			}
		}

		public PlayerIndex PlayerIndex
		{
			get
			{
				return _playerIndex;
			}
		}

		public GamePadState CurrentState
		{
			get
			{
				return _currentState;
			}
		}

		public GamePadState LastState
		{
			get
			{
				return _lastState;
			}
		}

		public ControllerButtons PressedButtons
		{
			get
			{
				return _pressedButtons;
			}
		}

		public ControllerButtons ReleasedButtons
		{
			get
			{
				return _releasedButtons;
			}
		}

		public ControllerDPad PressedDPad
		{
			get
			{
				return _pressedDPad;
			}
		}

		public ControllerDPad ReleasedDPad
		{
			get
			{
				return _releasedDPad;
			}
		}

		public GameController(PlayerIndex index)
		{
			_playerIndex = index;
		}

		public void SetVibration(float left, float right)
		{
			GamePad.SetVibration(_playerIndex, left, right);
		}

		public void Update()
		{
			_lastState = _currentState;
			_currentState = GamePad.GetState(_playerIndex);
			_caps = GamePad.GetCapabilities(_playerIndex);
			Buttons buttons = (Buttons)0;
			if (_lastState.Triggers.Left <= LeftTriggerPullThreshhold && _currentState.Triggers.Left > LeftTriggerPullThreshhold)
			{
				buttons |= Buttons.LeftTrigger;
			}
			if (_lastState.Triggers.Right <= RightTriggerPullThreshhold && _currentState.Triggers.Right > RightTriggerPullThreshhold)
			{
				buttons |= Buttons.RightTrigger;
			}
			if (_lastState.Buttons.A == ButtonState.Released && _currentState.Buttons.A == ButtonState.Pressed)
			{
				buttons |= Buttons.A;
			}
			if (_lastState.Buttons.X == ButtonState.Released && _currentState.Buttons.X == ButtonState.Pressed)
			{
				buttons |= Buttons.X;
			}
			if (_lastState.Buttons.B == ButtonState.Released && _currentState.Buttons.B == ButtonState.Pressed)
			{
				buttons |= Buttons.B;
			}
			if (_lastState.Buttons.Y == ButtonState.Released && _currentState.Buttons.Y == ButtonState.Pressed)
			{
				buttons |= Buttons.Y;
			}
			if (_lastState.Buttons.Start == ButtonState.Released && _currentState.Buttons.Start == ButtonState.Pressed)
			{
				buttons |= Buttons.Start;
			}
			if (_lastState.Buttons.Back == ButtonState.Released && _currentState.Buttons.Back == ButtonState.Pressed)
			{
				buttons |= Buttons.Back;
			}
			if (_lastState.Buttons.BigButton == ButtonState.Released && _currentState.Buttons.BigButton == ButtonState.Pressed)
			{
				buttons |= Buttons.BigButton;
			}
			if (_lastState.Buttons.LeftShoulder == ButtonState.Released && _currentState.Buttons.LeftShoulder == ButtonState.Pressed)
			{
				buttons |= Buttons.LeftShoulder;
			}
			if (_lastState.Buttons.LeftStick == ButtonState.Released && _currentState.Buttons.LeftStick == ButtonState.Pressed)
			{
				buttons |= Buttons.LeftStick;
			}
			if (_lastState.Buttons.RightShoulder == ButtonState.Released && _currentState.Buttons.RightShoulder == ButtonState.Pressed)
			{
				buttons |= Buttons.RightShoulder;
			}
			if (_lastState.Buttons.RightStick == ButtonState.Released && _currentState.Buttons.RightStick == ButtonState.Pressed)
			{
				buttons |= Buttons.RightStick;
			}
			_pressedButtons = new ControllerButtons(buttons);
			Buttons buttons2 = (Buttons)0;
			if (_lastState.Triggers.Left > LeftTriggerPullThreshhold && _currentState.Triggers.Left <= LeftTriggerPullThreshhold)
			{
				buttons2 |= Buttons.LeftTrigger;
			}
			if (_lastState.Triggers.Right > RightTriggerPullThreshhold && _currentState.Triggers.Right <= RightTriggerPullThreshhold)
			{
				buttons2 |= Buttons.RightTrigger;
			}
			if (_lastState.Buttons.A == ButtonState.Pressed && _currentState.Buttons.A == ButtonState.Released)
			{
				buttons2 |= Buttons.A;
			}
			if (_lastState.Buttons.X == ButtonState.Pressed && _currentState.Buttons.X == ButtonState.Released)
			{
				buttons2 |= Buttons.X;
			}
			if (_lastState.Buttons.B == ButtonState.Pressed && _currentState.Buttons.B == ButtonState.Released)
			{
				buttons2 |= Buttons.B;
			}
			if (_lastState.Buttons.Y == ButtonState.Pressed && _currentState.Buttons.Y == ButtonState.Released)
			{
				buttons2 |= Buttons.Y;
			}
			if (_lastState.Buttons.Start == ButtonState.Pressed && _currentState.Buttons.Start == ButtonState.Released)
			{
				buttons2 |= Buttons.Start;
			}
			if (_lastState.Buttons.Back == ButtonState.Pressed && _currentState.Buttons.Back == ButtonState.Released)
			{
				buttons2 |= Buttons.Back;
			}
			if (_lastState.Buttons.BigButton == ButtonState.Pressed && _currentState.Buttons.BigButton == ButtonState.Released)
			{
				buttons2 |= Buttons.BigButton;
			}
			if (_lastState.Buttons.LeftShoulder == ButtonState.Pressed && _currentState.Buttons.LeftShoulder == ButtonState.Released)
			{
				buttons2 |= Buttons.LeftShoulder;
			}
			if (_lastState.Buttons.LeftStick == ButtonState.Pressed && _currentState.Buttons.LeftStick == ButtonState.Released)
			{
				buttons2 |= Buttons.LeftStick;
			}
			if (_lastState.Buttons.RightShoulder == ButtonState.Pressed && _currentState.Buttons.RightShoulder == ButtonState.Released)
			{
				buttons2 |= Buttons.RightShoulder;
			}
			if (_lastState.Buttons.RightStick == ButtonState.Pressed && _currentState.Buttons.RightStick == ButtonState.Released)
			{
				buttons2 |= Buttons.RightStick;
			}
			_releasedButtons = new ControllerButtons(buttons2);
			bool upValue = false;
			bool downValue = false;
			bool leftValue = false;
			bool rightValue = false;
			if (_lastState.DPad.Up == ButtonState.Released && _currentState.DPad.Up == ButtonState.Pressed)
			{
				upValue = true;
			}
			if (_lastState.DPad.Down == ButtonState.Released && _currentState.DPad.Down == ButtonState.Pressed)
			{
				downValue = true;
			}
			if (_lastState.DPad.Left == ButtonState.Released && _currentState.DPad.Left == ButtonState.Pressed)
			{
				leftValue = true;
			}
			if (_lastState.DPad.Right == ButtonState.Released && _currentState.DPad.Right == ButtonState.Pressed)
			{
				rightValue = true;
			}
			_pressedDPad = new ControllerDPad(upValue, downValue, leftValue, rightValue);
			if (_lastState.DPad.Up == ButtonState.Pressed && _currentState.DPad.Up == ButtonState.Released)
			{
				upValue = true;
			}
			if (_lastState.DPad.Down == ButtonState.Pressed && _currentState.DPad.Down == ButtonState.Released)
			{
				downValue = true;
			}
			if (_lastState.DPad.Left == ButtonState.Pressed && _currentState.DPad.Left == ButtonState.Released)
			{
				leftValue = true;
			}
			if (_lastState.DPad.Right == ButtonState.Pressed && _currentState.DPad.Right == ButtonState.Released)
			{
				rightValue = true;
			}
			_releasedDPad = new ControllerDPad(upValue, downValue, leftValue, rightValue);
		}
	}
}
