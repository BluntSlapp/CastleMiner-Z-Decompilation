using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class RequestWorldInfoMessage : CastleMinerZMessage
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

		private RequestWorldInfoMessage()
		{
		}

		public static void Send(LocalNetworkGamer from)
		{
			RequestWorldInfoMessage sendInstance = Message.GetSendInstance<RequestWorldInfoMessage>();
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
		}

		protected override void SendData(BinaryWriter writer)
		{
		}
	}
}
