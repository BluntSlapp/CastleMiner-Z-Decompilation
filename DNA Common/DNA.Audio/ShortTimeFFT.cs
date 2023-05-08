using System;

namespace DNA.Audio
{
	public class ShortTimeFFT
	{
		private int FrameSize;

		private int[] _revTable;

		public ShortTimeFFT(int frameSize)
		{
			FrameSize = frameSize;
			_revTable = new int[frameSize * 2];
			for (int i = 2; i < 2 * FrameSize - 2; i += 2)
			{
				int num = 2;
				int num2 = 0;
				while (num < frameSize * 2)
				{
					if ((i & num) != 0)
					{
						num2++;
					}
					num2 <<= 1;
					num <<= 1;
				}
				if (i < num2)
				{
					_revTable[i] = num2;
					_revTable[num2] = i;
					_revTable[i + 1] = num2 + 1;
					_revTable[num2 + 1] = i + 1;
				}
			}
		}

		public void Transform(float[] fftBuffer, int sign)
		{
			int frameSize = FrameSize;
			int num = frameSize << 1;
			for (int i = 2; i < num - 2; i += 2)
			{
				int num2 = _revTable[i];
				if (i < num2)
				{
					float num3 = fftBuffer[i];
					fftBuffer[i] = fftBuffer[num2];
					fftBuffer[num2] = num3;
					num3 = fftBuffer[i + 1];
					fftBuffer[i + 1] = fftBuffer[num2 + 1];
					fftBuffer[num2 + 1] = num3;
				}
			}
			int num4 = (int)(Math.Log(frameSize) / Math.Log(2.0) + 0.5);
			int j = 0;
			int num5 = 2;
			for (; j < num4; j++)
			{
				num5 <<= 1;
				int num6 = num5 >> 1;
				float num7 = 1f;
				float num8 = 0f;
				float num9 = (float)(Math.PI / (double)(num6 >> 1));
				float num10 = (float)Math.Cos(num9);
				float num11 = (float)sign * (float)Math.Sin(num9);
				for (int k = 0; k < num6; k += 2)
				{
					float num13;
					for (int l = k; l < num; l += num5)
					{
						int num12 = l + num6;
						num13 = fftBuffer[num12] * num7 - fftBuffer[num12 + 1] * num8;
						float num14 = fftBuffer[num12] * num8 + fftBuffer[num12 + 1] * num7;
						fftBuffer[num12] = fftBuffer[l] - num13;
						fftBuffer[num12 + 1] = fftBuffer[l + 1] - num14;
						fftBuffer[l] += num13;
						fftBuffer[l + 1] += num14;
					}
					num13 = num7 * num10 - num8 * num11;
					num8 = num7 * num11 + num8 * num10;
					num7 = num13;
				}
			}
		}
	}
}
