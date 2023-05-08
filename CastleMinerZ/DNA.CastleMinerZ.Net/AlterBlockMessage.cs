using System.IO;
using DNA.CastleMinerZ.Terrain;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class AlterBlockMessage : CastleMinerZMessage
	{
		public IntVector3 BlockLocation;

		public BlockTypeEnum BlockType;

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.ReliableInOrder;
			}
		}

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.System;
			}
		}

		private AlterBlockMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, IntVector3 blockLocaion, BlockTypeEnum blockType)
		{
			AlterBlockMessage sendInstance = Message.GetSendInstance<AlterBlockMessage>();
			sendInstance.BlockLocation = blockLocaion;
			sendInstance.BlockType = blockType;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			BlockLocation = IntVector3.Read(reader);
			BlockType = (BlockTypeEnum)reader.ReadInt32();
		}

		protected override void SendData(BinaryWriter writer)
		{
			BlockLocation.Write(writer);
			writer.Write((int)BlockType);
		}
	}
}
