using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class ItemCrateMessage : CastleMinerZMessage
	{
		public IntVector3 Location;

		public int Index;

		public InventoryItem Item;

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.System;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.ReliableInOrder;
			}
		}

		public override bool Echo
		{
			get
			{
				return false;
			}
		}

		private ItemCrateMessage()
		{
		}

		public void Apply(WorldInfo info)
		{
			Crate create = info.GetCreate(Location, true);
			create.Inventory[Index] = Item;
		}

		public static void Send(LocalNetworkGamer from, InventoryItem item, Crate crate, int index)
		{
			crate.Inventory[index] = item;
			ItemCrateMessage sendInstance = Message.GetSendInstance<ItemCrateMessage>();
			sendInstance.Location = crate.Location;
			sendInstance.Index = index;
			sendInstance.Item = item;
			sendInstance.DoSend(from);
		}

		protected override void SendData(BinaryWriter writer)
		{
			Location.Write(writer);
			writer.Write(Index);
			if (Item != null)
			{
				writer.Write(true);
				Item.Write(writer);
			}
			else
			{
				writer.Write(false);
			}
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Location = IntVector3.Read(reader);
			Index = reader.ReadInt32();
			if (reader.ReadBoolean())
			{
				Item = InventoryItem.Create(reader);
			}
			else
			{
				Item = null;
			}
		}
	}
}
