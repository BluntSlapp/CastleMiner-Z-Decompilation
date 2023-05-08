using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class WorldInfoMessage : CastleMinerZMessage
	{
		public WorldInfo WorldInfo;

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

		private WorldInfoMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, WorldInfo worldInfo)
		{
			WorldInfoMessage sendInstance = Message.GetSendInstance<WorldInfoMessage>();
			sendInstance.WorldInfo = worldInfo;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			WorldInfo = new WorldInfo(reader);
		}

		protected override void SendData(BinaryWriter writer)
		{
			WorldInfo.Save(writer);
		}
	}
}
