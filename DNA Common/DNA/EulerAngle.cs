using System;
using Microsoft.Xna.Framework;

namespace DNA
{
	public struct EulerAngle
	{
		private Angle _yaw;

		private Angle _pitch;

		private Angle _roll;

		public Angle Yaw
		{
			get
			{
				return _yaw;
			}
			set
			{
				_yaw = value;
			}
		}

		public Angle Pitch
		{
			get
			{
				return _pitch;
			}
			set
			{
				_pitch = value;
			}
		}

		public Angle Roll
		{
			get
			{
				return _roll;
			}
			set
			{
				_roll = value;
			}
		}

		public EulerAngle(Angle yaw, Angle pitch, Angle roll)
		{
			_yaw = yaw;
			_pitch = pitch;
			_roll = roll;
		}

		public EulerAngle(Quaternion q)
		{
			Vector3 zero = Vector3.Zero;
			float rads = (float)Math.Atan2(2f * q.Y * q.W - 2f * q.X * q.Z, 1.0 - 2.0 * Math.Pow(q.Y, 2.0) - 2.0 * Math.Pow(q.Z, 2.0));
			float rads2 = (float)Math.Asin(2f * q.X * q.Y + 2f * q.Z * q.W);
			float rads3 = (float)Math.Atan2(2f * q.X * q.W - 2f * q.Y * q.Z, 1.0 - 2.0 * Math.Pow(q.X, 2.0) - 2.0 * Math.Pow(q.Z, 2.0));
			if ((double)(q.X * q.Y + q.Z * q.W) == 0.5)
			{
				rads = (float)(2.0 * Math.Atan2(q.X, q.W));
				rads3 = 0f;
			}
			else if ((double)(q.X * q.Y + q.Z * q.W) == -0.5)
			{
				rads = (float)(-2.0 * Math.Atan2(q.X, q.W));
				rads3 = 0f;
			}
			_yaw = Angle.FromRadians(rads);
			_pitch = Angle.FromRadians(rads3);
			_roll = Angle.FromRadians(rads2);
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() == typeof(EulerAngle))
			{
				return this == (EulerAngle)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(EulerAngle a, EulerAngle b)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(EulerAngle a, EulerAngle b)
		{
			throw new NotImplementedException();
		}
	}
}
