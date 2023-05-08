using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public static class PlaneTools
	{
		public static Plane Create(Vector3 point, Vector3 normal)
		{
			normal.Normalize();
			float d = 0f - (normal.X * point.X + normal.Y * point.Y + normal.Z * point.Z);
			return new Plane(normal, d);
		}

		public static PlaneIntersectionType SideOf(this Plane plane, Vector3 point)
		{
			float num = point.X * plane.Normal.X + point.Y * plane.Normal.Y + point.Z * plane.Normal.Z + plane.D;
			if (num == 0f)
			{
				return PlaneIntersectionType.Intersecting;
			}
			if (num > 0f)
			{
				return PlaneIntersectionType.Front;
			}
			return PlaneIntersectionType.Back;
		}

		public static bool Contains(this Plane plane, Vector3 point)
		{
			return plane.SideOf(point) == PlaneIntersectionType.Intersecting;
		}
	}
}
