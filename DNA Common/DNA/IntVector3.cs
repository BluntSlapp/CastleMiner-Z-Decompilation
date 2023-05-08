using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace DNA
{
	public struct IntVector3
	{
		public static IntVector3 Zero = new IntVector3(0, 0, 0);

		public int X;

		public int Y;

		public int Z;

		public IntVector3(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void SetValues(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Z);
		}

		public static IntVector3 Read(BinaryReader reader)
		{
			return new IntVector3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
		}

		public static IntVector3 Subtract(IntVector3 a, int x, int y, int z)
		{
			return new IntVector3(a.X - x, a.Y - y, a.Z - z);
		}

		public static IntVector3 Subtract(int x, int y, int z, IntVector3 b)
		{
			return new IntVector3(x - b.X, y - b.Y, z - b.Z);
		}

		public static IntVector3 Subtract(IntVector3 a, IntVector3 b)
		{
			return new IntVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static IntVector3 Add(IntVector3 a, int x, int y, int z)
		{
			return new IntVector3(a.X + x, a.Y + y, a.Z + z);
		}

		public static IntVector3 Add(IntVector3 a, IntVector3 b)
		{
			return new IntVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static IntVector3 FromVector3(Vector3 v)
		{
			return new IntVector3((int)Math.Floor(v.X), (int)Math.Floor(v.Y), (int)Math.Floor(v.Z));
		}

		public static Vector3 ToVector3(IntVector3 a)
		{
			return new Vector3(a.X, a.Y, a.Z);
		}

		public static bool operator ==(IntVector3 v1, IntVector3 v2)
		{
			if (v1.X == v2.X && v1.Y == v2.Y)
			{
				return v1.Z == v2.Z;
			}
			return false;
		}

		public static bool operator !=(IntVector3 v1, IntVector3 v2)
		{
			if (v1.X == v2.X && v1.Y == v2.Y)
			{
				return v1.Z != v2.Z;
			}
			return true;
		}

		public static bool operator ==(IntVector3 v1, Vector3 v2)
		{
			if ((float)v1.X == v2.X && (float)v1.Y == v2.Y)
			{
				return (float)v1.Z == v2.Z;
			}
			return false;
		}

		public static bool operator !=(IntVector3 v1, Vector3 v2)
		{
			if ((float)v1.X == v2.X && (float)v1.Y == v2.Y)
			{
				return (float)v1.Z != v2.Z;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is IntVector3)
			{
				return this == (IntVector3)obj;
			}
			if (obj is Vector3)
			{
				return this == (Vector3)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		public static Vector3 operator *(float v, IntVector3 vect)
		{
			return new Vector3((float)vect.X * v, (float)vect.Y * v, (float)vect.Z * v);
		}

		public static Vector3 operator *(IntVector3 vect, float v)
		{
			return new Vector3((float)vect.X * v, (float)vect.Y * v, (float)vect.Z * v);
		}

		public static Vector3 operator /(IntVector3 vect, float v)
		{
			return new Vector3((float)vect.X / v, (float)vect.Y / v, (float)vect.Z / v);
		}

		public static IntVector3 operator +(IntVector3 v1, IntVector3 v2)
		{
			return new IntVector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}

		public static IntVector3 operator -(IntVector3 v1, IntVector3 v2)
		{
			return new IntVector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}

		public static IntVector3 operator *(IntVector3 vect, int v)
		{
			return new IntVector3(vect.X * v, vect.Y * v, vect.Z * v);
		}

		public static IntVector3 operator /(IntVector3 vect, int v)
		{
			return new IntVector3(vect.X / v, vect.Y / v, vect.Z / v);
		}

		public static implicit operator Vector3(IntVector3 vect)
		{
			return new Vector3(vect.X, vect.Y, vect.Z);
		}

		public static explicit operator IntVector3(Vector3 vect)
		{
			return new IntVector3((int)Math.Floor(vect.X), (int)Math.Floor(vect.Y), (int)Math.Floor(vect.Z));
		}

		public bool Equals(IntVector3 other)
		{
			if (other.X == X && other.Y == Y)
			{
				return other.Z == Z;
			}
			return false;
		}

		public void SetToMin(IntVector3 a)
		{
			X = ((a.X < X) ? a.X : X);
			Y = ((a.Y < Y) ? a.Y : Y);
			Z = ((a.Z < Z) ? a.Z : Z);
		}

		public void SetToMax(IntVector3 a)
		{
			X = ((a.X > X) ? a.X : X);
			Y = ((a.Y > Y) ? a.Y : Y);
			Z = ((a.Z > Z) ? a.Z : Z);
		}

		public static IntVector3 Min(IntVector3 a, IntVector3 b)
		{
			return new IntVector3((a.X <= b.X) ? a.X : b.X, (a.Y <= b.Y) ? a.Y : b.Y, (a.Z <= b.Z) ? a.Z : b.Z);
		}

		public static IntVector3 Max(IntVector3 a, IntVector3 b)
		{
			return new IntVector3((a.X >= b.X) ? a.X : b.X, (a.Y >= b.Y) ? a.Y : b.Y, (a.Z >= b.Z) ? a.Z : b.Z);
		}

		public static void FillBoundingCorners(IntVector3 min, IntVector3 max, ref Vector3[] bounds)
		{
			bounds[0] = new Vector3(min.X, min.Y, min.Z);
			bounds[1] = new Vector3(min.X, min.Y, max.Z);
			bounds[2] = new Vector3(max.X, min.Y, min.Z);
			bounds[3] = new Vector3(max.X, min.Y, max.Z);
			bounds[4] = new Vector3(min.X, max.Y, min.Z);
			bounds[5] = new Vector3(min.X, max.Y, max.Z);
			bounds[6] = new Vector3(max.X, max.Y, min.Z);
			bounds[7] = new Vector3(max.X, max.Y, max.Z);
		}

		public static void FillBoundingCorners(IntVector3 min, IntVector3 max, ref IntVector3[] bounds)
		{
			bounds[0] = new IntVector3(min.X, min.Y, min.Z);
			bounds[1] = new IntVector3(min.X, min.Y, max.Z);
			bounds[2] = new IntVector3(max.X, min.Y, min.Z);
			bounds[3] = new IntVector3(max.X, min.Y, max.Z);
			bounds[4] = new IntVector3(min.X, max.Y, min.Z);
			bounds[5] = new IntVector3(min.X, max.Y, max.Z);
			bounds[6] = new IntVector3(max.X, max.Y, min.Z);
			bounds[7] = new IntVector3(max.X, max.Y, max.Z);
		}

		public override string ToString()
		{
			return X + "," + Y + "," + Z;
		}

		public int Dot(IntVector3 o)
		{
			return X * o.X + Y * o.Y + Z * o.Z;
		}
	}
}
