using System.IO;
using DNA.CastleMinerZ.AI;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class DragonAttackMessage : CastleMinerZMessage
	{
		public BaseDragonWaypoint Waypoint;

		public Vector3 Target;

		public int FireballIndex;

		public bool AnimatedAttack;

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
				return SendDataOptions.ReliableInOrder;
			}
		}

		private DragonAttackMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, BaseDragonWaypoint wpt, Vector3 target, int fbindex, bool animatedAttack)
		{
			DragonAttackMessage sendInstance = Message.GetSendInstance<DragonAttackMessage>();
			sendInstance.Waypoint = wpt;
			sendInstance.Target = target;
			sendInstance.FireballIndex = fbindex;
			sendInstance.AnimatedAttack = animatedAttack;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Waypoint = BaseDragonWaypoint.ReadBaseWaypoint(reader);
			Target = reader.ReadVector3();
			AnimatedAttack = reader.ReadBoolean();
			FireballIndex = reader.ReadInt32();
		}

		protected override void SendData(BinaryWriter writer)
		{
			Waypoint.Write(writer);
			writer.Write(Target);
			writer.Write(AnimatedAttack);
			writer.Write(FireballIndex);
		}
	}
}
