using System.IO;

namespace DNA.Security.Cryptography.Utilities.IO
{
	public sealed class Streams
	{
		private const int BufferSize = 512;

		private Streams()
		{
		}

		public static void Drain(Stream inStr)
		{
			byte[] array = new byte[512];
			while (inStr.Read(array, 0, array.Length) > 0)
			{
			}
		}

		public static byte[] ReadAll(Stream inStr)
		{
			MemoryStream memoryStream = new MemoryStream();
			PipeAll(inStr, memoryStream);
			return memoryStream.ToArray();
		}

		public static int ReadFully(Stream inStr, byte[] buf)
		{
			return ReadFully(inStr, buf, 0, buf.Length);
		}

		public static int ReadFully(Stream inStr, byte[] buf, int off, int len)
		{
			int i;
			int num;
			for (i = 0; i < len; i += num)
			{
				num = inStr.Read(buf, off + i, len - i);
				if (num < 1)
				{
					break;
				}
			}
			return i;
		}

		public static void PipeAll(Stream inStr, Stream outStr)
		{
			byte[] array = new byte[512];
			int count;
			while ((count = inStr.Read(array, 0, array.Length)) > 0)
			{
				outStr.Write(array, 0, count);
			}
		}
	}
}
