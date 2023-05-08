using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	[Serializable]
	public class Polygon
	{
		private Vector2[] _points;

		public Vector2[] Points
		{
			get
			{
				return _points;
			}
		}

		public RectangleF Extents
		{
			get
			{
				return DrawingTools.GetBoundingRect(_points);
			}
		}

		public Polygon(Vector2[] points)
		{
			_points = points;
		}

		public bool Contains(Vector2 point)
		{
			bool flag = false;
			int i = 0;
			int num = Points.Length - 1;
			for (; i < Points.Length; i++)
			{
				float x = Points[i].X;
				float y = Points[i].Y;
				float x2 = Points[num].X;
				float y2 = Points[num].Y;
				if (((y <= point.Y && point.Y < y2) || (y2 <= point.Y && point.Y < y)) && point.X < (x2 - x) * (point.Y - y) / (y2 - y) + x)
				{
					flag = !flag;
				}
				num = i;
			}
			return flag;
		}
	}
}
