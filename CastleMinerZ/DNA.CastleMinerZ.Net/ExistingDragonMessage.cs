using System.IO;
using DNA.CastleMinerZ.AI;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class ExistingDragonMessage : CastleMinerZMessage
	{
		public DragonTypeEnum EnemyTypeID;

		public byte NewClientID;

		public bool ForBiome;

		public float CurrentHealth;

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

		private ExistingDragonMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, byte newClientID, DragonTypeEnum enemyType, bool forBiome, float currentHealth)
		{
			ExistingDragonMessage sendInstance = Message.GetSendInstance<ExistingDragonMessage>();
			sendInstance.EnemyTypeID = enemyType;
			sendInstance.NewClientID = newClientID;
			sendInstance.ForBiome = forBiome;
			sendInstance.CurrentHealth = currentHealth;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			EnemyTypeID = (DragonTypeEnum)reader.ReadByte();
			NewClientID = reader.ReadByte();
			ForBiome = reader.ReadBoolean();
			CurrentHealth = reader.ReadSingle();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write((byte)EnemyTypeID);
			writer.Write(NewClientID);
			writer.Write(ForBiome);
			writer.Write(CurrentHealth);
		}
	}
}
