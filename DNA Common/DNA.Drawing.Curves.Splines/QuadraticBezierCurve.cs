using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Curves.Splines
{
	public class QuadraticBezierCurve : Spline
	{
		public struct ControlPoint
		{
			private Vector3 _handle;

			private Vector3 _location;

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

			public Vector3 Handle
			{
				get
				{
					return _handle;
				}
				set
				{
					_handle = value;
				}
			}

			public ControlPoint(Vector3 location, Vector3 handle)
			{
				_location = location;
				_handle = handle;
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
			Vector3 handle = cp1.Handle;
			Vector3 location2 = cp2.Location;
			float num = 1f - t;
			float num2 = num * num;
			float num3 = 2f * t * num;
			float num4 = t * t;
			float x = num2 * location.X + num3 * handle.X + num4 * location2.X;
			float y = num2 * location.Y + num3 * handle.Y + num4 * location2.Y;
			float z = num2 * location.Z + num3 * handle.Z + num4 * location2.Z;
			return new Vector3(x, y, z);
		}

		public override Vector3 ComputeVelocity(float t)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override Vector3 ComputeAcceleration(float t)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override bool Equals(object obj)
		{
			QuadraticBezierCurve quadraticBezierCurve = obj as QuadraticBezierCurve;
			if (quadraticBezierCurve == null)
			{
				return false;
			}
			return this == quadraticBezierCurve;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		public static bool operator ==(QuadraticBezierCurve a, QuadraticBezierCurve b)
		{
			throw new NotImplementedException();
		}

		public static bool operator !=(QuadraticBezierCurve a, QuadraticBezierCurve b)
		{
			throw new NotImplementedException();
		}
	}
}
