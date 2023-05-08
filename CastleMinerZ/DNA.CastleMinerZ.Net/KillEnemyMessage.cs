using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class KillEnemyMessage : CastleMinerZMessage
	{
		public int EnemyID;

		public int TargetID;

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
				return SendDataOptions.Reliable;
			}
		}

		private KillEnemyMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, int enemyid, int targetid, byte killerid, InventoryItemIDs itemid)
		{
			KillEnemyMessage sendInstance = Message.GetSendInstance<KillEnemyMessage>();
			sendInstance.EnemyID = enemyid;
			sendInstance.TargetID = targetid;
			sendInstance.WeaponID = itemid;
			sendInstance.KillerID = killerid;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			EnemyID = reader.ReadInt32();
			TargetID = reader.ReadInt32();
			WeaponID = (InventoryItemIDs)reader.ReadInt16();
			KillerID = reader.ReadByte();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(EnemyID);
			writer.Write(TargetID);
			writer.Write((short)WeaponID);
			writer.Write(KillerID);
		}
	}
}
