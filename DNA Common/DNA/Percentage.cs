namespace DNA
{
	[Serializable]
	public struct Percentage
	{
		public static readonly Percentage Zero = FromFraction(0f);

		public static readonly Percentage OneHundred = FromFraction(1f);

		private float _fraction;

		public float Fraction
		{
			get
			{
				return _fraction;
			}
			set
			{
				_fraction = value;
			}
		}

		public float Percent
		{
			get
			{
				return _fraction * 100f;
			}
			set
			{
				_fraction = value / 100f;
			}
		}

		public override int GetHashCode()
		{
			return _fraction.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _fraction == ((Percentage)obj)._fraction;
		}

		public static bool operator ==(Percentage a, Percentage b)
		{
			return a._fraction == b._fraction;
		}

		public static bool operator !=(Percentage a, Percentage b)
		{
			return a._fraction != b._fraction;
		}

		public string ToString(int decmialplaces)
		{
			return Fraction.ToString("P" + decmialplaces);
		}

		public override string ToString()
		{
			return Fraction.ToString("P");
		}

		public static Percentage FromFraction(float fraction)
		{
			return new Percentage(fraction);
		}

		public static Percentage FromPercent(float percent)
		{
			return new Percentage(percent / 100f);
		}

		public static Percentage Parse(string str)
		{
			str = str.TrimEnd(' ', '\t', '\n', '%');
			return new Percentage(float.Parse(str) / 100f);
		}

		public static bool operator <(Percentage p1, Percentage p2)
		{
			return p1._fraction < p2._fraction;
		}

		public static bool operator >(Percentage p1, Percentage p2)
		{
			return p1._fraction > p2._fraction;
		}

		public static Percentage operator +(Percentage p1, Percentage p2)
		{
			return new Percentage(p1._fraction + p2._fraction);
		}

		public static Percentage operator -(Percentage p1, Percentage p2)
		{
			return new Percentage(p1._fraction - p2._fraction);
		}

		private Percentage(float fraction)
		{
			_fraction = fraction;
		}
	}
}
