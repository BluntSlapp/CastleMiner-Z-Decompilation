using DNA.Drawing.UI;

namespace DNA
{
	public static class GameConsole
	{
		private static ConsoleElement _control;

		public static void SetControl(ConsoleElement control)
		{
			_control = control;
		}

		public static void Write(char value)
		{
			if (_control != null)
			{
				_control.Write(value);
			}
		}

		public static void Write(string value)
		{
			if (_control != null)
			{
				_control.Write(value);
			}
		}

		public static void WriteLine(string value)
		{
			if (_control != null)
			{
				_control.WriteLine(value);
			}
		}

		public static void WriteLine()
		{
			_control.WriteLine();
		}
	}
}
