using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Drawing2D
{
	public interface IPointShape2D : IShape2D, ICloneable
	{
		IList<Vector2> Points { get; }

		IList<LineF2D> GetLineSegments();

		float PointsInside(IShape2D shape);
	}
}
