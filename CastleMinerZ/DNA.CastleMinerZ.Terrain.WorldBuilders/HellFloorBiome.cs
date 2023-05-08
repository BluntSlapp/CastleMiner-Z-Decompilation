using System;
using DNA.Drawing.Noise;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class HellFloorBiome : Biome
	{
		private const int HellHeight = 32;

		private const int LavaLevel = 4;

		private const int MaxHillHeight = 32;

		private const float worldScale = 1f / 32f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public HellFloorBiome(WorldInfo worldInfo)
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
				num2 += _noiseFunction.ComputeNoise(1f / 32f * (float)worldX * (float)num + 200f, 1f / 32f * (float)worldZ * (float)num + 200f) / (float)num;
				num *= 2;
			}
			int num4 = 4 + (int)(num2 * 10f) + 3;
			for (int j = 0; j < 32; j++)
			{
				int y = j + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num5 = terrain.MakeIndexFromWorldIndexVector(a);
				if (j < num4)
				{
					terrain._blocks[num5] = Biome.BloodSToneBlock;
				}
				else if (j <= 4)
				{
					terrain._blocks[num5] = Biome.deepLavablock;
				}
			}
		}
	}
}
