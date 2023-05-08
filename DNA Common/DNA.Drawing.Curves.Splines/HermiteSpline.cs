using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public class HermiteSpline : Spline
	{
		public struct ControlPoint
		{
			private Vector3 _location;

			private Vector3 _inHandle;

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

			public ControlPoint(Vector3 inVect, Vector3 location, Vector3 outVect)
			{
				_inHandle = inVect;
				_location = location;
				_outHandle = outVect;
			}

			public void ReflectInHandle()
			{
				Out = -In;
			}

			public void ReflectOutHandle()
			{
				In = -Out;
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
			Vector3 location2 = cp2.Location;
			Vector3 @in = cp2.In;
			float num = t * t;
			float num2 = num * t;
			float num3 = 2f * num2 - 3f * num + 1f;
			float num4 = -2f * num2 + 3f * num;
			float num5 = num2 - 2f * num + t;
			float num6 = num2 - num;
			float x = num3 * location.X + num4 * location2.X + num5 * @out.X + num6 * @in.X;
			float y = num3 * location.Y + num4 * location2.Y + num5 * @out.Y + num6 * @in.Y;
			float z = num3 * location.Z + num4 * location2.Z + num5 * @out.Z + num6 * @in.Z;
			return new Vector3(x, y, z);
		}

		public override Vector3 ComputeVelocity(float t)
		{
			throw new NotImplementedException();
		}

		public override Vector3 ComputeAcceleration(float t)
		{
			throw new NotImplementedException();
		}

		public override bool Equals(object obj)
		{
			HermiteSpline hermiteSpline = obj as HermiteSpline;
			if (hermiteSpline == null)
			{
				return false;
			}
			return this == hermiteSpline;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(HermiteSpline a, HermiteSpline b)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(HermiteSpline a, HermiteSpline b)
		{
			throw new NotImplementedException();
		}
	}
}
