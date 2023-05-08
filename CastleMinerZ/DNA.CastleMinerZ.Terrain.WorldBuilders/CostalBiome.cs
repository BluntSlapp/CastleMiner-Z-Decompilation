using System;
using DNA.Drawing.Noise;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	internal class CostalBiome : Biome
	{
		private const int BedRockVariance = 3;

		private const int DirtThickness = 3;

		private const int CaveStart = 4;

		private const int CaveEnd = 64;

		private const int GroundPlane = 32;

		private const int MaxHillHeight = 32;

		private const int MaxValleyDepth = 20;

		private const float worldScale = 0.009375f;

		private const float caveDensity = 0.0625f;

		private const float coalDensity = 0.3125f;

		private const float ironDensity = 0.25f;

		private const float goldDensity = 0.25f;

		private const float diamondDensity = 0.375f;

		private const float lavaLakeDensity = 0.0625f;

		private const float lavaSpecDensity = 0.25f;

		private const float waterLevel = 32f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public CostalBiome(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new PerlinNoise(new Random(worldInfo.Seed));
			WaterDepth = 36f;
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float terrainblender)
		{
			terrain.WaterLevel = -31.5f;
			int num = 1;
			float num2 = 0f;
			int num3 = 4;
			for (int i = 0; i < num3; i++)
			{
				num2 += _noiseFunction.ComputeNoise(0.009375f * (float)worldX * (float)num, 0.009375f * (float)worldZ * (float)num) / (float)num;
				num *= 2;
			}
			num2 += _noiseFunction.ComputeNoise(0.00234375f * (float)worldX, 0.00234375f * (float)worldZ);
			num2 += _noiseFunction.ComputeNoise(0.0046875f * (float)worldX, 0.0046875f * (float)worldZ) / 2f;
			bool flag = false;
			float num4 = 32f + num2 * 32f;
			float num5 = 8f;
			if (num4 <= 32f + num5 && num4 >= 32f - num5)
			{
				float num6 = (num4 - 32f) / 4f + 32f;
				float num7 = Math.Abs(num4 - 32f) / num5;
				if (num7 < 0.75f)
				{
					flag = true;
					num7 = 0f;
				}
				else
				{
					num7 -= 0.75f;
					num7 *= 4f;
				}
				num4 = num4 * num7 + num6 * (1f - num7);
			}
			int num8 = (int)num4;
			if (num8 < 0)
			{
				num8 = 0;
			}
			int num9 = num8 - 3;
			num2 = _noiseFunction.ComputeNoise((float)worldX * 0.5f, (float)worldZ * 0.5f);
			int num10 = (int)(num2 * 4f);
			num2 = (num2 + 1f) / 2f;
			int num11 = 1 + (int)(num2 * 3f);
			num2 = _noiseFunction.ComputeNoise((float)worldX * 0.0625f, (float)worldZ * 0.0625f) * 10f;
			int num12 = (int)num2;
			if (num12 > 1)
			{
				num12 = 4;
			}
			for (int j = 0; j < 128; j++)
			{
				if (terrain._resetRequested)
				{
					break;
				}
				int num13 = j + minY;
				IntVector3 intVector = new IntVector3(worldX, num13, worldZ);
				int num14 = terrain.MakeIndexFromWorldIndexVector(intVector);
				if (j > num8)
				{
					continue;
				}
				if (j < num11)
				{
					terrain._blocks[num14] = Biome.bedrockBlock;
					continue;
				}
				terrain._blocks[num14] = Biome.rockblock;
				if (j >= num9)
				{
					if (num8 + num10 > 41)
					{
						if (num8 + num10 > 44)
						{
							if (num8 + num10 > 54)
							{
								terrain._blocks[num14] = Biome.snowBlock;
							}
							else
							{
								terrain._blocks[num14] = Biome.rockblock;
							}
						}
						else
						{
							terrain._blocks[num14] = Biome.dirtblock;
						}
					}
					else if (flag)
					{
						terrain._blocks[num14] = Biome.sandBlock;
					}
					else if (j == num8)
					{
						if ((float)j < 32f)
						{
							terrain._blocks[num14] = Biome.dirtblock;
						}
						else
						{
							terrain._blocks[num14] = Biome.grassblock;
						}
					}
					else
					{
						terrain._blocks[num14] = Biome.dirtblock;
					}
				}
				else
				{
					if (j < 32)
					{
						if (j < 16)
						{
							num2 = _noiseFunction.ComputeNoise(0.375f * (float)(worldX + 1750), 0.375f * (float)(num13 + 1750), 0.375f * (float)(worldZ + 1750));
							if (num2 > 0.75f)
							{
								terrain._blocks[num14] = Biome.diamondsBlock;
							}
						}
						num2 = _noiseFunction.ComputeNoise(0.25f * (float)(worldX + 777), 0.25f * (float)(num13 + 777), 0.25f * (float)(worldZ + 777));
						if (num2 < -0.75f)
						{
							terrain._blocks[num14] = Biome.goldBlock;
						}
						num2 = _noiseFunction.ComputeNoise(0.25f * (float)(worldX + 5432), 0.25f * (float)(num13 + 5432), 0.25f * (float)(worldZ + 5432));
						if (num2 > 0.8f)
						{
							terrain._blocks[num14] = Biome.surfaceLavablock;
						}
					}
					num2 = _noiseFunction.ComputeNoise(0.25f * (float)(worldX + 250), 0.25f * (float)(num13 + 250), 0.25f * (float)(worldZ + 250));
					if (num2 > 0.74f)
					{
						terrain._blocks[num14] = Biome.ironBlock;
					}
					num2 = _noiseFunction.ComputeNoise(0.3125f * (float)(worldX + 1000), 0.3125f * (float)(num13 + 1000), 0.3125f * (float)(worldZ + 1000));
					if (num2 < -0.5f)
					{
						terrain._blocks[num14] = Biome.coalBlock;
					}
				}
				if (j > 4 && j < 64)
				{
					Vector3 vector = intVector * 0.0625f * new Vector3(1f, 1.5f, 1f);
					num2 = _noiseFunction.ComputeNoise(vector);
					num2 += _noiseFunction.ComputeNoise(vector * 2f) / 2f;
					if (num2 < -0.35f)
					{
						terrain._blocks[num14] = Biome.emptyblock;
					}
				}
			}
		}
	}
}
