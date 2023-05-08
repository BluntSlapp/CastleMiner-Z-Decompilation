using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class ClientReadyForChunksMessage : CastleMinerZMessage
	{
		public override bool Echo
		{
			get
			{
				return false;
			}
		}

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

		private ClientReadyForChunksMessage()
		{
		}

		public static void Send(LocalNetworkGamer from)
		{
			ClientReadyForChunksMessage sendInstance = Message.GetSendInstance<ClientReadyForChunksMessage>();
			sendInstance.SendToHost(from);
		}

		protected override void SendData(BinaryWriter writer)
		{
		}

		protected override void RecieveData(BinaryReader reader)
		{
		}
	}
}
