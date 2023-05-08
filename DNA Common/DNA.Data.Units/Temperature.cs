using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Temperature
	{
		private float _celsiusDegrees;

		public float Fahrenheit
		{
			get
			{
				return _celsiusDegrees * 9f / 5f + 32f;
			}
		}

		public float Celsius
		{
			get
			{
				return _celsiusDegrees;
			}
		}

		public float Kelvin
		{
			get
			{
				return _celsiusDegrees + 273f;
			}
		}

		public static Temperature Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Celsius + " C";
		}

		public static Temperature FromCelsius(float celsius)
		{
			return new Temperature(celsius);
		}

		private Temperature(float celsius)
		{
			_celsiusDegrees = celsius;
		}

		public override int GetHashCode()
		{
			return _celsiusDegrees.GetHashCode();
		}

		public bool Equals(Temperature other)
		{
			return _celsiusDegrees == other._celsiusDegrees;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == typeof(Temperature))
			{
				return Equals((Temperature)obj);
			}
			return false;
		}

		public static bool operator ==(Temperature a, Temperature b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Temperature a, Temperature b)
		{
			return !a.Equals(b);
		}
	}
}
