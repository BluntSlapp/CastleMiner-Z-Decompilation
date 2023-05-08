using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Noise
{
	public class SimplexNoise
	{
		private const float Sqrt3 = 1.73205078f;

		private const float Sqrt5 = 2.236068f;

		private static readonly int[][] s_gradientVectors3 = new int[12][]
		{
			new int[3] { 1, 1, 0 },
			new int[3] { -1, 1, 0 },
			new int[3] { 1, -1, 0 },
			new int[3] { -1, -1, 0 },
			new int[3] { 1, 0, 1 },
			new int[3] { -1, 0, 1 },
			new int[3] { 1, 0, -1 },
			new int[3] { -1, 0, -1 },
			new int[3] { 0, 1, 1 },
			new int[3] { 0, -1, 1 },
			new int[3] { 0, 1, -1 },
			new int[3] { 0, -1, -1 }
		};

		private static readonly int[][] s_gradientVectors4 = new int[32][]
		{
			new int[4] { 0, 1, 1, 1 },
			new int[4] { 0, 1, 1, -1 },
			new int[4] { 0, 1, -1, 1 },
			new int[4] { 0, 1, -1, -1 },
			new int[4] { 0, -1, 1, 1 },
			new int[4] { 0, -1, 1, -1 },
			new int[4] { 0, -1, -1, 1 },
			new int[4] { 0, -1, -1, -1 },
			new int[4] { 1, 0, 1, 1 },
			new int[4] { 1, 0, 1, -1 },
			new int[4] { 1, 0, -1, 1 },
			new int[4] { 1, 0, -1, -1 },
			new int[4] { -1, 0, 1, 1 },
			new int[4] { -1, 0, 1, -1 },
			new int[4] { -1, 0, -1, 1 },
			new int[4] { -1, 0, -1, -1 },
			new int[4] { 1, 1, 0, 1 },
			new int[4] { 1, 1, 0, -1 },
			new int[4] { 1, -1, 0, 1 },
			new int[4] { 1, -1, 0, -1 },
			new int[4] { -1, 1, 0, 1 },
			new int[4] { -1, 1, 0, -1 },
			new int[4] { -1, -1, 0, 1 },
			new int[4] { -1, -1, 0, -1 },
			new int[4] { 1, 1, 1, 0 },
			new int[4] { 1, 1, -1, 0 },
			new int[4] { 1, -1, 1, 0 },
			new int[4] { 1, -1, -1, 0 },
			new int[4] { -1, 1, 1, 0 },
			new int[4] { -1, 1, -1, 0 },
			new int[4] { -1, -1, 1, 0 },
			new int[4] { -1, -1, -1, 0 }
		};

		private int[] _permute = new int[512];

		private static readonly int[][] s_simplex;

		public float ComputeNoise(float x, float y)
		{
			float num = 0.3660254f;
			float num2 = (x + y) * num;
			int num3 = FastFloor(x + num2);
			int num4 = FastFloor(y + num2);
			float num5 = 0.211324871f;
			float num6 = (float)(num3 + num4) * num5;
			float num7 = (float)num3 - num6;
			float num8 = (float)num4 - num6;
			float num9 = x - num7;
			float num10 = y - num8;
			int num11;
			int num12;
			if (num9 > num10)
			{
				num11 = 1;
				num12 = 0;
			}
			else
			{
				num11 = 0;
				num12 = 1;
			}
			float num13 = num9 - (float)num11 + num5;
			float num14 = num10 - (float)num12 + num5;
			float num15 = num9 - 1f + 2f * num5;
			float num16 = num10 - 1f + 2f * num5;
			int num17 = num3 & 0xFF;
			int num18 = num4 & 0xFF;
			int num19 = _permute[num17 + _permute[num18]] % 12;
			int num20 = _permute[num17 + num11 + _permute[num18 + num12]] % 12;
			int num21 = _permute[num17 + 1 + _permute[num18 + 1]] % 12;
			float num22 = 0.5f - num9 * num9 - num10 * num10;
			float num23;
			if (num22 < 0f)
			{
				num23 = 0f;
			}
			else
			{
				num22 *= num22;
				num23 = num22 * num22 * Dot(s_gradientVectors3[num19], num9, num10);
			}
			float num24 = 0.5f - num13 * num13 - num14 * num14;
			float num25;
			if (num24 < 0f)
			{
				num25 = 0f;
			}
			else
			{
				num24 *= num24;
				num25 = num24 * num24 * Dot(s_gradientVectors3[num20], num13, num14);
			}
			float num26 = 0.5f - num15 * num15 - num16 * num16;
			float num27;
			if (num26 < 0f)
			{
				num27 = 0f;
			}
			else
			{
				num26 *= num26;
				num27 = num26 * num26 * Dot(s_gradientVectors3[num21], num15, num16);
			}
			return 70f * (num23 + num25 + num27);
		}

		public float ComputeNoise(float x, float y, float z)
		{
			float num = 1f / 3f;
			float num2 = (x + y + z) * num;
			int num3 = FastFloor(x + num2);
			int num4 = FastFloor(y + num2);
			int num5 = FastFloor(z + num2);
			float num6 = 1f / 6f;
			float num7 = (float)(num3 + num4 + num5) * num6;
			float num8 = (float)num3 - num7;
			float num9 = (float)num4 - num7;
			float num10 = (float)num5 - num7;
			float num11 = x - num8;
			float num12 = y - num9;
			float num13 = z - num10;
			int num14;
			int num15;
			int num16;
			int num17;
			int num18;
			int num19;
			if (num11 >= num12)
			{
				if (num12 >= num13)
				{
					num14 = 1;
					num15 = 0;
					num16 = 0;
					num17 = 1;
					num18 = 1;
					num19 = 0;
				}
				else if (num11 >= num13)
				{
					num14 = 1;
					num15 = 0;
					num16 = 0;
					num17 = 1;
					num18 = 0;
					num19 = 1;
				}
				else
				{
					num14 = 0;
					num15 = 0;
					num16 = 1;
					num17 = 1;
					num18 = 0;
					num19 = 1;
				}
			}
			else if (num12 < num13)
			{
				num14 = 0;
				num15 = 0;
				num16 = 1;
				num17 = 0;
				num18 = 1;
				num19 = 1;
			}
			else if (num11 < num13)
			{
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 0;
				num18 = 1;
				num19 = 1;
			}
			else
			{
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 1;
				num18 = 1;
				num19 = 0;
			}
			float num20 = num11 - (float)num14 + num6;
			float num21 = num12 - (float)num15 + num6;
			float num22 = num13 - (float)num16 + num6;
			float num23 = num11 - (float)num17 + 2f * num6;
			float num24 = num12 - (float)num18 + 2f * num6;
			float num25 = num13 - (float)num19 + 2f * num6;
			float num26 = num11 - 1f + 3f * num6;
			float num27 = num12 - 1f + 3f * num6;
			float num28 = num13 - 1f + 3f * num6;
			int num29 = num3 & 0xFF;
			int num30 = num4 & 0xFF;
			int num31 = num5 & 0xFF;
			int num32 = _permute[num29 + _permute[num30 + _permute[num31]]] % 12;
			int num33 = _permute[num29 + num14 + _permute[num30 + num15 + _permute[num31 + num16]]] % 12;
			int num34 = _permute[num29 + num17 + _permute[num30 + num18 + _permute[num31 + num19]]] % 12;
			int num35 = _permute[num29 + 1 + _permute[num30 + 1 + _permute[num31 + 1]]] % 12;
			float num36 = 0.6f - num11 * num11 - num12 * num12 - num13 * num13;
			float num37;
			if (num36 < 0f)
			{
				num37 = 0f;
			}
			else
			{
				num36 *= num36;
				num37 = num36 * num36 * Dot(s_gradientVectors3[num32], num11, num12, num13);
			}
			float num38 = 0.6f - num20 * num20 - num21 * num21 - num22 * num22;
			float num39;
			if (num38 < 0f)
			{
				num39 = 0f;
			}
			else
			{
				num38 *= num38;
				num39 = num38 * num38 * Dot(s_gradientVectors3[num33], num20, num21, num22);
			}
			float num40 = 0.6f - num23 * num23 - num24 * num24 - num25 * num25;
			float num41;
			if (num40 < 0f)
			{
				num41 = 0f;
			}
			else
			{
				num40 *= num40;
				num41 = num40 * num40 * Dot(s_gradientVectors3[num34], num23, num24, num25);
			}
			float num42 = 0.6f - num26 * num26 - num27 * num27 - num28 * num28;
			float num43;
			if (num42 < 0f)
			{
				num43 = 0f;
			}
			else
			{
				num42 *= num42;
				num43 = num42 * num42 * Dot(s_gradientVectors3[num35], num26, num27, num28);
			}
			return 32f * (num37 + num39 + num41 + num43);
		}

		public float ComputeNoise(float x, float y, float z, float w)
		{
			float num = 0.309017f;
			float num2 = 0.1381966f;
			float num3 = (x + y + z + w) * num;
			int num4 = FastFloor(x + num3);
			int num5 = FastFloor(y + num3);
			int num6 = FastFloor(z + num3);
			int num7 = FastFloor(w + num3);
			float num8 = (float)(num4 + num5 + num6 + num7) * num2;
			float num9 = (float)num4 - num8;
			float num10 = (float)num5 - num8;
			float num11 = (float)num6 - num8;
			float num12 = (float)num7 - num8;
			float num13 = x - num9;
			float num14 = y - num10;
			float num15 = z - num11;
			float num16 = w - num12;
			int num17 = ((num13 > num14) ? 32 : 0);
			int num18 = ((num13 > num15) ? 16 : 0);
			int num19 = ((num14 > num15) ? 8 : 0);
			int num20 = ((num13 > num16) ? 4 : 0);
			int num21 = ((num14 > num16) ? 2 : 0);
			int num22 = ((num15 > num16) ? 1 : 0);
			int num23 = num17 + num18 + num19 + num20 + num21 + num22;
			int num24 = ((s_simplex[num23][0] >= 3) ? 1 : 0);
			int num25 = ((s_simplex[num23][1] >= 3) ? 1 : 0);
			int num26 = ((s_simplex[num23][2] >= 3) ? 1 : 0);
			int num27 = ((s_simplex[num23][3] >= 3) ? 1 : 0);
			int num28 = ((s_simplex[num23][0] >= 2) ? 1 : 0);
			int num29 = ((s_simplex[num23][1] >= 2) ? 1 : 0);
			int num30 = ((s_simplex[num23][2] >= 2) ? 1 : 0);
			int num31 = ((s_simplex[num23][3] >= 2) ? 1 : 0);
			int num32 = ((s_simplex[num23][0] >= 1) ? 1 : 0);
			int num33 = ((s_simplex[num23][1] >= 1) ? 1 : 0);
			int num34 = ((s_simplex[num23][2] >= 1) ? 1 : 0);
			int num35 = ((s_simplex[num23][3] >= 1) ? 1 : 0);
			float num36 = num13 - (float)num24 + num2;
			float num37 = num14 - (float)num25 + num2;
			float num38 = num15 - (float)num26 + num2;
			float num39 = num16 - (float)num27 + num2;
			float num40 = num13 - (float)num28 + 2f * num2;
			float num41 = num14 - (float)num29 + 2f * num2;
			float num42 = num15 - (float)num30 + 2f * num2;
			float num43 = num16 - (float)num31 + 2f * num2;
			float num44 = num13 - (float)num32 + 3f * num2;
			float num45 = num14 - (float)num33 + 3f * num2;
			float num46 = num15 - (float)num34 + 3f * num2;
			float num47 = num16 - (float)num35 + 3f * num2;
			float num48 = num13 - 1f + 4f * num2;
			float num49 = num14 - 1f + 4f * num2;
			float num50 = num15 - 1f + 4f * num2;
			float num51 = num16 - 1f + 4f * num2;
			int num52 = num4 & 0xFF;
			int num53 = num5 & 0xFF;
			int num54 = num6 & 0xFF;
			int num55 = num7 & 0xFF;
			int num56 = _permute[num52 + _permute[num53 + _permute[num54 + _permute[num55]]]] % 32;
			int num57 = _permute[num52 + num24 + _permute[num53 + num25 + _permute[num54 + num26 + _permute[num55 + num27]]]] % 32;
			int num58 = _permute[num52 + num28 + _permute[num53 + num29 + _permute[num54 + num30 + _permute[num55 + num31]]]] % 32;
			int num59 = _permute[num52 + num32 + _permute[num53 + num33 + _permute[num54 + num34 + _permute[num55 + num35]]]] % 32;
			int num60 = _permute[num52 + 1 + _permute[num53 + 1 + _permute[num54 + 1 + _permute[num55 + 1]]]] % 32;
			float num61 = 0.6f - num13 * num13 - num14 * num14 - num15 * num15 - num16 * num16;
			float num62;
			if (num61 < 0f)
			{
				num62 = 0f;
			}
			else
			{
				num61 *= num61;
				num62 = num61 * num61 * Dot(s_gradientVectors4[num56], num13, num14, num15, num16);
			}
			float num63 = 0.6f - num36 * num36 - num37 * num37 - num38 * num38 - num39 * num39;
			float num64;
			if (num63 < 0f)
			{
				num64 = 0f;
			}
			else
			{
				num63 *= num63;
				num64 = num63 * num63 * Dot(s_gradientVectors4[num57], num36, num37, num38, num39);
			}
			float num65 = 0.6f - num40 * num40 - num41 * num41 - num42 * num42 - num43 * num43;
			float num66;
			if (num65 < 0f)
			{
				num66 = 0f;
			}
			else
			{
				num65 *= num65;
				num66 = num65 * num65 * Dot(s_gradientVectors4[num58], num40, num41, num42, num43);
			}
			float num67 = 0.6f - num44 * num44 - num45 * num45 - num46 * num46 - num47 * num47;
			float num68;
			if (num67 < 0f)
			{
				num68 = 0f;
			}
			else
			{
				num67 *= num67;
				num68 = num67 * num67 * Dot(s_gradientVectors4[num59], num44, num45, num46, num47);
			}
			float num69 = 0.6f - num48 * num48 - num49 * num49 - num50 * num50 - num51 * num51;
			float num70;
			if (num69 < 0f)
			{
				num70 = 0f;
			}
			else
			{
				num69 *= num69;
				num70 = num69 * num69 * Dot(s_gradientVectors4[num60], num48, num49, num50, num51);
			}
			return 27f * (num62 + num64 + num66 + num68 + num70);
		}

		public float ComputeNoise(Vector2 v)
		{
			return ComputeNoise(v.X, v.Y);
		}

		public float ComputeNoise(Vector3 v)
		{
			return ComputeNoise(v.X, v.Y, v.Z);
		}

		public float ComputeNoise(Vector4 v)
		{
			return ComputeNoise(v.X, v.Y, v.Z, v.W);
		}

		private static int FastFloor(float x)
		{
			if (!(x > 0f))
			{
				return (int)x - 1;
			}
			return (int)x;
		}

		private static float Dot(int[] g, float x, float y)
		{
			return (float)g[0] * x + (float)g[1] * y;
		}

		private static float Dot(int[] g, float x, float y, float z)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z;
		}

		private static float Dot(int[] g, float x, float y, float z, float w)
		{
			return (float)g[0] * x + (float)g[1] * y + (float)g[2] * z + (float)g[3] * w;
		}

		private void Initalize(Random r)
		{
			for (int i = 0; i < 256; i++)
			{
				_permute[256 + i] = (_permute[i] = r.Next(256));
			}
		}

		public SimplexNoise()
		{
			Initalize(new Random());
		}

		public SimplexNoise(Random r)
		{
			Initalize(r);
		}

		static SimplexNoise()
		{
			int[][] array = new int[64][];
			array[0] = new int[4] { 0, 1, 2, 3 };
			array[1] = new int[4] { 0, 1, 3, 2 };
			int[] array2 = (array[2] = new int[4]);
			array[3] = new int[4] { 0, 2, 3, 1 };
			int[] array3 = (array[4] = new int[4]);
			int[] array4 = (array[5] = new int[4]);
			int[] array5 = (array[6] = new int[4]);
			array[7] = new int[4] { 1, 2, 3, 0 };
			array[8] = new int[4] { 0, 2, 1, 3 };
			int[] array6 = (array[9] = new int[4]);
			array[10] = new int[4] { 0, 3, 1, 2 };
			array[11] = new int[4] { 0, 3, 2, 1 };
			int[] array7 = (array[12] = new int[4]);
			int[] array8 = (array[13] = new int[4]);
			int[] array9 = (array[14] = new int[4]);
			array[15] = new int[4] { 1, 3, 2, 0 };
			int[] array10 = (array[16] = new int[4]);
			int[] array11 = (array[17] = new int[4]);
			int[] array12 = (array[18] = new int[4]);
			int[] array13 = (array[19] = new int[4]);
			int[] array14 = (array[20] = new int[4]);
			int[] array15 = (array[21] = new int[4]);
			int[] array16 = (array[22] = new int[4]);
			int[] array17 = (array[23] = new int[4]);
			array[24] = new int[4] { 1, 2, 0, 3 };
			int[] array18 = (array[25] = new int[4]);
			array[26] = new int[4] { 1, 3, 0, 2 };
			int[] array19 = (array[27] = new int[4]);
			int[] array20 = (array[28] = new int[4]);
			int[] array21 = (array[29] = new int[4]);
			array[30] = new int[4] { 2, 3, 0, 1 };
			array[31] = new int[4] { 2, 3, 1, 0 };
			array[32] = new int[4] { 1, 0, 2, 3 };
			array[33] = new int[4] { 1, 0, 3, 2 };
			int[] array22 = (array[34] = new int[4]);
			int[] array23 = (array[35] = new int[4]);
			int[] array24 = (array[36] = new int[4]);
			array[37] = new int[4] { 2, 0, 3, 1 };
			int[] array25 = (array[38] = new int[4]);
			array[39] = new int[4] { 2, 1, 3, 0 };
			int[] array26 = (array[40] = new int[4]);
			int[] array27 = (array[41] = new int[4]);
			int[] array28 = (array[42] = new int[4]);
			int[] array29 = (array[43] = new int[4]);
			int[] array30 = (array[44] = new int[4]);
			int[] array31 = (array[45] = new int[4]);
			int[] array32 = (array[46] = new int[4]);
			int[] array33 = (array[47] = new int[4]);
			array[48] = new int[4] { 2, 0, 1, 3 };
			int[] array34 = (array[49] = new int[4]);
			int[] array35 = (array[50] = new int[4]);
			int[] array36 = (array[51] = new int[4]);
			array[52] = new int[4] { 3, 0, 1, 2 };
			array[53] = new int[4] { 3, 0, 2, 1 };
			int[] array37 = (array[54] = new int[4]);
			array[55] = new int[4] { 3, 1, 2, 0 };
			array[56] = new int[4] { 2, 1, 0, 3 };
			int[] array38 = (array[57] = new int[4]);
			int[] array39 = (array[58] = new int[4]);
			int[] array40 = (array[59] = new int[4]);
			array[60] = new int[4] { 3, 1, 0, 2 };
			int[] array41 = (array[61] = new int[4]);
			array[62] = new int[4] { 3, 2, 0, 1 };
			array[63] = new int[4] { 3, 2, 1, 0 };
			s_simplex = array;
		}
	}
}
