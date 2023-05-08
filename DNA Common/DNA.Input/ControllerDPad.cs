namespace DNA.Input
{
	public struct ControllerDPad
	{
		private bool _up;

		private bool _down;

		private bool _left;

		private bool _right;

		public bool Down
		{
			get
			{
				return _down;
			}
		}

		public bool Left
		{
			get
			{
				return _left;
			}
		}

		public bool Right
		{
			get
			{
				return _right;
			}
		}

		public bool Up
		{
			get
			{
				return _up;
			}
		}

		public ControllerDPad(bool upValue, bool downValue, bool leftValue, bool rightValue)
		{
			_up = upValue;
			_down = downValue;
			_left = leftValue;
			_right = rightValue;
		}

		public static bool operator !=(ControllerDPad left, ControllerDPad right)
		{
			if (left.Up == right.Up && left.Down == right.Down && left.Right == right.Right)
			{
				return left.Left != right.Left;
			}
			return true;
		}

		public static bool operator ==(ControllerDPad left, ControllerDPad right)
		{
			if (left.Up == right.Up && left.Down == right.Down && left.Right == right.Right)
			{
				return left.Left == right.Left;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is ControllerDPad)
			{
				ControllerDPad controllerDPad = (ControllerDPad)obj;
				return this == controllerDPad;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _up.GetHashCode() ^ _down.GetHashCode() ^ _left.GetHashCode() ^ _right.GetHashCode();
		}

		public override string ToString()
		{
			string text = "";
			if (_up)
			{
				return text += "Up";
			}
			if (_down)
			{
				if (text.Length > 0)
				{
					text += "|";
				}
				return text += "Down";
			}
			if (_left)
			{
				if (text.Length > 0)
				{
					text += "|";
				}
				return text += "Left";
			}
			if (_right)
			{
				if (text.Length > 0)
				{
					text += "|";
				}
				return text += "Right";
			}
			return text;
		}
	}
}
