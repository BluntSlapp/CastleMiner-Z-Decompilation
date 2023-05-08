using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class SpeedUpEnemyMessage : CastleMinerZMessage
	{
		public int EnemyID;

		public int TargetID;

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

		private SpeedUpEnemyMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, int enemyid, int targetid)
		{
			SpeedUpEnemyMessage sendInstance = Message.GetSendInstance<SpeedUpEnemyMessage>();
			sendInstance.EnemyID = enemyid;
			sendInstance.TargetID = targetid;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			EnemyID = reader.ReadInt32();
			TargetID = reader.ReadInt32();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(EnemyID);
			writer.Write(TargetID);
		}
	}
}
