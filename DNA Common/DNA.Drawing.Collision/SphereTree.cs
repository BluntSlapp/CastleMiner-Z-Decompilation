using System;
using System.Collections.Generic;
using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Collision
{
	public class SphereTree : CollisionMap
	{
		public class SphereTreeReader : ContentTypeReader<SphereTree>
		{
			protected override SphereTree Read(ContentReader input, SphereTree existingInstance)
			{
				if (existingInstance != null)
				{
					existingInstance.Load((BinaryReader)(object)input);
					return existingInstance;
				}
				SphereTree sphereTree = new SphereTree();
				sphereTree.Load((BinaryReader)(object)input);
				return sphereTree;
			}
		}

		public class SphereTreeNode
		{
			public BoundingSphere Sphere;

			public SphereTreeNode[] Children = new SphereTreeNode[8];

			public List<Triangle3D> Triangles = new List<Triangle3D>();

			public static int RayTriangleTests;

			public bool IsLeaf
			{
				get
				{
					for (int i = 0; i < Children.Length; i++)
					{
						if (Children[i] != null)
						{
							return false;
						}
					}
					return true;
				}
			}

			public SphereTreeNode()
			{
			}

			public static SphereTreeNode Load(BinaryReader reader)
			{
				Vector3 center = reader.ReadVector3();
				float radius = reader.ReadSingle();
				SphereTreeNode sphereTreeNode = new SphereTreeNode(new BoundingSphere(center, radius));
				int num = reader.ReadInt32();
				sphereTreeNode.Triangles = new List<Triangle3D>(num);
				for (int i = 0; i < num; i++)
				{
					sphereTreeNode.Triangles.Add(Triangle3D.Read(reader));
				}
				byte b = reader.ReadByte();
				for (int j = 0; j < 8; j++)
				{
					if ((b & (1 << j)) != 0)
					{
						sphereTreeNode.Children[j] = Load(reader);
					}
				}
				return sphereTreeNode;
			}

			public void Save(BinaryWriter writer)
			{
				writer.Write(Sphere.Center);
				writer.Write(Sphere.Radius);
				writer.Write(Triangles.Count);
				foreach (Triangle3D triangle in Triangles)
				{
					triangle.Write(writer);
				}
				byte b = 0;
				for (int i = 0; i < 8; i++)
				{
					if (Children[i] != null)
					{
						b = (byte)(b | (byte)(1 << i));
					}
				}
				writer.Write(b);
				for (int j = 0; j < 8; j++)
				{
					if (Children[j] != null)
					{
						Children[j].Save(writer);
					}
				}
			}

			public SphereTreeNode(BoundingSphere sphere, IList<Triangle3D> tris)
			{
				Sphere = sphere;
				Triangles.AddRange(tris);
			}

			public SphereTreeNode(BoundingSphere sphere, Triangle3D tri)
			{
				Sphere = sphere;
				Triangles.Add(tri);
			}

			public SphereTreeNode(BoundingSphere sphere)
			{
				Sphere = sphere;
			}

			public void FindCollisions(BoundingSphere sphere, List<ContactPoint> contacts)
			{
				if (!Sphere.Intersects(sphere))
				{
					return;
				}
				for (int i = 0; i < Children.Length; i++)
				{
					if (Children[i] != null)
					{
						Children[i].FindCollisions(sphere, contacts);
					}
				}
				for (int j = 0; j < Triangles.Count; j++)
				{
					Triangle3D tri = Triangles[j];
					Plane plane = tri.GetPlane();
					Vector3 normal = tri.Normal;
					float num = Math.Abs(plane.DotCoordinate(sphere.Center));
					Vector3 vector = sphere.Center - tri.A;
					Vector3.Dot(normal, vector);
					Vector3 vector2 = num * normal;
					Vector3 vector3 = sphere.Center + vector2;
					Vector3 vector4 = tri.B - tri.A;
					Vector3 vector5 = tri.C - tri.A;
					float num2 = Vector3.Dot(vector4, vector4);
					float num3 = Vector3.Dot(vector4, vector5);
					float num4 = Vector3.Dot(vector5, vector5);
					Vector3 vector6 = vector3 - tri.A;
					float num5 = Vector3.Dot(vector6, vector4);
					float num6 = Vector3.Dot(vector6, vector5);
					float num7 = num3 * num3 - num2 * num4;
					float num8 = (num3 * num6 - num4 * num5) / num7;
					if ((double)num8 >= 0.0 && (double)num8 <= 1.0)
					{
						float num9 = (num3 * num5 - num2 * num6) / num7;
						if ((double)num9 >= 0.0 && (double)(num8 + num9) <= 1.0 && num <= sphere.Radius)
						{
							Vector3 dir = (sphere.Radius - num) * normal;
							contacts.Add(new ContactPoint(dir, tri));
						}
					}
				}
			}

			public void FindCollisions(BoundingSphere sphere, Ellipsoid ellipsoid, List<ContactPoint> contacts)
			{
				if (!Sphere.Intersects(sphere))
				{
					return;
				}
				for (int i = 0; i < Children.Length; i++)
				{
					if (Children[i] != null)
					{
						Children[i].FindCollisions(sphere, ellipsoid, contacts);
					}
				}
				Triangle3D result = default(Triangle3D);
				for (int j = 0; j < Triangles.Count; j++)
				{
					Plane plane = Triangles[j].GetPlane();
					ellipsoid.TransformWorldSpaceTriToUnitSphereSpace(Triangles[j], ref result);
					Plane plane2 = ellipsoid.TransformWorldSpacePlaneToUnitSphereSpace(plane);
					float d = plane2.D;
					if (!(d <= 1f))
					{
						continue;
					}
					Vector3.Dot(plane2.Normal, result.A);
					Vector3 vector = (0f - d) * plane2.Normal;
					Vector3 vector2 = result.B - result.A;
					Vector3 vector3 = result.C - result.A;
					float num = Vector3.Dot(vector2, vector2);
					float num2 = Vector3.Dot(vector2, vector3);
					float num3 = Vector3.Dot(vector3, vector3);
					Vector3 vector4 = vector - result.A;
					float num4 = Vector3.Dot(vector4, vector2);
					float num5 = Vector3.Dot(vector4, vector3);
					float num6 = num2 * num2 - num * num3;
					float num7 = (num2 * num5 - num3 * num4) / num6;
					if ((double)num7 >= 0.0 && (double)num7 <= 1.0)
					{
						float num8 = (num2 * num4 - num * num5) / num6;
						if ((double)num8 >= 0.0 && (double)(num7 + num8) <= 1.0)
						{
							contacts.Add(new ContactPoint(plane2.Normal, Triangles[j]));
							break;
						}
					}
					Vector3 value = Vector3.Zero;
					LineF3D lineF3D = new LineF3D(result.A, result.B);
					LineF3D lineF3D2 = new LineF3D(result.A, result.C);
					LineF3D lineF3D3 = new LineF3D(result.B, result.C);
					Vector3 vector5 = lineF3D.ClosetPointTo(Vector3.Zero);
					Vector3 vector6 = lineF3D2.ClosetPointTo(Vector3.Zero);
					Vector3 vector7 = lineF3D3.ClosetPointTo(Vector3.Zero);
					float num9 = vector5.LengthSquared();
					float num10 = vector6.LengthSquared();
					float num11 = vector7.LengthSquared();
					bool flag = false;
					float num12 = 1f;
					if (num9 <= num12)
					{
						num12 = num9;
						value = vector5;
						flag = true;
					}
					if (num10 <= num12)
					{
						num12 = num10;
						value = vector6;
						flag = true;
					}
					if (num11 <= num12)
					{
						num12 = num11;
						value = vector7;
						flag = true;
					}
					if (flag)
					{
						value = ellipsoid.TransformUnitSphereSpaceVectorToWorldSpace(Vector3.Negate(value));
						contacts.Add(new ContactPoint(value, Triangles[j]));
					}
				}
			}

			public static bool RaySphereIntersection(BoundingSphere sphere, Ray ray, out float t1out, out float t2out)
			{
				Vector3 vector = ray.Position - sphere.Center;
				float num = Vector3.Dot(vector, vector) - sphere.Radius * sphere.Radius;
				float num2 = Vector3.Dot(ray.Direction, vector);
				t1out = float.MinValue;
				t2out = float.MaxValue;
				float num3;
				if (num <= 0f)
				{
					num3 = num2 * num2 - num;
					float num4 = (float)Math.Sqrt(num3);
					t1out = float.MinValue;
					t2out = 0f - num2 + num4;
					return true;
				}
				if (num2 >= 0f)
				{
					return false;
				}
				num3 = num2 * num2 - num;
				if (num3 < 0f)
				{
					return false;
				}
				if (num3 > 0f)
				{
					float num4 = (float)Math.Sqrt(num3);
					t1out = 0f - num2 - num4;
					t2out = 0f - num2 + num4;
					if (t1out > t2out)
					{
						float num5 = t1out;
						t1out = t2out;
						t2out = num5;
					}
					return true;
				}
				t2out = (t1out = 0f - num2);
				return true;
			}

			public RayQueryResult? CollidesWith(Ray ray, float minT, float maxT)
			{
				float t1out;
				float t2out;
				if (!RaySphereIntersection(Sphere, ray, out t1out, out t2out))
				{
					return null;
				}
				if (!(t1out < 0f) && t1out > maxT)
				{
					return null;
				}
				RayQueryResult? result = null;
				for (int i = 0; i < Children.Length; i++)
				{
					if (Children[i] != null)
					{
						RayQueryResult? rayQueryResult = Children[i].CollidesWith(ray, minT, maxT);
						if (rayQueryResult.HasValue && rayQueryResult.Value.T < maxT)
						{
							maxT = rayQueryResult.Value.T;
							result = rayQueryResult;
						}
					}
				}
				for (int j = 0; j < Triangles.Count; j++)
				{
					RayTriangleTests++;
					float? num = Triangles[j].Intersects(ray);
					if (num.HasValue)
					{
						float value = num.Value;
						if (value >= minT && value < maxT)
						{
							maxT = value;
							result = new RayQueryResult(value, Triangles[j]);
						}
					}
				}
				return result;
			}

			public void RefineBoundingSphere()
			{
				List<Vector3> list = new List<Vector3>();
				GetPoints(list);
				BoundingSphere boundingSphere = (Sphere = BoundingSphere.CreateFromPoints((IEnumerable<Vector3>)list));
			}

			public void GetPoints(List<Vector3> points)
			{
				foreach (Triangle3D triangle in Triangles)
				{
					points.Add(triangle.A);
					points.Add(triangle.B);
					points.Add(triangle.C);
				}
				SphereTreeNode[] children = Children;
				foreach (SphereTreeNode sphereTreeNode in children)
				{
					if (sphereTreeNode != null)
					{
						sphereTreeNode.GetPoints(points);
					}
				}
			}

			public void GetTriangles(List<Triangle3D> tris)
			{
				tris.AddRange(Triangles);
				SphereTreeNode[] children = Children;
				foreach (SphereTreeNode sphereTreeNode in children)
				{
					if (sphereTreeNode != null)
					{
						sphereTreeNode.GetTriangles(tris);
					}
				}
			}
		}

		private const float mergeBias = 0.5f;

		private const int GridSize = 128;

		private const int Version = 1;

		public SphereTreeNode _root;

		private readonly string FileID = "TASSPT";

		public SphereTree(Model model)
		{
			List<Triangle3D> tris = model.ExtractModelTris(false);
			Build(tris);
		}

		public SphereTree()
		{
			_root = new SphereTreeNode();
		}

		public void Build(IList<Triangle3D> tris)
		{
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < tris.Count; i++)
			{
				list.Add(tris[i].A);
				list.Add(tris[i].B);
				list.Add(tris[i].C);
			}
			BoundingBox boundingBox = BoundingBox.CreateFromPoints((IEnumerable<Vector3>)list);
			Vector3 vector = boundingBox.Max - boundingBox.Min;
			float num = Math.Max(vector.X, Math.Max(vector.Y, vector.Z));
			Vector3 vector2 = boundingBox.Min + vector / 2f;
			Vector3 vector3 = new Vector3(num, num, num);
			new BoundingBox(vector2 - vector3 / 2f, vector2 + vector3 / 2f);
			float num2 = num / 128f;
			int num3 = 128;
			SphereTreeNode[,,] array = new SphereTreeNode[num3, num3, num3];
			List<SphereTreeNode> list2 = new List<SphereTreeNode>();
			foreach (Triangle3D tri in tris)
			{
				Vector3 vector4 = tri.Centroid - boundingBox.Min;
				int num4 = (int)(vector4.X / num2);
				int num5 = (int)(vector4.Y / num2);
				int num6 = (int)(vector4.Z / num2);
				if (array[num4, num5, num6] == null)
				{
					list2.Add(array[num4, num5, num6] = new SphereTreeNode());
				}
				array[num4, num5, num6].Triangles.Add(tri);
			}
			foreach (SphereTreeNode item in list2)
			{
				item.RefineBoundingSphere();
			}
			SphereTreeNode[,,] array2;
			do
			{
				num3 /= 2;
				array2 = new SphereTreeNode[num3, num3, num3];
				list2.Clear();
				for (int j = 0; j < num3; j++)
				{
					for (int k = 0; k < num3; k++)
					{
						for (int l = 0; l < num3; l++)
						{
							SphereTreeNode sphereTreeNode = new SphereTreeNode();
							bool flag = true;
							for (int m = 0; m < 8; m++)
							{
								sphereTreeNode.Children[m] = array[j * 2 + ((m & 4) >> 2), k * 2 + ((m & 2) >> 1), l * 2 + (m & 1)];
								if (sphereTreeNode.Children[m] != null)
								{
									flag = false;
								}
							}
							if (!flag)
							{
								array2[j, k, l] = sphereTreeNode;
								list2.Add(sphereTreeNode);
							}
						}
					}
				}
				foreach (SphereTreeNode item2 in list2)
				{
					item2.RefineBoundingSphere();
				}
				array = array2;
			}
			while (num3 > 1);
			_root = array2[0, 0, 0];
		}

		public override void FindCollisions(Ellipsoid ellipsoid, List<ContactPoint> contacts)
		{
			contacts.Clear();
			BoundingSphere boundingSphere = ellipsoid.GetBoundingSphere();
			_root.FindCollisions(boundingSphere, ellipsoid, contacts);
		}

		public override void FindCollisions(BoundingSphere sphere, List<ContactPoint> contacts)
		{
			contacts.Clear();
			_root.FindCollisions(sphere, contacts);
		}

		public override void GetTriangles(List<Triangle3D> tris)
		{
			tris.Clear();
			_root.GetTriangles(tris);
		}

		public override void Save(BinaryWriter writer)
		{
			writer.Write(FileID);
			writer.Write(1);
			_root.Save(writer);
		}

		public override void Load(BinaryReader reader)
		{
			string text = reader.ReadString();
			if (text != FileID)
			{
				throw new Exception("Bad SPT file Format");
			}
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad SPT version");
			}
			_root = SphereTreeNode.Load(reader);
		}

		protected override RayQueryResult? CollidesWith(Ray ray, float min, float max)
		{
			return _root.CollidesWith(ray, min, max);
		}
	}
}
