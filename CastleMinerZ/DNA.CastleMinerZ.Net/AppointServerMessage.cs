using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class AppointServerMessage : CastleMinerZMessage
	{
		public byte PlayerID;

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

		private AppointServerMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, byte playerID)
		{
			AppointServerMessage sendInstance = Message.GetSendInstance<AppointServerMessage>();
			sendInstance.PlayerID = playerID;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			PlayerID = reader.ReadByte();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(PlayerID);
		}
	}
}
