using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Collision
{
	public class BSPTree : CollisionMap
	{
		public class BSPTreeReader : ContentTypeReader<BSPTree>
		{
			protected override BSPTree Read(ContentReader input, BSPTree existingInstance)
			{
				if (existingInstance != null)
				{
					existingInstance.Load((BinaryReader)(object)input);
					return existingInstance;
				}
				BSPTree bSPTree = new BSPTree();
				bSPTree.Load((BinaryReader)(object)input);
				return bSPTree;
			}
		}

		private class BSPTreeNode
		{
			private class SplitCounts
			{
				public int Node;

				public int Positive;

				public int Negative;

				public int Spanning;

				public int Coincident;

				public float DivisionFactor
				{
					get
					{
						if (Positive > Negative)
						{
							return (float)Negative / (float)Positive;
						}
						if (Positive < Negative)
						{
							return (float)Positive / (float)Negative;
						}
						return 1f;
					}
				}

				public SplitCounts(int node, int positive, int negative, int spanning, int coincident)
				{
					Node = node;
					Positive = positive;
					Negative = negative;
					Spanning = spanning;
					Coincident = coincident;
				}
			}

			private class BestSplitFinder
			{
				private int startPoly;

				private int endPoly;

				private float minRelation;

				public float bestRatio = float.MinValue;

				public int leastSplits = int.MaxValue;

				public SplitCounts bestPolygon;

				public bool isConvex;

				private Triangle3D[] triList;

				private Thread _thread;

				public BestSplitFinder(Triangle3D[] tlist, float minr, int sPoly, int epoly)
				{
					triList = tlist;
					minRelation = minr;
					startPoly = sPoly;
					endPoly = epoly;
				}

				public void FindSplitAsync()
				{
					if (startPoly == 0 && endPoly >= triList.Length)
					{
						FindSplit();
						return;
					}
					_thread = new Thread(FindSplit);
					_thread.Name = "Find Poly Split Worker " + startPoly + "-" + endPoly;
					_thread.Start();
				}

				public void EndFindSplit()
				{
					if (_thread != null)
					{
						_thread.Join();
					}
				}

				public void FindSplit()
				{
					for (int i = startPoly; i < endPoly; i++)
					{
						int num = 0;
						int num2 = 0;
						int num3 = 0;
						int num4 = 0;
						Plane plane = triList[i].GetPlane();
						for (int j = 0; j < triList.Length; j++)
						{
							if (i != j)
							{
								Triangle3D triangle3D = triList[j];
								int num5 = 0;
								int num6 = 0;
								float num7 = plane.DotCoordinate(triangle3D.A);
								if (num7 > 0f)
								{
									num5++;
								}
								else if (num7 < 0f)
								{
									num6++;
								}
								num7 = plane.DotCoordinate(triangle3D.B);
								if (num7 > 0f)
								{
									num5++;
								}
								else if (num7 < 0f)
								{
									num6++;
								}
								num7 = plane.DotCoordinate(triangle3D.C);
								if (num7 > 0f)
								{
									num5++;
								}
								else if (num7 < 0f)
								{
									num6++;
								}
								if (num5 > 0 && num6 == 0)
								{
									num++;
								}
								else if (num5 == 0 && num6 > 0)
								{
									num2++;
								}
								else if (num5 != 0 || num6 != 0)
								{
									num3++;
								}
								else
								{
									num4++;
								}
							}
						}
						SplitCounts splitCounts = new SplitCounts(i, num, num2, num3, num4);
						if (num2 > 0 || num3 > 0)
						{
							isConvex = false;
						}
						if (splitCounts.DivisionFactor >= minRelation && (splitCounts.Spanning < leastSplits || (splitCounts.Spanning == leastSplits && splitCounts.DivisionFactor > bestRatio)))
						{
							bestPolygon = splitCounts;
							leastSplits = splitCounts.Spanning;
							bestRatio = splitCounts.DivisionFactor;
						}
					}
				}
			}

			public BSPTreeNode Front;

			public BSPTreeNode Back;

			public Plane SplitPlane;

			public Triangle3D[] TriList;

			public static int AvailibleThread = (Environment.ProcessorCount - 1) * 15;

			public static object threadLock = new object();

			private static void ChooseDividingPolygon(LinkedList<Triangle3D> splitList)
			{
				if (splitList.Count < 3)
				{
					return;
				}
				LinkedListNode<Triangle3D>[] array = new LinkedListNode<Triangle3D>[splitList.Count];
				Triangle3D[] array2 = new Triangle3D[splitList.Count];
				LinkedListNode<Triangle3D> linkedListNode = splitList.First;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = linkedListNode.Value;
					array[i] = linkedListNode;
					linkedListNode = linkedListNode.Next;
				}
				int num = Environment.ProcessorCount;
				if (array2.Length < num * 500)
				{
					num = 1;
				}
				int num2 = array2.Length / num + 1;
				SplitCounts splitCounts = null;
				float num3 = 0.8f;
				bool flag = true;
				while (splitCounts == null)
				{
					float num4 = float.MinValue;
					int num5 = int.MaxValue;
					List<BestSplitFinder> list = new List<BestSplitFinder>();
					int num6 = 0;
					for (int j = 0; j < num; j++)
					{
						int num7 = num6 + num2;
						if (num7 > array2.Length)
						{
							num7 = array2.Length;
						}
						BestSplitFinder bestSplitFinder = new BestSplitFinder(array2, num3, num6, num7);
						bestSplitFinder.FindSplitAsync();
						list.Add(bestSplitFinder);
						num6 = num7 + 1;
					}
					foreach (BestSplitFinder item in list)
					{
						item.EndFindSplit();
						if (!item.isConvex)
						{
							flag = false;
						}
						if (item.bestPolygon != null)
						{
							SplitCounts bestPolygon = item.bestPolygon;
							if (bestPolygon.Spanning < num5 || (bestPolygon.Spanning == num5 && bestPolygon.DivisionFactor > num4))
							{
								splitCounts = bestPolygon;
								num5 = bestPolygon.Spanning;
								num4 = bestPolygon.DivisionFactor;
							}
						}
					}
					num3 /= 2f;
					if (flag)
					{
						return;
					}
				}
				splitList.Remove(array[splitCounts.Node]);
				splitList.AddFirst(array[splitCounts.Node]);
			}

			public static void SortList(Plane splitPlane, LinkedList<Triangle3D> triList, LinkedList<Triangle3D> frontList, LinkedList<Triangle3D> backList, int percisionDigits)
			{
				LinkedListNode<Triangle3D> linkedListNode = triList.First;
				while (linkedListNode != null)
				{
					int num = percisionDigits;
					bool flag = true;
					while (flag)
					{
						bool flag2 = false;
						Triangle3D value = linkedListNode.Value;
						double value2 = splitPlane.DotCoordinate(value.A);
						double value3 = splitPlane.DotCoordinate(value.B);
						double value4 = splitPlane.DotCoordinate(value.C);
						value2 = Math.Round(value2, num);
						value3 = Math.Round(value3, num);
						value4 = Math.Round(value4, num);
						if (value2 == 0.0 && value3 == 0.0 && value4 == 0.0)
						{
							if (Vector3.Dot(vector2: new Plane(value.B, value.A, value.C).Normal, vector1: splitPlane.Normal) > 0f)
							{
								frontList.AddFirst(value);
							}
							else
							{
								backList.AddLast(value);
							}
							flag2 = true;
						}
						else if (value2 <= 0.0 && value3 <= 0.0 && value4 <= 0.0)
						{
							flag2 = true;
							backList.AddLast(value);
						}
						else if (!(value2 >= 0.0) || !(value3 >= 0.0) || !(value4 >= 0.0))
						{
							try
							{
								Triangle3D[] array = value.Slice(splitPlane, num);
								if (array.Length == 1)
								{
									throw new PrecisionException();
								}
								for (int i = 0; i < array.Length; i++)
								{
									if (Math.Round(array[i].Area, percisionDigits - 1) > 0.0)
									{
										triList.AddLast(array[i]);
									}
								}
							}
							catch (PrecisionException)
							{
								num--;
								if (num < 0)
								{
									throw new Exception("Slice Error");
								}
								continue;
							}
							flag2 = true;
						}
						LinkedListNode<Triangle3D> next = linkedListNode.Next;
						if (flag2)
						{
							triList.Remove(linkedListNode);
						}
						linkedListNode = next;
						flag = false;
					}
				}
			}

			public void Build(LinkedList<Triangle3D> splitList, int percisionDigits)
			{
				LinkedList<Triangle3D> linkedList = new LinkedList<Triangle3D>();
				LinkedList<Triangle3D> linkedList2 = new LinkedList<Triangle3D>();
				ChooseDividingPolygon(splitList);
				linkedList.AddFirst(splitList.First.Value);
				Triangle3D value = splitList.First.Value;
				splitList.RemoveFirst();
				SplitPlane = new Plane(value.B, value.A, value.C);
				SplitPlane.Normalize();
				SortList(SplitPlane, splitList, linkedList, linkedList2, percisionDigits);
				TriList = new Triangle3D[linkedList.Count];
				linkedList.CopyTo(TriList, 0);
				Thread thread = null;
				bool flag = false;
				bool flag2 = false;
				if (splitList.Count > 0)
				{
					Front = new BSPTreeNode();
					flag = true;
				}
				if (linkedList2.Count > 0)
				{
					Back = new BSPTreeNode();
					flag2 = true;
				}
				if (flag)
				{
					Front.Build(splitList, percisionDigits);
				}
				if (flag2)
				{
					Back.Build(linkedList2, percisionDigits);
				}
				if (thread != null)
				{
					thread.Join();
					lock (threadLock)
					{
						AvailibleThread++;
					}
				}
			}

			public static BSPTreeNode Load(BinaryReader reader)
			{
				int num = reader.ReadInt32();
				if (num == 0)
				{
					return null;
				}
				BSPTreeNode bSPTreeNode = new BSPTreeNode();
				bSPTreeNode.TriList = new Triangle3D[num];
				for (int i = 0; i < num; i++)
				{
					bSPTreeNode.TriList[i] = Triangle3D.Read(reader);
				}
				Triangle3D triangle3D = bSPTreeNode.TriList[bSPTreeNode.TriList.Length - 1];
				bSPTreeNode.SplitPlane = new Plane(triangle3D.B, triangle3D.A, triangle3D.C);
				bSPTreeNode.SplitPlane.Normalize();
				bSPTreeNode.Front = Load(reader);
				bSPTreeNode.Back = Load(reader);
				return bSPTreeNode;
			}

			public void Save(BinaryWriter writer)
			{
				if (TriList == null)
				{
					writer.Write(0);
					return;
				}
				writer.Write(TriList.Length);
				Triangle3D[] triList = TriList;
				foreach (Triangle3D triangle3D in triList)
				{
					triangle3D.Write(writer);
				}
				if (Front == null)
				{
					writer.Write(0);
				}
				else
				{
					Front.Save(writer);
				}
				if (Back == null)
				{
					writer.Write(0);
				}
				else
				{
					Back.Save(writer);
				}
			}

			private Triangle3D? CollideWithTriangles(Ray ray)
			{
				for (int i = 0; i < TriList.Length; i++)
				{
					if (TriList[i].Intersects(ray).HasValue)
					{
						return TriList[i];
					}
				}
				return null;
			}

			public RayQueryResult? CollidesWith(Ray ray, float minT, float maxT, int percisionDigits)
			{
				Vector3 value = ray.Position + ray.Direction * minT;
				float num = SplitPlane.DotCoordinate(value);
				float? num2 = ray.Intersects(SplitPlane);
				if (num2.HasValue)
				{
					num2 = (float)Math.Round(num2.Value, percisionDigits);
				}
				if (!num2.HasValue || num2.Value < minT || num2.Value > maxT)
				{
					if (num >= 0f)
					{
						if (Front == null)
						{
							return null;
						}
						return Front.CollidesWith(ray, minT, maxT, percisionDigits);
					}
					if (Back == null)
					{
						return null;
					}
					return Back.CollidesWith(ray, minT, maxT, percisionDigits);
				}
				if (num >= 0f)
				{
					RayQueryResult? result = null;
					if (Front != null)
					{
						result = Front.CollidesWith(ray, minT, Math.Min(maxT, num2.Value), percisionDigits);
					}
					if (result.HasValue)
					{
						return result;
					}
					Triangle3D? triangle3D = CollideWithTriangles(ray);
					if (triangle3D.HasValue)
					{
						return new RayQueryResult(num2.Value, triangle3D.Value);
					}
					if (Back == null)
					{
						return null;
					}
					return Back.CollidesWith(ray, Math.Max(minT, num2.Value), maxT, percisionDigits);
				}
				RayQueryResult? result2 = null;
				if (Back != null)
				{
					result2 = Back.CollidesWith(ray, minT, Math.Min(maxT, num2.Value), percisionDigits);
				}
				if (result2.HasValue)
				{
					return result2;
				}
				if (Front == null)
				{
					return null;
				}
				return Front.CollidesWith(ray, Math.Max(minT, num2.Value), maxT, percisionDigits);
			}

			private void CollideWithTriangles(Ellipsoid ellipsoid, float distanceToPlane, List<ContactPoint> contacts)
			{
				Vector3 normal = SplitPlane.Normal;
				float num = ellipsoid.Radius.X * normal.X * ellipsoid.Radius.X * normal.X + ellipsoid.Radius.Y * normal.Y * ellipsoid.Radius.Y * normal.Y + ellipsoid.Radius.Z * normal.Z * ellipsoid.Radius.Z * normal.Z;
				float num2 = distanceToPlane * distanceToPlane;
				for (int i = 0; i < TriList.Length; i++)
				{
					Triangle3D tri = TriList[i];
					Vector3 vector = ellipsoid.Center - tri.A;
					Vector3.Dot(normal, vector);
					Vector3 vector2 = distanceToPlane * normal;
					Vector3 vector3 = ellipsoid.Center + vector2;
					Vector3 vector4 = tri.B - tri.A;
					Vector3 vector5 = tri.C - tri.A;
					float num3 = Vector3.Dot(vector4, vector4);
					float num4 = Vector3.Dot(vector4, vector5);
					float num5 = Vector3.Dot(vector5, vector5);
					Vector3 vector6 = vector3 - tri.A;
					float num6 = Vector3.Dot(vector6, vector4);
					float num7 = Vector3.Dot(vector6, vector5);
					float num8 = num4 * num4 - num3 * num5;
					float num9 = (num4 * num7 - num5 * num6) / num8;
					if ((double)num9 >= 0.0 && (double)num9 <= 1.0)
					{
						float num10 = (num4 * num6 - num3 * num7) / num8;
						if ((double)num10 >= 0.0 && (double)(num9 + num10) <= 1.0 && num2 <= num)
						{
							float num11 = (float)Math.Sqrt(num);
							Vector3 dir = (num11 - distanceToPlane) * normal;
							contacts.Add(new ContactPoint(dir, tri));
						}
					}
				}
			}

			private void CollideWithTriangles(Ellipsoid ellipsoid, Plane localPlane, float distanceToLocalPlane, List<ContactPoint> contacts)
			{
				Triangle3D result = default(Triangle3D);
				for (int i = 0; i < TriList.Length; i++)
				{
					ellipsoid.TransformWorldSpaceTriToUnitSphereSpace(TriList[i], ref result);
					Vector3.Dot(localPlane.Normal, result.A);
					Vector3 vector = (0f - distanceToLocalPlane) * localPlane.Normal;
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
							float p = ellipsoid.CalculateWorldSpacePenetration(vector);
							contacts.Add(new ContactPoint(SplitPlane.Normal, TriList[i], p));
							return;
						}
					}
				}
				Vector3 vector5 = Vector3.Zero;
				for (int j = 0; j < TriList.Length; j++)
				{
					ellipsoid.TransformWorldSpaceTriToUnitSphereSpace(TriList[j], ref result);
					LineF3D lineF3D = new LineF3D(result.A, result.B);
					LineF3D lineF3D2 = new LineF3D(result.A, result.C);
					LineF3D lineF3D3 = new LineF3D(result.B, result.C);
					Vector3 vector6 = lineF3D.ClosetPointTo(Vector3.Zero);
					Vector3 vector7 = lineF3D2.ClosetPointTo(Vector3.Zero);
					Vector3 vector8 = lineF3D3.ClosetPointTo(Vector3.Zero);
					float num9 = vector6.LengthSquared();
					float num10 = vector7.LengthSquared();
					float num11 = vector8.LengthSquared();
					bool flag = false;
					float num12 = 1f;
					if (num9 <= num12)
					{
						num12 = num9;
						vector5 = vector6;
						flag = true;
					}
					if (num10 <= num12)
					{
						num12 = num10;
						vector5 = vector7;
						flag = true;
					}
					if (num11 <= num12)
					{
						num12 = num11;
						vector5 = vector8;
						flag = true;
					}
					if (flag)
					{
						float p2 = ellipsoid.CalculateWorldSpacePenetration(vector5);
						vector5 = ellipsoid.TransformUnitSphereSpaceVectorToWorldSpace(Vector3.Negate(vector5));
						contacts.Add(new ContactPoint(vector5, TriList[j], p2));
					}
				}
			}

			public void FindCollisions(Ellipsoid ellipsoid, List<ContactPoint> contacts)
			{
				Plane localPlane = ellipsoid.TransformWorldSpacePlaneToUnitSphereSpace(SplitPlane);
				float d = localPlane.D;
				if (d >= 0f)
				{
					if (Front != null)
					{
						Front.FindCollisions(ellipsoid, contacts);
					}
					if (d <= 1f)
					{
						CollideWithTriangles(ellipsoid, localPlane, d, contacts);
						if (Back != null)
						{
							Back.FindCollisions(ellipsoid, contacts);
						}
					}
				}
				else
				{
					if (Back != null)
					{
						Back.FindCollisions(ellipsoid, contacts);
					}
					if (d >= -1f && Front != null)
					{
						Front.FindCollisions(ellipsoid, contacts);
					}
				}
			}

			public void FindCollisions(BoundingSphere sphere, List<ContactPoint> contacts)
			{
				float num = SplitPlane.DotCoordinate(sphere.Center);
				if (num >= 0f)
				{
					if (Front != null)
					{
						Front.FindCollisions(sphere, contacts);
					}
					if (num <= sphere.Radius)
					{
						CollideWithTriangles(sphere, num, contacts);
						if (Back != null)
						{
							Back.FindCollisions(sphere, contacts);
						}
					}
				}
				else
				{
					if (Back != null)
					{
						Back.FindCollisions(sphere, contacts);
					}
					if (num >= 0f - sphere.Radius && Front != null)
					{
						Front.FindCollisions(sphere, contacts);
					}
				}
			}

			private void CollideWithTriangles(BoundingSphere sphere, float distanceToPlane, List<ContactPoint> contacts)
			{
				Vector3 normal = SplitPlane.Normal;
				float num = distanceToPlane * distanceToPlane;
				float num2 = sphere.Radius * sphere.Radius;
				for (int i = 0; i < TriList.Length; i++)
				{
					Triangle3D tri = TriList[i];
					Vector3 vector = sphere.Center - tri.A;
					Vector3.Dot(normal, vector);
					Vector3 vector2 = distanceToPlane * normal;
					Vector3 vector3 = sphere.Center + vector2;
					Vector3 vector4 = tri.B - tri.A;
					Vector3 vector5 = tri.C - tri.A;
					float num3 = Vector3.Dot(vector4, vector4);
					float num4 = Vector3.Dot(vector4, vector5);
					float num5 = Vector3.Dot(vector5, vector5);
					Vector3 vector6 = vector3 - tri.A;
					float num6 = Vector3.Dot(vector6, vector4);
					float num7 = Vector3.Dot(vector6, vector5);
					float num8 = num4 * num4 - num3 * num5;
					float num9 = (num4 * num7 - num5 * num6) / num8;
					if ((double)num9 >= 0.0 && (double)num9 <= 1.0)
					{
						float num10 = (num4 * num6 - num3 * num7) / num8;
						if ((double)num10 >= 0.0 && (double)(num9 + num10) <= 1.0 && num <= num2)
						{
							Vector3 dir = (sphere.Radius - distanceToPlane) * normal;
							contacts.Add(new ContactPoint(dir, tri));
						}
					}
				}
			}

			public void GetData(List<Plane> planes, List<Triangle3D> triangles)
			{
				planes.Add(SplitPlane);
				if (TriList != null)
				{
					triangles.AddRange(TriList);
				}
				if (Front != null)
				{
					Front.GetData(planes, triangles);
				}
				if (Back != null)
				{
					Back.GetData(planes, triangles);
				}
			}
		}

		private const int Version = 1;

		private BSPTreeNode _root = new BSPTreeNode();

		private int _percisionDigits;

		private readonly string FileID = "TASBSP";

		public BSPTree()
		{
			_root = new BSPTreeNode();
		}

		public BSPTree(Model model, int percisionDigits)
		{
			Build(model, percisionDigits);
		}

		public void Build(IList<Triangle3D> polys, int percisionDigits)
		{
			LinkedList<Triangle3D> linkedList = new LinkedList<Triangle3D>();
			int num = 0;
			foreach (Triangle3D poly in polys)
			{
				double num2 = Math.Round(poly.Area, percisionDigits);
				if (num2 != 0.0)
				{
					linkedList.AddLast(poly);
				}
				else
				{
					num++;
				}
			}
			_percisionDigits = percisionDigits;
			_root = new BSPTreeNode();
			_root.Build(linkedList, percisionDigits);
		}

		public void Build(Model model, int percisionDigits)
		{
			List<Triangle3D> polys = model.ExtractModelTris(true);
			Build(polys, percisionDigits);
		}

		protected override RayQueryResult? CollidesWith(Ray ray, float min, float max)
		{
			return _root.CollidesWith(ray, min, max, _percisionDigits);
		}

		public override void FindCollisions(Ellipsoid ellipsoid, List<ContactPoint> contacts)
		{
			contacts.Clear();
			_root.FindCollisions(ellipsoid, contacts);
		}

		public override void FindCollisions(BoundingSphere sphere, List<ContactPoint> contacts)
		{
			contacts.Clear();
			_root.FindCollisions(sphere, contacts);
		}

		public override void Save(BinaryWriter writer)
		{
			writer.Write(FileID);
			writer.Write(1);
			writer.Write(_percisionDigits);
			_root.Save(writer);
		}

		public override void Load(BinaryReader reader)
		{
			string text = reader.ReadString();
			if (text != FileID)
			{
				throw new Exception("Bad BSP file Format");
			}
			int num = reader.ReadInt32();
			if (num != 1)
			{
				throw new Exception("Bad BSP version");
			}
			_percisionDigits = reader.ReadInt32();
			_root = BSPTreeNode.Load(reader);
		}

		public void GetData(List<Plane> planes, List<Triangle3D> triangles)
		{
			_root.GetData(planes, triangles);
		}

		public override void GetTriangles(List<Triangle3D> tris)
		{
			GetData(new List<Plane>(), tris);
		}
	}
}
