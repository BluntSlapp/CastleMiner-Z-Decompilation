using System;
using System.IO;
using DNA.Threading;

namespace DNA.IO
{
	public static class StreamTools
	{
		public static void ReadFile(this Stream destination, string sourcePath)
		{
			using (FileStream source = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				destination.CopyStream(source);
			}
		}

		public static void CopyStream(this Stream destination, Stream source)
		{
			destination.CopyStream(source, source.Position, source.Length - source.Position, null);
		}

		public static void CopyStream(this Stream destination, Stream source, IProgressMonitor monitor)
		{
			destination.CopyStream(source, source.Position, source.Length - source.Position, monitor);
		}

		public static void CopyStream(this Stream destination, Stream source, long length)
		{
			destination.CopyStream(source, source.Position, length, null);
		}

		public static void CopyStream(this Stream destination, Stream source, long length, IProgressMonitor monitor)
		{
			destination.CopyStream(source, source.Position, length, monitor);
		}

		public static void ShiftStream(this Stream stream, int shiftBytes)
		{
			if (shiftBytes == 0)
			{
				return;
			}
			byte[] buffer = new byte[4096];
			int num = (int)(stream.Length - stream.Position);
			if (shiftBytes > 0)
			{
				long num2 = stream.Length;
				stream.SetLength(stream.Length + shiftBytes);
				while (num > 4096)
				{
					stream.Position = num2 - 4096;
					stream.Read(buffer, 0, 4096);
					stream.Position = num2 + shiftBytes;
					stream.Write(buffer, 0, 4096);
					num2 -= 4096;
					num -= num;
				}
				stream.Position = num2 - num;
				stream.Read(buffer, 0, num);
				stream.Position = num2 + shiftBytes;
				stream.Write(buffer, 0, num);
			}
			else
			{
				long num3 = stream.Position;
				while (num > 4096)
				{
					stream.Position = num3 - 4096;
					stream.Read(buffer, 0, 4096);
					stream.Position = num3 + shiftBytes;
					stream.Write(buffer, 0, 4096);
					num3 += 4096;
					num -= num;
				}
				stream.Position = num3 - num;
				stream.Read(buffer, 0, num);
				stream.Position = num3 + shiftBytes;
				stream.Write(buffer, 0, num);
				stream.SetLength(stream.Length + shiftBytes);
			}
		}

		public static void CopyStream(this Stream destination, Stream source, long startPosition, long length, IProgressMonitor progress)
		{
			if (progress != null)
			{
				progress.StatusText = "Copying Streams";
			}
			byte[] buffer = new byte[4096];
			long num = length;
			long num2 = 0L;
			int num3 = 0;
			int num4 = 0;
			int count = (int)((num < 4096) ? num : 4096);
			source.Position = startPosition;
			while (num > 0)
			{
				num3 = source.Read(buffer, 0, count);
				if (num3 == 0)
				{
					throw new Exception("Stream Terminated early");
				}
				destination.Write(buffer, 0, num3);
				num2 += num3;
				num -= num3;
				if (progress != null)
				{
					num4++;
					if (num4 == 10)
					{
						progress.Complete = Percentage.FromFraction((float)num2 / (float)length);
						num4 = 0;
					}
				}
				count = (int)((num < 4096) ? num : 4096);
			}
			if (progress != null)
			{
				progress.Complete = Percentage.FromFraction(1f);
			}
			destination.Flush();
		}
	}
}
