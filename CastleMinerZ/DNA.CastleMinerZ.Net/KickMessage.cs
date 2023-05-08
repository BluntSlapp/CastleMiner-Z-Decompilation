using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class KickMessage : Message
	{
		public bool Banned;

		public byte PlayerID;

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.ReliableInOrder;
			}
		}

		private KickMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, NetworkGamer kickedPlayer, bool banned)
		{
			KickMessage sendInstance = Message.GetSendInstance<KickMessage>();
			sendInstance.Banned = banned;
			sendInstance.PlayerID = kickedPlayer.Id;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Banned = reader.ReadBoolean();
			PlayerID = reader.ReadByte();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(Banned);
			writer.Write(PlayerID);
		}
	}
}
