using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public struct Circle : IShape2D, ICloneable
	{
		public Vector2 _center;

		public float Radius;

		public RectangleF BoundingBox
		{
			get
			{
				return new RectangleF(_center.X - Radius, _center.Y - Radius, Radius * 2f, Radius * 2f);
			}
		}

		public Vector2 Center
		{
			get
			{
				return _center;
			}
			set
			{
				_center = value;
			}
		}

		public float Area
		{
			get
			{
				return (float)(Math.PI * (double)Radius * (double)Radius);
			}
		}

		public Circle(Vector2 center, float radius)
		{
			_center = center;
			Radius = radius;
		}

		public OrientedBoundingRect GetOrientedBoundingRect()
		{
			throw new NotImplementedException();
		}

		public void Transform(Matrix mat)
		{
			_center = Vector2.Transform(_center, mat);
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
			return (p - _center).Length() <= Radius;
		}

		public int Contains(IList<Vector2> points)
		{
			int num = 0;
			for (int i = 0; i < points.Count; i++)
			{
				Vector2 vector = points[i];
				if (!Contains(points[i]))
				{
					num++;
				}
			}
			return num;
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
			return new Circle(_center, Radius);
		}
	}
}
