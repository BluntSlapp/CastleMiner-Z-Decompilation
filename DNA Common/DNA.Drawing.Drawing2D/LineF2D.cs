using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public struct LineF2D : IShape2D, ICloneable
	{
		public Vector2 _start;

		public Vector2 _end;

		public Vector2 Start
		{
			get
			{
				return _start;
			}
			set
			{
				_start = value;
			}
		}

		public Vector2 End
		{
			get
			{
				return _end;
			}
			set
			{
				_end = value;
			}
		}

		public bool IsVertical
		{
			get
			{
				return _end.X == _start.X;
			}
		}

		public bool IsHorizonal
		{
			get
			{
				return _end.Y == _start.Y;
			}
		}

		public double LengthSq
		{
			get
			{
				return End.X * End.X + Start.X * Start.X;
			}
		}

		public double Length
		{
			get
			{
				return Vector2.Distance(Start, End);
			}
		}

		public RectangleF BoundingBox
		{
			get
			{
				return ComputeBoundingBox();
			}
		}

		public Vector2 Center
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float Area
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private RectangleF ComputeBoundingBox()
		{
			float x;
			float x2;
			if (_start.X < _end.X)
			{
				x = _start.X;
				x2 = _end.X;
			}
			else
			{
				x = _end.X;
				x2 = _start.X;
			}
			float y;
			float y2;
			if (_start.Y < _end.Y)
			{
				y = _start.Y;
				y2 = _end.Y;
			}
			else
			{
				y = _end.Y;
				y2 = _start.Y;
			}
			return new RectangleF(x, y, x2 - x, y2 - y);
		}

		public LineF2D(float sx, float sy, float ex, float ey)
		{
			_start = new Vector2(sx, sy);
			_end = new Vector2(ex, ey);
		}

		public LineF2D(Vector2 start, Vector2 end)
		{
			_start = start;
			_end = end;
		}

		public Vector2 GetValue(float t)
		{
			float x = (_end.X - _start.X) * t + _start.X;
			float y = (_end.Y - _start.Y) * t + _start.Y;
			return new Vector2(x, y);
		}

		public float DistanceSquare()
		{
			float num = End.Y - Start.Y;
			float num2 = End.X - Start.X;
			return num * num + num2 * num2;
		}

		public float Intersect(Vector2 pointOnLine)
		{
			if (IsVertical)
			{
				float num = _end.Y - _start.Y;
				return (pointOnLine.Y - _start.Y) / num;
			}
			float num2 = _end.X - _start.X;
			return (pointOnLine.X - _start.X) / num2;
		}

		public bool HorizontalIntersect(float x, out float y)
		{
			float num = _end.X - _start.X;
			if (num == 0f)
			{
				y = float.NaN;
				return false;
			}
			float num2 = (x - _start.X) / num;
			y = _start.Y + num2 * (_end.Y - _start.Y);
			if (num2 >= 0f)
			{
				return num2 <= 1f;
			}
			return false;
		}

		public bool VerticalIntersect(float y, out float x)
		{
			float num = _end.Y - _start.Y;
			if (num == 0f)
			{
				x = float.NaN;
				return false;
			}
			float num2 = (y - _start.Y) / num;
			x = _start.X + num2 * (_end.X - _start.X);
			if (num2 >= 0f)
			{
				return num2 <= 1f;
			}
			return false;
		}

		public bool Intersects(LineF2D line)
		{
			Vector2 intersection;
			bool coincident;
			return Intersects(line, out intersection, out coincident);
		}

		public bool Intersects(Circle circle, out float t1, out float t2)
		{
			Vector2 center = circle.Center;
			float radius = circle.Radius;
			Vector2 value = _end - _start;
			Vector2 value2 = _start - center;
			double num = value.LengthSquared();
			double num2 = 2f * Vector2.Dot(value, value2);
			double num3 = center.LengthSquared() + _start.LengthSquared() - 2f * Vector2.Dot(center, _start) - radius * radius;
			double num4 = num2 * num2 - 4.0 * num * num3;
			if (num == 0.0 || num4 < 0.0)
			{
				t1 = 0f;
				t2 = 0f;
				return false;
			}
			double num5 = Math.Sqrt(num4);
			t1 = (float)((0.0 - num2 + num5) / (2.0 * num));
			t2 = (float)((0.0 - num2 - num5) / (2.0 * num));
			if (t1 < 0f || t1 > 1f)
			{
				if (t2 >= 0f && t2 <= 1f)
				{
					t1 = t2;
					return true;
				}
				return false;
			}
			return true;
		}

		public bool Intersects(Vector2 start, Vector2 end, out float t, out bool coincident)
		{
			coincident = false;
			Vector2 v = new Vector2(_end.X - _start.X, _end.Y - _start.Y);
			Vector2 vector = new Vector2(end.X - start.X, end.Y - start.Y);
			if (v.LengthSquared() == 0f || vector.LengthSquared() == 0f)
			{
				t = -1f;
				return false;
			}
			Vector2 v2 = new Vector2(_start.X - start.X, _start.Y - start.Y);
			float num = vector.Cross(v2);
			float num2 = v.Cross(vector);
			float num3 = v.Cross(v2);
			if (num2 == 0f)
			{
				t = -1f;
				if (num == 0f && num3 == 0f)
				{
					coincident = true;
					return true;
				}
				return false;
			}
			float num4 = num / num2;
			float num5 = num3 / num2;
			if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
			{
				t = num4;
				return true;
			}
			t = -1f;
			return false;
		}

		public bool Intersects(LineF2D line, out float t, out bool coincident)
		{
			coincident = false;
			Vector2 v = new Vector2(_end.X - _start.X, _end.Y - _start.Y);
			Vector2 vector = new Vector2(line._end.X - line._start.X, line._end.Y - line._start.Y);
			if (v.LengthSquared() == 0f || vector.LengthSquared() == 0f)
			{
				t = -1f;
				return false;
			}
			Vector2 v2 = new Vector2(_start.X - line._start.X, _start.Y - line._start.Y);
			float num = vector.Cross(v2);
			float num2 = v.Cross(vector);
			float num3 = v.Cross(v2);
			if (num2 == 0f)
			{
				t = -1f;
				if (num == 0f && num3 == 0f)
				{
					coincident = true;
					return true;
				}
				return false;
			}
			float num4 = num / num2;
			float num5 = num3 / num2;
			if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
			{
				t = num4;
				return true;
			}
			t = -1f;
			return false;
		}

		public bool Intersects(LineF2D line, out Vector2 intersection, out bool coincident)
		{
			coincident = false;
			Vector2 v = new Vector2(_end.X - _start.X, _end.Y - _start.Y);
			Vector2 vector = new Vector2(line._end.X - line._start.X, line._end.Y - line._start.Y);
			if (v.LengthSquared() == 0f || vector.LengthSquared() == 0f)
			{
				intersection = Vector2.Zero;
				return false;
			}
			Vector2 v2 = new Vector2(_start.X - line._start.X, _start.Y - line._start.Y);
			float num = vector.Cross(v2);
			float num2 = v.Cross(vector);
			float num3 = v.Cross(v2);
			if (num2 == 0f)
			{
				intersection = Vector2.Zero;
				if (num == 0f && num3 == 0f)
				{
					coincident = true;
					return true;
				}
				return false;
			}
			float num4 = num / num2;
			float num5 = num3 / num2;
			if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
			{
				intersection = new Vector2(_start.X + num4 * v.X, _start.Y + num4 * v.Y);
				return true;
			}
			intersection = Vector2.Zero;
			return false;
		}

		public Vector2 ShortestVectorTo(Vector2 pnt)
		{
			double num = Vector2.DistanceSquared(_start, _end);
			double num2 = ((num == 0.0) ? 0.0 : ((double)((pnt.X - _start.X) * (_end.X - _start.X) + (pnt.Y - _start.Y) * (_end.Y - _start.Y)) / num));
			if (num2 < 0.0)
			{
				return _start - pnt;
			}
			if (num2 > 1.0)
			{
				return _end - pnt;
			}
			double num3 = (double)_start.X + num2 * (double)(_end.X - _start.X);
			double num4 = (double)_start.Y + num2 * (double)(_end.Y - _start.Y);
			double num5 = num3 - (double)pnt.X;
			double num6 = num4 - (double)pnt.Y;
			return new Vector2((float)num5, (float)num6);
		}

		public double DistanceTo(Vector2 pnt)
		{
			return ShortestVectorTo(pnt).Length();
		}

		public override string ToString()
		{
			return ((object)Start).ToString() + "-" + ((object)End).ToString();
		}

		public OrientedBoundingRect GetOrientedBoundingRect()
		{
			throw new NotImplementedException();
		}

		public void Transform(Matrix mat)
		{
			throw new NotImplementedException();
		}

		public bool Touches(IShape2D s)
		{
			throw new NotImplementedException();
		}

		public bool IsAbove(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public float DistanceTo(IShape2D poly)
		{
			throw new NotImplementedException();
		}

		public float DistanceSquaredTo(IShape2D poly)
		{
			throw new NotImplementedException();
		}

		public bool IsBelow(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public bool IsLeftOf(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public bool IsRightOf(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public bool CompletelyContains(IShape2D region)
		{
			throw new NotImplementedException();
		}

		public bool Contains(Vector2 p)
		{
			throw new NotImplementedException();
		}

		public int Contains(IList<Vector2> points)
		{
			throw new NotImplementedException();
		}

		public IShape2D Intersection(IShape2D s)
		{
			throw new NotImplementedException();
		}

		public bool BoundsIntersect(IShape2D shape)
		{
			throw new NotImplementedException();
		}

		public float Contains(IShape2D shape)
		{
			throw new NotImplementedException();
		}

		public object Clone()
		{
			throw new NotImplementedException();
		}
	}
}
