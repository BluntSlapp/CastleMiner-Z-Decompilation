using System.Collections.Generic;

namespace DNA.CastleMinerZ.Inventory
{
	public class Receipe
	{
		public static List<Receipe> CookBook;

		private List<InventoryItem> _ingredients;

		private InventoryItem _result;

		public List<InventoryItem> Ingredients
		{
			get
			{
				return _ingredients;
			}
		}

		public InventoryItem Result
		{
			get
			{
				return _result;
			}
		}

		public Receipe(InventoryItem result, params InventoryItem[] ingredients)
		{
			_result = result;
			_ingredients = new List<InventoryItem>(ingredients);
		}

		static Receipe()
		{
			CookBook = new List<Receipe>();
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 4), InventoryItem.CreateItem(InventoryItemIDs.LogBlock, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Stick, 4), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Torch, 4), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1), InventoryItem.CreateItem(InventoryItemIDs.Stick, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.LanternBlock, 4), InventoryItem.CreateItem(InventoryItemIDs.Torch, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1), InventoryItem.CreateItem(InventoryItemIDs.SandBlock, 4), InventoryItem.CreateItem(InventoryItemIDs.Coal, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BrassCasing, 200), InventoryItem.CreateItem(InventoryItemIDs.Copper, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.IronCasing, 200), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldCasing, 200), InventoryItem.CreateItem(InventoryItemIDs.Gold, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Bullets, 100), InventoryItem.CreateItem(InventoryItemIDs.RockBlock, 1), InventoryItem.CreateItem(InventoryItemIDs.BrassCasing, 100), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.IronBullets, 100), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1), InventoryItem.CreateItem(InventoryItemIDs.BrassCasing, 100), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldBullets, 100), InventoryItem.CreateItem(InventoryItemIDs.Gold, 1), InventoryItem.CreateItem(InventoryItemIDs.IronCasing, 100), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondBullets, 100), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1), InventoryItem.CreateItem(InventoryItemIDs.GoldCasing, 100), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Compass, 1), InventoryItem.CreateItem(InventoryItemIDs.SandBlock, 1), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Clock, 1), InventoryItem.CreateItem(InventoryItemIDs.SandBlock, 1), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 1), InventoryItem.CreateItem(InventoryItemIDs.Copper, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GPS, 1), InventoryItem.CreateItem(InventoryItemIDs.SandBlock, 1), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.TeleportGPS, 1), InventoryItem.CreateItem(InventoryItemIDs.SandBlock, 1), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1), InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Crate, 1), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 10), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.StonePickAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.RockBlock, 4), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.CopperPickAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.Copper, 2), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.IronPickAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldPickAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2), InventoryItem.CreateItem(InventoryItemIDs.Iron, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondPickAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 2), InventoryItem.CreateItem(InventoryItemIDs.Gold, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BloodstonePickAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 10), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.StoneSpade, 1), InventoryItem.CreateItem(InventoryItemIDs.RockBlock, 2), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.CopperSpade, 1), InventoryItem.CreateItem(InventoryItemIDs.Copper, 1), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.IronSpade, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldSpade, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondSpade, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.StoneAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.RockBlock, 4), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.CopperAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.Copper, 2), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.IronAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2), InventoryItem.CreateItem(InventoryItemIDs.Stick, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondAxe, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 2), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Iron, 1), InventoryItem.CreateItem(InventoryItemIDs.IronOre, 2), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Gold, 1), InventoryItem.CreateItem(InventoryItemIDs.GoldOre, 2), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Copper, 1), InventoryItem.CreateItem(InventoryItemIDs.CopperOre, 2), InventoryItem.CreateItem(InventoryItemIDs.Coal, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Iron, 1), InventoryItem.CreateItem(InventoryItemIDs.IronOre, 2), InventoryItem.CreateItem(InventoryItemIDs.LogBlock, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Gold, 1), InventoryItem.CreateItem(InventoryItemIDs.GoldOre, 2), InventoryItem.CreateItem(InventoryItemIDs.LogBlock, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Copper, 1), InventoryItem.CreateItem(InventoryItemIDs.CopperOre, 2), InventoryItem.CreateItem(InventoryItemIDs.LogBlock, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Knife, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldKnife, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondKnife, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BloodStoneKnife, 1), InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 10), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Pistol, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldPistol, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondPistol, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 2), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BloodStonePistol, 1), InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 30), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.SMGGun, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 3), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldSMGGun, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 3), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondSMGGun, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 3), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BloodStoneSMGGun, 1), InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 20), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BoltActionRifle, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 3), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldBoltActionRifle, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 3), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondBoltActionRifle, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 3), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBoltActionRifle, 1), InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 20), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.PumpShotgun, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 3), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldPumpShotgun, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 3), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondPumpShotgun, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 3), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BloodStonePumpShotgun, 1), InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 20), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.AssultRifle, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 5), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldAssultRifle, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 5), InventoryItem.CreateItem(InventoryItemIDs.Iron, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondAssultRifle, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 5), InventoryItem.CreateItem(InventoryItemIDs.Gold, 3)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.BloodStoneAssultRifle, 1), InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 50), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 6)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.CopperWall, 1), InventoryItem.CreateItem(InventoryItemIDs.Copper, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Copper, 1), InventoryItem.CreateItem(InventoryItemIDs.CopperWall, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.IronWall, 1), InventoryItem.CreateItem(InventoryItemIDs.Iron, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Iron, 1), InventoryItem.CreateItem(InventoryItemIDs.IronWall, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.GoldenWall, 1), InventoryItem.CreateItem(InventoryItemIDs.Gold, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Gold, 1), InventoryItem.CreateItem(InventoryItemIDs.GoldenWall, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.DiamondWall, 1), InventoryItem.CreateItem(InventoryItemIDs.Diamond, 2)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1), InventoryItem.CreateItem(InventoryItemIDs.DiamondWall, 1)));
			CookBook.Add(new Receipe(InventoryItem.CreateItem(InventoryItemIDs.Door, 1), InventoryItem.CreateItem(InventoryItemIDs.WoodBlock, 5), InventoryItem.CreateItem(InventoryItemIDs.Iron, 1)));
		}
	}
}
