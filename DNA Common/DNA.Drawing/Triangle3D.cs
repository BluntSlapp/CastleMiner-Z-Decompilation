using System;
using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct Triangle3D
	{
		private Vector3 _a;

		private Vector3 _b;

		private Vector3 _c;

		public Vector3 Normal
		{
			get
			{
				return Vector3.Cross(_b - _a, _c - _a);
			}
		}

		public LineF3D AB
		{
			get
			{
				return new LineF3D(_a, _b);
			}
		}

		public LineF3D AC
		{
			get
			{
				return new LineF3D(_a, _c);
			}
		}

		public LineF3D BC
		{
			get
			{
				return new LineF3D(_b, _c);
			}
		}

		public Vector3 A
		{
			get
			{
				return _a;
			}
			set
			{
				_a = value;
			}
		}

		public Vector3 B
		{
			get
			{
				return _b;
			}
			set
			{
				_b = value;
			}
		}

		public Vector3 C
		{
			get
			{
				return _c;
			}
			set
			{
				_c = value;
			}
		}

		public Vector3 Centroid
		{
			get
			{
				return (_a + _b + _c) / 3f;
			}
		}

		public float Area
		{
			get
			{
				Vector3 vector = _a - _b;
				Vector3 vector2 = _c - _b;
				return Vector3.Cross(vector, vector2).Length();
			}
		}

		public BoundingBox GetBoundingBox()
		{
			Vector3 min = new Vector3(Math.Min(Math.Min(_a.X, _b.X), _c.X), Math.Min(Math.Min(_a.Y, _b.Y), _c.Y), Math.Min(Math.Min(_a.Z, _b.Z), _c.Z));
			Vector3 max = new Vector3(Math.Max(Math.Max(_a.X, _b.X), _c.X), Math.Max(Math.Max(_a.Y, _b.Y), _c.Y), Math.Max(Math.Max(_a.Z, _b.Z), _c.Z));
			return new BoundingBox(min, max);
		}

		public BoundingSphere GetBoundingSphere()
		{
			BoundingBox boundingBox = GetBoundingBox();
			Vector3 vector = boundingBox.Min + (boundingBox.Max - boundingBox.Min) / 2f;
			float val = (_a - vector).LengthSquared();
			float val2 = (_b - vector).LengthSquared();
			float val3 = (_c - vector).LengthSquared();
			float radius = (float)Math.Sqrt(Math.Max(Math.Max(val, val2), val3));
			return new BoundingSphere(vector, radius);
		}

		public static Triangle3D Transform(Triangle3D triangle, Matrix matrix)
		{
			return new Triangle3D(Vector3.Transform(triangle._a, matrix), Vector3.Transform(triangle._b, matrix), Vector3.Transform(triangle._c, matrix));
		}

		public static Triangle3D Read(BinaryReader reader)
		{
			return new Triangle3D(reader.ReadVector3(), reader.ReadVector3(), reader.ReadVector3());
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(A);
			writer.Write(B);
			writer.Write(C);
		}

		public Triangle3D(Vector3 a, Vector3 b, Vector3 c)
		{
			_a = a;
			_b = b;
			_c = c;
		}

		public Vector3 BarycentricCoordinate(Vector3 point)
		{
			Vector3 result = default(Vector3);
			Vector3 vector = _b - _a;
			Vector3 vector2 = _c - _a;
			Vector3 vector3 = point - _a;
			Vector3 vector4 = Vector3.Cross(vector, vector3);
			Vector3 vector5 = Vector3.Cross(vector2, vector3);
			float num = 1f / Vector3.Cross(vector, vector2).Length();
			result.Y = vector5.Length() * num;
			result.Z = vector4.Length() * num;
			result.X = 1f - result.Y - result.Z;
			return result;
		}

		public Plane GetPlane()
		{
			return new Plane(B, A, C);
		}

		public float? Intersects(Ray ray)
		{
			Vector3 vector = _b - _a;
			Vector3 vector2 = _c - _a;
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Vector3 direction = ray.Direction;
			Vector3 vector4 = ray.Position - _a;
			float num = 0f - Vector3.Dot(vector3, vector4);
			float num2 = Vector3.Dot(vector3, direction);
			if (num2 == 0f)
			{
				return null;
			}
			float num3 = num / num2;
			if (num3 < 0f)
			{
				return null;
			}
			Vector3 vector5 = ray.Position + num3 * direction;
			float num4 = Vector3.Dot(vector, vector);
			float num5 = Vector3.Dot(vector, vector2);
			float num6 = Vector3.Dot(vector2, vector2);
			Vector3 vector6 = vector5 - _a;
			float num7 = Vector3.Dot(vector6, vector);
			float num8 = Vector3.Dot(vector6, vector2);
			float num9 = num5 * num5 - num4 * num6;
			float num10 = (num5 * num8 - num6 * num7) / num9;
			if ((double)num10 < 0.0 || (double)num10 > 1.0)
			{
				return null;
			}
			float num11 = (num5 * num7 - num4 * num8) / num9;
			if ((double)num11 < 0.0 || (double)(num10 + num11) > 1.0)
			{
				return null;
			}
			return num3;
		}

		public Triangle3D[] SliceHorizontal(float yValue, int precisionDigits)
		{
			Plane plane = DrawingTools.PlaneFromPointNormal(new Vector3(0f, yValue, 0f), new Vector3(0f, 1f, 0f));
			LineF3D aB = AB;
			LineF3D aC = AC;
			LineF3D bC = BC;
			float t;
			bool parallel;
			float t2;
			if (aB.Intersects(plane, out t, out parallel, precisionDigits))
			{
				if (parallel)
				{
					return new Triangle3D[1] { this };
				}
				if (t == 0f)
				{
					if (bC.Intersects(plane, out t2, out parallel, precisionDigits))
					{
						if (t2 == 0f || t2 == 1f)
						{
							return new Triangle3D[1] { this };
						}
						Vector3 value = bC.GetValue(t2);
						value.Y = yValue;
						return new Triangle3D[2]
						{
							new Triangle3D(_a, _b, value),
							new Triangle3D(_a, value, _c)
						};
					}
					return new Triangle3D[1] { this };
				}
				if (t == 1f)
				{
					if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
					{
						Vector3 value = aC.GetValue(t2);
						value.Y = yValue;
						if (t2 == 0f || t2 == 1f)
						{
							return new Triangle3D[1]
							{
								new Triangle3D(aC.GetValue(0f), _b, value)
							};
						}
						return new Triangle3D[2]
						{
							new Triangle3D(_b, _c, value),
							new Triangle3D(_b, value, _a)
						};
					}
					return new Triangle3D[1] { this };
				}
				Vector3 value2 = aB.GetValue(t);
				value2.Y = yValue;
				if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					if (t2 == 1f)
					{
						return new Triangle3D[2]
						{
							new Triangle3D(value2, _b, _c),
							new Triangle3D(value2, _c, _a)
						};
					}
					Vector3 value = aC.GetValue(t2);
					value.Y = yValue;
					return new Triangle3D[3]
					{
						new Triangle3D(value, _a, value2),
						new Triangle3D(value2, _b, _c),
						new Triangle3D(value, value2, _c)
					};
				}
				if (bC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					Vector3 value = bC.GetValue(t2);
					value.Y = yValue;
					return new Triangle3D[3]
					{
						new Triangle3D(value2, _b, value),
						new Triangle3D(_a, value2, value),
						new Triangle3D(_a, value, _c)
					};
				}
				throw new Exception("Slice Error");
			}
			if (bC.Intersects(plane, out t, out parallel, precisionDigits))
			{
				if (t == 1f)
				{
					return new Triangle3D[1] { this };
				}
				Vector3 value2 = bC.GetValue(t);
				value2.Y = yValue;
				if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					Vector3 value = aC.GetValue(t2);
					value.Y = yValue;
					return new Triangle3D[3]
					{
						new Triangle3D(value, _a, value2),
						new Triangle3D(value2, _b, _a),
						new Triangle3D(value, value2, _c)
					};
				}
				throw new Exception("Slice Error");
			}
			return new Triangle3D[1] { this };
		}

		public Triangle3D[] SliceVertical(float xValue, int precisionDigits)
		{
			Plane plane = DrawingTools.PlaneFromPointNormal(new Vector3(xValue, 0f, 0f), new Vector3(1f, 0f, 0f));
			LineF3D aB = AB;
			LineF3D aC = AC;
			LineF3D bC = BC;
			float t;
			bool parallel;
			float t2;
			if (aB.Intersects(plane, out t, out parallel, precisionDigits))
			{
				if (parallel)
				{
					return new Triangle3D[1] { this };
				}
				if (t == 0f)
				{
					if (bC.Intersects(plane, out t2, out parallel, precisionDigits))
					{
						if (t2 == 0f || t2 == 1f)
						{
							return new Triangle3D[1] { this };
						}
						Vector3 value = bC.GetValue(t2);
						value.X = xValue;
						return new Triangle3D[2]
						{
							new Triangle3D(_a, _b, value),
							new Triangle3D(_a, value, _c)
						};
					}
					return new Triangle3D[1] { this };
				}
				if (t == 1f)
				{
					if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
					{
						Vector3 value = aC.GetValue(t2);
						value.X = xValue;
						if (t2 == 0f || t2 == 1f)
						{
							return new Triangle3D[1]
							{
								new Triangle3D(aC.GetValue(0f), _b, value)
							};
						}
						return new Triangle3D[2]
						{
							new Triangle3D(_b, _c, value),
							new Triangle3D(_b, value, _a)
						};
					}
					return new Triangle3D[1] { this };
				}
				Vector3 value2 = aB.GetValue(t);
				value2.X = xValue;
				if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					if (t2 == 1f)
					{
						return new Triangle3D[2]
						{
							new Triangle3D(value2, _b, _c),
							new Triangle3D(value2, _c, _a)
						};
					}
					Vector3 value = aC.GetValue(t2);
					value.X = xValue;
					return new Triangle3D[3]
					{
						new Triangle3D(value, _a, value2),
						new Triangle3D(value2, _b, _c),
						new Triangle3D(value, value2, _c)
					};
				}
				if (bC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					Vector3 value = bC.GetValue(t2);
					value.X = xValue;
					return new Triangle3D[3]
					{
						new Triangle3D(value2, _b, value),
						new Triangle3D(_a, value2, value),
						new Triangle3D(_a, value, _c)
					};
				}
				throw new Exception("Slice Error");
			}
			if (bC.Intersects(plane, out t, out parallel, precisionDigits))
			{
				if (t == 1f)
				{
					return new Triangle3D[1] { this };
				}
				Vector3 value2 = bC.GetValue(t);
				value2.X = xValue;
				if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					Vector3 value = aC.GetValue(t2);
					value.X = xValue;
					return new Triangle3D[3]
					{
						new Triangle3D(value, _a, value2),
						new Triangle3D(value2, _b, _a),
						new Triangle3D(value, value2, _c)
					};
				}
				throw new Exception("Slice Error");
			}
			return new Triangle3D[1] { this };
		}

		public Triangle3D[] Slice(Plane plane, int precisionDigits)
		{
			plane.DotCoordinate(A);
			plane.DotCoordinate(B);
			plane.DotCoordinate(C);
			LineF3D aB = AB;
			LineF3D aC = AC;
			LineF3D bC = BC;
			float t;
			bool parallel;
			float t2;
			if (aB.Intersects(plane, out t, out parallel, precisionDigits))
			{
				if (parallel)
				{
					return new Triangle3D[1] { this };
				}
				if (t == 0f)
				{
					if (bC.Intersects(plane, out t2, out parallel, precisionDigits))
					{
						if (parallel)
						{
							throw new PrecisionException();
						}
						if (t2 == 0f || t2 == 1f)
						{
							return new Triangle3D[1] { this };
						}
						Vector3 value = bC.GetValue(t2);
						return new Triangle3D[2]
						{
							new Triangle3D(_a, _b, value),
							new Triangle3D(_a, value, _c)
						};
					}
					return new Triangle3D[1] { this };
				}
				if (t == 1f)
				{
					if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
					{
						if (parallel)
						{
							throw new PrecisionException();
						}
						Vector3 value = aC.GetValue(t2);
						if (t2 == 0f || t2 == 1f)
						{
							return new Triangle3D[1]
							{
								new Triangle3D(aC.GetValue(0f), _b, value)
							};
						}
						return new Triangle3D[2]
						{
							new Triangle3D(_b, _c, value),
							new Triangle3D(_b, value, _a)
						};
					}
					return new Triangle3D[1] { this };
				}
				Vector3 value2 = aB.GetValue(t);
				if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					if (t2 <= 0f || parallel)
					{
						throw new PrecisionException();
					}
					if (t2 == 1f)
					{
						return new Triangle3D[2]
						{
							new Triangle3D(value2, _b, _c),
							new Triangle3D(value2, _c, _a)
						};
					}
					Vector3 value = aC.GetValue(t2);
					return new Triangle3D[3]
					{
						new Triangle3D(value, _a, value2),
						new Triangle3D(value2, _b, _c),
						new Triangle3D(value, value2, _c)
					};
				}
				if (bC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					if (t2 <= 0f || t2 >= 1f || parallel)
					{
						throw new PrecisionException();
					}
					Vector3 value = bC.GetValue(t2);
					return new Triangle3D[3]
					{
						new Triangle3D(value2, _b, value),
						new Triangle3D(_a, value2, value),
						new Triangle3D(_a, value, _c)
					};
				}
				throw new PrecisionException();
			}
			if (bC.Intersects(plane, out t, out parallel, precisionDigits))
			{
				if (t == 0f || parallel)
				{
					throw new PrecisionException();
				}
				if (t == 1f)
				{
					return new Triangle3D[1] { this };
				}
				Vector3 value2 = bC.GetValue(t);
				if (aC.Intersects(plane, out t2, out parallel, precisionDigits))
				{
					if (t2 <= 0f || t2 >= 1f || parallel)
					{
						throw new PrecisionException();
					}
					Vector3 value = aC.GetValue(t2);
					return new Triangle3D[3]
					{
						new Triangle3D(value, _a, value2),
						new Triangle3D(value2, _a, _b),
						new Triangle3D(value, value2, _c)
					};
				}
				throw new PrecisionException();
			}
			return new Triangle3D[1] { this };
		}

		public override string ToString()
		{
			return ((object)A).ToString() + "-" + ((object)B).ToString() + "-" + ((object)C).ToString();
		}
	}
}
