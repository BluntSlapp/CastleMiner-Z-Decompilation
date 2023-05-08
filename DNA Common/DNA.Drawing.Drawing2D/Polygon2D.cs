using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	[Serializable]
	public class Polygon2D : Shape2D, IPointShape2D, IShape2D, ICloneable
	{
		public struct IntersectionData
		{
			public float TargetT;

			public LineF2D Line;

			public bool Coincident;

			public IntersectionData(float t, LineF2D line, bool coincident)
			{
				TargetT = t;
				Line = line;
				Coincident = coincident;
			}
		}

		private Vector2[] _points = new Vector2[0];

		private RectangleF _boundingBox;

		private Vector2 _center;

		private float _area;

		public IList<Vector2> Points
		{
			get
			{
				return _points;
			}
		}

		public override RectangleF BoundingBox
		{
			get
			{
				return _boundingBox;
			}
		}

		public override Vector2 Center
		{
			get
			{
				return _center;
			}
		}

		public override float Area
		{
			get
			{
				return _area;
			}
		}

		public override object Clone()
		{
			return new Polygon2D(_points);
		}

		public override OrientedBoundingRect GetOrientedBoundingRect()
		{
			return OrientedBoundingRect.FromPoints(_points);
		}

		public IList<LineF2D> GetLineSegments()
		{
			LineF2D[] array = new LineF2D[_points.Length];
			for (int i = 0; i < _points.Length - 1; i++)
			{
				array[i] = new LineF2D(_points[i], _points[i + 1]);
			}
			array[_points.Length - 1] = new LineF2D(_points[_points.Length - 1], _points[0]);
			return array;
		}

		public override void Transform(Matrix mat)
		{
			for (int i = 0; i < _points.Length; i++)
			{
				_points[i] = Vector2.Transform(_points[i], mat);
			}
			ComputeBoundingBox();
		}

		public Polygon2D(IList<Vector2> points)
		{
			int num = points.Count;
			if (points[0] != points[points.Count - 1])
			{
				num++;
			}
			_points = new Vector2[num];
			for (int i = 0; i < points.Count; i++)
			{
				Vector2 vector = points[i];
				if (_points.Length == 0 || _points[_points.Length - 1] != vector)
				{
					_points[i] = vector;
				}
			}
			if (points[0] != points[points.Count - 1])
			{
				_points[points.Count] = points[0];
			}
			ComputeBoundingBox();
			ComputeArea();
		}

		public Rectangle ComputeTransformedBoundingBox(Matrix mat)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			for (int i = 0; i < _points.Length; i++)
			{
				Vector2 vector = _points[i];
				Vector3 vector2 = Vector3.Transform(new Vector3(vector.X, vector.Y, 0f), mat);
				num = Math.Min(num, vector2.X);
				num2 = Math.Min(num2, vector2.Y);
				num3 = Math.Max(num3, vector2.X);
				num4 = Math.Max(num4, vector2.Y);
			}
			return new Rectangle((int)num, (int)num2, (int)(num3 - num), (int)(num4 - num2));
		}

		private void ComputeBoundingBox()
		{
			float num = 2.14748365E+09f;
			float num2 = 2.14748365E+09f;
			float num3 = -2.14748365E+09f;
			float num4 = -2.14748365E+09f;
			for (int i = 0; i < _points.Length; i++)
			{
				Vector2 vector = _points[i];
				num = Math.Min(num, vector.X);
				num2 = Math.Min(num2, vector.Y);
				num3 = Math.Max(num3, vector.X);
				num4 = Math.Max(num4, vector.Y);
			}
			_boundingBox = new RectangleF(num, num2, num3 - num + 1f, num4 - num2 + 1f);
			_center = new Vector2(_boundingBox.Left + _boundingBox.Width / 2f, _boundingBox.Top + _boundingBox.Height / 2f);
		}

		private void ComputeArea()
		{
			_area = Math.Abs(GetPolygonArea(_points));
			_area += _points.Length;
		}

		public bool PortionBelow(Vector2 p)
		{
			return !PortionAbove(p);
		}

		public bool PortionAbove(Vector2 p)
		{
			for (int i = 0; i < _points.Length; i++)
			{
				if (_points[i].Y < p.Y)
				{
					return true;
				}
			}
			return false;
		}

		public float PointsInside(IShape2D region)
		{
			if (region == this)
			{
				return 0f;
			}
			if (!BoundsIntersect(region))
			{
				return 0f;
			}
			int num = 0;
			for (int i = 0; i < _points.Length; i++)
			{
				if (region.Contains(_points[i]))
				{
					num++;
				}
			}
			return (float)num / (float)_points.Length;
		}

		private bool Touches(Polygon2D s)
		{
			return false;
		}

		public override bool Touches(IShape2D s)
		{
			if (BoundsIntersect(s))
			{
				for (int i = 0; i < _points.Length; i++)
				{
					Vector2 p = _points[i];
					if (s.Contains(p))
					{
						return true;
					}
				}
				if (!(s is IPointShape2D))
				{
					throw new NotImplementedException();
				}
				IPointShape2D pointShape2D = (IPointShape2D)s;
				IList<Vector2> points = pointShape2D.Points;
				for (int j = 0; j < points.Count; j++)
				{
					Vector2 p2 = points[j];
					if (Contains(p2))
					{
						return true;
					}
				}
				IList<LineF2D> lineSegments = GetLineSegments();
				IList<LineF2D> lineSegments2 = pointShape2D.GetLineSegments();
				for (int k = 0; k < lineSegments.Count; k++)
				{
					for (int l = 0; l < lineSegments2.Count; l++)
					{
						if (lineSegments[k].Intersects(lineSegments2[l]))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public override bool IsAbove(Vector2 p)
		{
			if (p.X < _boundingBox.Left || p.X > _boundingBox.Right || p.Y > _boundingBox.Bottom)
			{
				return false;
			}
			Vector2 vector = _points[0];
			float num = p.X + 0.5f;
			for (int i = 1; i < _points.Length; i++)
			{
				Vector2 vector2 = _points[i];
				float num2 = vector2.X - num;
				float num3 = vector.X - num;
				if ((num2 > 0f && num3 < 0f) || (num2 < 0f && num3 > 0f))
				{
					float num4 = (num - vector.X) / (vector2.X - vector.X);
					float num5 = vector.Y + num4 * (vector2.Y - vector.Y);
					if (num5 < p.Y)
					{
						return false;
					}
				}
				vector = vector2;
			}
			return true;
		}

		public bool LowestParametricIntersection(LineF2D targetLine, out float lowestT)
		{
			lowestT = float.MaxValue;
			for (int i = 0; i < _points.Length - 1; i++)
			{
				float t;
				bool coincident;
				if (new LineF2D(_points[i], _points[i + 1]).Intersects(targetLine, out t, out coincident) && !coincident && lowestT > t)
				{
					lowestT = t;
				}
			}
			return lowestT != float.MaxValue;
		}

		public void ParametricIntersections(LineF2D targetLine, List<IntersectionData> intersections)
		{
			for (int i = 0; i < _points.Length - 1; i++)
			{
				LineF2D line = new LineF2D(_points[i], _points[i + 1]);
				float t;
				bool coincident;
				if (line.Intersects(targetLine, out t, out coincident) && !coincident)
				{
					intersections.Add(new IntersectionData(t, line, coincident));
				}
			}
		}

		public IList<float> ParametricIntersections(LineF2D targetLine)
		{
			List<float> list = new List<float>();
			for (int i = 0; i < _points.Length - 1; i++)
			{
				float t;
				bool coincident;
				if (new LineF2D(_points[i], _points[i + 1]).Intersects(targetLine, out t, out coincident) && !coincident)
				{
					list.Add(t);
				}
			}
			return list;
		}

		public IList<Vector2> Intersections(LineF2D targetLine)
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < _points.Length - 1; i++)
			{
				Vector2 intersection;
				bool coincident;
				if (new LineF2D(_points[i], _points[i + 1]).Intersects(targetLine, out intersection, out coincident) && !coincident)
				{
					list.Add(intersection);
				}
			}
			return list;
		}

		public Vector2 ShortestVectorTo(Vector2 point)
		{
			Vector2 result = new LineF2D(_points[0], _points[1]).ShortestVectorTo(point);
			float num = result.LengthSquared();
			Vector2 start = _points[1];
			for (int i = 2; i < _points.Length; i++)
			{
				Vector2 vector = new LineF2D(start, _points[i]).ShortestVectorTo(point);
				float num2 = vector.LengthSquared();
				if (num2 < num)
				{
					num = num2;
					result = vector;
				}
				start = _points[i];
			}
			return result;
		}

		public override float DistanceSquaredTo(IShape2D shape)
		{
			if (shape is IPointShape2D)
			{
				IPointShape2D pointShape2D = (IPointShape2D)shape;
				float num = float.MaxValue;
				IList<Vector2> points = pointShape2D.Points;
				for (int i = 0; i < _points.Length; i++)
				{
					for (int j = 0; j < points.Count; j++)
					{
						num = Math.Min((float)_points[i].DistanceSquared(points[j]), num);
					}
				}
				return num;
			}
			throw new NotImplementedException();
		}

		public override bool IsBelow(Vector2 p)
		{
			if (p.X < _boundingBox.Left || p.X > _boundingBox.Right || p.Y < _boundingBox.Top)
			{
				return false;
			}
			Vector2 vector = _points[0];
			float num = p.X + 0.5f;
			for (int i = 1; i < _points.Length; i++)
			{
				Vector2 vector2 = _points[i];
				float num2 = vector2.X - num;
				float num3 = vector.X - num;
				if ((num2 > 0f && num3 < 0f) || (num2 < 0f && num3 > 0f))
				{
					float num4 = (num - vector.X) / (vector2.X - vector.X);
					float num5 = vector.Y + num4 * (vector2.Y - vector.Y);
					if (num5 > p.Y)
					{
						return false;
					}
				}
				vector = vector2;
			}
			return true;
		}

		public override bool IsLeftOf(Vector2 p)
		{
			if (p.Y < _boundingBox.Top || p.Y > _boundingBox.Bottom || p.X < _boundingBox.Left)
			{
				return false;
			}
			Vector2 vector = _points[0];
			float num = p.Y + 0.5f;
			for (int i = 1; i < _points.Length; i++)
			{
				Vector2 vector2 = _points[i];
				float num2 = vector2.Y - num;
				float num3 = vector.Y - num;
				if ((num2 > 0f && num3 < 0f) || (num2 < 0f && num3 > 0f))
				{
					float num4 = (num - vector.Y) / (vector2.Y - vector.Y);
					float num5 = vector.X + num4 * (vector2.X - vector.X);
					if (num5 < p.X)
					{
						return false;
					}
				}
				vector = vector2;
			}
			return true;
		}

		public override bool IsRightOf(Vector2 p)
		{
			if (p.Y < _boundingBox.Top || p.Y > _boundingBox.Bottom || p.X > _boundingBox.Right)
			{
				return false;
			}
			Vector2 vector = _points[0];
			float num = p.Y + 0.5f;
			for (int i = 1; i < _points.Length; i++)
			{
				Vector2 vector2 = _points[i];
				float num2 = vector2.Y - num;
				float num3 = vector.Y - num;
				if ((num2 > 0f && num3 < 0f) || (num2 < 0f && num3 > 0f))
				{
					float num4 = (num - vector.Y) / (vector2.Y - vector.Y);
					float num5 = vector.X + num4 * (vector2.X - vector.X);
					if (num5 > p.X)
					{
						return false;
					}
				}
				vector = vector2;
			}
			return true;
		}

		public override bool CompletelyContains(IShape2D shape)
		{
			if (shape is IPointShape2D)
			{
				IPointShape2D pointShape2D = (IPointShape2D)shape;
				if (pointShape2D.PointsInside(this) == 1f)
				{
					return true;
				}
				return false;
			}
			throw new NotImplementedException();
		}

		public override bool Contains(Vector2 p)
		{
			if (!BoundingBox.Contains(p))
			{
				return false;
			}
			Vector2 vector = _points[0];
			float y = p.Y;
			int num = 0;
			for (int i = 1; i < _points.Length; i++)
			{
				Vector2 vector2 = _points[i];
				float num2 = vector2.Y - y;
				float num3 = vector.Y - y;
				if ((num2 >= 0f && num3 < 0f) || (num2 < 0f && num3 >= 0f))
				{
					float num4 = (y - vector.Y) / (vector2.Y - vector.Y);
					float num5 = vector.X + num4 * (vector2.X - vector.X);
					if (num5 < p.X)
					{
						num++;
					}
				}
				vector = vector2;
			}
			return (num & 1) != 0;
		}

		public override IShape2D Intersection(IShape2D s)
		{
			throw new NotImplementedException();
		}

		public bool BoundsIntersect(Polygon2D shape)
		{
			if (!(_boundingBox.Left >= shape._boundingBox.Right) && !(_boundingBox.Right <= shape._boundingBox.Left) && !(_boundingBox.Top >= shape._boundingBox.Bottom))
			{
				return !(_boundingBox.Bottom <= shape._boundingBox.Top);
			}
			return false;
		}

		public override float Contains(IShape2D shape)
		{
			if (shape == this)
			{
				return 0f;
			}
			float area = shape.Area;
			if (shape.Area == 0f)
			{
				return 0f;
			}
			if (!BoundsIntersect(shape))
			{
				return 0f;
			}
			RectangleF rectangleF = RectangleF.Intersect(_boundingBox, shape.BoundingBox);
			float num = rectangleF.Width * rectangleF.Height;
			return num / area;
		}

		private static float GetPolygonArea(IList<Vector2> points)
		{
			float num = 0f;
			Vector2 vector = points[0];
			for (int i = 0; i < points.Count; i++)
			{
				Vector2 vector2 = points[i];
				num += (vector.X + vector2.X) * (vector.Y - vector2.Y);
				vector = vector2;
			}
			return num * 0.5f;
		}

		private static float GetTriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			return (p1.X + p2.X) * (p1.Y - p2.Y) + (p2.X + p3.X) * (p2.Y - p3.Y) + (p3.X + p1.X) * (p3.Y - p1.Y);
		}

		private static bool IsEar(IList<Vector2> points, LinkedListNode<int> currentNode)
		{
			int value = currentNode.Value;
			int num = ((currentNode.Previous != null) ? currentNode.Previous.Value : currentNode.List.Last.Value);
			int num2 = ((currentNode.Next != null) ? currentNode.Next.Value : currentNode.List.First.Value);
			Vector2 vector = points[num];
			Vector2 vector2 = points[value];
			Vector2 vector3 = points[num2];
			if (GetTriangleArea(vector, vector2, vector3) < 0f)
			{
				foreach (int item in currentNode.List)
				{
					if (item != num && item != num2 && item != value && DrawingTools.PointInTriangle(vector, vector2, vector3, points[item]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}
