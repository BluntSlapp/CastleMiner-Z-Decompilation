using System;
using DNA.Drawing.Noise;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	internal class DesertBiome : Biome
	{
		private const int BedRockVariance = 3;

		private const int DirtThickness = 3;

		private const int GroundPlane = 66;

		private const int MaxHillHeight = 16;

		private const int MaxValleyDepth = 0;

		private const float worldScale = 0.009375f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public DesertBiome(WorldInfo worldInfo)
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
			int num4 = 66 + (int)(num2 * 16f);
			if (num4 <= 66)
			{
				num4 = 66;
			}
			int num5 = num4 - 3;
			for (int j = 0; j <= num4; j++)
			{
				if (terrain._resetRequested)
				{
					break;
				}
				int y = j + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num6 = terrain.MakeIndexFromWorldIndexVector(a);
				if (j <= num5)
				{
					terrain._blocks[num6] = Biome.rockblock;
				}
				else
				{
					terrain._blocks[num6] = Biome.sandBlock;
				}
			}
		}
	}
}
