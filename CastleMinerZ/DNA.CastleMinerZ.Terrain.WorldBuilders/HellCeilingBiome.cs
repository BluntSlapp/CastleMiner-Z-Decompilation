using System;
using DNA.Drawing.Noise;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class HellCeilingBiome : Biome
	{
		private const int HellHeight = 32;

		private const int MaxHillHeight = 32;

		private const float worldScale = 1f / 32f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public HellCeilingBiome(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new PerlinNoise(new Random(worldInfo.Seed));
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			int num = (int)MathHelper.Lerp(0f, 32f, blender);
			int num2 = 1;
			float num3 = 0f;
			int num4 = 4;
			for (int i = 0; i < num4; i++)
			{
				num3 += _noiseFunction.ComputeNoise(1f / 32f * (float)worldX * (float)num2 + 1000f, 1f / 32f * (float)worldZ * (float)num2 + 1000f) / (float)num2;
				num2 *= 2;
			}
			num3 += 1f;
			int num5 = num - (int)(num3 * 4f);
			for (int j = 0; j <= num5; j++)
			{
				int y = j + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num6 = terrain.MakeIndexFromWorldIndexVector(a);
				if (j < num5)
				{
					terrain._blocks[num6] = Biome.emptyblock;
				}
				else if (j == num5 && terrain._blocks[num6] == Biome.rockblock)
				{
					terrain._blocks[num6] = Biome.BloodSToneBlock;
				}
			}
		}
	}
}
