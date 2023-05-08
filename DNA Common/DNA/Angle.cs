using System;
using Microsoft.Xna.Framework;

namespace DNA
{
	public struct Angle
	{
		public static readonly Angle Zero = FromRadians(0f);

		private float _radians;

		public float Radians
		{
			get
			{
				return _radians;
			}
			set
			{
				_radians = value;
			}
		}

		public float Degrees
		{
			get
			{
				return (float)((double)(_radians * 180f) / Math.PI);
			}
			set
			{
				_radians = (float)((double)value * Math.PI / 180.0);
			}
		}

		public double Sin
		{
			get
			{
				return Math.Sin(_radians);
			}
		}

		public double Cos
		{
			get
			{
				return Math.Cos(_radians);
			}
		}

		public double Tan
		{
			get
			{
				return Math.Tan(_radians);
			}
		}

		public float Revolutions
		{
			get
			{
				return (float)((double)_radians / (Math.PI * 2.0));
			}
			set
			{
				_radians = (float)((double)(value * 2f) * Math.PI);
			}
		}

		public static Angle ASin(double value)
		{
			return new Angle((float)Math.Asin(value));
		}

		public static Angle ACos(double value)
		{
			return new Angle((float)Math.Acos(value));
		}

		public static Angle ATan(double value)
		{
			return new Angle((float)Math.Atan(value));
		}

		public static Angle ATan2(double y, double x)
		{
			return new Angle((float)Math.Atan2(y, x));
		}

		public static Angle Lerp(Angle a, Angle b, float t)
		{
			return FromRadians((b.Radians - a.Radians) * t + a.Radians);
		}

		private static float NormalizeRadians(float rads)
		{
			rads = (float)Math.IEEERemainder(rads, Math.PI * 2.0);
			if (rads < 0f)
			{
				rads += (float)Math.PI * 2f;
			}
			return rads;
		}

		public void Normalize()
		{
			_radians = NormalizeRadians(_radians);
		}

		public static double DegreesToRadians(double degs)
		{
			return degs * Math.PI / 180.0;
		}

		public static float DegreesToRadians(float degs)
		{
			return (float)((double)degs * Math.PI / 180.0);
		}

		public static float RadiansToDegrees(float rads)
		{
			return (float)((double)(rads * 180f) / Math.PI);
		}

		public static Angle FromLocations(Point pivot, Point point)
		{
			int num = point.X - pivot.X;
			int num2 = point.Y - pivot.Y;
			float num3 = (float)num2 / (float)num;
			float num4 = 0f;
			if (num == 0)
			{
				num4 = ((num2 > 0) ? ((float)Math.PI / 2f) : ((num2 >= 0) ? 0f : (-(float)Math.PI / 2f)));
			}
			else
			{
				num4 = (float)Math.Atan(num3);
				if (num < 0)
				{
					num4 += (float)Math.PI;
				}
			}
			num4 = NormalizeRadians(num4);
			return FromRadians(num4);
		}

		public static Angle FromRadians(double rads)
		{
			return new Angle((float)rads);
		}

		public static Angle FromDegrees(double degs)
		{
			double radians = DegreesToRadians(degs);
			return new Angle(radians);
		}

		public static Angle FromRadians(float rads)
		{
			return new Angle(rads);
		}

		public static Angle FromDegrees(float degs)
		{
			return new Angle(DegreesToRadians(degs));
		}

		public static Angle FromRevolutions(float revs)
		{
			return new Angle((float)((double)(revs * 2f) * Math.PI));
		}

		public static Angle operator +(Angle a, Angle b)
		{
			return new Angle(a._radians + b._radians);
		}

		public static Angle operator -(Angle a)
		{
			return new Angle(0f - a._radians);
		}

		public static Angle operator -(Angle a, Angle b)
		{
			return new Angle(a._radians - b._radians);
		}

		public static Angle operator /(Angle a, float b)
		{
			return new Angle(a._radians / b);
		}

		public static Angle operator /(Angle a, double b)
		{
			return new Angle((double)a._radians / b);
		}

		public static float operator /(Angle a, Angle b)
		{
			return a._radians / b._radians;
		}

		public static bool operator <(Angle a, Angle b)
		{
			return a._radians < b._radians;
		}

		public static bool operator >(Angle a, Angle b)
		{
			return a._radians > b._radians;
		}

		public static Angle operator *(float b, Angle a)
		{
			return new Angle(a._radians * b);
		}

		public static Angle operator *(double b, Angle a)
		{
			return new Angle((double)a._radians * b);
		}

		public static Angle operator *(Angle a, float b)
		{
			return new Angle(a._radians * b);
		}

		public static Angle operator *(Angle a, double b)
		{
			return new Angle((double)a._radians * b);
		}

		private Angle(double radians)
		{
			_radians = (float)radians;
		}

		private Angle(float radians)
		{
			_radians = radians;
		}

		public override string ToString()
		{
			return Degrees.ToString();
		}

		public static Angle Parse(string s)
		{
			return FromDegrees(float.Parse(s));
		}

		public override int GetHashCode()
		{
			return _radians.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Angle angle = (Angle)obj;
			return _radians == angle._radians;
		}

		public static bool operator ==(Angle a, Angle b)
		{
			return a._radians == b._radians;
		}

		public static bool operator !=(Angle a, Angle b)
		{
			return a._radians != b._radians;
		}
	}
}
