using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class BroadcastTextMessage : CastleMinerZMessage
	{
		public string Message;

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

		private BroadcastTextMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, string message)
		{
			BroadcastTextMessage sendInstance = DNA.Net.Message.GetSendInstance<BroadcastTextMessage>();
			sendInstance.Message = message;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Message = reader.ReadString();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(Message);
		}
	}
}
