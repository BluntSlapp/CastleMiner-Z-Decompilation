using System.IO;
using DNA.CastleMinerZ.AI;
using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class UpdateDragonMessage : CastleMinerZMessage
	{
		public static int UpdateCount;

		public BaseDragonWaypoint Waypoint;

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
				if (UpdateCount < 2)
				{
					return SendDataOptions.ReliableInOrder;
				}
				return SendDataOptions.None;
			}
		}

		private UpdateDragonMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, BaseDragonWaypoint wpt)
		{
			UpdateDragonMessage sendInstance = Message.GetSendInstance<UpdateDragonMessage>();
			sendInstance.Waypoint = wpt;
			sendInstance.DoSend(from);
			UpdateCount++;
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Waypoint = BaseDragonWaypoint.ReadBaseWaypoint(reader);
		}

		protected override void SendData(BinaryWriter writer)
		{
			Waypoint.Write(writer);
		}
	}
}
