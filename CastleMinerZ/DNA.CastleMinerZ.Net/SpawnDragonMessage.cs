using System.IO;
using DNA.CastleMinerZ.AI;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class SpawnDragonMessage : CastleMinerZMessage
	{
		public DragonTypeEnum EnemyTypeID;

		public byte SpawnerID;

		public bool ForBiome;

		public float Health;

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

		private SpawnDragonMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, byte spawnerid, DragonTypeEnum enemyType, bool forBiome, float health)
		{
			SpawnDragonMessage sendInstance = Message.GetSendInstance<SpawnDragonMessage>();
			sendInstance.EnemyTypeID = enemyType;
			sendInstance.SpawnerID = spawnerid;
			sendInstance.ForBiome = forBiome;
			sendInstance.Health = health;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			EnemyTypeID = (DragonTypeEnum)reader.ReadByte();
			SpawnerID = reader.ReadByte();
			ForBiome = reader.ReadBoolean();
			Health = reader.ReadSingle();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write((byte)EnemyTypeID);
			writer.Write(SpawnerID);
			writer.Write(ForBiome);
			writer.Write(Health);
		}
	}
}
