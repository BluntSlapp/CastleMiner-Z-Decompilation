using System;
using DNA.Drawing.Noise;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class DecentBiome : Biome
	{
		private const int DirtThickness = 3;

		private const int GroundPlane = 64;

		private const int MaxHillHeight = 16;

		private const int MaxValleyDepth = 10;

		private const float worldScale = 0.009375f;

		private const float caveDensity = 0.0625f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public DecentBiome(WorldInfo worldInfo)
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
			int num4 = (int)MathHelper.Lerp(64f, -16f, blender) + (int)(num2 * 16f);
			for (int j = 0; j < 128; j++)
			{
				int y = j + minY;
				IntVector3 intVector = new IntVector3(worldX, y, worldZ);
				int num5 = terrain.MakeIndexFromWorldIndexVector(intVector);
				if (j <= num4)
				{
					terrain._blocks[num5] = Biome.rockblock;
				}
				Vector3 vector = intVector * 0.0625f * new Vector3(1f, 1f, 1f);
				num2 = _noiseFunction.ComputeNoise(vector);
				num2 += _noiseFunction.ComputeNoise(vector * 2f) / 2f;
				if (num2 < blender * 2f - 1f)
				{
					terrain._blocks[num5] = Biome.emptyblock;
				}
			}
		}
	}
}
