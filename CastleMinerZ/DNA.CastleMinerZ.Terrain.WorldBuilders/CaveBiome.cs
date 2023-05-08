using System;
using DNA.Drawing.Noise;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class CaveBiome : Biome
	{
		private const float caveDensity = 0.0625f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public CaveBiome(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new PerlinNoise(new Random(worldInfo.Seed));
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			for (int i = 0; i < 128; i++)
			{
				int y = i + minY;
				IntVector3 intVector = new IntVector3(worldX, y, worldZ);
				int num = terrain.MakeIndexFromWorldIndexVector(intVector);
				int num2 = terrain._blocks[num];
				if (Biome.emptyblock != num2 && Biome.uninitblock != num2 && Biome.sandBlock != num2)
				{
					Vector3 vector = intVector * 0.0625f * new Vector3(1f, 1.5f, 1f);
					float num3 = _noiseFunction.ComputeNoise(vector);
					num3 += _noiseFunction.ComputeNoise(vector * 2f) / 2f;
					if (num3 < -0.35f)
					{
						terrain._blocks[num] = Biome.emptyblock;
					}
				}
			}
		}
	}
}
