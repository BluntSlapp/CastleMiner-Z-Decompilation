using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;

namespace DNA.CastleMinerZ.Inventory
{
	public abstract class ItemBlockInventoryItemClass : BlockInventoryItemClass
	{
		public ItemBlockInventoryItemClass(InventoryItemIDs id, BlockTypeEnum blockType, string description1, string description2)
			: base(id, blockType, description1, description2, 0.025f)
		{
		}

		public abstract Entity CreateWorldEntity(bool attachedToLocalPlayer, BlockTypeEnum blockType);
	}
}
