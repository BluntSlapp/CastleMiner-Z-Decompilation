using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Collision
{
	public abstract class CollisionMap
	{
		public struct ContactPoint
		{
			public Vector3 PenetrationDirection;

			public Triangle3D Triangle;

			public float PenetrationDepth;

			public ContactPoint(Vector3 dir, Triangle3D tri)
			{
				PenetrationDirection = dir;
				Triangle = tri;
				PenetrationDepth = 0f;
			}

			public ContactPoint(Vector3 dir, Triangle3D tri, float p)
			{
				PenetrationDirection = dir;
				Triangle = tri;
				PenetrationDepth = p;
			}
		}

		public struct RayQueryResult
		{
			public Triangle3D Triangle;

			public float T;

			public RayQueryResult(float t, Triangle3D tri)
			{
				T = t;
				Triangle = tri;
			}
		}

		public float? CollidesWith(LineF3D line)
		{
			Triangle3D triangle;
			return CollidesWith(line, out triangle);
		}

		public float? CollidesWith(Ray ray)
		{
			Triangle3D triangle;
			return CollidesWith(ray, out triangle);
		}

		public float? CollidesWith(Ray ray, out Triangle3D triangle)
		{
			RayQueryResult? rayQueryResult = CollidesWith(ray, 0f, float.MaxValue);
			if (!rayQueryResult.HasValue)
			{
				triangle = default(Triangle3D);
				return null;
			}
			triangle = rayQueryResult.Value.Triangle;
			return rayQueryResult.Value.T;
		}

		public float? CollidesWith(LineF3D line, out Triangle3D triangle)
		{
			Ray ray = new Ray(line.Start, line.End - line.Start);
			RayQueryResult? rayQueryResult = CollidesWith(ray, 0f, 1f);
			if (!rayQueryResult.HasValue)
			{
				triangle = default(Triangle3D);
				return null;
			}
			triangle = rayQueryResult.Value.Triangle;
			return rayQueryResult.Value.T;
		}

		protected abstract RayQueryResult? CollidesWith(Ray ray, float min, float max);

		public abstract void GetTriangles(List<Triangle3D> tris);

		public abstract void FindCollisions(Ellipsoid ellipsoid, List<ContactPoint> contacts);

		public abstract void FindCollisions(BoundingSphere sphere, List<ContactPoint> contacts);

		public abstract void Load(BinaryReader reader);

		public abstract void Save(BinaryWriter reader);
	}
}
