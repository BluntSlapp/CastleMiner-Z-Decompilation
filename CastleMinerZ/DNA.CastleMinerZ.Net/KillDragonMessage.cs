using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class KillDragonMessage : CastleMinerZMessage
	{
		public Vector3 Location;

		public InventoryItemIDs WeaponID;

		public byte KillerID;

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.EnemyMessage;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.ReliableInOrder;
			}
		}

		private KillDragonMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, Vector3 location, byte killerid, InventoryItemIDs itemid)
		{
			KillDragonMessage sendInstance = Message.GetSendInstance<KillDragonMessage>();
			sendInstance.Location = location;
			sendInstance.WeaponID = itemid;
			sendInstance.KillerID = killerid;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Location = reader.ReadVector3();
			WeaponID = (InventoryItemIDs)reader.ReadInt16();
			KillerID = reader.ReadByte();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(Location);
			writer.Write((short)WeaponID);
			writer.Write(KillerID);
		}
	}
}
