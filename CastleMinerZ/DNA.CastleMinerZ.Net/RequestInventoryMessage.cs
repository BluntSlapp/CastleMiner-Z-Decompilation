using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	internal class RequestInventoryMessage : CastleMinerZMessage
	{
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

		private RequestInventoryMessage()
		{
		}

		public static void Send(LocalNetworkGamer from)
		{
			RequestInventoryMessage sendInstance = Message.GetSendInstance<RequestInventoryMessage>();
			sendInstance.DoSend(from);
		}

		protected override void SendData(BinaryWriter writer)
		{
		}

		protected override void RecieveData(BinaryReader reader)
		{
		}
	}
}
