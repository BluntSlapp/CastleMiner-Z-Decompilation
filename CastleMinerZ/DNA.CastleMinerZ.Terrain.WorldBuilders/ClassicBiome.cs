using System;
using DNA.Drawing.Noise;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class ClassicBiome : Biome
	{
		private const int BedRockVariance = 3;

		private const int DirtThickness = 3;

		private const int CaveEnd = 118;

		private const int GroundPlane = 86;

		private const int MaxHillHeight = 32;

		private const int MaxValleyDepth = 20;

		private const float worldScale = 0.009375f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public ClassicBiome(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new PerlinNoise(new Random(worldInfo.Seed));
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			int num = (int)MathHelper.Lerp(0f, 32f, blender);
			int num2 = (int)MathHelper.Lerp(32f, 86f, blender);
			int num3 = 1;
			float num4 = 0f;
			int num5 = 4;
			for (int i = 0; i < num5; i++)
			{
				num4 += _noiseFunction.ComputeNoise(0.009375f * (float)worldX * (float)num3, 0.009375f * (float)worldZ * (float)num3) / (float)num3;
				num3 *= 2;
			}
			int num6 = num2 + (int)(num4 * (float)num);
			bool flag = false;
			if (num6 <= 66)
			{
				num6 = 66;
				flag = true;
			}
			if (num6 >= 128)
			{
				num6 = 127;
			}
			int num7 = num6 - 3;
			num4 = _noiseFunction.ComputeNoise((float)worldX * 0.5f, (float)worldZ * 0.5f);
			int num8 = (int)(num4 * 4f);
			for (int j = 0; j <= num6; j++)
			{
				if (terrain._resetRequested)
				{
					break;
				}
				int y = j + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num9 = terrain.MakeIndexFromWorldIndexVector(a);
				terrain._blocks[num9] = Biome.rockblock;
				if (j < num7)
				{
					continue;
				}
				if (num6 + num8 > 95)
				{
					if (num6 + num8 > 98)
					{
						if (num6 + num8 > 108)
						{
							terrain._blocks[num9] = Biome.snowBlock;
						}
						else
						{
							terrain._blocks[num9] = Biome.rockblock;
						}
					}
					else
					{
						terrain._blocks[num9] = Biome.dirtblock;
					}
				}
				else if (flag)
				{
					terrain._blocks[num9] = Biome.sandBlock;
				}
				else if (j == num6)
				{
					terrain._blocks[num9] = Biome.grassblock;
				}
				else
				{
					terrain._blocks[num9] = Biome.dirtblock;
				}
			}
		}
	}
}
