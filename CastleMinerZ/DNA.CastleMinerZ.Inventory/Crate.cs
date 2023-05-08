using System.IO;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Inventory
{
	public class Crate
	{
		public bool Destroyed;

		private IntVector3 _location;

		public InventoryItem[] Inventory = new InventoryItem[32];

		public IntVector3 Location
		{
			get
			{
				return _location;
			}
		}

		public Crate(IntVector3 location)
		{
			_location = location;
		}

		public void EjectContents()
		{
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (Inventory[i] != null)
				{
					Vector3 location = IntVector3.ToVector3(Location) + new Vector3(0.5f);
					PickupManager.Instance.CreateUpwardPickup(Inventory[i], location, 3f);
					Inventory[i] = null;
				}
			}
		}

		public Crate(BinaryReader reader)
		{
			Read(reader);
		}

		public void Write(BinaryWriter writer)
		{
			_location.Write(writer);
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (Inventory[i] == null)
				{
					writer.Write(false);
					continue;
				}
				writer.Write(true);
				Inventory[i].Write(writer);
			}
		}

		public void Read(BinaryReader reader)
		{
			_location = IntVector3.Read(reader);
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (reader.ReadBoolean())
				{
					Inventory[i] = InventoryItem.Create(reader);
					if (Inventory[i] != null && !Inventory[i].IsValid())
					{
						Inventory[i] = null;
					}
				}
				else
				{
					Inventory[i] = null;
				}
			}
		}
	}
}
