using System;
using DNA.Drawing.Noise;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class TreeTestBiome : Biome
	{
		private const float treeScale = 0.4375f;

		private const float TreeDescrim = 0.6f;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		public TreeTestBiome(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new PerlinNoise(new Random(worldInfo.Seed));
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			IntVector3 a = new IntVector3(worldX, minY, worldZ);
			int num = terrain.MakeIndexFromWorldIndexVector(a);
			terrain._blocks[num] = Biome.bedrockBlock;
			a = new IntVector3(worldX, minY + 1, worldZ);
			num = terrain.MakeIndexFromWorldIndexVector(a);
			terrain._blocks[num] = Biome.grassblock;
			int num2 = 2;
			float num3 = _noiseFunction.ComputeNoise((float)worldX * 0.4375f, (float)worldZ * 0.4375f);
			if (num3 > 0.6f)
			{
				float num4 = 9f * (num3 - 0.6f);
				int num5 = 6 + (int)num4;
				for (int i = num2; i < num2 + num5; i++)
				{
					int y = i + minY;
					a = new IntVector3(worldX, y, worldZ);
					num = terrain.MakeIndexFromWorldIndexVector(a);
					terrain._blocks[num] = Biome.LogBlock;
				}
				for (int j = num2 + num5; j < num2 + num5 + 2; j++)
				{
					int y2 = j + minY;
					a = new IntVector3(worldX, y2, worldZ);
					num = terrain.MakeIndexFromWorldIndexVector(a);
					terrain._blocks[num] = Biome.LeafBlock;
				}
			}
			else
			{
				if (!(num3 > -0.25f))
				{
					return;
				}
				float num6 = float.MinValue;
				int num7 = 3;
				for (int k = -num7; k < num7; k++)
				{
					for (int l = -num7; l < num7; l++)
					{
						num6 = Math.Max(num6, _noiseFunction.ComputeNoise((float)(worldX + k) * 0.4375f, (float)(worldZ + l) * 0.4375f));
					}
				}
				if (num6 > 0.6f)
				{
					float num8 = 9f * (num6 - 0.6f);
					int num9 = 6 + (int)num8;
					int num10 = 1 + (int)((num3 + 0.25f) * 4f);
					int num11 = num9 - 2;
					for (int m = num2 + num11; m < num2 + num11 + num10; m++)
					{
						int y3 = m + minY;
						a = new IntVector3(worldX, y3, worldZ);
						num = terrain.MakeIndexFromWorldIndexVector(a);
						terrain._blocks[num] = Biome.LeafBlock;
					}
				}
			}
		}
	}
}
