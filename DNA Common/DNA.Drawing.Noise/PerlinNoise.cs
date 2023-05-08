using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Noise
{
	public class PerlinNoise
	{
		private static readonly int[][] s_gradientVectors = new int[32][]
		{
			new int[4] { 0, -1, -1, -1 },
			new int[4] { -1, 0, -1, -1 },
			new int[4] { 1, 0, -1, -1 },
			new int[4] { 0, 1, -1, -1 },
			new int[4] { -1, -1, 0, -1 },
			new int[4] { 1, -1, 0, -1 },
			new int[4] { -1, 1, 0, -1 },
			new int[4] { 1, 1, 0, -1 },
			new int[4] { 0, -1, 1, -1 },
			new int[4] { -1, 0, 1, -1 },
			new int[4] { 1, 0, 1, -1 },
			new int[4] { 0, 1, 1, -1 },
			new int[4] { 1, 1, 0, 1 },
			new int[4] { -1, 1, 0, 1 },
			new int[4] { 0, -1, -1, 1 },
			new int[4] { 0, -1, 1, 1 },
			new int[4] { -1, -1, -1, 0 },
			new int[4] { 1, -1, -1, 0 },
			new int[4] { -1, 1, -1, 0 },
			new int[4] { 1, 1, -1, 0 },
			new int[4] { -1, -1, 1, 0 },
			new int[4] { 1, -1, 1, 0 },
			new int[4] { -1, 1, 1, 0 },
			new int[4] { 1, 1, 1, 0 },
			new int[4] { -1, 0, -1, 1 },
			new int[4] { 1, 0, -1, 1 },
			new int[4] { 0, 1, -1, 1 },
			new int[4] { -1, -1, 0, 1 },
			new int[4] { 1, -1, 0, 1 },
			new int[4] { -1, 0, 1, 1 },
			new int[4] { 1, 0, 1, 1 },
			new int[4] { 0, 1, 1, 1 }
		};

		private static int[] _permute = new int[1024];

		public float ComputeNoise(float x)
		{
			int num = FastFloor(x);
			int num2 = num & 0xFF;
			x -= (float)num;
			float t = Fade(x);
			float a = (float)s_gradientVectors[_permute[num2] & 3][0] * x;
			float b = (float)s_gradientVectors[_permute[num2 + 1] & 3][0] * (x - 1f);
			return Lerp(t, a, b);
		}

		public float ComputeNoise(Vector2 v)
		{
			return ComputeNoise(v.X, v.Y);
		}

		public float ComputeNoise(float x, float y)
		{
			int num = FastFloor(x);
			int num2 = FastFloor(y);
			int num3 = num & 0xFF;
			int num4 = num2 & 0xFF;
			x -= (float)num;
			y -= (float)num2;
			float t = Fade(x);
			float t2 = Fade(y);
			int num5 = _permute[num3] + num4;
			int num6 = _permute[num3 + 1] + num4;
			float a = Grad(_permute[num5], x, y);
			float b = Grad(_permute[num6], x - 1f, y);
			float a2 = Grad(_permute[num5 + 1], x, y - 1f);
			float b2 = Grad(_permute[num6 + 1], x - 1f, y - 1f);
			float a3 = Lerp(t, a, b);
			float b3 = Lerp(t, a2, b2);
			return Lerp(t2, a3, b3);
		}

		public float ComputeNoise(Vector3 v)
		{
			return ComputeNoise(v.X, v.Y, v.Z);
		}

		public float ComputeNoise(float x, float y, float z)
		{
			int num = FastFloor(x);
			int num2 = FastFloor(y);
			int num3 = FastFloor(z);
			int num4 = num & 0xFF;
			int num5 = num2 & 0xFF;
			int num6 = num3 & 0xFF;
			x -= (float)num;
			y -= (float)num2;
			z -= (float)num3;
			float t = Fade(x);
			float t2 = Fade(y);
			float t3 = Fade(z);
			int num7 = _permute[num4] + num5;
			int num8 = _permute[num7] + num6;
			int num9 = _permute[num7 + 1] + num6;
			int num10 = _permute[num4 + 1] + num5;
			int num11 = _permute[num10] + num6;
			int num12 = _permute[num10 + 1] + num6;
			float a = Grad(_permute[num8], x, y, z);
			float b = Grad(_permute[num11], x - 1f, y, z);
			float a2 = Grad(_permute[num9], x, y - 1f, z);
			float b2 = Grad(_permute[num12], x - 1f, y - 1f, z);
			float a3 = Grad(_permute[num8 + 1], x, y, z - 1f);
			float b3 = Grad(_permute[num11 + 1], x - 1f, y, z - 1f);
			float a4 = Grad(_permute[num9 + 1], x, y - 1f, z - 1f);
			float b4 = Grad(_permute[num12 + 1], x - 1f, y - 1f, z - 1f);
			float a5 = Lerp(t, a, b);
			float b5 = Lerp(t, a2, b2);
			float a6 = Lerp(t, a3, b3);
			float b6 = Lerp(t, a4, b4);
			float a7 = Lerp(t2, a5, b5);
			float b7 = Lerp(t2, a6, b6);
			return Lerp(t3, a7, b7);
		}

		public float ComputeNoise(Vector4 v)
		{
			return ComputeNoise(v.X, v.Y, v.Z, v.W);
		}

		public float ComputeNoise(float x, float y, float z, float t)
		{
			int num = FastFloor(x);
			int num2 = FastFloor(y);
			int num3 = FastFloor(z);
			int num4 = FastFloor(t);
			int num5 = num & 0xFF;
			int num6 = num2 & 0xFF;
			int num7 = num3 & 0xFF;
			int num8 = num4 & 0xFF;
			x -= (float)num;
			y -= (float)num2;
			z -= (float)num3;
			t -= (float)num4;
			float t2 = Fade(x);
			float t3 = Fade(y);
			float t4 = Fade(z);
			float t5 = Fade(t);
			int num9 = _permute[num5] + num6;
			int num10 = _permute[num9] + num7;
			int num11 = _permute[num10] + num8;
			int num12 = _permute[num10 + 1] + num8;
			int num13 = _permute[num9 + 1] + num7;
			int num14 = _permute[num13] + num8;
			int num15 = _permute[num13 + 1] + num8;
			int num16 = _permute[num5 + 1] + num6;
			int num17 = _permute[num16] + num7;
			int num18 = _permute[num17] + num8;
			int num19 = _permute[num17 + 1] + num8;
			int num20 = _permute[num16 + 1] + num7;
			int num21 = _permute[num20] + num8;
			int num22 = _permute[num20 + 1] + num8;
			float a = Grad(_permute[num11], x, y, z, t);
			float a2 = Grad(_permute[num18], x - 1f, y, z, t);
			float a3 = Grad(_permute[num14], x, y - 1f, z, t);
			float a4 = Grad(_permute[num21], x - 1f, y - 1f, z, t);
			float a5 = Grad(_permute[num12], x, y, z - 1f, t);
			float a6 = Grad(_permute[num19], x - 1f, y, z - 1f, t);
			float a7 = Grad(_permute[num15], x, y - 1f, z - 1f, t);
			float a8 = Grad(_permute[num22], x - 1f, y - 1f, z - 1f, t);
			float b = Grad(_permute[num11 + 1], x, y, z, t - 1f);
			float b2 = Grad(_permute[num18 + 1], x - 1f, y, z, t - 1f);
			float b3 = Grad(_permute[num14 + 1], x, y - 1f, z, t - 1f);
			float b4 = Grad(_permute[num21 + 1], x - 1f, y - 1f, z, t - 1f);
			float b5 = Grad(_permute[num12 + 1], x, y, z - 1f, t - 1f);
			float b6 = Grad(_permute[num19 + 1], x - 1f, y, z - 1f, t - 1f);
			float b7 = Grad(_permute[num15 + 1], x, y - 1f, z - 1f, t - 1f);
			float b8 = Grad(_permute[num22 + 1], x - 1f, y - 1f, z - 1f, t - 1f);
			float b9 = Lerp(t5, a8, b8);
			float a9 = Lerp(t5, a4, b4);
			float b10 = Lerp(t5, a6, b6);
			float a10 = Lerp(t5, a2, b2);
			float b11 = Lerp(t5, a7, b7);
			float a11 = Lerp(t5, a3, b3);
			float b12 = Lerp(t5, a5, b5);
			float a12 = Lerp(t5, a, b);
			float b13 = Lerp(t4, a9, b9);
			float a13 = Lerp(t4, a10, b10);
			float b14 = Lerp(t4, a11, b11);
			float a14 = Lerp(t4, a12, b12);
			float b15 = Lerp(t3, a13, b13);
			float a15 = Lerp(t3, a14, b14);
			return Lerp(t2, a15, b15);
		}

		private static float Fade(float t)
		{
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}

		private static float Lerp(float t, float a, float b)
		{
			return a + t * (b - a);
		}

		private static int FastFloor(float x)
		{
			if (!(x > 0f))
			{
				return (int)x - 1;
			}
			return (int)x;
		}

		private static float Grad(int hash, float x, float y, float z, float t)
		{
			int num = hash & 0x1F;
			int[] array = s_gradientVectors[num];
			return x * (float)array[0] + y * (float)array[1] + z * (float)array[2] + t * (float)array[3];
		}

		private static float Grad(int hash, float x, float y, float z)
		{
			int num = hash & 0xF;
			int[] array = s_gradientVectors[num];
			return x * (float)array[0] + y * (float)array[1] + z * (float)array[2];
		}

		private static float Grad(int hash, float x, float y)
		{
			int num = hash & 7;
			int[] array = s_gradientVectors[num];
			return x * (float)array[0] + y * (float)array[1];
		}

		private void Initalize(Random r)
		{
			for (int i = 0; i < 256; i++)
			{
				_permute[256 + i] = (_permute[i] = r.Next(256));
			}
			for (int j = 0; j < 512; j++)
			{
				_permute[512 + j] = _permute[j];
			}
		}

		public PerlinNoise()
		{
			Initalize(new Random());
		}

		public PerlinNoise(Random r)
		{
			Initalize(r);
		}
	}
}
