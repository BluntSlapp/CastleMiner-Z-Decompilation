using System;
using DNA.Drawing.Noise;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class TestBiome : Biome
	{
		private const int BedRockVariance = 3;

		private const int DirtThickness = 3;

		private const int GroundPlane = 64;

		private const int MaxHillHeight = 32;

		private const int MaxValleyDepth = 20;

		private const float worldScale = 0.009375f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public TestBiome(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new PerlinNoise(new Random(worldInfo.Seed));
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			int num = 1;
			float num2 = 0f;
			int num3 = 4;
			for (int i = 0; i < num3; i++)
			{
				num2 += _noiseFunction.ComputeNoise(0.009375f * (float)worldX * (float)num, 0.009375f * (float)worldZ * (float)num) / (float)num;
				num *= 2;
			}
			int num4 = 64 + (int)(num2 * 32f);
			bool flag = false;
			if (num4 <= 44)
			{
				num4 = 44;
				flag = true;
			}
			int num5 = num4 - 3;
			num2 = _noiseFunction.ComputeNoise((float)worldX * 0.5f, (float)worldZ * 0.5f);
			num2 = (num2 + 1f) / 2f;
			int num6 = 1 + (int)(num2 * 3f);
			for (int j = 0; j < 128; j++)
			{
				if (terrain._resetRequested)
				{
					break;
				}
				int y = j + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num7 = terrain.MakeIndexFromWorldIndexVector(a);
				if (j > num4)
				{
					continue;
				}
				if (j < num6)
				{
					terrain._blocks[num7] = Biome.bedrockBlock;
					continue;
				}
				terrain._blocks[num7] = Biome.rockblock;
				if (j >= num5)
				{
					if (flag)
					{
						terrain._blocks[num7] = Biome.sandBlock;
					}
					else if (j == num4)
					{
						terrain._blocks[num7] = Biome.grassblock;
					}
					else
					{
						terrain._blocks[num7] = Biome.dirtblock;
					}
				}
			}
		}
	}
}
