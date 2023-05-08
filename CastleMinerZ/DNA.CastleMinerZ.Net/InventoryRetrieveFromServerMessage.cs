using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	internal class InventoryRetrieveFromServerMessage : CastleMinerZMessage
	{
		public PlayerInventory Inventory;

		public byte playerID;

		public bool Default;

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

		private InventoryRetrieveFromServerMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, Player player, bool isdefault)
		{
			InventoryRetrieveFromServerMessage sendInstance = Message.GetSendInstance<InventoryRetrieveFromServerMessage>();
			sendInstance.Inventory = player.PlayerInventory;
			sendInstance.playerID = player.Gamer.Id;
			sendInstance.Default = isdefault;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Inventory = new PlayerInventory((Player)base.Sender.Tag, false);
			Inventory.Load(reader);
			playerID = reader.ReadByte();
			Default = reader.ReadBoolean();
		}

		protected override void SendData(BinaryWriter writer)
		{
			Inventory.Save(writer);
			writer.Write(playerID);
			writer.Write(Default);
		}
	}
}
