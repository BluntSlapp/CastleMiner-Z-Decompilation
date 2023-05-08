using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	[Serializable]
	public struct Line
	{
		private Point _start;

		private Point _end;

		public Point Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
			}
		}

		public Point End
		{
			get
			{
				return _end;
			}
			set
			{
				_end = value;
			}
		}

		public double Length
		{
			get
			{
				return Start.Distance(End);
			}
		}

		public Line(Point start, Point end)
		{
			_start = start;
			_end = end;
		}

		public Line(int x1, int y1, int x2, int y2)
		{
			_start = new Point(x1, y1);
			_end = new Point(x2, y2);
		}

		public double DistanceTo(Point pnt)
		{
			double length = Length;
			double num = ((length == 0.0) ? 0.0 : ((double)((pnt.X - _start.X) * (_end.X - _start.X) + (pnt.Y - _start.Y) * (_end.Y - _start.Y)) / (length * length)));
			if (num < 0.0 || num > 1.0)
			{
				double num2 = pnt.Distance(_start);
				double num3 = pnt.Distance(_end);
				if (!(num2 < num3))
				{
					return num3;
				}
				return num2;
			}
			double num4 = (double)_start.X + num * (double)(_end.X - _start.X);
			double num5 = (double)_start.Y + num * (double)(_end.Y - _start.Y);
			double num6 = (double)pnt.X - num4;
			double num7 = (double)pnt.Y - num5;
			return Math.Sqrt(num6 * num6 + num7 * num7);
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == typeof(Line))
			{
				return this == (Line)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(Line a, Line b)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(Line a, Line b)
		{
			throw new NotImplementedException();
		}
	}
}
