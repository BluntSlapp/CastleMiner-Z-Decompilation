using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct Ellipsoid
	{
		public Vector3 Center;

		public Vector3 Radius;

		public Vector3 ReciprocalRadius;

		public BoundingSphere GetBoundingSphere()
		{
			return new BoundingSphere(Center, Math.Max(Radius.X, Math.Max(Radius.Y, Radius.Z)));
		}

		public Ellipsoid(Vector3 center, Vector3 scale, Quaternion orientation)
		{
			Center = center;
			Radius = scale;
			ReciprocalRadius = new Vector3(1f / scale.X, 1f / scale.Y, 1f / scale.Z);
		}

		public Vector3 TransformWorldSpacePointToUnitSphereSpace(Vector3 point)
		{
			return Vector3.Multiply(point - Center, ReciprocalRadius);
		}

		public Vector3 TransformUnitSphereSpacePointToWorldSpace(Vector3 point)
		{
			return Vector3.Multiply(point, Radius) + Center;
		}

		public Vector3 TransformWorldSpaceVectorToUnitSphereSpace(Vector3 vector)
		{
			Vector3 result = Vector3.Multiply(vector, Radius);
			result.Normalize();
			return result;
		}

		public Vector3 TransformUnitSphereSpaceVectorToWorldSpace(Vector3 vector)
		{
			Vector3 result = Vector3.Multiply(vector, ReciprocalRadius);
			result.Normalize();
			return result;
		}

		public Plane TransformWorldSpacePlaneToUnitSphereSpace(Plane plane)
		{
			Plane result = default(Plane);
			result.D = plane.D + Vector3.Dot(plane.Normal, Center);
			result.Normal = Vector3.Multiply(plane.Normal, Radius);
			float num = 1f / result.Normal.Length();
			result.Normal *= num;
			result.D *= num;
			return result;
		}

		public Triangle3D TransformWorldSpaceTriToUnitSphereSpace(Triangle3D tri, ref Triangle3D result)
		{
			result.A = TransformWorldSpacePointToUnitSphereSpace(tri.A);
			result.B = TransformWorldSpacePointToUnitSphereSpace(tri.B);
			result.C = TransformWorldSpacePointToUnitSphereSpace(tri.C);
			return result;
		}

		public float CalculateWorldSpacePenetration(Vector3 point)
		{
			Vector3 value = TransformUnitSphereSpacePointToWorldSpace(Vector3.Normalize(point));
			Vector3 value2 = TransformUnitSphereSpacePointToWorldSpace(point);
			return Vector3.Distance(value, value2);
		}
	}
}
