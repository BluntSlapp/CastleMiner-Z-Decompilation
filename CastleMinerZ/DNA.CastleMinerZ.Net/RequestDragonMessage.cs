using System.IO;
using DNA.CastleMinerZ.AI;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class RequestDragonMessage : CastleMinerZMessage
	{
		public DragonTypeEnum EnemyTypeID;

		public bool ForBiome;

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

		private RequestDragonMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, DragonTypeEnum enemyType, bool forBiome)
		{
			RequestDragonMessage sendInstance = Message.GetSendInstance<RequestDragonMessage>();
			sendInstance.EnemyTypeID = enemyType;
			sendInstance.ForBiome = forBiome;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			EnemyTypeID = (DragonTypeEnum)reader.ReadByte();
			ForBiome = reader.ReadBoolean();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write((byte)EnemyTypeID);
			writer.Write(ForBiome);
		}
	}
}
