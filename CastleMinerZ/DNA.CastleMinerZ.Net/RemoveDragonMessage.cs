using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class RemoveDragonMessage : CastleMinerZMessage
	{
		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.EnemyMessage;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.Reliable;
			}
		}

		private RemoveDragonMessage()
		{
		}

		public static void Send(LocalNetworkGamer from)
		{
			RemoveDragonMessage sendInstance = Message.GetSendInstance<RemoveDragonMessage>();
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
