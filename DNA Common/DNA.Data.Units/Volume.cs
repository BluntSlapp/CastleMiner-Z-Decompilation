using System;

namespace DNA.Data.Units
{
	[Serializable]
	public struct Volume
	{
		private const float tablespoonsPerLiter = 67.6280441f;

		private const float teaspoonsPerLiter = 202.88414f;

		private const float cupsPerLiter = 4.22675276f;

		private const float gallonsPerLiter = 0.264172047f;

		private const float pintsPerLiter = 2.11337638f;

		private const float quartsPerLiter = 1.05668819f;

		private const float fluidOuncesPerLiter = 33.8140221f;

		private const float cubicInchesPerLiter = 61.0237427f;

		private const float millilitersPerLiter = 1000f;

		private float _liters;

		public float Liters
		{
			get
			{
				return _liters;
			}
		}

		public float CubicInches
		{
			get
			{
				return _liters * 61.0237427f;
			}
		}

		public float Tablespoons
		{
			get
			{
				return _liters * 67.6280441f;
			}
		}

		public float Teaspoons
		{
			get
			{
				return _liters * 202.88414f;
			}
		}

		public float Cups
		{
			get
			{
				return _liters * 4.22675276f;
			}
		}

		public float Gallons
		{
			get
			{
				return _liters * 0.264172047f;
			}
		}

		public float Pints
		{
			get
			{
				return _liters * 2.11337638f;
			}
		}

		public float Quarts
		{
			get
			{
				return _liters * 1.05668819f;
			}
		}

		public float FluidOunces
		{
			get
			{
				return _liters * 33.8140221f;
			}
		}

		public static Volume Parse(string unit, float amount)
		{
			unit = unit.ToLower();
			switch (unit)
			{
			case "c":
				return FromCups(amount);
			case "cup":
				return FromCups(amount);
			case "cups":
				return FromCups(amount);
			case "tbsp":
				return FromTablespoons(amount);
			case "tablespoon":
				return FromTablespoons(amount);
			case "tablespoons":
				return FromTablespoons(amount);
			case "tsp":
				return FromTeaspoons(amount);
			case "teaspoon":
				return FromTeaspoons(amount);
			case "teaspoons":
				return FromTeaspoons(amount);
			case "pts":
				return FromPints(amount);
			case "pint":
				return FromPints(amount);
			case "pints":
				return FromPints(amount);
			case "qts":
				return FromQuarts(amount);
			case "quart":
				return FromQuarts(amount);
			case "quarts":
				return FromQuarts(amount);
			case "gal":
				return FromGallons(amount);
			case "gallon":
				return FromGallons(amount);
			case "gallons":
				return FromGallons(amount);
			case "fl oz":
				return FromFluidOunces(amount);
			case "fluid ounce":
				return FromFluidOunces(amount);
			case "fluid ounces":
				return FromFluidOunces(amount);
			case "ml":
				return FromMilliliters(amount);
			case "milliliter":
				return FromMilliliters(amount);
			case "milliliters":
				return FromMilliliters(amount);
			case "l":
				return FromLiters(amount);
			case "liter":
				return FromLiters(amount);
			case "liters":
				return FromLiters(amount);
			default:
				throw new ArgumentException("Unreconized Unit " + unit);
			}
		}

		public static Volume Parse(string str)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return Liters + " L";
		}

		public static Volume FromPints(float pints)
		{
			return new Volume(pints / 2.11337638f);
		}

		public static Volume FromGallons(float gallons)
		{
			return new Volume(gallons / 0.264172047f);
		}

		public static Volume FromMilliliters(float milliliters)
		{
			return new Volume(milliliters / 1000f);
		}

		public static Volume FromLiters(float liters)
		{
			return new Volume(liters);
		}

		public static Volume FromCups(float cups)
		{
			return new Volume(cups / 4.22675276f);
		}

		public static Volume FromTablespoons(float tbs)
		{
			return new Volume(tbs / 67.6280441f);
		}

		public static Volume FromTeaspoons(float tsp)
		{
			return new Volume(tsp / 202.88414f);
		}

		public static Volume FromFluidOunces(float floz)
		{
			return new Volume(floz / 33.8140221f);
		}

		public static Volume FromQuarts(float qts)
		{
			return new Volume(qts / 1.05668819f);
		}

		public static Volume FromCubicInches(float in2)
		{
			return new Volume(in2 / 61.0237427f);
		}

		public static Volume operator +(Volume m1, Volume m2)
		{
			return FromLiters(m1._liters + m2._liters);
		}

		public static Volume operator -(Volume m1, Volume m2)
		{
			return FromLiters(m1._liters - m2._liters);
		}

		public static double operator /(Volume m1, Volume m2)
		{
			return m1._liters / m2._liters;
		}

		public static Volume operator *(Volume m1, float scalar)
		{
			return FromLiters(m1._liters * scalar);
		}

		public static Volume operator /(Volume m1, float scalar)
		{
			return FromLiters(m1._liters / scalar);
		}

		private Volume(float liters)
		{
			_liters = liters;
		}

		public override int GetHashCode()
		{
			return _liters.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Volume volume = (Volume)obj;
			return _liters == volume._liters;
		}

		public static bool operator ==(Volume a, Volume b)
		{
			return a._liters == b._liters;
		}

		public static bool operator !=(Volume a, Volume b)
		{
			return a._liters != b._liters;
		}
	}
}
