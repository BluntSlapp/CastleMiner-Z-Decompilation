using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	internal class DoorOpenCloseMessage : CastleMinerZMessage
	{
		public IntVector3 Location;

		public bool Opened;

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

		private DoorOpenCloseMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, IntVector3 blockLocaion, bool opened)
		{
			DoorOpenCloseMessage sendInstance = Message.GetSendInstance<DoorOpenCloseMessage>();
			sendInstance.Location = blockLocaion;
			sendInstance.Opened = opened;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Location = IntVector3.Read(reader);
			Opened = reader.ReadBoolean();
		}

		protected override void SendData(BinaryWriter writer)
		{
			Location.Write(writer);
			writer.Write(Opened);
		}
	}
}
