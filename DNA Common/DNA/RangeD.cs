using System;

namespace DNA
{
	public struct RangeD
	{
		private double _min;

		private double _max;

		public bool Degenerate
		{
			get
			{
				return _min == _max;
			}
		}

		public double Min
		{
			get
			{
				return _min;
			}
		}

		public double Max
		{
			get
			{
				return _max;
			}
		}

		public double Span
		{
			get
			{
				return Max - Min;
			}
		}

		public double ToSpan(double val)
		{
			return (val - Min) / Span;
		}

		public bool InRange(double t)
		{
			if (t >= Min)
			{
				return t <= Max;
			}
			return false;
		}

		public bool Overlaps(RangeF r)
		{
			if (!((double)r.Min > Max))
			{
				return !((double)r.Max < Min);
			}
			return false;
		}

		public RangeD(double min, double max)
		{
			_min = min;
			_max = max;
			if (_max < _min)
			{
				throw new ArgumentException("Max must be Greator than Min");
			}
		}

		public override int GetHashCode()
		{
			return _min.GetHashCode() ^ _max.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			RangeD rangeD = (RangeD)obj;
			if (_min == rangeD._min)
			{
				return _max == rangeD._max;
			}
			return false;
		}

		public static bool operator ==(RangeD a, RangeD b)
		{
			if (a._min == b._min)
			{
				return a._max == b._max;
			}
			return false;
		}

		public static bool operator !=(RangeD a, RangeD b)
		{
			if (a._min == b._min)
			{
				return a._max != b._max;
			}
			return true;
		}
	}
}
