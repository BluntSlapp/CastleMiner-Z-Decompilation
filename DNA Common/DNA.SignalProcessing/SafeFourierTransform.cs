using System;

namespace DNA.SignalProcessing
{
	public class SafeFourierTransform : ISpectralTransform
	{
		public virtual void Transform(float[] data)
		{
			RealFT(data, 1);
			float num = 1f / (float)data.Length;
			for (int i = 0; i < data.Length; i++)
			{
				data[i] *= num;
			}
		}

		public virtual void InverseTransform(float[] data)
		{
			RealFT(data, -1);
			for (int i = 0; i < data.Length; i++)
			{
				data[i] *= 2f;
			}
		}

		private static void RealFT(float[] data, int isign)
		{
			uint num = (uint)data.Length;
			float num2 = 0.5f;
			double num3 = Math.PI / (double)(num >> 1);
			float num4;
			if (isign == 1)
			{
				num4 = -0.5f;
				Four1(data, 1);
			}
			else
			{
				num4 = 0.5f;
				num3 = 0.0 - num3;
			}
			double num5 = Math.Sin(0.5 * num3);
			double num6 = -2.0 * num5 * num5;
			double num7 = Math.Sin(num3);
			double num8 = 1.0 + num6;
			double num9 = num7;
			uint num10 = num + 3;
			float num16;
			for (uint num11 = 2u; num11 <= num >> 2; num11++)
			{
				uint num12 = num11 + num11 - 1;
				uint num13 = 1 + num12;
				uint num14 = num10 - num13;
				uint num15 = 1 + num14;
				num12--;
				num13--;
				num14--;
				num15--;
				num16 = num2 * (data[num12] + data[num14]);
				float num17 = num2 * (data[num13] - data[num15]);
				float num18 = (0f - num4) * (data[num13] + data[num15]);
				float num19 = num4 * (data[num12] - data[num14]);
				data[num12] = (float)((double)num16 + num8 * (double)num18 - num9 * (double)num19);
				data[num13] = (float)((double)num17 + num8 * (double)num19 + num9 * (double)num18);
				data[num14] = (float)((double)num16 - num8 * (double)num18 + num9 * (double)num19);
				data[num15] = (float)((double)(0f - num17) + num8 * (double)num19 + num9 * (double)num18);
				num8 = (num5 = num8) * num6 - num9 * num7 + num8;
				num9 = num9 * num6 + num5 * num7 + num9;
			}
			num16 = data[0];
			data[0] = num16 + data[1];
			data[1] = num16 - data[1];
			if (isign == -1)
			{
				data[0] *= num2;
				data[1] *= num2;
				Four1(data, -1);
			}
		}

		private static void Four1(float[] data, int isign)
		{
			if (!data.Length.IsPowerOf2())
			{
				throw new ArgumentException("Array Lenght must be power of 2");
			}
			uint num = (uint)data.Length >> 1;
			uint num2 = num << 1;
			uint num3 = 1u;
			for (uint num4 = 1u; num4 < num2; num4 += 2)
			{
				if (num3 > num4)
				{
					float num5 = data[num3 - 1];
					data[num3 - 1] = data[num4 - 1];
					data[num4 - 1] = num5;
					num5 = data[num3];
					data[num3] = data[num4];
					data[num4] = num5;
				}
				uint num6 = num;
				while (num6 >= 2 && num3 > num6)
				{
					num3 -= num6;
					num6 >>= 1;
				}
				num3 += num6;
			}
			uint num7 = 2u;
			while (num2 > num7)
			{
				uint num8 = num7 << 1;
				double num9 = (double)isign * (Math.PI * 4.0 / (double)num7);
				double num10 = Math.Sin(0.5 * num9);
				double num11 = -2.0 * num10 * num10;
				double num12 = Math.Sin(num9);
				double num13 = 1.0;
				double num14 = 0.0;
				for (uint num6 = 1u; num6 < num7; num6 += 2)
				{
					for (uint num4 = num6; num4 <= num2; num4 += num8)
					{
						num3 = num4 + num7;
						float num15 = (float)(num13 * (double)data[num3 - 1] - num14 * (double)data[num3]);
						float num16 = (float)(num13 * (double)data[num3] + num14 * (double)data[num3 - 1]);
						data[num3 - 1] = data[num4 - 1] - num15;
						data[num3] = data[num4] - num16;
						data[num4 - 1] += num15;
						data[num4] += num16;
					}
					num13 = (num10 = num13) * num11 - num14 * num12 + num13;
					num14 = num14 * num11 + num10 * num12 + num14;
				}
				num7 = num8;
			}
		}

		public void Transform(float[,] data)
		{
			throw new NotSupportedException();
		}

		public void InverseTransform(float[,] data)
		{
			throw new NotSupportedException();
		}

		public void Transform(float[,,] data)
		{
			throw new NotSupportedException();
		}

		public void InverseTransform(float[,,] data)
		{
			throw new NotSupportedException();
		}
	}
}
