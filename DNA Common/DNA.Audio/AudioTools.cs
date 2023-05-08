using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA.Audio
{
	public static class AudioTools
	{
		public static void ShortTimeFourierTransform(float[] fftBuffer, int fftFrameSize, int sign)
		{
			int num = fftFrameSize << 1;
			for (int i = 2; i < 2 * fftFrameSize - 2; i += 2)
			{
				int num2 = 2;
				int num3 = 0;
				while (num2 < num)
				{
					if ((i & num2) != 0)
					{
						num3++;
					}
					num3 <<= 1;
					num2 <<= 1;
				}
				if (i < num3)
				{
					float num4 = fftBuffer[i];
					fftBuffer[i] = fftBuffer[num3];
					fftBuffer[num3] = num4;
					num4 = fftBuffer[i + 1];
					fftBuffer[i + 1] = fftBuffer[num3 + 1];
					fftBuffer[num3 + 1] = num4;
				}
			}
			int num5 = (int)(Math.Log(fftFrameSize) / Math.Log(2.0) + 0.5);
			int j = 0;
			int num6 = 2;
			for (; j < num5; j++)
			{
				num6 <<= 1;
				int num7 = num6 >> 1;
				float num8 = 1f;
				float num9 = 0f;
				float num10 = (float)(Math.PI / (double)(num7 >> 1));
				float num11 = (float)Math.Cos(num10);
				float num12 = (float)sign * (float)Math.Sin(num10);
				for (int num3 = 0; num3 < num7; num3 += 2)
				{
					float num14;
					for (int i = num3; i < num; i += num6)
					{
						int num13 = i + num7;
						num14 = fftBuffer[num13] * num8 - fftBuffer[num13 + 1] * num9;
						float num15 = fftBuffer[num13] * num9 + fftBuffer[num13 + 1] * num8;
						fftBuffer[num13] = fftBuffer[i] - num14;
						fftBuffer[num13 + 1] = fftBuffer[i + 1] - num15;
						fftBuffer[i] += num14;
						fftBuffer[i + 1] += num15;
					}
					num14 = num8 * num11 - num9 * num12;
					num9 = num8 * num12 + num9 * num11;
					num8 = num14;
				}
			}
		}

		public static Microphone GetMic(SignedInGamer gamer)
		{
			foreach (Microphone item in Microphone.get_All())
			{
				if (gamer.IsHeadset(item))
				{
					try
					{
						MicrophoneState state = item.State;
					}
					catch (NoMicrophoneConnectedException)
					{
						return null;
					}
					return item;
				}
			}
			return null;
		}
	}
}
