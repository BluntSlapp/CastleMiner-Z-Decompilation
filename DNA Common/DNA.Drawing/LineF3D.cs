using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public struct LineF3D
	{
		public Vector3 Start;

		public Vector3 End;

		public float LengthSquared
		{
			get
			{
				return (End - Start).LengthSquared();
			}
		}

		public Vector3 Center
		{
			get
			{
				return 0.5f * (Start + End);
			}
		}

		public Vector3 Direction
		{
			get
			{
				Vector3 result = End - Start;
				result.Normalize();
				return result;
			}
		}

		public float Length
		{
			get
			{
				return (End - Start).Length();
			}
		}

		public LineF3D(Vector3 start, Vector3 end)
		{
			Start = start;
			End = end;
		}

		public Vector3 GetValue(float t)
		{
			return (End - Start) * t + Start;
		}

		public float DistanceTo(Vector3 point)
		{
			Vector3 value = ClosetPointTo(point);
			return Vector3.Distance(value, point);
		}

		public Vector3 ClosetPointTo(Vector3 point)
		{
			Vector3 vector = End - Start;
			Vector3 vector2 = point - Start;
			float num = Vector3.Dot(vector, vector2);
			if (num <= 0f)
			{
				return Start;
			}
			float num2 = Vector3.Dot(vector2, vector2);
			if (num2 >= num)
			{
				return End;
			}
			float num3 = num2 / num;
			return Start + vector * num3;
		}

		public float? Intersects(Plane plane)
		{
			float num = plane.DotCoordinate(Start);
			float num2 = num - plane.DotCoordinate(End);
			if (num2 != 0f)
			{
				float num3 = num / num2;
				if ((double)num3 >= 0.0 && num3 <= 1f)
				{
					return num3;
				}
			}
			return null;
		}

		public bool Intersects(Plane plane, out float t, out bool parallel, int precisionDigits)
		{
			double num = plane.DotCoordinate(Start);
			double num2 = num - (double)plane.DotCoordinate(End);
			if (Math.Round(num2, precisionDigits) == 0.0)
			{
				t = float.NaN;
				parallel = true;
				return num == 0.0;
			}
			parallel = false;
			double value = num / num2;
			t = (float)Math.Round(value, precisionDigits);
			if ((double)t >= 0.0)
			{
				return t <= 1f;
			}
			return false;
		}

		public bool Intersects(Triangle3D triangle, out float t, out bool parallel)
		{
			Vector3 vector = triangle.B - triangle.A;
			Vector3 vector2 = triangle.C - triangle.A;
			Vector3 vector3 = Vector3.Cross(vector, vector2);
			Vector3 vector4 = End - Start;
			Vector3 vector5 = Start - triangle.A;
			float num = 0f - Vector3.Dot(vector3, vector5);
			float num2 = Vector3.Dot(vector3, vector4);
			if (num2 == 0f)
			{
				parallel = true;
				if (num == 0f)
				{
					t = 0f;
					return true;
				}
				t = float.NaN;
				return false;
			}
			parallel = false;
			t = num / num2;
			if (t < 0f || t > 1f)
			{
				return false;
			}
			Vector3 vector6 = Start + t * vector4;
			float num3 = Vector3.Dot(vector, vector);
			float num4 = Vector3.Dot(vector, vector2);
			float num5 = Vector3.Dot(vector2, vector2);
			Vector3 vector7 = vector6 - triangle.A;
			float num6 = Vector3.Dot(vector7, vector);
			float num7 = Vector3.Dot(vector7, vector2);
			float num8 = num4 * num4 - num3 * num5;
			float num9 = (num4 * num7 - num5 * num6) / num8;
			if ((double)num9 < 0.0 || (double)num9 > 1.0)
			{
				return false;
			}
			float num10 = (num4 * num6 - num3 * num7) / num8;
			if ((double)num10 < 0.0 || (double)(num9 + num10) > 1.0)
			{
				return false;
			}
			return true;
		}

		public int Intersects(Capsule capsule, out float? t1, out float? t2)
		{
			Ray ray = new Ray(Start, End - Start);
			int num = ray.Intersects(capsule, out t1, out t2);
			if ((t1.HasValue && t1 < 0f) || t1 > 1f)
			{
				num--;
				t1 = null;
			}
			if ((t2.HasValue && t2 < 0f) || t2 > 1f)
			{
				num--;
				t2 = null;
			}
			if (!t1.HasValue && t2.HasValue)
			{
				t1 = t2;
				t2 = null;
			}
			return num;
		}

		public bool Intersects(BoundingSphere sphere, out float t1, out float t2)
		{
			Vector3 center = sphere.Center;
			float radius = sphere.Radius;
			Vector3 vector = End - Start;
			Vector3 vector2 = Start - center;
			double num = vector.LengthSquared();
			double num2 = 2f * Vector3.Dot(vector, vector2);
			double num3 = center.LengthSquared() + Start.LengthSquared() - 2f * Vector3.Dot(center, Start) - radius * radius;
			double num4 = num2 * num2 - 4.0 * num * num3;
			if (num == 0.0 || num4 < 0.0)
			{
				t1 = 0f;
				t2 = 0f;
				return false;
			}
			double num5 = Math.Sqrt(num4);
			t1 = (float)((0.0 - num2 + num5) / (2.0 * num));
			t2 = (float)((0.0 - num2 - num5) / (2.0 * num));
			if (t1 < 0f || t1 > 1f)
			{
				if (t2 >= 0f && t2 <= 1f)
				{
					t1 = t2;
					return true;
				}
				return false;
			}
			return true;
		}

		public bool Intersects(Plane plane, out Vector3 pos, out bool parallel, int precisionDigits)
		{
			float t;
			bool result = Intersects(plane, out t, out parallel, precisionDigits);
			if (parallel)
			{
				pos = new Vector3(float.NaN, float.NaN, float.NaN);
				return result;
			}
			pos = GetValue(t);
			return result;
		}

		public bool Intersects(LineF3D line, out float intersection)
		{
			throw new NotImplementedException();
		}

		public bool Intersects(LineF3D line, out Vector3 intersection)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return "(" + Start.X + "," + Start.Y + "," + Start.Z + ")-(" + End.X + "," + End.Y + "," + End.Z + ")";
		}
	}
}
