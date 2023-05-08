using System;
using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class TimeConnectedMessage : CastleMinerZMessage
	{
		public TimeSpan TimeConnected;

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
				return MessageTypes.PlayerUpdate;
			}
		}

		private TimeConnectedMessage()
		{
		}

		public void Apply(Player player)
		{
			player.TimeConnected = TimeConnected;
		}

		public static void Send(LocalNetworkGamer from, Player player)
		{
			TimeConnectedMessage sendInstance = Message.GetSendInstance<TimeConnectedMessage>();
			sendInstance.TimeConnected = player.TimeConnected;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			TimeConnected = new TimeSpan(reader.ReadInt64());
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(TimeConnected.Ticks);
		}
	}
}
