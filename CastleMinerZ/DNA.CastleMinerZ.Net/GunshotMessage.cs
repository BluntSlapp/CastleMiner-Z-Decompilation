using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class GunshotMessage : CastleMinerZMessage
	{
		public Vector3 Direction;

		public InventoryItemIDs ItemID;

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
				return SendDataOptions.Reliable;
			}
		}

		private GunshotMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, Matrix m, float innacuracy, InventoryItemIDs item)
		{
			GunshotMessage sendInstance = Message.GetSendInstance<GunshotMessage>();
			Vector3 value = m.Forward + m.Up * 0.015f;
			float num = MathTools.RandomFloat(0f - innacuracy, innacuracy);
			float num2 = MathTools.RandomFloat(0f - innacuracy, innacuracy);
			value += m.Right * num + m.Up * num2;
			sendInstance.Direction = Vector3.Normalize(value);
			sendInstance.ItemID = item;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			Direction = reader.ReadVector3();
			ItemID = (InventoryItemIDs)reader.ReadInt16();
		}

		protected override void SendData(BinaryWriter writer)
		{
			writer.Write(Direction);
			writer.Write((short)ItemID);
		}
	}
}
