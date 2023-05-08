using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public static class DrawingTools
	{
		private const int X = 0;

		private const int Y = 1;

		private const int Z = 2;

		private const int W = 3;

		private const float SQRTHALF = 0.707106769f;

		private static BasicEffect _wireFrameEffect;

		private static VertexPositionColor[] _wireFrameVerts;

		private static float[][] mat_id = new float[4][]
		{
			new float[4] { 1f, 0f, 0f, 0f },
			new float[4] { 0f, 1f, 0f, 0f },
			new float[4] { 0f, 0f, 1f, 0f },
			new float[4] { 0f, 0f, 0f, 1f }
		};

		private static readonly Quaternion qxtoz = new Quaternion(0f, 0.707106769f, 0f, 0.707106769f);

		private static readonly Quaternion qytoz = new Quaternion(0.707106769f, 0f, 0f, 0.707106769f);

		private static readonly Quaternion qppmm = new Quaternion(0.5f, 0.5f, -0.5f, -0.5f);

		private static readonly Quaternion qpppp = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);

		private static readonly Quaternion qmpmm = new Quaternion(-0.5f, 0.5f, -0.5f, -0.5f);

		private static readonly Quaternion qpppm = new Quaternion(0.5f, 0.5f, 0.5f, -0.5f);

		private static readonly Quaternion q0001 = new Quaternion(0f, 0f, 0f, 1f);

		private static readonly Quaternion q1000 = new Quaternion(1f, 0f, 0f, 0f);

		public static void DrawLine(this GraphicsDevice graphicsDevice, Matrix view, Matrix projection, LineF3D line, Color color)
		{
			if (_wireFrameVerts == null)
			{
				_wireFrameVerts = new VertexPositionColor[2];
			}
			_wireFrameVerts[0].Color = color;
			_wireFrameVerts[0].Position = line.Start;
			_wireFrameVerts[1].Color = Color.White;
			_wireFrameVerts[1].Position = line.End;
			if (_wireFrameEffect == null)
			{
				_wireFrameEffect = new BasicEffect(graphicsDevice);
			}
			_wireFrameEffect.LightingEnabled = false;
			_wireFrameEffect.TextureEnabled = false;
			_wireFrameEffect.VertexColorEnabled = true;
			_wireFrameEffect.Projection = projection;
			_wireFrameEffect.View = view;
			_wireFrameEffect.World = Matrix.Identity;
			for (int i = 0; i < _wireFrameEffect.CurrentTechnique.Passes.Count; i++)
			{
				EffectPass effectPass = _wireFrameEffect.CurrentTechnique.Passes[i];
				effectPass.Apply();
				graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, _wireFrameVerts, 0, 1);
			}
		}

		public static void DrawOutlinedText(this SpriteBatch spriteBatch, SpriteFont font, StringBuilder builder, Vector2 location, Color textColor, Color outlineColor, int outlineWidth, float scale, float rotation, Vector2 orgin)
		{
			spriteBatch.DrawString(font, builder, new Vector2(location.X + (float)outlineWidth * scale, location.Y + (float)outlineWidth * scale), outlineColor, rotation, orgin, scale, SpriteEffects.None, 1f);
			spriteBatch.DrawString(font, builder, new Vector2(location.X - (float)outlineWidth * scale, location.Y - (float)outlineWidth * scale), outlineColor, rotation, orgin, scale, SpriteEffects.None, 1f);
			spriteBatch.DrawString(font, builder, new Vector2(location.X - (float)outlineWidth * scale, location.Y + (float)outlineWidth * scale), outlineColor, rotation, orgin, scale, SpriteEffects.None, 1f);
			spriteBatch.DrawString(font, builder, new Vector2(location.X + (float)outlineWidth * scale, location.Y - (float)outlineWidth * scale), outlineColor, rotation, orgin, scale, SpriteEffects.None, 1f);
			spriteBatch.DrawString(font, builder, new Vector2(location.X, location.Y), textColor, rotation, orgin, scale, SpriteEffects.None, 1f);
		}

		public static void DrawOutlinedText(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 location, Color textColor, Color outlineColor, int outlineWidth, float scale, float rotation, Vector2 orgin)
		{
			spriteBatch.DrawString(font, text, new Vector2(location.X + (float)outlineWidth * scale, location.Y + (float)outlineWidth * scale), outlineColor, rotation, orgin, scale, SpriteEffects.None, 1f);
			spriteBatch.DrawString(font, text, new Vector2(location.X - (float)outlineWidth * scale, location.Y - (float)outlineWidth * scale), outlineColor, rotation, orgin, scale, SpriteEffects.None, 1f);
			spriteBatch.DrawString(font, text, new Vector2(location.X - (float)outlineWidth * scale, location.Y + (float)outlineWidth * scale), outlineColor, rotation, orgin, scale, SpriteEffects.None, 1f);
			spriteBatch.DrawString(font, text, new Vector2(location.X + (float)outlineWidth * scale, location.Y - (float)outlineWidth * scale), outlineColor, rotation, orgin, scale, SpriteEffects.None, 1f);
			spriteBatch.DrawString(font, text, new Vector2(location.X, location.Y), textColor, rotation, orgin, scale, SpriteEffects.None, 1f);
		}

		public static void DrawOutlinedText(this SpriteBatch spriteBatch, SpriteFont font, StringBuilder builder, Vector2 location, Color textColor, Color outlineColor, int outlineWidth)
		{
			spriteBatch.DrawString(font, builder, new Vector2(location.X + (float)outlineWidth, location.Y + (float)outlineWidth), outlineColor);
			spriteBatch.DrawString(font, builder, new Vector2(location.X - (float)outlineWidth, location.Y - (float)outlineWidth), outlineColor);
			spriteBatch.DrawString(font, builder, new Vector2(location.X - (float)outlineWidth, location.Y + (float)outlineWidth), outlineColor);
			spriteBatch.DrawString(font, builder, new Vector2(location.X + (float)outlineWidth, location.Y - (float)outlineWidth), outlineColor);
			spriteBatch.DrawString(font, builder, new Vector2(location.X, location.Y), textColor);
		}

		public static void DrawOutlinedText(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 location, Color textColor, Color outlineColor, int outlineWidth)
		{
			spriteBatch.DrawString(font, text, new Vector2(location.X + (float)outlineWidth, location.Y + (float)outlineWidth), outlineColor);
			spriteBatch.DrawString(font, text, new Vector2(location.X - (float)outlineWidth, location.Y - (float)outlineWidth), outlineColor);
			spriteBatch.DrawString(font, text, new Vector2(location.X - (float)outlineWidth, location.Y + (float)outlineWidth), outlineColor);
			spriteBatch.DrawString(font, text, new Vector2(location.X + (float)outlineWidth, location.Y - (float)outlineWidth), outlineColor);
			spriteBatch.DrawString(font, text, new Vector2(location.X, location.Y), textColor);
		}

		public static void ExtractData(this Model mdl, List<Vector3> vtcs, List<TriangleVertexIndices> idcs, bool includeNoncoll)
		{
			Matrix identity = Matrix.Identity;
			foreach (ModelMesh mesh in mdl.Meshes)
			{
				identity = mesh.ParentBone.GetAbsoluteTransform();
				mesh.ExtractModelMeshData(ref identity, vtcs, idcs, includeNoncoll);
			}
		}

		public static Matrix GetAbsoluteTransform(this ModelBone bone)
		{
			if (bone == null)
			{
				return Matrix.Identity;
			}
			return bone.Transform * bone.Parent.GetAbsoluteTransform();
		}

		public static List<Triangle3D> ExtractModelTris(this Model model, bool includeNoncoll)
		{
			List<Triangle3D> list = new List<Triangle3D>();
			Matrix[] array = new Matrix[((ReadOnlyCollection<ModelBone>)(object)model.Bones).Count];
			model.CopyAbsoluteBoneTransformsTo(array);
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes).Count; i++)
			{
				ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)(object)model.Meshes)[i];
				List<Vector3> list2 = new List<Vector3>();
				List<TriangleVertexIndices> list3 = new List<TriangleVertexIndices>();
				modelMesh.ExtractModelMeshData(ref array[modelMesh.ParentBone.Index], list2, list3, includeNoncoll);
				foreach (TriangleVertexIndices item2 in list3)
				{
					Triangle3D item = new Triangle3D(list2[item2.A], list2[item2.B], list2[item2.C]);
					list.Add(item);
				}
			}
			return list;
		}

		public static void ExtractModelMeshData(this ModelMesh mm, ref Matrix xform, List<Vector3> vertices, List<TriangleVertexIndices> indices, bool includeNoncoll)
		{
			foreach (ModelMeshPart meshPart in mm.MeshParts)
			{
				if (!includeNoncoll)
				{
					EffectAnnotation effectAnnotation = meshPart.Effect.CurrentTechnique.Annotations["collide"];
					if (effectAnnotation != null && !effectAnnotation.GetValueBoolean())
					{
						Console.WriteLine("Ignoring model mesh part {1} because it's set to not collide.", mm.Name);
						continue;
					}
				}
				meshPart.ExtractModelMeshPartData(ref xform, vertices, indices);
			}
		}

		public static void ExtractModelMeshPartData(this ModelMeshPart meshPart, ref Matrix transform, List<Vector3> vertices, List<TriangleVertexIndices> indices)
		{
			int count = vertices.Count;
			VertexDeclaration vertexDeclaration = meshPart.VertexBuffer.VertexDeclaration;
			VertexElement[] vertexElements = vertexDeclaration.GetVertexElements();
			VertexElement vertexElement = default(VertexElement);
			VertexElement[] array = vertexElements;
			for (int i = 0; i < array.Length; i++)
			{
				VertexElement vertexElement2 = array[i];
				if (vertexElement2.VertexElementUsage == VertexElementUsage.Position && vertexElement2.VertexElementFormat == VertexElementFormat.Vector3)
				{
					vertexElement = vertexElement2;
					break;
				}
			}
			if (vertexElement.VertexElementUsage != 0 || vertexElement.VertexElementFormat != VertexElementFormat.Vector3)
			{
				throw new Exception("Model uses unsupported vertex format!");
			}
			Vector3[] array2 = new Vector3[meshPart.NumVertices];
			meshPart.VertexBuffer.GetData<Vector3>(meshPart.VertexOffset * vertexDeclaration.VertexStride + vertexElement.Offset, array2, 0, meshPart.NumVertices, vertexDeclaration.VertexStride);
			for (int j = 0; j != array2.Length; j++)
			{
				Vector3.Transform(ref array2[j], ref transform, out array2[j]);
			}
			vertices.AddRange(array2);
			if (meshPart.IndexBuffer.IndexElementSize != 0)
			{
				throw new Exception("Model uses 32-bit indices, which are not supported.");
			}
			short[] array3 = new short[meshPart.PrimitiveCount * 3];
			meshPart.IndexBuffer.GetData(meshPart.StartIndex * 2, array3, 0, meshPart.PrimitiveCount * 3);
			TriangleVertexIndices[] array4 = new TriangleVertexIndices[meshPart.PrimitiveCount];
			for (int k = 0; k != array4.Length; k++)
			{
				array4[k].A = array3[k * 3] + count;
				array4[k].B = array3[k * 3 + 1] + count;
				array4[k].C = array3[k * 3 + 2] + count;
			}
			indices.AddRange(array4);
		}

		public static Plane PlaneFromPointNormal(Vector3 point, Vector3 normal)
		{
			normal.Normalize();
			float d = 0f - (normal.X * point.X + normal.Y * point.Y + normal.Z * point.Z);
			return new Plane(normal, d);
		}

		public static Point CenterOf(this Rectangle r)
		{
			return new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
		}

		public static void GenerateComplementBasis(out Vector3 u, out Vector3 v, Vector3 w)
		{
			v = default(Vector3);
			u = default(Vector3);
			if (Math.Abs(w.X) >= Math.Abs(w.Y))
			{
				float num = 1f / (float)Math.Sqrt(w.X * w.X + w.Z * w.Z);
				u.X = (0f - w.Z) * num;
				u.Y = 0f;
				u.Z = w.X * num;
				v.X = w.Y * u.Z;
				v.Y = w.Z * u.X - w.X * u.Z;
				v.Z = (0f - w.Y) * u.X;
			}
			else
			{
				float num = 1f / (float)Math.Sqrt(w.Y * w.Y + w.Z * w.Z);
				u.X = 0f;
				u.Y = w.Z * num;
				u.Z = (0f - w.Y) * num;
				v.X = w.Y * u.Z - w.Z * u.Y;
				v.Y = (0f - w.X) * u.Z;
				v.Z = w.X * u.Y;
			}
		}

		public static int Intersects(this Ray ray, Capsule capsule, out float? t1, out float? t2)
		{
			float num = ray.Direction.Length();
			t1 = null;
			t2 = null;
			Vector3 direction = capsule.Segment.Direction;
			Vector3 u;
			Vector3 v;
			GenerateComplementBasis(out u, out v, direction);
			float num2 = capsule.Radius * capsule.Radius;
			float num3 = capsule.Segment.Length / 2f;
			Vector3 position = ray.Position;
			Vector3 vector = ray.Direction / num;
			float num4 = 0.001f;
			Vector3 vector2 = position - capsule.Segment.Center;
			Vector3 vector3 = new Vector3(Vector3.Dot(u, vector2), Vector3.Dot(v, vector2), Vector3.Dot(direction, vector2));
			float num5 = Vector3.Dot(direction, vector);
			if (Math.Abs(num5) >= 1f - num4)
			{
				float num6 = num2 - vector3.X * vector3.X - vector3.Y * vector3.Y;
				if (num6 < 0f)
				{
					return 0;
				}
				float num7 = (float)Math.Sqrt(num6) + num3;
				if (num5 > 0f)
				{
					t1 = 0f - vector3.Z - num7;
					t2 = 0f - vector3.Z + num7;
					t1 /= num;
					t2 /= num;
				}
				else
				{
					t1 = vector3.Z - num7;
					t2 = vector3.Z + num7;
					t1 /= num;
					t2 /= num;
				}
				return 2;
			}
			Vector3 vector4 = new Vector3(Vector3.Dot(u, vector), Vector3.Dot(v, vector), num5);
			float num8 = vector3.X * vector3.X + vector3.Y * vector3.Y - num2;
			float num9 = vector3.X * vector4.X + vector3.Y * vector4.Y;
			float num10 = vector4.X * vector4.X + vector4.Y * vector4.Y;
			float num11 = num9 * num9 - num8 * num10;
			if (num11 < 0f)
			{
				return 0;
			}
			if (num11 > num4)
			{
				float num12 = (float)Math.Sqrt(num11);
				float num13 = 1f / num10;
				float num14 = (0f - num9 - num12) * num13;
				float value = vector3.Z + num14 * vector4.Z;
				if (Math.Abs(value) <= num3)
				{
					if (!t1.HasValue)
					{
						t1 = num14 / num;
					}
					else
					{
						t2 = num14 / num;
					}
				}
				num14 = (0f - num9 + num12) * num13;
				value = vector3.Z + num14 * vector4.Z;
				if (Math.Abs(value) <= num3)
				{
					if (!t1.HasValue)
					{
						t1 = num14 / num;
					}
					else
					{
						t2 = num14 / num;
					}
				}
				if (t1.HasValue && t2.HasValue)
				{
					return 2;
				}
			}
			else
			{
				float num14 = (0f - num9) / num10;
				float value = vector3.Z + num14 * vector4.Z;
				if (Math.Abs(value) <= num3)
				{
					t1 = num14 / num;
					return 1;
				}
			}
			float num15 = vector3.Z + num3;
			num9 += num15 * vector4.Z;
			num8 += num15 * num15;
			num11 = num9 * num9 - num8;
			if (num11 > num4)
			{
				float num12 = (float)Math.Sqrt(num11);
				float num14 = 0f - num9 - num12;
				float value = vector3.Z + num14 * vector4.Z;
				if (value <= 0f - num3)
				{
					if (!t1.HasValue)
					{
						t1 = num14 / num;
					}
					else
					{
						t2 = num14 / num;
					}
					if (t1.HasValue && t2.HasValue)
					{
						if (t1 > t2)
						{
							float? num16 = t1;
							t1 = t2;
							t2 = num16;
						}
						return 2;
					}
				}
				num14 = 0f - num9 + num12;
				value = vector3.Z + num14 * vector4.Z;
				if (value <= 0f - num3)
				{
					if (!t1.HasValue)
					{
						t1 = num14 / num;
					}
					else
					{
						t2 = num14 / num;
					}
					if (t1.HasValue && t2.HasValue)
					{
						if (t1 > t2)
						{
							float? num17 = t1;
							t1 = t2;
							t2 = num17;
						}
						return 2;
					}
				}
			}
			else if (Math.Abs(num11) <= num4)
			{
				float num14 = 0f - num9;
				float value = vector3.Z + num14 * vector4.Z;
				if (value <= 0f - num3)
				{
					if (!t1.HasValue)
					{
						t1 = num14 / num;
					}
					else
					{
						t2 = num14 / num;
					}
					if (t1.HasValue && t2.HasValue)
					{
						if (t1 > t2)
						{
							float? num18 = t1;
							t1 = t2;
							t2 = num18;
						}
						return 2;
					}
				}
			}
			num9 -= 2f * num3 * vector4.Z;
			num8 -= 4f * num3 * vector3.Z;
			num11 = num9 * num9 - num8;
			if (num11 > num4)
			{
				float num12 = (float)Math.Sqrt(num11);
				float num14 = 0f - num9 - num12;
				float value = vector3.Z + num14 * vector4.Z;
				if (value >= num3)
				{
					if (!t1.HasValue)
					{
						t1 = num14 / num;
					}
					else
					{
						t2 = num14 / num;
					}
					if (t1.HasValue && t2.HasValue)
					{
						if (t1 > t2)
						{
							float? num19 = t1;
							t1 = t2;
							t2 = num19;
						}
						return 2;
					}
				}
				num14 = 0f - num9 + num12;
				value = vector3.Z + num14 * vector4.Z;
				if (value >= num3)
				{
					if (!t1.HasValue)
					{
						t1 = num14 / num;
					}
					else
					{
						t2 = num14 / num;
					}
					if (t1.HasValue && t2.HasValue)
					{
						if (t1 > t2)
						{
							float? num20 = t1;
							t1 = t2;
							t2 = num20;
						}
						return 2;
					}
				}
			}
			else if (Math.Abs(num11) <= num4)
			{
				float num14 = 0f - num9;
				float value = vector3.Z + num14 * vector4.Z;
				if (value >= num3)
				{
					if (!t1.HasValue)
					{
						t1 = num14 / num;
					}
					else
					{
						t2 = num14 / num;
					}
					if (t1.HasValue && t2.HasValue)
					{
						if (t1 > t2)
						{
							float? num21 = t1;
							t1 = t2;
							t2 = num21;
						}
						return 2;
					}
				}
			}
			int num22 = 0;
			if (t1.HasValue)
			{
				num22++;
			}
			if (t2.HasValue)
			{
				num22++;
			}
			return num22;
		}

		public static RectangleF GetBoundingRect(Vector2[] points)
		{
			Vector2 vector;
			Vector2 vector2 = (vector = points[0]);
			for (int i = 1; i < points.Length; i++)
			{
				if (points[i].X < vector.X)
				{
					vector.X = points[i].X;
				}
				if (points[i].Y < vector.Y)
				{
					vector.Y = points[i].Y;
				}
				if (points[i].X > vector2.X)
				{
					vector2.X = points[i].X;
				}
				if (points[i].Y > vector2.Y)
				{
					vector2.Y = points[i].Y;
				}
			}
			return new RectangleF(vector.X, vector.Y, vector2.X - vector.X, vector2.Y - vector.Y);
		}

		public static double Distance(this Point p1, Point p2)
		{
			int num = p1.X - p2.X;
			int num2 = p1.Y - p2.Y;
			return Math.Sqrt(num * num + num2 * num2);
		}

		public static int DistanceSquared(this Point p1, Point p2)
		{
			int num = p1.X - p2.X;
			int num2 = p1.Y - p2.Y;
			return num * num + num2 * num2;
		}

		public static double DistanceSquared(this Vector2 p1, Vector2 p2)
		{
			float num = p1.X - p2.X;
			float num2 = p1.Y - p2.Y;
			return num * num + num2 * num2;
		}

		public static double Distance(this Vector2 p1, Vector2 p2)
		{
			float num = p1.X - p2.X;
			float num2 = p1.Y - p2.Y;
			return Math.Sqrt(num * num + num2 * num2);
		}

		public static bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
		{
			Vector2 vector = c - a;
			Vector2 vector2 = b - a;
			Vector2 value = p - a;
			float num = Vector2.Dot(vector, vector);
			float num2 = Vector2.Dot(vector, vector2);
			float num3 = Vector2.Dot(vector, value);
			float num4 = Vector2.Dot(vector2, vector2);
			float num5 = Vector2.Dot(vector2, value);
			float num6 = 1f / (num * num4 - num2 * num2);
			float num7 = (num4 * num3 - num2 * num5) * num6;
			float num8 = (num * num5 - num2 * num3) * num6;
			if (num7 >= 0f && num8 >= 0f)
			{
				return num7 + num8 <= 1f;
			}
			return false;
		}

		public static bool PointInTriangle(Point vert1, Point vert2, Point vert3, Point point)
		{
			Vector2 p = new Vector2(point.X, point.Y);
			Vector2 a = new Vector2(vert1.X, vert1.Y);
			Vector2 b = new Vector2(vert2.X, vert2.Y);
			Vector2 c = new Vector2(vert3.X, vert3.Y);
			return PointInTriangle(a, b, c, p);
		}

		public static RectangleF FindBounds(IList<Vector2> points)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			float num4 = float.MinValue;
			for (int i = 0; i < points.Count; i++)
			{
				Vector2 vector = points[i];
				num = Math.Min(vector.X, num);
				num2 = Math.Min(vector.Y, num2);
				num3 = Math.Max(vector.X, num3);
				num4 = Math.Max(vector.Y, num4);
			}
			return new RectangleF(num, num2, num3 - num, num4 - num2);
		}

		public static int[] GetConvexHullIndices(IList<Vector2> points)
		{
			if (points.Count <= 3)
			{
				int[] array = new int[points.Count];
				for (int i = 0; i < points.Count; i++)
				{
					array[i] = i;
				}
				return array;
			}
			LinkedList<int> linkedList = new LinkedList<int>();
			LinkedList<int> linkedList2 = new LinkedList<int>();
			LinkedList<int> linkedList3 = new LinkedList<int>();
			int num = -1;
			float num2 = float.MaxValue;
			int num3 = -1;
			float num4 = float.MinValue;
			for (int j = 0; j < points.Count; j++)
			{
				Vector2 vector = points[j];
				if (vector.X <= num2)
				{
					num = j;
					num2 = vector.X;
				}
				if (vector.X >= num4)
				{
					num3 = j;
					num4 = vector.X;
				}
			}
			Vector2 vector2 = points[num];
			Vector2 vector3 = points[num3];
			Vector2 v = new Vector2(vector3.X - vector2.X, vector3.Y - vector2.Y);
			float num5 = float.MinValue;
			LinkedListNode<int> linkedListNode = null;
			float num6 = float.MaxValue;
			LinkedListNode<int> linkedListNode2 = null;
			for (int k = 0; k < points.Count; k++)
			{
				if (k == num || k == num3)
				{
					continue;
				}
				Vector2 vector4 = points[k];
				Vector2 v2 = new Vector2(vector2.X - vector4.X, vector2.Y - vector4.Y);
				float num7 = v.Cross(v2);
				if (num7 > 0f)
				{
					LinkedListNode<int> linkedListNode3 = linkedList.AddLast(k);
					if (num7 > num5)
					{
						num5 = num7;
						linkedListNode = linkedListNode3;
					}
				}
				if (num7 < 0f)
				{
					LinkedListNode<int> linkedListNode4 = linkedList2.AddLast(k);
					if (num7 < num6)
					{
						num6 = num7;
						linkedListNode2 = linkedListNode4;
					}
				}
			}
			LinkedListNode<int> linkedListNode5 = linkedList3.AddFirst(num);
			LinkedListNode<int> linkedListNode6 = linkedList3.AddLast(num3);
			if (linkedListNode != null)
			{
				linkedList.Remove(linkedListNode);
			}
			if (linkedListNode2 != null)
			{
				linkedList2.Remove(linkedListNode2);
			}
			if (linkedListNode != null)
			{
				linkedList3.AddAfter(linkedListNode5, linkedListNode);
			}
			if (linkedListNode2 != null)
			{
				linkedList3.AddAfter(linkedListNode6, linkedListNode2);
			}
			if (linkedListNode != null)
			{
				QuickHull(points, linkedList, linkedList3, linkedListNode6, linkedListNode5, linkedListNode);
			}
			if (linkedListNode2 != null)
			{
				QuickHull(points, linkedList2, linkedList3, linkedListNode5, linkedListNode6, linkedListNode2);
			}
			int[] array2 = new int[linkedList3.Count];
			linkedList3.CopyTo(array2, 0);
			return array2;
		}

		private static void QuickHull(IList<Vector2> points, LinkedList<int> pointList, LinkedList<int> hull, LinkedListNode<int> aNode, LinkedListNode<int> bNode, LinkedListNode<int> cNode)
		{
			Vector2 vector = points[aNode.Value];
			Vector2 vector2 = points[bNode.Value];
			Vector2 vector3 = points[cNode.Value];
			Vector2 vector4 = new Vector2(vector3.X - vector.X, vector3.Y - vector.Y);
			Vector2 vector5 = new Vector2(vector2.X - vector.X, vector2.Y - vector.Y);
			Vector2 v = new Vector2(vector3.X - vector2.X, vector3.Y - vector2.Y);
			float num = Vector2.Dot(vector4, vector4);
			float num2 = Vector2.Dot(vector4, vector5);
			float num3 = Vector2.Dot(vector5, vector5);
			float num4 = 1f / (num * num3 - num2 * num2);
			LinkedList<int> linkedList = new LinkedList<int>();
			LinkedList<int> linkedList2 = new LinkedList<int>();
			LinkedListNode<int> linkedListNode = null;
			LinkedListNode<int> linkedListNode2 = null;
			float num5 = float.MinValue;
			float num6 = float.MinValue;
			foreach (int point in pointList)
			{
				Vector2 vector6 = points[point];
				Vector2 vector7 = new Vector2(vector6.X - vector.X, vector6.Y - vector.Y);
				float num7 = Vector2.Dot(vector4, vector7);
				float num8 = Vector2.Dot(vector5, vector7);
				float num9 = (num3 * num7 - num2 * num8) * num4;
				float num10 = (num * num8 - num2 * num7) * num4;
				if (!(num9 >= 0f))
				{
					continue;
				}
				if (num10 <= 0f)
				{
					LinkedListNode<int> linkedListNode3 = linkedList.AddLast(point);
					float num11 = vector4.Cross(vector7);
					if (num11 < 0f)
					{
						num11 = 0f;
					}
					if (num11 > num6)
					{
						num6 = num11;
						linkedListNode = linkedListNode3;
					}
				}
				else if (num9 + num10 >= 1f)
				{
					Vector2 v2 = new Vector2(vector2.X - vector6.X, vector2.Y - vector6.Y);
					LinkedListNode<int> linkedListNode4 = linkedList2.AddLast(point);
					float num12 = v.Cross(v2);
					if (num12 < 0f)
					{
						num12 = 0f;
					}
					if (num12 > num5)
					{
						num5 = num12;
						linkedListNode2 = linkedListNode4;
					}
				}
			}
			if (linkedListNode2 != null)
			{
				linkedList2.Remove(linkedListNode2);
				hull.AddAfter(bNode, linkedListNode2);
			}
			if (linkedListNode != null)
			{
				linkedList.Remove(linkedListNode);
				hull.AddAfter(cNode, linkedListNode);
			}
			if (linkedListNode2 != null)
			{
				QuickHull(points, linkedList2, hull, cNode, bNode, linkedListNode2);
			}
			if (linkedListNode != null)
			{
				QuickHull(points, linkedList, hull, aNode, cNode, linkedListNode);
			}
		}

		public static Vector2[] GetConvexHull(IList<Vector2> points)
		{
			int[] convexHullIndices = GetConvexHullIndices(points);
			Vector2[] array = new Vector2[convexHullIndices.Length];
			for (int i = 0; i < convexHullIndices.Length; i++)
			{
				array[i] = points[convexHullIndices[i]];
			}
			return array;
		}

		public static Color ModulateColors(Color c1, Color c2)
		{
			int r = c1.R * c2.R / 255;
			int g = c1.G * c2.G / 255;
			int b = c1.B * c2.B / 255;
			int a = c1.A * c2.A / 255;
			return new Color(r, g, b, a);
		}

		public static void Decompose(this Matrix m, out Vector3 translation, out Vector3 scale, out Quaternion rotation)
		{
			Matrix m2 = m;
			float[][] array = MakeArrayMatrix();
			float[][] s = MakeArrayMatrix();
			float[][] array2 = MakeArrayMatrix();
			float[][] array3 = ToArrayMatrix(ref m2);
			translation = new Vector3(array3[0][3], array3[1][3], array3[2][3]);
			float num = polar_decomp(array3, array, s);
			float num2;
			if ((double)num < 0.0)
			{
				mat_copy_eq_neg(array, array, 3);
				num2 = -1f;
			}
			else
			{
				num2 = 1f;
			}
			rotation = Qt_FromMatrix(array);
			scale = spect_decomp(s, array2);
			Quaternion quaternion = Qt_FromMatrix(array2);
			Quaternion qR = snuggle(quaternion, ref scale);
			quaternion = Qt_Mul(quaternion, qR);
			if (num2 == -1f)
			{
				rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI);
			}
		}

		private static float[][] ToArrayMatrix(ref Matrix m)
		{
			return new float[4][]
			{
				new float[4] { m.M11, m.M21, m.M31, m.M41 },
				new float[4] { m.M12, m.M22, m.M32, m.M42 },
				new float[4] { m.M13, m.M23, m.M33, m.M43 },
				new float[4] { m.M14, m.M24, m.M34, m.M44 }
			};
		}

		private static float[][] MakeArrayMatrix()
		{
			return new float[4][]
			{
				new float[4] { 1f, 0f, 0f, 0f },
				new float[4] { 0f, 1f, 0f, 0f },
				new float[4] { 0f, 0f, 1f, 0f },
				new float[4] { 0f, 0f, 0f, 1f }
			};
		}

		private static void mat_pad(float[][] A)
		{
			A[3][0] = (A[0][3] = (A[3][1] = (A[1][3] = (A[3][2] = (A[2][3] = 0f)))));
			A[3][3] = 1f;
		}

		private static void mat_copy_eq(float[][] C, float[][] A, int n)
		{
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					C[i][j] = A[i][j];
				}
			}
		}

		private static void mat_copy_eq_neg(float[][] C, float[][] A, int n)
		{
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					C[i][j] = 0f - A[i][j];
				}
			}
		}

		private static void mat_copy_minuseq(float[][] C, float[][] A, int n)
		{
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					C[i][j] -= A[i][j];
				}
			}
		}

		private static void mat_tpose(float[][] AT, float[][] A, int n)
		{
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					AT[i][j] = A[j][i];
				}
			}
		}

		private static void mat_binop(float[][] C, float sa, float[][] A, float sb, float[][] B, int n)
		{
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < n; j++)
				{
					C[i][j] = sa * A[i][j] + sb * B[i][j];
				}
			}
		}

		private static void mat_mult(float[][] A, float[][] B, float[][] AB)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					AB[i][j] = A[i][0] * B[0][j] + A[i][1] * B[1][j] + A[i][2] * B[2][j];
				}
			}
		}

		private static float vdot(float[] va, float[] vb)
		{
			return va[0] * vb[0] + va[1] * vb[1] + va[2] * vb[2];
		}

		private static void vcross(float[] va, float[] vb, float[] v)
		{
			v[0] = va[1] * vb[2] - va[2] * vb[1];
			v[1] = va[2] * vb[0] - va[0] * vb[2];
			v[2] = va[0] * vb[1] - va[1] * vb[0];
		}

		private static void adjoint_transpose(float[][] M, float[][] MadjT)
		{
			vcross(M[1], M[2], MadjT[0]);
			vcross(M[2], M[0], MadjT[1]);
			vcross(M[0], M[1], MadjT[2]);
		}

		internal static Quaternion Qt_Conj(Quaternion q)
		{
			Quaternion result = default(Quaternion);
			result.X = 0f - q.X;
			result.Y = 0f - q.Y;
			result.Z = 0f - q.Z;
			result.W = q.W;
			return result;
		}

		internal static Quaternion Qt_Mul(Quaternion qL, Quaternion qR)
		{
			Quaternion result = default(Quaternion);
			result.W = qL.W * qR.W - qL.X * qR.X - qL.Y * qR.Y - qL.Z * qR.Z;
			result.X = qL.W * qR.X + qL.X * qR.W + qL.Y * qR.Z - qL.Z * qR.Y;
			result.Y = qL.W * qR.Y + qL.Y * qR.W + qL.Z * qR.X - qL.X * qR.Z;
			result.Z = qL.W * qR.Z + qL.Z * qR.W + qL.X * qR.Y - qL.Y * qR.X;
			return result;
		}

		private static Quaternion Qt_Scale(Quaternion q, float w)
		{
			Quaternion result = default(Quaternion);
			result.W = q.W * w;
			result.X = q.X * w;
			result.Y = q.Y * w;
			result.Z = q.Z * w;
			return result;
		}

		private static Quaternion Qt_FromMatrix(float[][] mat)
		{
			Quaternion quaternion = default(Quaternion);
			float num = mat[0][0] + mat[1][1] + mat[2][2];
			if ((double)num >= 0.0)
			{
				float num2 = (float)Math.Sqrt(num + mat[3][3]);
				quaternion.W = num2 * 0.5f;
				num2 = 0.5f / num2;
				quaternion.X = (mat[2][1] - mat[1][2]) * num2;
				quaternion.Y = (mat[0][2] - mat[2][0]) * num2;
				quaternion.Z = (mat[1][0] - mat[0][1]) * num2;
			}
			else
			{
				int num3 = 0;
				if (mat[1][1] > mat[0][0])
				{
					num3 = 1;
				}
				if (mat[2][2] > mat[num3][num3])
				{
					num3 = 2;
				}
				switch (num3)
				{
				case 0:
				{
					int num10 = 0;
					int num11 = 1;
					int num12 = 2;
					float num2 = (float)Math.Sqrt(mat[num10][num10] - (mat[num11][num11] + mat[num12][num12]) + mat[3][3]);
					quaternion.X = num2 * 0.5f;
					num2 = 0.5f / num2;
					quaternion.Y = (mat[num10][num11] + mat[num11][num10]) * num2;
					quaternion.Z = (mat[num12][num10] + mat[num10][num12]) * num2;
					quaternion.W = (mat[num12][num11] - mat[num11][num12]) * num2;
					break;
				}
				case 1:
				{
					int num7 = 1;
					int num8 = 2;
					int num9 = 0;
					float num2 = (float)Math.Sqrt(mat[num7][num7] - (mat[num8][num8] + mat[num9][num9]) + mat[3][3]);
					quaternion.Y = num2 * 0.5f;
					num2 = 0.5f / num2;
					quaternion.Z = (mat[num7][num8] + mat[num8][num7]) * num2;
					quaternion.X = (mat[num9][num7] + mat[num7][num9]) * num2;
					quaternion.W = (mat[num9][num8] - mat[num8][num9]) * num2;
					break;
				}
				case 2:
				{
					int num4 = 2;
					int num5 = 0;
					int num6 = 1;
					float num2 = (float)Math.Sqrt(mat[num4][num4] - (mat[num5][num5] + mat[num6][num6]) + mat[3][3]);
					quaternion.Z = num2 * 0.5f;
					num2 = 0.5f / num2;
					quaternion.X = (mat[num4][num5] + mat[num5][num4]) * num2;
					quaternion.Y = (mat[num6][num4] + mat[num4][num6]) * num2;
					quaternion.W = (mat[num6][num5] - mat[num5][num6]) * num2;
					break;
				}
				}
			}
			if ((double)mat[3][3] != 1.0)
			{
				return Qt_Scale(quaternion, 1f / (float)Math.Sqrt(mat[3][3]));
			}
			return quaternion;
		}

		private static float mat_norm(float[][] M, bool tpose)
		{
			float num = 0f;
			for (int i = 0; i < 3; i++)
			{
				float num2 = ((!tpose) ? (Math.Abs(M[i][0]) + Math.Abs(M[i][1]) + Math.Abs(M[i][2])) : (Math.Abs(M[0][i]) + Math.Abs(M[1][i]) + Math.Abs(M[2][i])));
				if (num < num2)
				{
					num = num2;
				}
			}
			return num;
		}

		private static float norm_inf(float[][] M)
		{
			return mat_norm(M, false);
		}

		private static float norm_one(float[][] M)
		{
			return mat_norm(M, true);
		}

		private static int find_max_col(float[][] M)
		{
			float num = 0f;
			int result = -1;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					float num2 = M[i][j];
					if ((double)num2 < 0.0)
					{
						num2 = 0f - num2;
					}
					if (num2 > num)
					{
						num = num2;
						result = j;
					}
				}
			}
			return result;
		}

		private static void make_reflector(float[] v, float[] u)
		{
			float num = (float)Math.Sqrt(vdot(v, v));
			u[0] = v[0];
			u[1] = v[1];
			u[2] = v[2] + (((double)v[2] < 0.0) ? (0f - num) : num);
			num = (float)Math.Sqrt(2f / vdot(u, u));
			u[0] *= num;
			u[1] *= num;
			u[2] *= num;
		}

		private static void reflect_cols(float[][] M, float[] u)
		{
			for (int i = 0; i < 3; i++)
			{
				float num = u[0] * M[0][i] + u[1] * M[1][i] + u[2] * M[2][i];
				for (int j = 0; j < 3; j++)
				{
					M[j][i] -= u[j] * num;
				}
			}
		}

		private static void reflect_rows(float[][] M, float[] u)
		{
			for (int i = 0; i < 3; i++)
			{
				float num = vdot(u, M[i]);
				for (int j = 0; j < 3; j++)
				{
					M[i][j] -= u[j] * num;
				}
			}
		}

		private static void do_rank1(float[][] M, float[][] Q)
		{
			float[] array = new float[3];
			float[] array2 = new float[3];
			mat_copy_eq(Q, mat_id, 4);
			int num = find_max_col(M);
			if (num >= 0)
			{
				array[0] = M[0][num];
				array[1] = M[1][num];
				array[2] = M[2][num];
				make_reflector(array, array);
				reflect_cols(M, array);
				array2[0] = M[2][0];
				array2[1] = M[2][1];
				array2[2] = M[2][2];
				make_reflector(array2, array2);
				reflect_rows(M, array2);
				float num2 = M[2][2];
				if ((double)num2 < 0.0)
				{
					Q[2][2] = -1f;
				}
				reflect_cols(Q, array);
				reflect_rows(Q, array2);
			}
		}

		private static void do_rank2(float[][] M, float[][] MadjT, float[][] Q)
		{
			float[] array = new float[3];
			float[] array2 = new float[3];
			int num = find_max_col(MadjT);
			if (num < 0)
			{
				do_rank1(M, Q);
				return;
			}
			array[0] = MadjT[0][num];
			array[1] = MadjT[1][num];
			array[2] = MadjT[2][num];
			make_reflector(array, array);
			reflect_cols(M, array);
			vcross(M[0], M[1], array2);
			make_reflector(array2, array2);
			reflect_rows(M, array2);
			float num2 = M[0][0];
			float num3 = M[0][1];
			float num4 = M[1][0];
			float num5 = M[1][1];
			if (num2 * num5 > num3 * num4)
			{
				float num6 = num5 + num2;
				float num7 = num4 - num3;
				float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
				num6 /= num8;
				num7 /= num8;
				Q[0][0] = (Q[1][1] = num6);
				Q[0][1] = 0f - (Q[1][0] = num7);
			}
			else
			{
				float num6 = num5 - num2;
				float num7 = num4 + num3;
				float num8 = (float)Math.Sqrt(num6 * num6 + num7 * num7);
				num6 /= num8;
				num7 /= num8;
				Q[0][0] = 0f - (Q[1][1] = num6);
				Q[0][1] = (Q[1][0] = num7);
			}
			Q[0][2] = (Q[2][0] = (Q[1][2] = (Q[2][1] = 0f)));
			Q[2][2] = 1f;
			reflect_cols(Q, array);
			reflect_rows(Q, array2);
		}

		private static float polar_decomp(float[][] M, float[][] Q, float[][] S)
		{
			float[][] array = MakeArrayMatrix();
			float[][] array2 = MakeArrayMatrix();
			float[][] array3 = MakeArrayMatrix();
			mat_tpose(array, M, 3);
			float num = norm_one(array);
			float num2 = norm_inf(array);
			float num3;
			float num7;
			do
			{
				adjoint_transpose(array, array2);
				num3 = vdot(array[0], array2[0]);
				if ((double)num3 == 0.0)
				{
					do_rank2(array, array2, array);
					break;
				}
				float num4 = norm_one(array2);
				float num5 = norm_inf(array2);
				float num6 = (float)Math.Sqrt((float)Math.Sqrt(num4 * num5 / (num * num2)) / Math.Abs(num3));
				float sa = num6 * 0.5f;
				float sb = 0.5f / (num6 * num3);
				mat_copy_eq(array3, array, 3);
				mat_binop(array, sa, array, sb, array2, 3);
				mat_copy_minuseq(array3, array, 3);
				num7 = norm_one(array3);
				num = norm_one(array);
				num2 = norm_inf(array);
			}
			while (num7 > num * 1E-06f);
			mat_tpose(Q, array, 3);
			mat_pad(Q);
			mat_mult(array, M, S);
			mat_pad(S);
			for (int i = 0; i < 3; i++)
			{
				for (int j = i; j < 3; j++)
				{
					S[i][j] = (S[j][i] = 0.5f * (S[i][j] + S[j][i]));
				}
			}
			return num3;
		}

		private static Vector3 spect_decomp(float[][] S, float[][] U)
		{
			Vector3 result = default(Vector3);
			float[] array = new float[3];
			float[] array2 = new float[3];
			int[] array3 = new int[3] { 1, 2, 0 };
			mat_copy_eq(U, mat_id, 4);
			array[0] = S[0][0];
			array[1] = S[1][1];
			array[2] = S[2][2];
			array2[0] = S[1][2];
			array2[1] = S[2][0];
			array2[2] = S[0][1];
			for (int num = 20; num > 0; num--)
			{
				float num2 = Math.Abs(array2[0]) + Math.Abs(array2[1]) + Math.Abs(array2[2]);
				if ((double)num2 == 0.0)
				{
					break;
				}
				for (int num3 = 2; num3 >= 0; num3--)
				{
					int num4 = array3[num3];
					int num5 = array3[num4];
					float num6 = Math.Abs(array2[num3]);
					float num7 = 100f * num6;
					if ((double)num6 > 0.0)
					{
						float num8 = array[num5] - array[num4];
						float num9 = Math.Abs(num8);
						float num10;
						if (num9 + num7 == num9)
						{
							num10 = array2[num3] / num8;
						}
						else
						{
							float num11 = 0.5f * num8 / array2[num3];
							num10 = 1f / (Math.Abs(num11) + (float)Math.Sqrt(num11 * num11 + 1f));
							if (num11 < 0f)
							{
								num10 = 0f - num10;
							}
						}
						float num12 = 1f / (float)Math.Sqrt(num10 * num10 + 1f);
						float num13 = num10 * num12;
						float num14 = num13 / (num12 + 1f);
						float num15 = num10 * array2[num3];
						array2[num3] = 0f;
						array[num4] -= num15;
						array[num5] += num15;
						float num16 = array2[num5];
						array2[num5] -= num13 * (array2[num4] + num14 * array2[num5]);
						array2[num4] += num13 * (num16 - num14 * array2[num4]);
						for (int num17 = 2; num17 >= 0; num17--)
						{
							float num18 = U[num17][num4];
							float num19 = U[num17][num5];
							U[num17][num4] -= num13 * (num19 + num14 * num18);
							U[num17][num5] += num13 * (num18 - num14 * num19);
						}
					}
				}
			}
			result.X = array[0];
			result.Y = array[1];
			result.Z = array[2];
			return result;
		}

		private static float sgn(int n, float v)
		{
			if (n == 0)
			{
				return v;
			}
			return 0f - v;
		}

		private static void swap(float[] a, int i, int j)
		{
			a[3] = a[i];
			a[i] = a[j];
			a[j] = a[3];
		}

		private static void cycle(float[] a, int p)
		{
			if (p != 0)
			{
				a[3] = a[0];
				a[0] = a[1];
				a[1] = a[2];
				a[2] = a[3];
			}
			else
			{
				a[3] = a[2];
				a[2] = a[1];
				a[1] = a[0];
				a[0] = a[3];
			}
		}

		private static Quaternion snuggle(Quaternion q, ref Vector3 k)
		{
			Quaternion quaternion = default(Quaternion);
			float[] array = new float[4];
			int num = -1;
			array[0] = k.X;
			array[1] = k.Y;
			array[2] = k.Z;
			if (array[0] == array[1])
			{
				num = ((array[0] != array[2]) ? 2 : 3);
			}
			else if (array[0] == array[2])
			{
				num = 1;
			}
			else if (array[1] == array[2])
			{
				num = 0;
			}
			if (num >= 0)
			{
				int[] array2 = new int[3];
				float[] array3 = new float[3];
				Quaternion qL;
				switch (num)
				{
				case 0:
					q = Qt_Mul(q, qL = qxtoz);
					swap(array, 0, 2);
					break;
				case 1:
					q = Qt_Mul(q, qL = qytoz);
					swap(array, 1, 2);
					break;
				case 2:
					qL = q0001;
					break;
				default:
					return Qt_Conj(q);
				}
				q = Qt_Conj(q);
				array3[0] = q.Z * q.Z + q.W * q.W - 0.5f;
				array3[1] = q.X * q.Z - q.Y * q.W;
				array3[2] = q.Y * q.Z + q.X * q.W;
				for (int i = 0; i < 3; i++)
				{
					array2[i] = ((array3[i] < 0f) ? 1 : 0);
					if (array2[i] != 0)
					{
						array3[i] = 0f - array3[i];
					}
				}
				int num2 = ((array3[0] > array3[1]) ? ((!(array3[0] > array3[2])) ? 2 : 0) : ((array3[1] > array3[2]) ? 1 : 2));
				switch (num2)
				{
				case 0:
					quaternion = ((array2[0] == 0) ? q0001 : q1000);
					break;
				case 1:
					quaternion = ((array2[1] == 0) ? qpppp : qppmm);
					cycle(array, 0);
					break;
				case 2:
					quaternion = ((array2[2] == 0) ? qpppm : qmpmm);
					cycle(array, 1);
					break;
				}
				Quaternion quaternion2 = Qt_Mul(q, quaternion);
				float num3 = (float)Math.Sqrt(array3[num2] + 0.5f);
				quaternion = Qt_Mul(quaternion, new Quaternion(0f, 0f, (0f - quaternion2.Z) / num3, quaternion2.W / num3));
				quaternion = Qt_Mul(qL, Qt_Conj(quaternion));
			}
			else
			{
				float[] array4 = new float[4];
				float[] array5 = new float[4];
				int[] array6 = new int[4];
				int num4 = 0;
				array4[0] = q.X;
				array4[1] = q.Y;
				array4[2] = q.Z;
				array4[3] = q.W;
				for (int i = 0; i < 4; i++)
				{
					array5[i] = 0f;
					array6[i] = ((array4[i] < 0f) ? 1 : 0);
					if (array6[i] != 0)
					{
						array4[i] = 0f - array4[i];
					}
					num4 ^= array6[i];
				}
				int num5 = ((!(array4[0] > array4[1])) ? 1 : 0);
				int num6 = ((!(array4[2] > array4[3])) ? 3 : 2);
				if (array4[num5] > array4[num6])
				{
					if (array4[num5 ^ 1] > array4[num6])
					{
						num6 = num5;
						num5 ^= 1;
					}
					else
					{
						num6 ^= num5;
						num5 ^= num6;
						num6 ^= num5;
					}
				}
				else if (array4[num6 ^ 1] > array4[num5])
				{
					num5 = num6 ^ 1;
				}
				double num7 = (double)(array4[0] + array4[1] + array4[2] + array4[3]) * 0.5;
				double num8 = (array4[num6] + array4[num5]) * 0.707106769f;
				double num9 = array4[num6];
				if (num7 > num8)
				{
					if (num7 > num9)
					{
						for (int j = 0; j < 4; j++)
						{
							array5[j] = sgn(array6[j], 0.5f);
						}
						cycle(array, num4);
					}
					else
					{
						array5[num6] = sgn(array6[num6], 1f);
					}
				}
				else if (num8 > num9)
				{
					array5[num6] = sgn(array6[num6], 0.707106769f);
					array5[num5] = sgn(array6[num5], 0.707106769f);
					if (num5 > num6)
					{
						num6 ^= num5;
						num5 ^= num6;
						num6 ^= num5;
					}
					int[] array7 = new int[3] { 1, 2, 0 };
					if (num6 == 3)
					{
						num6 = array7[num5];
						num5 = 3 - num6 - num5;
					}
					swap(array, num6, num5);
				}
				else
				{
					array5[num6] = sgn(array6[num6], 1f);
				}
				quaternion.X = 0f - array5[0];
				quaternion.Y = 0f - array5[1];
				quaternion.Z = 0f - array5[2];
				quaternion.W = array5[3];
			}
			k.X = array[0];
			k.Y = array[1];
			k.Z = array[2];
			return quaternion;
		}
	}
}
