using System;
using DNA.CastleMinerZ.Terrain;

namespace DNA.CastleMinerZ.Inventory
{
	public class SpadeInventoryItem : InventoryItem
	{
		public SpadeInventoryItem(InventoryItemClass cls, int stackCount)
			: base(cls, stackCount)
		{
			SpadeInventoryClass spadeInventoryClass = (SpadeInventoryClass)base.ItemClass;
		}

		public override TimeSpan TimeToDig(BlockTypeEnum blockType)
		{
			SpadeInventoryClass spadeInventoryClass = (SpadeInventoryClass)base.ItemClass;
			switch (spadeInventoryClass.Material)
			{
			case ToolMaterialTypes.Wood:
				return base.TimeToDig(blockType);
			case ToolMaterialTypes.Stone:
				switch (blockType)
				{
				case BlockTypeEnum.Grass:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.Dirt:
					return TimeSpan.FromSeconds(1.0);
				case BlockTypeEnum.Sand:
					return TimeSpan.FromSeconds(0.75);
				case BlockTypeEnum.Snow:
					return TimeSpan.FromSeconds(0.75);
				}
				break;
			case ToolMaterialTypes.Copper:
				switch (blockType)
				{
				case BlockTypeEnum.Grass:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.Dirt:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.Sand:
					return TimeSpan.FromSeconds(0.5);
				case BlockTypeEnum.Snow:
					return TimeSpan.FromSeconds(0.5);
				}
				break;
			case ToolMaterialTypes.Iron:
				switch (blockType)
				{
				case BlockTypeEnum.Grass:
					return TimeSpan.FromSeconds(0.25);
				case BlockTypeEnum.Dirt:
					return TimeSpan.FromSeconds(0.25);
				case BlockTypeEnum.Sand:
					return TimeSpan.FromSeconds(0.25);
				case BlockTypeEnum.Snow:
					return TimeSpan.FromSeconds(0.25);
				}
				break;
			case ToolMaterialTypes.Gold:
				switch (blockType)
				{
				case BlockTypeEnum.Grass:
					return TimeSpan.FromSeconds(0.1);
				case BlockTypeEnum.Dirt:
					return TimeSpan.FromSeconds(0.1);
				case BlockTypeEnum.Sand:
					return TimeSpan.FromSeconds(0.1);
				case BlockTypeEnum.Snow:
					return TimeSpan.FromSeconds(0.1);
				}
				break;
			case ToolMaterialTypes.Diamond:
				switch (blockType)
				{
				case BlockTypeEnum.Grass:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Dirt:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Sand:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Snow:
					return TimeSpan.FromSeconds(0.0);
				}
				break;
			case ToolMaterialTypes.BloodStone:
				switch (blockType)
				{
				case BlockTypeEnum.Grass:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Dirt:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Sand:
					return TimeSpan.FromSeconds(0.0);
				case BlockTypeEnum.Snow:
					return TimeSpan.FromSeconds(0.0);
				}
				break;
			}
			return base.TimeToDig(blockType);
		}
	}
}
