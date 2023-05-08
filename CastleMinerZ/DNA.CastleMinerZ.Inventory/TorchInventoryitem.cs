using DNA.CastleMinerZ.Terrain;

namespace DNA.CastleMinerZ.Inventory
{
	public class TorchInventoryitem : BlockInventoryItem
	{
		public TorchInventoryitem(BlockInventoryItemClass classtype, int stackCount)
			: base(classtype, stackCount)
		{
		}

		public override BlockTypeEnum GetConstructedBlockType(BlockFace face, IntVector3 position)
		{
			return (BlockTypeEnum)(27 + face);
		}
	}
}
