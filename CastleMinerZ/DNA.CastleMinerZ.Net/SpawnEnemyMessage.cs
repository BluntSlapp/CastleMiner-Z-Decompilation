using System.IO;
using DNA.CastleMinerZ.AI;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class SpawnEnemyMessage : CastleMinerZMessage
	{
		public Vector3 SpawnPosition;

		public EnemyTypeEnum EnemyTypeID;

		public int EnemyID;

		public int RandomSeed;

		public EnemyType.InitPackage InitPkg;

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

		private SpawnEnemyMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, Vector3 pos, EnemyTypeEnum enemyType, float midnight, int id, int seed)
		{
			SpawnEnemyMessage sendInstance = Message.GetSendInstance<SpawnEnemyMessage>();
			sendInstance.SpawnPosition = pos;
			sendInstance.EnemyTypeID = enemyType;
			sendInstance.EnemyID = id;
			sendInstance.RandomSeed = seed;
			sendInstance.InitPkg = EnemyType.Types[(int)enemyType].CreateInitPackage(midnight);
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			SpawnPosition = reader.ReadVector3();
			EnemyTypeID = (EnemyTypeEnum)reader.ReadByte();
			EnemyID = reader.ReadInt32();
			RandomSeed = reader.ReadInt32();
			InitPkg = EnemyType.InitPackage.Read(reader);
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(SpawnPosition);
			writer.Write((byte)EnemyTypeID);
			writer.Write(EnemyID);
			writer.Write(RandomSeed);
			InitPkg.Write(writer);
		}
	}
}
