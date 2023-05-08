using Microsoft.Xna.Framework.Input;

namespace DNA.Input
{
	public struct ControllerButtons
	{
		private Buttons _buttons;

		public bool A
		{
			get
			{
				return (_buttons & Buttons.A) != 0;
			}
		}

		public bool B
		{
			get
			{
				return (_buttons & Buttons.B) != 0;
			}
		}

		public bool Back
		{
			get
			{
				return (_buttons & Buttons.Back) != 0;
			}
		}

		public bool BigButton
		{
			get
			{
				return (_buttons & Buttons.BigButton) != 0;
			}
		}

		public bool LeftShoulder
		{
			get
			{
				return (_buttons & Buttons.LeftShoulder) != 0;
			}
		}

		public bool LeftStick
		{
			get
			{
				return (_buttons & Buttons.LeftStick) != 0;
			}
		}

		public bool RightShoulder
		{
			get
			{
				return (_buttons & Buttons.RightShoulder) != 0;
			}
		}

		public bool RightStick
		{
			get
			{
				return (_buttons & Buttons.RightStick) != 0;
			}
		}

		public bool Start
		{
			get
			{
				return (_buttons & Buttons.Start) != 0;
			}
		}

		public bool X
		{
			get
			{
				return (_buttons & Buttons.X) != 0;
			}
		}

		public bool Y
		{
			get
			{
				return (_buttons & Buttons.Y) != 0;
			}
		}

		public bool RightTrigger
		{
			get
			{
				return (_buttons & Buttons.RightTrigger) != 0;
			}
		}

		public bool LeftTrigger
		{
			get
			{
				return (_buttons & Buttons.LeftTrigger) != 0;
			}
		}

		public ControllerButtons(Buttons buttons)
		{
			_buttons = buttons;
		}

		public static bool operator !=(ControllerButtons left, ControllerButtons right)
		{
			return left._buttons != right._buttons;
		}

		public static bool operator ==(ControllerButtons left, ControllerButtons right)
		{
			return left._buttons != right._buttons;
		}

		public override bool Equals(object obj)
		{
			if (obj is ControllerButtons)
			{
				return ((ControllerButtons)obj)._buttons == _buttons;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((object)_buttons).GetHashCode();
		}

		public override string ToString()
		{
			return ((object)_buttons).ToString();
		}
	}
}
