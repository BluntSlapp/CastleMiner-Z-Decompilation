using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public interface IShape2D : ICloneable
	{
		RectangleF BoundingBox { get; }

		Vector2 Center { get; }

		float Area { get; }

		OrientedBoundingRect GetOrientedBoundingRect();

		void Transform(Matrix mat);

		bool Touches(IShape2D s);

		bool IsAbove(Vector2 p);

		float DistanceTo(IShape2D poly);

		float DistanceSquaredTo(IShape2D poly);

		bool IsBelow(Vector2 p);

		bool IsLeftOf(Vector2 p);

		bool IsRightOf(Vector2 p);

		bool CompletelyContains(IShape2D region);

		bool Contains(Vector2 p);

		int Contains(IList<Vector2> points);

		IShape2D Intersection(IShape2D s);

		bool BoundsIntersect(IShape2D shape);

		float Contains(IShape2D shape);
	}
}
