namespace DNA.Drawing.Curves
{
	public class LinearFunction : ISlopeFunction, IFunction
	{
		private float _intercept;

		private float _slope;

		public float Start
		{
			get
			{
				return _intercept;
			}
			set
			{
				_intercept = value;
			}
		}

		public float End
		{
			get
			{
				return GetValue(1f);
			}
			set
			{
				SetFunction(_intercept, value);
			}
		}

		public RangeF Range
		{
			get
			{
				return new RangeF(Start, End);
			}
		}

		public RangeF SlopeRange
		{
			get
			{
				return new RangeF(_slope, _slope);
			}
		}

		private void SetFunction(float start, float end)
		{
			_intercept = start;
			_slope = end - start;
		}

		public float GetValue(float x)
		{
			return _slope * x + _intercept;
		}

		public float GetSlope(float x)
		{
			return _slope;
		}
	}
}
