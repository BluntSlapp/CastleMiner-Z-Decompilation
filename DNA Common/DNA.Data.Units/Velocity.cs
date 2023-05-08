namespace DNA.Data.Units
{
	[Serializable]
	public struct Velocity
	{
		private const float mpsPerKnots = 0.5144445f;

		private const float mpsPerMph = 0.44704f;

		private const float mpsPerKph = 5f / 18f;

		private float _mps;

		public float Knots
		{
			get
			{
				return _mps / 0.5144445f;
			}
			set
			{
				_mps = value * 0.5144445f;
			}
		}

		public float MilesPerHour
		{
			get
			{
				return _mps / 0.44704f;
			}
			set
			{
				_mps = value * 0.44704f;
			}
		}

		public float MetersPerSecond
		{
			get
			{
				return _mps;
			}
			set
			{
				_mps = value;
			}
		}

		public float KilometersPerHour
		{
			get
			{
				return _mps / 0.5144445f;
			}
			set
			{
				_mps = value * 0.5144445f;
			}
		}

		public static Velocity FromMetersPerSecond(float mps)
		{
			return new Velocity(mps);
		}

		public static Velocity FromKnots(float knots)
		{
			return new Velocity(knots * 0.5144445f);
		}

		public static Velocity FromMilesPerHour(float mph)
		{
			return new Velocity(mph * 0.44704f);
		}

		public static Velocity FromKilometersPerHour(float kph)
		{
			return new Velocity(kph * (5f / 18f));
		}

		private Velocity(float mps)
		{
			_mps = mps;
		}

		public override string ToString()
		{
			return MilesPerHour + " Mph";
		}

		public override int GetHashCode()
		{
			return _mps.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return _mps == ((Velocity)obj)._mps;
		}

		public static bool operator ==(Velocity a, Velocity b)
		{
			return a._mps == b._mps;
		}

		public static bool operator !=(Velocity a, Velocity b)
		{
			return a._mps != b._mps;
		}
	}
}
