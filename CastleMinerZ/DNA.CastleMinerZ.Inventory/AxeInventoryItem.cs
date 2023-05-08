using System;
using DNA.CastleMinerZ.Terrain;

namespace DNA.CastleMinerZ.Inventory
{
	public class AxeInventoryItem : InventoryItem
	{
		public AxeInventoryItem(InventoryItemClass cls, int stackCount)
			: base(cls, stackCount)
		{
			AxeInventoryClass axeInventoryClass = (AxeInventoryClass)base.ItemClass;
		}

		public override TimeSpan TimeToDig(BlockTypeEnum blockType)
		{
			AxeInventoryClass axeInventoryClass = (AxeInventoryClass)base.ItemClass;
			switch (axeInventoryClass.Material)
			{
			case ToolMaterialTypes.Wood:
				return base.TimeToDig(blockType);
			case ToolMaterialTypes.Stone:
				switch (blockType)
				{
				case BlockTypeEnum.Log:
					return TimeSpan.FromSeconds(2.0);
				case BlockTypeEnum.Wood:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.Leaves:
					return TimeSpan.FromSeconds(0.5);
				}
				break;
			case ToolMaterialTypes.Copper:
				switch (blockType)
				{
				case BlockTypeEnum.Log:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.Wood:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.Leaves:
					return TimeSpan.FromSeconds(0.1);
				}
				break;
			case ToolMaterialTypes.Iron:
				switch (blockType)
				{
				case BlockTypeEnum.Log:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.Wood:
					return TimeSpan.FromSeconds(0.25);
				case BlockTypeEnum.Leaves:
					return TimeSpan.FromSeconds(0.0);
				}
				break;
			case ToolMaterialTypes.Gold:
				switch (blockType)
				{
				case BlockTypeEnum.Log:
					return TimeSpan.FromSeconds(0.25);
				case BlockTypeEnum.Wood:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Leaves:
					return TimeSpan.FromSeconds(0.0);
				}
				break;
			case ToolMaterialTypes.Diamond:
				switch (blockType)
				{
				case BlockTypeEnum.Log:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Wood:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Leaves:
					return TimeSpan.FromSeconds(0.0);
				}
				break;
			case ToolMaterialTypes.BloodStone:
				switch (blockType)
				{
				case BlockTypeEnum.Log:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Wood:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Leaves:
					return TimeSpan.FromSeconds(0.0);
				}
				break;
			}
			return base.TimeToDig(blockType);
		}
	}
}
