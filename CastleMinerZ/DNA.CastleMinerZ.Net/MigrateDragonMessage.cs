using System.IO;
using DNA.CastleMinerZ.AI;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class MigrateDragonMessage : CastleMinerZMessage
	{
		public DragonHostMigrationInfo MigrationInfo;

		public byte TargetID;

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

		private MigrateDragonMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, byte targetID, DragonHostMigrationInfo miginfo)
		{
			MigrateDragonMessage sendInstance = Message.GetSendInstance<MigrateDragonMessage>();
			sendInstance.MigrationInfo = miginfo;
			sendInstance.TargetID = targetID;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			MigrationInfo = DragonHostMigrationInfo.Read(reader);
			TargetID = reader.ReadByte();
		}

		protected override void SendData(BinaryWriter writer)
		{
			MigrationInfo.Write(writer);
			writer.Write(TargetID);
		}
	}
}
