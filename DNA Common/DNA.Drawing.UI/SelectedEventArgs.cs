using System;

namespace DNA.Drawing.UI
{
	public class SelectedEventArgs : EventArgs
	{
		public object Tag;

		public SelectedEventArgs()
		{
		}

		public SelectedEventArgs(object tag)
		{
			Tag = tag;
		}
	}
}
