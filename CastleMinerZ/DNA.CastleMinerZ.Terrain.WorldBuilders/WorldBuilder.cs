namespace DNA.CastleMinerZ.Terrain.WorldBuilders
{
	public abstract class WorldBuilder
	{
		public float WaterDepth = 12f;

		protected WorldInfo WorldInfo;

		public WorldBuilder(WorldInfo worldInfo)
		{
			WorldInfo = worldInfo;
		}

		public abstract void BuildWorldChunk(BlockTerrain terrain, IntVector3 minLoc);
	}
}
