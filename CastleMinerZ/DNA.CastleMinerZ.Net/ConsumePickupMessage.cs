using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class ConsumePickupMessage : CastleMinerZMessage
	{
		public Vector3 PickupPosition;

		public InventoryItem Item;

		public byte PickerUpper;

		public int PickupID;

		public int SpawnerID;

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.PickupMessage;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.Reliable;
			}
		}

		private ConsumePickupMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, byte pickerupper, Vector3 pos, int spawnerID, int pickupID, InventoryItem item)
		{
			ConsumePickupMessage sendInstance = Message.GetSendInstance<ConsumePickupMessage>();
			sendInstance.PickupPosition = pos;
			sendInstance.Item = item;
			sendInstance.PickupID = pickupID;
			sendInstance.SpawnerID = spawnerID;
			sendInstance.PickerUpper = pickerupper;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			PickupPosition = reader.ReadVector3();
			PickupID = reader.ReadInt32();
			SpawnerID = reader.ReadInt32();
			PickerUpper = reader.ReadByte();
			Item = InventoryItem.Create(reader);
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(PickupPosition);
			writer.Write(PickupID);
			writer.Write(SpawnerID);
			writer.Write(PickerUpper);
			Item.Write(writer);
		}
	}
}
