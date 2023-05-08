using System;

namespace DNA.Drawing.Curves
{
	public class CubicFunction : ISlopeFunction, IFunction
	{
		private float _startValue;

		private float _startSlope;

		private float _a;

		private float _b;

		public float StartValue
		{
			get
			{
				return _startValue;
			}
			set
			{
				SetCurve(value, _startSlope, EndValue, EndSlope);
			}
		}

		public float StartSlope
		{
			get
			{
				return _startSlope;
			}
			set
			{
				SetCurve(StartValue, value, EndValue, EndSlope);
			}
		}

		public float EndValue
		{
			get
			{
				return GetValue(1f);
			}
			set
			{
				SetCurve(StartValue, StartSlope, value, EndSlope);
			}
		}

		public float EndSlope
		{
			get
			{
				return GetSlope(1f);
			}
			set
			{
				SetCurve(StartValue, StartSlope, EndValue, value);
			}
		}

		public RangeF SlopeRange
		{
			get
			{
				if (StartSlope < EndSlope)
				{
					return new RangeF(StartSlope, EndSlope);
				}
				return new RangeF(EndSlope, StartSlope);
			}
		}

		public RangeF Range
		{
			get
			{
				float num;
				float num2 = (num = StartValue);
				float endValue = EndValue;
				if (endValue > num2)
				{
					num2 = endValue;
				}
				else if (endValue < num)
				{
					num = endValue;
				}
				if (_a == 0f)
				{
					if (_b != 0f)
					{
						endValue = -0.5f * _startSlope / _b;
						if (endValue > 0f && endValue < 1f)
						{
							endValue = GetValue(endValue);
							if (endValue < num)
							{
								num = endValue;
							}
							else if (endValue > num2)
							{
								num2 = endValue;
							}
						}
					}
				}
				else
				{
					float num3 = 3f * _a;
					float num4 = 2f * _b;
					float num5 = num4 * num4 - 4f * num3 * _startSlope;
					if (num5 >= 0f)
					{
						num3 = 0.5f / num3;
						num4 = (0f - num4) * num3;
						num5 = (float)(Math.Sqrt(num5) * (double)num3);
						endValue = num4 - num5;
						if (endValue > 0f && endValue < 1f)
						{
							endValue = GetValue(endValue);
							if (endValue < num)
							{
								num = endValue;
							}
							else if (endValue > num2)
							{
								num2 = endValue;
							}
						}
						endValue = num4 + num5;
						if (endValue > 0f && endValue < 1f)
						{
							endValue = GetValue(endValue);
							if (endValue < num)
							{
								num = endValue;
							}
							else if (endValue > num2)
							{
								num2 = endValue;
							}
						}
					}
				}
				return new RangeF(num, num2);
			}
		}

		public CubicFunction()
		{
		}

		public CubicFunction(float startValue, float startSlope, float endValue, float endSlope)
		{
			SetCurve(startValue, startSlope, endValue, endSlope);
		}

		public void SetCurve(float startValue, float startSlope, float endValue, float endSlope)
		{
			_a = endSlope + startSlope + 2f * (startValue - endValue);
			_b = (endSlope - startSlope - 3f * _a) * 0.5f;
			_startSlope = startSlope;
			_startValue = startValue;
		}

		public float GetSlope(float x)
		{
			return (3f * _a * x + 2f * _b) * x + _startSlope;
		}

		public float GetValue(float x)
		{
			return ((_a * x + _b) * x + _startSlope) * x + _startValue;
		}
	}
}
