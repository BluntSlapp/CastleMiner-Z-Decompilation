using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	internal class InventoryStoreOnServerMessage : CastleMinerZMessage
	{
		public PlayerInventory Inventory;

		public bool FinalSave;

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

		private InventoryStoreOnServerMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, PlayerInventory playerInventory, bool final)
		{
			InventoryStoreOnServerMessage sendInstance = Message.GetSendInstance<InventoryStoreOnServerMessage>();
			sendInstance.Inventory = playerInventory;
			sendInstance.FinalSave = final;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Inventory = new PlayerInventory((Player)base.Sender.Tag, false);
			Inventory.Load(reader);
			FinalSave = reader.ReadBoolean();
		}

		protected override void SendData(BinaryWriter writer)
		{
			Inventory.Save(writer);
			writer.Write(FinalSave);
		}
	}
}
