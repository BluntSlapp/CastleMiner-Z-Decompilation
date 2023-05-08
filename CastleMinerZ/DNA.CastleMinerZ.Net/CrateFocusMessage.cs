using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class CrateFocusMessage : CastleMinerZMessage
	{
		public IntVector3 Location;

		public Point ItemIndex;

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.PlayerUpdate;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.ReliableInOrder;
			}
		}

		private CrateFocusMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, IntVector3 location, Point index)
		{
			CrateFocusMessage sendInstance = Message.GetSendInstance<CrateFocusMessage>();
			sendInstance.Location = location;
			sendInstance.ItemIndex = index;
			sendInstance.DoSend(from);
		}

		protected override void SendData(BinaryWriter writer)
		{
			Location.Write(writer);
			writer.Write(ItemIndex.X);
			writer.Write(ItemIndex.Y);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Location = IntVector3.Read(reader);
			ItemIndex.X = reader.ReadInt32();
			ItemIndex.Y = reader.ReadInt32();
		}
	}
}
