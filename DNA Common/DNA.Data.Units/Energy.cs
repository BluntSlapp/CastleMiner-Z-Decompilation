using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Energy
	{
		private const float joulesPerCalorie = 4.184f;

		private const float btusPerCalorie = 0.003965667f;

		private float _calories;

		public float KiloCalories
		{
			get
			{
				return _calories * 0.001f;
			}
		}

		public float Calories
		{
			get
			{
				return _calories;
			}
		}

		public static Energy Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Calories + " Cals";
		}

		public override int GetHashCode()
		{
			return _calories.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _calories == ((Energy)obj)._calories;
		}

		public static bool operator ==(Energy a, Energy b)
		{
			return a._calories == b._calories;
		}

		public static bool operator !=(Energy a, Energy b)
		{
			return a._calories != b._calories;
		}

		public static Energy FromKilocalories(float kilocals)
		{
			return new Energy(kilocals * 1000f);
		}

		public static Energy FromCalories(float calories)
		{
			return new Energy(calories);
		}

		public static Energy operator +(Energy m1, Energy m2)
		{
			return FromCalories(m1._calories + m2._calories);
		}

		public static Energy operator -(Energy m1, Energy m2)
		{
			return FromCalories(m1._calories - m2._calories);
		}

		public static double operator /(Energy m1, Energy m2)
		{
			return m1._calories / m2._calories;
		}

		public static Energy operator *(Energy m1, float scalar)
		{
			return FromCalories(m1._calories * scalar);
		}

		public static Energy operator /(Energy m1, float scalar)
		{
			return FromCalories(m1._calories / scalar);
		}

		private Energy(float calories)
		{
			_calories = calories;
		}
	}
}
