using DNA.Net;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.Net
{
	public abstract class CastleMinerZMessage : Message
	{
		public enum MessageTypes
		{
			System,
			Broadcast,
			PlayerUpdate,
			EnemyMessage,
			PickupMessage
		}

		public abstract MessageTypes MessageType { get; }

		protected bool SendToHost(LocalNetworkGamer sender)
		{
			bool result = false;
			if (CastleMinerZGame.Instance != null)
			{
				NetworkGamer gamerFromID = CastleMinerZGame.Instance.GetGamerFromID(CastleMinerZGame.Instance.TerrainServerID);
				if (gamerFromID != null && !gamerFromID.HasLeftSession)
				{
					DoSend(sender, gamerFromID);
					result = true;
				}
			}
			return result;
		}
	}
}
