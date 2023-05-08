using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class DestroyCrateMessage : CastleMinerZMessage
	{
		public IntVector3 Location;

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.System;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.ReliableInOrder;
			}
		}

		private DestroyCrateMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, IntVector3 location)
		{
			DestroyCrateMessage sendInstance = Message.GetSendInstance<DestroyCrateMessage>();
			sendInstance.Location = location;
			sendInstance.DoSend(from);
		}

		protected override void SendData(BinaryWriter writer)
		{
			Location.Write(writer);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Location = IntVector3.Read(reader);
		}
	}
}
