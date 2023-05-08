using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.UI;
using DNA.Input;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Inventory
{
	public class BlockInventoryItem : InventoryItem
	{
		public BlockTypeEnum BlockTypeID
		{
			get
			{
				return ((BlockInventoryItemClass)base.ItemClass).BlockType._type;
			}
		}

		public BlockInventoryItem(BlockInventoryItemClass classtype, int stackCount)
			: base(classtype, stackCount)
		{
		}

		public virtual BlockTypeEnum GetConstructedBlockType(BlockFace face, IntVector3 position)
		{
			return BlockTypeID;
		}

		public virtual void AlterBlock(Player player, IntVector3 addSpot, BlockFace inFace)
		{
			AlterBlockMessage.Send((LocalNetworkGamer)player.Gamer, addSpot, GetConstructedBlockType(inFace, addSpot));
		}

		public virtual bool CanPlaceHere(IntVector3 addSpot, BlockFace inFace)
		{
			return true;
		}

		public override void ProcessInput(InGameHUD hud, CastleMinerZControllerMapping controller)
		{
			Trigger use = controller.Use;
			if (base.CoolDownTimer.Expired && (controller.Use.Pressed || controller.Shoulder.Pressed) && base.StackCount > 0)
			{
				base.CoolDownTimer.Reset();
				if (hud.Build(this))
				{
					hud.PlayerInventory.Consume(this, 1);
				}
				hud.LocalPlayer.UsingTool = true;
				CastleMinerZPlayerStats.ItemStats itemStats = CastleMinerZGame.Instance.PlayerStats.GetItemStats(base.ItemClass.ID);
				itemStats.Used++;
			}
			else
			{
				hud.LocalPlayer.UsingTool = false;
			}
		}
	}
}
