using System;

namespace DNA
{
	public struct RangeI
	{
		private int _min;

		private int _max;

		public bool Degenerate
		{
			get
			{
				return _min == _max;
			}
		}

		public int Min
		{
			get
			{
				return _min;
			}
		}

		public int Max
		{
			get
			{
				return _max;
			}
		}

		public int Span
		{
			get
			{
				return Max - Min;
			}
		}

		public override int GetHashCode()
		{
			return _min.GetHashCode() ^ _max.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			RangeI rangeI = (RangeI)obj;
			return rangeI._max == rangeI.Min;
		}

		public static bool operator ==(RangeI a, RangeI b)
		{
			if (a._max == b._max)
			{
				return a._min == b._min;
			}
			return false;
		}

		public static bool operator !=(RangeI a, RangeI b)
		{
			if (a._max != b._max)
			{
				return a._min != b._min;
			}
			return false;
		}

		public bool InRange(int t)
		{
			if (t >= Min)
			{
				return t <= Max;
			}
			return false;
		}

		public bool Overlaps(RangeI r)
		{
			if (r.Min <= Max)
			{
				return r.Max >= Min;
			}
			return false;
		}

		public RangeI(int min, int max)
		{
			_min = min;
			_max = max;
			if (_max < _min)
			{
				throw new ArgumentException("Max must be Greator than Min");
			}
		}
	}
}
