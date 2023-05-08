using System;
using DNA.Drawing.Noise;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class TreeDepositer : Biome
	{
		private const float treeScale = 0.4375f;

		private const float TreeDescrim = 0.6f;

		public const int TreeWidth = 3;

		private PerlinNoise _noiseFunction = new PerlinNoise(new Random(1));

		private int TreeHeight = 5;

		public TreeDepositer(WorldInfo worldInfo)
			: base(worldInfo)
		{
			_noiseFunction = new PerlinNoise(new Random(worldInfo.Seed));
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			float num = _noiseFunction.ComputeNoise((float)worldX * 0.4375f, (float)worldZ * 0.4375f);
			if (!(num > 0.6f))
			{
				return;
			}
			int num2;
			for (num2 = 124; num2 > 0; num2--)
			{
				int y = num2 + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num3 = terrain.MakeIndexFromWorldIndexVector(a);
				if (Block.GetTypeIndex(terrain._blocks[num3]) == BlockTypeEnum.Grass)
				{
					break;
				}
			}
			if (num2 <= 1)
			{
				return;
			}
			num2++;
			float num4 = 9f * (num - 0.6f);
			int num5 = TreeHeight + (int)num4;
			for (int i = 0; i < num5; i++)
			{
				int y2 = num2 + i + minY;
				IntVector3 a2 = new IntVector3(worldX, y2, worldZ);
				int num6 = terrain.MakeIndexFromWorldIndexVector(a2);
				BlockTypeEnum typeIndex = Block.GetTypeIndex(terrain._blocks[num6]);
				if (typeIndex != 0 && typeIndex != BlockTypeEnum.NumberOfBlocks)
				{
					num5 = i;
					break;
				}
				terrain._blocks[num6] = Biome.LogBlock;
			}
			int num7 = num2 + num5;
			for (int j = -3; j <= 3; j++)
			{
				for (int k = -3; k <= 3; k++)
				{
					for (int l = -3; l <= 3; l++)
					{
						IntVector3 intVector = new IntVector3(worldX + j, num7 + l + minY, worldZ + k);
						int num8 = terrain.MakeIndexFromWorldIndexVector(intVector);
						BlockTypeEnum typeIndex2 = Block.GetTypeIndex(terrain._blocks[num8]);
						if (typeIndex2 == BlockTypeEnum.Empty || typeIndex2 == BlockTypeEnum.NumberOfBlocks)
						{
							float num9 = _noiseFunction.ComputeNoise(intVector * 0.5f);
							float num10 = 1f - (float)Math.Sqrt(j * j + l * l + k * k) / 3f;
							if (num9 + num10 > 0.25f)
							{
								terrain._blocks[num8] = Biome.LeafBlock;
							}
						}
					}
				}
			}
		}
	}
}
