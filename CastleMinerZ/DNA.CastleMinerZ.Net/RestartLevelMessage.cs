using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class RestartLevelMessage : CastleMinerZMessage
	{
		public override bool Echo
		{
			get
			{
				return true;
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

		private RestartLevelMessage()
		{
		}

		public static void Send(LocalNetworkGamer from)
		{
			RestartLevelMessage sendInstance = Message.GetSendInstance<RestartLevelMessage>();
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
