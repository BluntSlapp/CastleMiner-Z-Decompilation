using System;
using DNA.CastleMinerZ.AI;
using DNA.Drawing;

namespace DNA.CastleMinerZ.Inventory
{
	public class BareHandInventoryItemClass : InventoryItem.InventoryItemClass
	{
		public BareHandInventoryItemClass()
			: base(InventoryItemIDs.BareHands, "Bare Hands", " ", " ", 1, TimeSpan.FromSeconds(0.5))
		{
			_playerMode = PlayerMode.Fist;
			EnemyDamage = 0.025f;
			EnemyDamageType = DamageType.BLUNT;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			return new Entity();
		}
	}
}
