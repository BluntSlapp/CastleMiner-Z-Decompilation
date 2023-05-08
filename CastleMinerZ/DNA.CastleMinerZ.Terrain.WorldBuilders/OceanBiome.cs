using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public class OceanBiome : Biome
	{
		private const int GroundPlane = 66;

		private const int SandThickness = 3;

		public OceanBiome(WorldInfo winfo)
			: base(winfo)
		{
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			int num = (int)MathHelper.Lerp(66f, 4f, blender);
			int num2 = num - 3;
			for (int i = 0; i < num; i++)
			{
				if (terrain._resetRequested)
				{
					break;
				}
				int y = i + minY;
				IntVector3 a = new IntVector3(worldX, y, worldZ);
				int num3 = terrain.MakeIndexFromWorldIndexVector(a);
				if (i <= num2)
				{
					terrain._blocks[num3] = Biome.bedrockBlock;
				}
				else if (i <= num)
				{
					terrain._blocks[num3] = Biome.sandBlock;
				}
			}
		}
	}
}
