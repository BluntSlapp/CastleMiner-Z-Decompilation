using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Mass
	{
		private const float lbsPerGram = 0.00220462261f;

		private const float kgsPerGram = 0.001f;

		private const float ouncesPerGram = 0.03527396f;

		private const float milligramsPerGram = 1000f;

		private const float microgramsPerGram = 1000000f;

		private float _grams;

		public float Pounds
		{
			get
			{
				return _grams * 0.00220462261f;
			}
		}

		public float KiloGrams
		{
			get
			{
				return _grams * 0.001f;
			}
		}

		public float Grams
		{
			get
			{
				return _grams;
			}
		}

		public float Milligrams
		{
			get
			{
				return _grams * 1000f;
			}
		}

		public float Micrograms
		{
			get
			{
				return _grams * 1000000f;
			}
		}

		public float Ounces
		{
			get
			{
				return _grams * 0.03527396f;
			}
		}

		public override int GetHashCode()
		{
			return _grams.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _grams == ((Mass)obj)._grams;
		}

		public static bool operator ==(Mass a, Mass b)
		{
			return a._grams == b._grams;
		}

		public static bool operator !=(Mass a, Mass b)
		{
			return a._grams != b._grams;
		}

		public static Mass Parse(string unit, float amount)
		{
			switch (unit)
			{
			case "oz":
				return FromOunces(amount);
			case "ounce":
				return FromOunces(amount);
			case "ounces":
				return FromOunces(amount);
			case "lbs":
				return FromPounds(amount);
			case "pound":
				return FromPounds(amount);
			case "pounds":
				return FromPounds(amount);
			case "g":
				return FromGrams(amount);
			case "gram":
				return FromGrams(amount);
			case "grams":
				return FromGrams(amount);
			case "kgs":
				return FromKiloGrams(amount);
			case "kilogram":
				return FromKiloGrams(amount);
			case "kilograms":
				return FromKiloGrams(amount);
			default:
				throw new ArgumentException("Unreconized Unit " + unit);
			}
		}

		public static Mass Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Pounds + " lbs";
		}

		public static Mass FromOunces(float oz)
		{
			return new Mass(oz / 0.03527396f);
		}

		public static Mass FromGrams(float grams)
		{
			return new Mass(grams);
		}

		public static Mass operator +(Mass m1, Mass m2)
		{
			return FromGrams(m1.Grams + m2.Grams);
		}

		public static Mass operator -(Mass m1, Mass m2)
		{
			return FromGrams(m1.Grams - m2.Grams);
		}

		public static double operator /(Mass m1, Mass m2)
		{
			return m1._grams / m2._grams;
		}

		public static Mass operator *(Mass m1, float scalar)
		{
			return FromGrams(m1.Grams * scalar);
		}

		public static Mass operator /(Mass m1, float scalar)
		{
			return FromGrams(m1.Grams / scalar);
		}

		public static Mass FromPounds(float pounds)
		{
			return new Mass(pounds / 0.00220462261f);
		}

		public static Mass FromKiloGrams(float kilograms)
		{
			return new Mass(kilograms / 0.001f);
		}

		private Mass(float grams)
		{
			_grams = grams;
		}
	}
}
