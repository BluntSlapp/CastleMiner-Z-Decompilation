using System;
using DNA.CastleMinerZ.Terrain;

namespace DNA.CastleMinerZ.Inventory
{
	public class PickInventoryItem : InventoryItem
	{
		public PickInventoryItem(InventoryItemClass cls, int stackCount)
			: base(cls, stackCount)
		{
			PickInventoryItemClass pickInventoryItemClass = (PickInventoryItemClass)base.ItemClass;
		}

		public override InventoryItem CreatesWhenDug(BlockTypeEnum block)
		{
			PickInventoryItemClass pickInventoryItemClass = (PickInventoryItemClass)base.ItemClass;
			switch (pickInventoryItemClass.Material)
			{
			case ToolMaterialTypes.Wood:
				return base.CreatesWhenDug(block);
			case ToolMaterialTypes.Stone:
				switch (block)
				{
				case BlockTypeEnum.CoalOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Coal, 1);
				case BlockTypeEnum.IronOre:
					return InventoryItem.CreateItem(InventoryItemIDs.IronOre, 1);
				case BlockTypeEnum.CopperOre:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperOre, 1);
				}
				break;
			case ToolMaterialTypes.Copper:
				switch (block)
				{
				case BlockTypeEnum.CoalOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Coal, 1);
				case BlockTypeEnum.IronOre:
					return InventoryItem.CreateItem(InventoryItemIDs.IronOre, 1);
				case BlockTypeEnum.CopperOre:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperOre, 1);
				case BlockTypeEnum.CopperWall:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperWall, 1);
				}
				break;
			case ToolMaterialTypes.Iron:
				switch (block)
				{
				case BlockTypeEnum.CoalOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Coal, 1);
				case BlockTypeEnum.IronOre:
					return InventoryItem.CreateItem(InventoryItemIDs.IronOre, 1);
				case BlockTypeEnum.CopperOre:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperOre, 1);
				case BlockTypeEnum.GoldOre:
					return InventoryItem.CreateItem(InventoryItemIDs.GoldOre, 1);
				case BlockTypeEnum.CopperWall:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperWall, 1);
				case BlockTypeEnum.IronWall:
					return InventoryItem.CreateItem(InventoryItemIDs.IronWall, 1);
				}
				break;
			case ToolMaterialTypes.Gold:
				switch (block)
				{
				case BlockTypeEnum.CoalOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Coal, 1);
				case BlockTypeEnum.IronOre:
					return InventoryItem.CreateItem(InventoryItemIDs.IronOre, 1);
				case BlockTypeEnum.CopperOre:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperOre, 1);
				case BlockTypeEnum.DiamondOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1);
				case BlockTypeEnum.GoldOre:
					return InventoryItem.CreateItem(InventoryItemIDs.GoldOre, 1);
				case BlockTypeEnum.CopperWall:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperWall, 1);
				case BlockTypeEnum.IronWall:
					return InventoryItem.CreateItem(InventoryItemIDs.IronWall, 1);
				case BlockTypeEnum.GoldenWall:
					return InventoryItem.CreateItem(InventoryItemIDs.GoldenWall, 1);
				}
				break;
			case ToolMaterialTypes.Diamond:
				switch (block)
				{
				case BlockTypeEnum.CoalOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Coal, 1);
				case BlockTypeEnum.IronOre:
					return InventoryItem.CreateItem(InventoryItemIDs.IronOre, 1);
				case BlockTypeEnum.CopperOre:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperOre, 1);
				case BlockTypeEnum.GoldOre:
					return InventoryItem.CreateItem(InventoryItemIDs.GoldOre, 1);
				case BlockTypeEnum.DiamondOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1);
				case BlockTypeEnum.CopperWall:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperWall, 1);
				case BlockTypeEnum.IronWall:
					return InventoryItem.CreateItem(InventoryItemIDs.IronWall, 1);
				case BlockTypeEnum.GoldenWall:
					return InventoryItem.CreateItem(InventoryItemIDs.GoldenWall, 1);
				case BlockTypeEnum.DiamondWall:
					return InventoryItem.CreateItem(InventoryItemIDs.DiamondWall, 1);
				}
				break;
			case ToolMaterialTypes.BloodStone:
				switch (block)
				{
				case BlockTypeEnum.CoalOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Coal, 1);
				case BlockTypeEnum.IronOre:
					return InventoryItem.CreateItem(InventoryItemIDs.IronOre, 1);
				case BlockTypeEnum.CopperOre:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperOre, 1);
				case BlockTypeEnum.GoldOre:
					return InventoryItem.CreateItem(InventoryItemIDs.GoldOre, 1);
				case BlockTypeEnum.DiamondOre:
					return InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1);
				case BlockTypeEnum.CopperWall:
					return InventoryItem.CreateItem(InventoryItemIDs.CopperWall, 1);
				case BlockTypeEnum.IronWall:
					return InventoryItem.CreateItem(InventoryItemIDs.IronWall, 1);
				case BlockTypeEnum.GoldenWall:
					return InventoryItem.CreateItem(InventoryItemIDs.GoldenWall, 1);
				case BlockTypeEnum.DiamondWall:
					return InventoryItem.CreateItem(InventoryItemIDs.DiamondWall, 1);
				}
				break;
			}
			return base.CreatesWhenDug(block);
		}

		public override TimeSpan TimeToDig(BlockTypeEnum blockType)
		{
			PickInventoryItemClass pickInventoryItemClass = (PickInventoryItemClass)base.ItemClass;
			switch (pickInventoryItemClass.Material)
			{
			case ToolMaterialTypes.Wood:
				return base.TimeToDig(blockType);
			case ToolMaterialTypes.Stone:
				switch (blockType)
				{
				case BlockTypeEnum.Rock:
					return TimeSpan.FromSeconds(2.0);
				case BlockTypeEnum.Ice:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.CoalOre:
					return TimeSpan.FromSeconds(3.0);
				case BlockTypeEnum.CopperOre:
					return TimeSpan.FromSeconds(6.0);
				case BlockTypeEnum.IronOre:
					return TimeSpan.FromSeconds(9.0);
				}
				break;
			case ToolMaterialTypes.Copper:
				switch (blockType)
				{
				case BlockTypeEnum.Rock:
					return TimeSpan.FromSeconds(1.5);
				case BlockTypeEnum.Ice:
					return TimeSpan.FromSeconds(0.75);
				case BlockTypeEnum.CoalOre:
					return TimeSpan.FromSeconds(1.5);
				case BlockTypeEnum.CopperOre:
					return TimeSpan.FromSeconds(3.0);
				case BlockTypeEnum.IronOre:
					return TimeSpan.FromSeconds(6.0);
				case BlockTypeEnum.CopperWall:
					return TimeSpan.FromSeconds(3.0);
				}
				break;
			case ToolMaterialTypes.Iron:
				switch (blockType)
				{
				case BlockTypeEnum.Rock:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.Ice:
					return TimeSpan.FromSeconds(0.25);
				case BlockTypeEnum.CoalOre:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.CopperOre:
					return TimeSpan.FromSeconds(1.5);
				case BlockTypeEnum.IronOre:
					return TimeSpan.FromSeconds(3.0);
				case BlockTypeEnum.GoldOre:
					return TimeSpan.FromSeconds(6.0);
				case BlockTypeEnum.CopperWall:
					return TimeSpan.FromSeconds(1.5);
				case BlockTypeEnum.IronWall:
					return TimeSpan.FromSeconds(3.0);
				}
				break;
			case ToolMaterialTypes.Gold:
				switch (blockType)
				{
				case BlockTypeEnum.Rock:
					return TimeSpan.FromSeconds(0.25);
				case BlockTypeEnum.Ice:
					return TimeSpan.FromSeconds(0.1);
				case BlockTypeEnum.CoalOre:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.CopperOre:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.IronOre:
					return TimeSpan.FromSeconds(2.0);
				case BlockTypeEnum.GoldOre:
					return TimeSpan.FromSeconds(3.0);
				case BlockTypeEnum.DiamondOre:
					return TimeSpan.FromSeconds(5.0);
				case BlockTypeEnum.CopperWall:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.IronWall:
					return TimeSpan.FromSeconds(2.0);
				case BlockTypeEnum.GoldenWall:
					return TimeSpan.FromSeconds(3.0);
				}
				break;
			case ToolMaterialTypes.Diamond:
				switch (blockType)
				{
				case BlockTypeEnum.Rock:
					return TimeSpan.FromSeconds(0.1);
				case BlockTypeEnum.Ice:
					return TimeSpan.FromSeconds(0.05);
				case BlockTypeEnum.CoalOre:
					return TimeSpan.FromSeconds(0.25);
				case BlockTypeEnum.CopperOre:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.IronOre:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.GoldOre:
					return TimeSpan.FromSeconds(2.0);
				case BlockTypeEnum.DiamondOre:
					return TimeSpan.FromSeconds(3.0);
				case BlockTypeEnum.BloodStone:
					return TimeSpan.FromSeconds(10.0);
				case BlockTypeEnum.CopperWall:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.IronWall:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.GoldenWall:
					return TimeSpan.FromSeconds(2.0);
				case BlockTypeEnum.DiamondWall:
					return TimeSpan.FromSeconds(3.0);
				}
				break;
			case ToolMaterialTypes.BloodStone:
				switch (blockType)
				{
				case BlockTypeEnum.Rock:
					return TimeSpan.FromSeconds(0.01);
				case BlockTypeEnum.Ice:
					return TimeSpan.FromSeconds(0.01);
				case BlockTypeEnum.CoalOre:
					return TimeSpan.FromSeconds(0.1);
				case BlockTypeEnum.CopperOre:
					return TimeSpan.FromSeconds(0.2);
				case BlockTypeEnum.IronOre:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.GoldOre:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.DiamondOre:
					return TimeSpan.FromSeconds(1.5);
				case BlockTypeEnum.BloodStone:
					return TimeSpan.FromSeconds(3.0);
				case BlockTypeEnum.CopperWall:
					return TimeSpan.FromSeconds(0.2);
				case BlockTypeEnum.IronWall:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.GoldenWall:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.DiamondWall:
					return TimeSpan.FromSeconds(1.5);
				}
				break;
			}
			return base.TimeToDig(blockType);
		}
	}
}
