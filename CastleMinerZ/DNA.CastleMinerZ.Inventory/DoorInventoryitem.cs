using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Inventory
{
	public class DoorInventoryitem : BlockInventoryItem
	{
		public DoorInventoryitem(BlockInventoryItemClass classtype, int stackCount)
			: base(classtype, stackCount)
		{
		}

		public override void AlterBlock(Player player, IntVector3 addSpot, BlockFace inFace)
		{
			base.AlterBlock(player, addSpot, inFace);
			AlterBlockMessage.Send((LocalNetworkGamer)player.Gamer, addSpot + new IntVector3(0, 1, 0), BlockTypeEnum.UpperDoorClosed);
		}

		public override bool CanPlaceHere(IntVector3 addSpot, BlockFace inFace)
		{
			BlockTerrain.Instance.GetBlockWithChanges(addSpot + new IntVector3(1, 0, 0));
			BlockTerrain.Instance.GetBlockWithChanges(addSpot + new IntVector3(-1, 0, 0));
			BlockTerrain.Instance.GetBlockWithChanges(addSpot + new IntVector3(0, 0, 1));
			BlockTerrain.Instance.GetBlockWithChanges(addSpot + new IntVector3(0, 0, -1));
			BlockTypeEnum blockWithChanges = BlockTerrain.Instance.GetBlockWithChanges(addSpot + new IntVector3(0, -1, 0));
			BlockTypeEnum blockWithChanges2 = BlockTerrain.Instance.GetBlockWithChanges(addSpot + new IntVector3(0, 1, 0));
			if (BlockType.IsEmpty(blockWithChanges))
			{
				return false;
			}
			if (!BlockType.IsEmpty(blockWithChanges2))
			{
				return false;
			}
			return true;
		}

		public override BlockTypeEnum GetConstructedBlockType(BlockFace face, IntVector3 position)
		{
			BlockTypeEnum blockWithChanges = BlockTerrain.Instance.GetBlockWithChanges(position + new IntVector3(1, 0, 0));
			BlockTypeEnum blockWithChanges2 = BlockTerrain.Instance.GetBlockWithChanges(position + new IntVector3(-1, 0, 0));
			BlockTypeEnum blockWithChanges3 = BlockTerrain.Instance.GetBlockWithChanges(position + new IntVector3(0, 0, 1));
			BlockTypeEnum blockWithChanges4 = BlockTerrain.Instance.GetBlockWithChanges(position + new IntVector3(0, 0, -1));
			if (!BlockType.IsEmpty(blockWithChanges) && !BlockType.IsEmpty(blockWithChanges2))
			{
				return BlockTypeEnum.LowerDoorClosedX;
			}
			if (!BlockType.IsEmpty(blockWithChanges3) && !BlockType.IsEmpty(blockWithChanges4))
			{
				return BlockTypeEnum.LowerDoorClosedZ;
			}
			if (!BlockType.IsEmpty(blockWithChanges) || !BlockType.IsEmpty(blockWithChanges2))
			{
				return BlockTypeEnum.LowerDoorClosedX;
			}
			if (!BlockType.IsEmpty(blockWithChanges3) || !BlockType.IsEmpty(blockWithChanges4))
			{
				return BlockTypeEnum.LowerDoorClosedZ;
			}
			return BlockTypeEnum.LowerDoorClosedZ;
		}
	}
}
