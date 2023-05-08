using System.IO;
using Microsoft.Xna.Framework.Net;

namespace DNA.Net
{
	public class VoiceChatMessage : Message
	{
		public byte[] AudioBuffer = new byte[0];

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.Chat;
			}
		}

		private VoiceChatMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, byte[] _audioBuffer)
		{
			VoiceChatMessage sendInstance = Message.GetSendInstance<VoiceChatMessage>();
			sendInstance.AudioBuffer = _audioBuffer;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (AudioBuffer.Length != num)
			{
				AudioBuffer = new byte[num];
			}
			AudioBuffer = reader.ReadBytes(num);
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(AudioBuffer.Length);
			writer.Write(AudioBuffer);
		}
	}
}
