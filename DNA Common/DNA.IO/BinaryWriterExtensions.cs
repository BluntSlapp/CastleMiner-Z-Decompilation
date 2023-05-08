using System.IO;
using Microsoft.Xna.Framework;

namespace DNA.IO
{
	public static class BinaryWriterExtensions
	{
		public static void Write(this BinaryWriter writer, Angle angle)
		{
			writer.Write(angle.Radians);
		}

		public static void Write(this BinaryWriter writer, Quaternion value)
		{
			writer.Write(value.X);
			writer.Write(value.Y);
			writer.Write(value.Z);
			writer.Write(value.W);
		}

		public static void Write(this BinaryWriter writer, Matrix matrix)
		{
			writer.Write(matrix.M11);
			writer.Write(matrix.M12);
			writer.Write(matrix.M13);
			writer.Write(matrix.M14);
			writer.Write(matrix.M21);
			writer.Write(matrix.M22);
			writer.Write(matrix.M23);
			writer.Write(matrix.M24);
			writer.Write(matrix.M31);
			writer.Write(matrix.M32);
			writer.Write(matrix.M33);
			writer.Write(matrix.M34);
			writer.Write(matrix.M41);
			writer.Write(matrix.M42);
			writer.Write(matrix.M43);
			writer.Write(matrix.M44);
		}

		public static void Write(this BinaryWriter writer, Vector3 value)
		{
			writer.Write(value.X);
			writer.Write(value.Y);
			writer.Write(value.Z);
		}

		public static void Write(this BinaryWriter writer, IntVector3 value)
		{
			writer.Write(value.X);
			writer.Write(value.Y);
			writer.Write(value.Z);
		}

		public static Angle ReadAngle(this BinaryReader reader)
		{
			return Angle.FromRadians(reader.ReadSingle());
		}

		public static Matrix ReadMatrix(this BinaryReader reader)
		{
			return new Matrix(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static Vector3 ReadVector3(this BinaryReader reader)
		{
			return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public static IntVector3 ReadIntVector3(this BinaryReader reader)
		{
			return new IntVector3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
		}

		public static Quaternion ReadQuaternion(this BinaryReader reader)
		{
			return new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}
	}
}
