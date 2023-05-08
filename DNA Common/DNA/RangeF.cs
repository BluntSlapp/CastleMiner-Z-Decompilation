using System;

namespace DNA
{
	public struct RangeF
	{
		private float _min;

		private float _max;

		public bool Degenerate
		{
			get
			{
				return _min == _max;
			}
		}

		public float Min
		{
			get
			{
				return _min;
			}
		}

		public float Max
		{
			get
			{
				return _max;
			}
		}

		public float Span
		{
			get
			{
				return Max - Min;
			}
		}

		public float ToSpan(float val)
		{
			return (val - Min) / Span;
		}

		public bool InRange(float t)
		{
			if (t >= Min)
			{
				return t <= Max;
			}
			return false;
		}

		public bool Overlaps(RangeF r)
		{
			if (!(r.Min > Max))
			{
				return !(r.Max < Min);
			}
			return false;
		}

		public RangeF(float min, float max)
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
			RangeF rangeF = (RangeF)obj;
			if (_min == rangeF._min)
			{
				return _max == rangeF._max;
			}
			return false;
		}

		public static bool operator ==(RangeF a, RangeF b)
		{
			if (a._min == b._min)
			{
				return a._max == b._max;
			}
			return false;
		}

		public static bool operator !=(RangeF a, RangeF b)
		{
			if (a._min == b._min)
			{
				return a._max != b._max;
			}
			return true;
		}
	}
}
