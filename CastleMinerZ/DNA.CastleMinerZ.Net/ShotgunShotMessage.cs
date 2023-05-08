using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.IO;
using DNA.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public class ShotgunShotMessage : CastleMinerZMessage
	{
		public Vector3[] Directions = new Vector3[5];

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

		private ShotgunShotMessage()
		{
		}

		public static void Send(LocalNetworkGamer from, Matrix m, float innacuracy, InventoryItemIDs item)
		{
			ShotgunShotMessage sendInstance = Message.GetSendInstance<ShotgunShotMessage>();
			for (int i = 0; i < 5; i++)
			{
				Vector3 value = m.Forward + m.Up * 0.015f;
				float num = MathTools.RandomFloat(0f - innacuracy, innacuracy);
				float num2 = MathTools.RandomFloat(0f - innacuracy, innacuracy);
				value += m.Right * num + m.Up * num2;
				sendInstance.Directions[i] = Vector3.Normalize(value);
			}
			sendInstance.ItemID = item;
			sendInstance.DoSend(from);
		}

		protected override void RecieveData(BinaryReader reader)
		{
			for (int i = 0; i < 5; i++)
			{
				Directions[i] = reader.ReadVector3();
			}
			ItemID = (InventoryItemIDs)reader.ReadInt16();
		}

		protected override void SendData(BinaryWriter writer)
		{
			for (int i = 0; i < 5; i++)
			{
				writer.Write(Directions[i]);
			}
			writer.Write((short)ItemID);
		}
	}
}
