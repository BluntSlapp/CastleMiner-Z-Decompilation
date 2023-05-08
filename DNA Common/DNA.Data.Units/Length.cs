using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Length
	{
		private const float feetPerMeter = 3.28084f;

		private const float milesPerMeter = 0.0006213712f;

		private const float inchesPerMeter = 39.37008f;

		private const float centemetersPerMeter = 100f;

		private float _meters;

		public float Feet
		{
			get
			{
				return _meters * 3.28084f;
			}
		}

		public float Inches
		{
			get
			{
				return _meters * 39.37008f;
			}
		}

		public float Centimeters
		{
			get
			{
				return _meters * 100f;
			}
		}

		public float Meters
		{
			get
			{
				return _meters;
			}
		}

		public float Miles
		{
			get
			{
				return _meters * 0.0006213712f;
			}
		}

		public override int GetHashCode()
		{
			return _meters.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _meters == ((Length)obj)._meters;
		}

		public static bool operator ==(Length a, Length b)
		{
			return a._meters == b._meters;
		}

		public static bool operator !=(Length a, Length b)
		{
			return a._meters != b._meters;
		}

		public static Length Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Feet + " ft";
		}

		public static Length FromMeters(float meters)
		{
			return new Length(meters);
		}

		public static Length FromInches(float inches)
		{
			return new Length(inches / 39.37008f);
		}

		public static Length FromFeet(float feet)
		{
			return new Length(feet / 3.28084f);
		}

		public static Length FromCentimeters(float cm)
		{
			return new Length(cm / 100f);
		}

		public static Length FromMiles(float miles)
		{
			return new Length(miles / 0.0006213712f);
		}

		public static Length operator +(Length m1, Length m2)
		{
			return FromMeters(m1.Meters + m2.Meters);
		}

		public static Length operator *(Length m1, float c)
		{
			return FromMeters(m1.Meters * c);
		}

		public static Length operator *(float c, Length m1)
		{
			return FromMeters(m1.Meters * c);
		}

		public static Length operator /(Length m1, float c)
		{
			return FromMeters(m1.Meters / c);
		}

		public static double operator /(Length m1, Length m2)
		{
			return m1.Meters / m2.Meters;
		}

		public static bool operator <(Length m1, Length m2)
		{
			return m1._meters < m2._meters;
		}

		public static bool operator >(Length m1, Length m2)
		{
			return m1._meters > m2._meters;
		}

		private Length(float meters)
		{
			_meters = meters;
		}
	}
}
