using System.IO;
using DNA.IO.Compression.Zip.Compression;
using DNA.IO.Compression.Zip.Compression.Streams;

namespace DNA.IO.Compression
{
	public static class CompressionTools
	{
		private static bool UseHeaders;

		public static byte[] Compress(byte[] data)
		{
			Deflater defl = new Deflater(Deflater.DefaultCompression, !UseHeaders);
			MemoryStream memoryStream = new MemoryStream();
			DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(memoryStream, defl);
			BinaryWriter binaryWriter = new BinaryWriter(deflaterOutputStream);
			binaryWriter.Write(data.Length);
			binaryWriter.Write(data, 0, data.Length);
			binaryWriter.Flush();
			deflaterOutputStream.Finish();
			return memoryStream.ToArray();
		}

		public static byte[] Decompress(byte[] data)
		{
			MemoryStream baseInputStream = new MemoryStream(data);
			Inflater inf = new Inflater(!UseHeaders);
			InflaterInputStream input = new InflaterInputStream(baseInputStream, inf);
			BinaryReader binaryReader = new BinaryReader(input);
			int count = binaryReader.ReadInt32();
			return binaryReader.ReadBytes(count);
		}
	}
}
