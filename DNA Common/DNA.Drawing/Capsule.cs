using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct Capsule
	{
		public float Radius;

		public LineF3D Segment;

		public Capsule(LineF3D segment, float radius)
		{
			Segment = segment;
			Radius = radius;
		}

		public bool Contains(Vector3 point)
		{
			Vector3 value = Segment.ClosetPointTo(point);
			return Vector3.DistanceSquared(value, point) < Radius * Radius;
		}
	}
}
