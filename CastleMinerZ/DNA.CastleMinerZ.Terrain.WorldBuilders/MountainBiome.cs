using System;
using DNA.Drawing.Noise;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class MountainBiome : Biome
	{
		private const int BedRockVariance = 3;

		private const int DirtThickness = 3;

		private const int GroundPlane = 86;

		private const int MaxHillHeight = 32;

		private const int MaxValleyDepth = 21;

		private const float worldScale = 0.021875f;

		private const float roadDensity = 1f / 160f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public MountainBiome(WorldInfo worldInfo)
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
				num4 += _noiseFunction.ComputeNoise(0.021875f * (float)worldX * (float)num3, 0.021875f * (float)worldZ * (float)num3) / (float)num3;
				num3 *= 2;
			}
			int num6 = num2 + (int)(num4 * (float)num);
			num3 = 1;
			num4 = 0f;
			num5 = 6;
			Vector2 vector = new Vector2((float)worldX * (1f / 160f), (float)worldZ * (1f / 160f));
			for (int j = 0; j < num5; j++)
			{
				num4 += _noiseFunction.ComputeNoise(vector * num3) / (float)num3;
				num3 *= 2;
			}
			num4 = (num4 + 1f) / 2f;
			float num7 = num4;
			num7 -= (float)Math.Floor(num7);
			num7 -= 0.5f;
			num4 = (float)Math.Pow(2.0, (0f - num7) * num7 * 1000f);
			num6 -= (int)(num4 * (float)num);
			bool flag = false;
			if (num6 <= 65)
			{
				num6 = 65;
				flag = true;
			}
			num4 = _noiseFunction.ComputeNoise(vector + new Vector2(1000f, 1000f)) * blender;
			num6 += (int)(num4 * 10f);
			int num8 = (int)(_noiseFunction.ComputeNoise((float)worldX * 0.5f, (float)worldZ * 0.5f) * 4f);
			int num9 = num6 - 4;
			int num10 = 98;
			if (num9 < num10)
			{
				num9 = num10;
			}
			num9 += num8;
			if (num6 >= 128)
			{
				num6 = 127;
			}
			for (int k = 0; k <= num6; k++)
			{
				int y = k + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num11 = terrain.MakeIndexFromWorldIndexVector(a);
				terrain._blocks[num11] = Biome.rockblock;
				if (flag || k >= num9)
				{
					terrain._blocks[num11] = Biome.snowBlock;
				}
				else
				{
					terrain._blocks[num11] = Biome.rockblock;
				}
			}
		}
	}
}
