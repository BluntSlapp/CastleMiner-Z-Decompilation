using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	[Serializable]
	public abstract class Shape2D : IShape2D, ICloneable
	{
		public abstract RectangleF BoundingBox { get; }

		public virtual Vector2 Center
		{
			get
			{
				RectangleF boundingBox = BoundingBox;
				return new Vector2(boundingBox.X + boundingBox.Width / 2f, boundingBox.Y + boundingBox.Height / 2f);
			}
		}

		public abstract float Area { get; }

		public abstract OrientedBoundingRect GetOrientedBoundingRect();

		public abstract void Transform(Matrix mat);

		public static IShape2D Transform(IShape2D poly, Matrix mat)
		{
			IShape2D shape2D = (IShape2D)poly.Clone();
			shape2D.Transform(mat);
			return shape2D;
		}

		public abstract bool Touches(IShape2D s);

		public abstract bool IsAbove(Vector2 p);

		public float DistanceTo(IShape2D poly)
		{
			return (float)Math.Sqrt(DistanceSquaredTo(poly));
		}

		public abstract float DistanceSquaredTo(IShape2D poly);

		public abstract bool IsBelow(Vector2 p);

		public abstract bool IsLeftOf(Vector2 p);

		public abstract bool IsRightOf(Vector2 p);

		public abstract bool CompletelyContains(IShape2D region);

		public abstract bool Contains(Vector2 p);

		public abstract IShape2D Intersection(IShape2D s);

		public int Contains(IList<Vector2> points)
		{
			int num = 0;
			for (int i = 0; i < points.Count; i++)
			{
				if (Contains(points[i]))
				{
					num++;
				}
			}
			return num;
		}

		public bool BoundsIntersect(IShape2D shape)
		{
			RectangleF boundingBox = BoundingBox;
			RectangleF boundingBox2 = shape.BoundingBox;
			return boundingBox.IntersectsWith(boundingBox2);
		}

		public abstract float Contains(IShape2D shape);

		public static int CompareByArea(IShape2D a, IShape2D b)
		{
			if (b.Area == a.Area)
			{
				return 0;
			}
			if (!(b.Area > a.Area))
			{
				return -1;
			}
			return 1;
		}

		public static int CompareByHeight(IShape2D p1, IShape2D p2)
		{
			if (p1.BoundingBox.Height > p2.BoundingBox.Height)
			{
				return -1;
			}
			if (p1.BoundingBox.Height < p2.BoundingBox.Height)
			{
				return 1;
			}
			return 0;
		}

		public abstract object Clone();
	}
}
