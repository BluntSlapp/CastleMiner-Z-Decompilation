using System.IO;
using DNA.CastleMinerZ.Terrain;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class DigMessage : CastleMinerZMessage
	{
		public bool Placing;

		public Vector3 Location;

		public Vector3 Direction;

		public BlockTypeEnum BlockType;

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.PlayerUpdate;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.Reliable;
			}
		}

		private DigMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, bool placing, Vector3 location, Vector3 direction, BlockTypeEnum blockDug)
		{
			DigMessage sendInstance = Message.GetSendInstance<DigMessage>();
			sendInstance.Placing = placing;
			sendInstance.Location = location;
			sendInstance.Direction = direction;
			sendInstance.BlockType = blockDug;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Placing = reader.ReadBoolean();
			Location = reader.ReadVector3();
			Direction = reader.ReadVector3();
			BlockType = (BlockTypeEnum)reader.ReadInt32();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(Placing);
			writer.Write(Location);
			writer.Write(Direction);
			writer.Write((int)BlockType);
		}
	}
}
