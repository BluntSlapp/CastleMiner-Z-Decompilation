using System;
using DNA.Drawing.Noise;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	internal class ArcticBiome : Biome
	{
		private const int DirtThickness = 3;

		private const int GroundPlane = 64;

		private const int MaxHillHeight = 16;

		private const int MaxValleyDepth = 10;

		private const float worldScale = 0.009375f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public ArcticBiome(WorldInfo worldInfo)
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
			int num4 = 64 + (int)(num2 * 16f);
			bool flag = false;
			if (num4 <= 54)
			{
				num4 = 54;
				flag = true;
			}
			int num5 = num4 - 3;
			for (int j = 0; j < 128; j++)
			{
				int y = j + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num6 = terrain.MakeIndexFromWorldIndexVector(a);
				if (j == num4)
				{
					if (flag)
					{
						terrain._blocks[num6] = Biome.iceBlock;
					}
					else
					{
						terrain._blocks[num6] = Biome.snowBlock;
					}
				}
				else if (j == num4 - 1)
				{
					terrain._blocks[num6] = Biome.iceBlock;
				}
				else if (j <= num4)
				{
					terrain._blocks[num6] = Biome.rockblock;
				}
				else if (j <= num5)
				{
					terrain._blocks[num6] = Biome.snowBlock;
				}
			}
		}
	}
}
