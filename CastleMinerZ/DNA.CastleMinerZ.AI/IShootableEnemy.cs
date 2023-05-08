using DNA.CastleMinerZ.Inventory;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public interface IShootableEnemy
	{
		void TakeDamage(Vector3 damagePosition, Vector3 damageDirection, InventoryItem.InventoryItemClass itemClass, byte shooterID);
	}
}
