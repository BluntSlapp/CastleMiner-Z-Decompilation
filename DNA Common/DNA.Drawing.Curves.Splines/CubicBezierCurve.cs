using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public class CubicBezierCurve : Spline
	{
		public struct ControlPoint
		{
			private Vector3 _inHandle;

			private Vector3 _location;

			private Vector3 _outHandle;

			public Vector3 In
			{
				get
				{
					return _inHandle;
				}
				set
				{
					_inHandle = value;
				}
			}

			public Vector3 Location
			{
				get
				{
					return _location;
				}
				set
				{
					_location = value;
				}
			}

			public Vector3 Out
			{
				get
				{
					return _outHandle;
				}
				set
				{
					_outHandle = value;
				}
			}

			public ControlPoint(Vector3 inPoint, Vector3 location, Vector3 outPoint)
			{
				_inHandle = inPoint;
				_location = location;
				_outHandle = outPoint;
			}

			public void ReflectInHandle()
			{
				Vector3 vector = In - Location;
				Out = Location - vector;
			}

			public void ReflectOutHandle()
			{
				Vector3 vector = Out - Location;
				In = Location - vector;
			}

			public override int GetHashCode()
			{
				throw new NotImplementedException();
			}

			public bool Equals(ControlPoint other)
			{
				throw new NotImplementedException();
			}

			public override bool Equals(object obj)
			{
				if (obj.GetType() == typeof(ControlPoint))
				{
					return Equals((ControlPoint)obj);
				}
				return false;
			}

			public static bool operator ==(ControlPoint a, ControlPoint b)
			{
				return a.Equals(b);
			}

			public static bool operator !=(ControlPoint a, ControlPoint b)
			{
				return !a.Equals(b);
			}
		}

		private List<ControlPoint> _controlPoints = new List<ControlPoint>();

		public List<ControlPoint> ControlPoints
		{
			get
			{
				return _controlPoints;
			}
			set
			{
				_controlPoints = value;
			}
		}

		public override Vector3 ComputeValue(float t)
		{
			int controlPointIndex = Spline.GetControlPointIndex(_controlPoints.Count, ref t);
			return ComputeValue(t, ControlPoints[controlPointIndex], ControlPoints[controlPointIndex + 1]);
		}

		private static Vector3 ComputeValue(float t, ControlPoint cp1, ControlPoint cp2)
		{
			Vector3 location = cp1.Location;
			Vector3 @out = cp1.Out;
			Vector3 @in = cp2.In;
			Vector3 location2 = cp2.Location;
			float num = 1f - t;
			float num2 = num * num * num;
			float num3 = 3f * t * num * num;
			float num4 = 3f * t * t * num;
			float num5 = t * t * t;
			float x = num2 * location.X + num3 * @out.X + num4 * @in.X + num5 * location2.X;
			float y = num2 * location.Y + num3 * @out.Y + num4 * @in.Y + num5 * location2.Y;
			float z = num2 * location.Z + num3 * @out.Z + num4 * @in.Z + num5 * location2.Z;
			return new Vector3(x, y, z);
		}

		public override Vector3 ComputeVelocity(float t)
		{
			int controlPointIndex = Spline.GetControlPointIndex(_controlPoints.Count, ref t);
			return ComputeVelocity(t, ControlPoints[controlPointIndex], ControlPoints[controlPointIndex + 1]);
		}

		public static Vector3 ComputeVelocity(float t, ControlPoint cp1, ControlPoint cp2)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override Vector3 ComputeAcceleration(float t)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}
	}
}
