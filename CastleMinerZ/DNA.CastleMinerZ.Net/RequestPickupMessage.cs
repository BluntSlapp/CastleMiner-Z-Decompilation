using System.IO;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class RequestPickupMessage : CastleMinerZMessage
	{
		public int PickupID;

		public int SpawnerID;

		public override MessageTypes MessageType
		{
			get
			{
				return MessageTypes.PickupMessage;
			}
		}

		protected override SendDataOptions SendDataOptions
		{
			get
			{
				return SendDataOptions.Reliable;
			}
		}

		private RequestPickupMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, int spawnerID, int pickupID)
		{
			RequestPickupMessage sendInstance = Message.GetSendInstance<RequestPickupMessage>();
			sendInstance.PickupID = pickupID;
			sendInstance.SpawnerID = spawnerID;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			PickupID = reader.ReadInt32();
			SpawnerID = reader.ReadInt32();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(PickupID);
			writer.Write(SpawnerID);
		}
	}
}
