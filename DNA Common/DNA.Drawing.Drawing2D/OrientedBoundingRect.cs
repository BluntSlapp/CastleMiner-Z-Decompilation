using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public struct OrientedBoundingRect
	{
		public static readonly OrientedBoundingRect Empty = new OrientedBoundingRect(new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));

		private Vector2 _size;

		private Vector2 _location;

		private Vector2 _axis;

		public Vector2 Size
		{
			get
			{
				return _size;
			}
		}

		public Vector2 Location
		{
			get
			{
				return _location;
			}
		}

		public Angle Rotation
		{
			get
			{
				return Angle.FromDegrees(90f) + Angle.ATan2(_axis.Y, _axis.X);
			}
		}

		public Vector2 Axis
		{
			get
			{
				return _axis;
			}
		}

		public Vector2 Center
		{
			get
			{
				Vector2 vector = new Vector2(_axis.Y, 0f - _axis.X);
				return new Vector2(_location.X + _axis.X * _size.Y / 2f + vector.X * _size.X / 2f, _location.Y + _axis.Y * _size.Y / 2f + vector.Y * _size.X / 2f);
			}
		}

		public Matrix Orientation
		{
			get
			{
				return Matrix.CreateScale(_size.X, _size.Y, 1f) * Matrix.CreateRotationZ(Rotation.Radians) * Matrix.CreateTranslation(_location.X, _location.Y, 0f);
			}
		}

		private OrientedBoundingRect(Vector2 location, Vector2 size, Vector2 axis)
		{
			_size = size;
			_location = location;
			_axis = axis;
		}

		public static OrientedBoundingRect FromPoints(IList<Vector2> opoints)
		{
			Vector2[] convexHull = DrawingTools.GetConvexHull(opoints);
			if (convexHull.Length <= 0)
			{
				return Empty;
			}
			RectangleF rectangleF = DrawingTools.FindBounds(convexHull);
			Vector2 location = new Vector2(rectangleF.Left + rectangleF.Width / 2f, rectangleF.Top + rectangleF.Height / 2f);
			Vector2 vector = new Vector2(0f, 0f);
			for (int i = 0; i < convexHull.Length; i++)
			{
				Vector2 vector2 = convexHull[i];
				Vector2 vector3 = new Vector2(vector2.X - location.X, vector2.Y - location.Y);
				if (vector3.X < 0f)
				{
					vector3 = -vector3;
				}
				vector += vector3;
			}
			Vector2 vector4 = new Vector2(0f, 0f);
			for (int j = 0; j < convexHull.Length; j++)
			{
				Vector2 vector5 = convexHull[j];
				Vector2 vector6 = new Vector2(vector5.X - location.X, vector5.Y - location.Y);
				if (vector6.Y < 0f)
				{
					vector6 = -vector6;
				}
				vector4 += vector6;
			}
			if (vector4.Length() > vector.Length())
			{
				vector = vector4;
			}
			vector.Normalize();
			Vector2 vector7 = new Vector2(vector.Y, 0f - vector.X);
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			for (int k = 0; k < convexHull.Length; k++)
			{
				Vector2 vector8 = convexHull[k];
				Vector2 vector9 = new Vector2(location.X - vector8.X, location.Y - vector8.Y);
				float val = vector.X * vector9.Y - vector9.X * vector.Y;
				float val2 = 0f - (vector7.X * vector9.Y - vector9.X * vector7.Y);
				num = Math.Min(num, val);
				num2 = Math.Min(num2, val2);
				num3 = Math.Max(num3, val);
				num4 = Math.Max(num4, val2);
			}
			float x = num3 - num;
			float y = num4 - num2;
			Vector2 vector10 = num * vector7;
			Vector2 vector11 = num2 * vector;
			location.X = location.X + vector10.X + vector11.X;
			location.Y = location.Y + vector10.Y + vector11.Y;
			return new OrientedBoundingRect(location, new Vector2(x, y), vector);
		}

		public static OrientedBoundingRect FromPoints(IList<Point> opoints)
		{
			Vector2[] array = new Vector2[opoints.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new Vector2(opoints[i].X, opoints[i].Y);
			}
			return FromPoints(array);
		}

		public static OrientedBoundingRect FromPoints(IList<Vector3> points)
		{
			throw new Exception();
		}
	}
}
