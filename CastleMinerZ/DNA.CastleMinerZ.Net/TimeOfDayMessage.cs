using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class TimeOfDayMessage : CastleMinerZMessage
	{
		public float TimeOfDay;

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

		private TimeOfDayMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, float timeOfDay)
		{
			TimeOfDayMessage sendInstance = Message.GetSendInstance<TimeOfDayMessage>();
			sendInstance.TimeOfDay = timeOfDay;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			TimeOfDay = reader.ReadSingle();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(TimeOfDay);
		}
	}
}
