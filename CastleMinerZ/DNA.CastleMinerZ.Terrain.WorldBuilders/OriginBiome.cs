using System;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class OriginBiome : Biome
	{
		public const int MaxHeight = 125;

		public OriginBiome(WorldInfo worldInfo)
			: base(worldInfo)
		{
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			int num = worldX * worldX + worldZ * worldZ;
			if (num >= 15625)
			{
				return;
			}
			int num2 = (int)Math.Round(Math.Sqrt(num));
			if (num2 > 4)
			{
				return;
			}
			int num3 = num2 * 8;
			int num4 = 125 - num3;
			for (int i = 0; i < num4; i++)
			{
				int y = i + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num5 = terrain.MakeIndexFromWorldIndexVector(a);
				terrain._blocks[num5] = Biome.bedrockBlock;
				if (i == num4 - 1 && num2 < 2)
				{
					terrain._blocks[num5] = Biome.fixedLanternblock;
				}
			}
		}
	}
}
