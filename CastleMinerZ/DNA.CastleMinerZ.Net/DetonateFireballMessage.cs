using System.IO;
using DNA.CastleMinerZ.AI;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class DetonateFireballMessage : CastleMinerZMessage
	{
		public Vector3 Location;

		public int Index;

		public byte NumBlocks;

		public DragonTypeEnum EType;

		public IntVector3[] BlocksToRemove;

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

		private DetonateFireballMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, Vector3 location, int index, int numblocks, IntVector3[] blocks, DragonTypeEnum dragonType)
		{
			DetonateFireballMessage sendInstance = Message.GetSendInstance<DetonateFireballMessage>();
			sendInstance.Location = location;
			sendInstance.Index = index;
			sendInstance.NumBlocks = (byte)numblocks;
			sendInstance.BlocksToRemove = blocks;
			sendInstance.EType = dragonType;
			sendInstance.DoSend(from);
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(Location);
			writer.Write(Index);
			writer.Write((byte)EType);
			writer.Write(NumBlocks);
			for (int i = 0; i < NumBlocks; i++)
			{
				BlocksToRemove[i].Write(writer);
			}
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Location = reader.ReadVector3();
			Index = reader.ReadInt32();
			EType = (DragonTypeEnum)reader.ReadByte();
			NumBlocks = reader.ReadByte();
			BlocksToRemove = new IntVector3[NumBlocks];
			for (int i = 0; i < NumBlocks; i++)
			{
				BlocksToRemove[i] = IntVector3.Read(reader);
			}
		}
	}
}
