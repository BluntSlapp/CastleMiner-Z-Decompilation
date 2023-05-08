namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	internal class FlatLandBiome : Biome
	{
		public FlatLandBiome(WorldInfo worldInfo)
			: base(worldInfo)
		{
		}

		public override void BuildColumn(BlockTerrain terrain, int worldX, int worldZ, int minY, float blender)
		{
			IntVector3 a = new IntVector3(worldX, minY, worldZ);
			int num = terrain.MakeIndexFromWorldIndexVector(a);
			terrain._blocks[num] = Biome.bedrockBlock;
			a = new IntVector3(worldX, minY + 1, worldZ);
			num = terrain.MakeIndexFromWorldIndexVector(a);
			terrain._blocks[num] = Biome.grassblock;
		}
	}
}
