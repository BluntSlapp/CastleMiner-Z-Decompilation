namespace DNA.Drawing.Curves
{
	public class ConstantFunction : ISlopeFunction, IFunction
	{
		private float _value;

		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public RangeF Range
		{
			get
			{
				return new RangeF(Value, Value);
			}
		}

		public RangeF SlopeRange
		{
			get
			{
				return new RangeF(0f, 0f);
			}
		}

		public float GetValue(float x)
		{
			return _value;
		}

		public float GetSlope(float x)
		{
			return 0f;
		}
	}
}
