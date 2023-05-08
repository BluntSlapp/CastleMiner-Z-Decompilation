using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	internal class ProvideDeltaListMessage : CastleMinerZMessage
	{
		public int[] Delta;

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.Reliable;
			}
		}

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.System;
			}
		}

		private ProvideDeltaListMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, NetworkGamer recipient, int[] delta)
		{
			ProvideDeltaListMessage sendInstance = Message.GetSendInstance<ProvideDeltaListMessage>();
			sendInstance.Delta = delta;
			if (recipient == null)
			{
				sendInstance.DoSend(from);
			}
			else
			{
				sendInstance.DoSend(from, recipient);
			}
		}

		protected override void RecieveData(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num > 0)
			{
				Delta = new int[num];
				for (int i = 0; i < num; i++)
				{
					Delta[i] = reader.ReadInt32();
				}
			}
			else
			{
				Delta = null;
			}
		}

		protected override void SendData(BinaryWriter writer)
		{
			int num = ((Delta != null) ? Delta.Length : 0);
			writer.Write(num);
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					writer.Write(Delta[i]);
				}
			}
		}
	}
}
