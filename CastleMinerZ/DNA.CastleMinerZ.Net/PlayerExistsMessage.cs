using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class PlayerExistsMessage : CastleMinerZMessage
	{
		public bool RequestResponse;

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

		private PlayerExistsMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, bool requestResponse)
		{
			PlayerExistsMessage sendInstance = Message.GetSendInstance<PlayerExistsMessage>();
			sendInstance.RequestResponse = requestResponse;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			RequestResponse = reader.ReadBoolean();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(RequestResponse);
		}
	}
}
