using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class ChangeCarriedItemMessage : CastleMinerZMessage
	{
		public InventoryItemIDs ItemID;

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.PlayerUpdate;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.Reliable;
			}
		}

		private ChangeCarriedItemMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, InventoryItemIDs id)
		{
			ChangeCarriedItemMessage sendInstance = Message.GetSendInstance<ChangeCarriedItemMessage>();
			sendInstance.ItemID = id;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			ItemID = (InventoryItemIDs)reader.ReadInt16();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write((short)ItemID);
		}
	}
}
