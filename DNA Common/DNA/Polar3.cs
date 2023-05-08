using System;
using Microsoft.Xna.Framework;

namespace DNA
{
	public struct Polar3
	{
		private float _radius;

		private Angle _zenith;

		private Angle _azimuth;

		public float Radius
		{
			get
			{
				return _radius;
			}
		}

		public Angle Zenith
		{
			get
			{
				return _zenith;
			}
		}

		public Angle Azimuth
		{
			get
			{
				return _azimuth;
			}
		}

		public Vector3 GetCartesian()
		{
			float x = (float)((double)Radius * Math.Sin(Zenith.Radians) * Math.Cos(Azimuth.Radians));
			float y = (float)((double)Radius * Math.Sin(Zenith.Radians) * Math.Sin(Azimuth.Radians));
			float z = (float)((double)Radius * Math.Cos(Zenith.Radians));
			return new Vector3(x, y, z);
		}

		public float DistanceTo(Polar3 p)
		{
			Vector3 cartesian = GetCartesian();
			Vector3 cartesian2 = p.GetCartesian();
			return Vector3.Distance(cartesian, cartesian2);
		}

		public float ArcLength(Polar3 p3)
		{
			Vector3 cartesian = GetCartesian();
			Vector3 cartesian2 = p3.GetCartesian();
			Angle angle = cartesian.AngleBetween(cartesian2);
			float num = (float)((double)(2f * p3.Radius) * Math.PI);
			return angle.Revolutions * num;
		}

		public Polar3(Angle zenith, Angle azimuth, float radius)
		{
			_radius = radius;
			_zenith = zenith;
			_azimuth = azimuth;
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == typeof(Polar3))
			{
				return this == (Polar3)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(Polar3 a, Polar3 b)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(Polar3 a, Polar3 b)
		{
			throw new NotImplementedException();
		}
	}
}
