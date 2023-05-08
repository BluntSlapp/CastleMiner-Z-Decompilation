using System.IO;
using Microsoft.Xna.Framework.Net;

namespace DNA.Net
{
	public class ChatMessage : Message
	{
		public string Message = "";

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.Chat;
			}
		}

		private ChatMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, string message)
		{
			ChatMessage sendInstance = DNA.Net.Message.GetSendInstance<ChatMessage>();
			sendInstance.Message = message;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Message = reader.ReadString();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(Message);
		}
	}
}
