using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;

namespace DNA.IO.Storage
{
	public sealed class PlayerSaveDevice : SaveDevice
	{
		private const string playerException = "Player {0} must be signed in to get a player specific storage device.";

		public PlayerIndex Player { get; private set; }

		public PlayerSaveDevice(PlayerIndex player, string containerName, byte[] keyOld, byte[] key)
			: base(containerName, keyOld, key)
		{
			Player = player;
		}

		protected override void GetStorageDevice(AsyncCallback callback, SuccessCallback resultCallback)
		{
			if (Gamer.SignedInGamers[Player] == null)
			{
				throw new InvalidOperationException(string.Format("Player {0} must be signed in to get a player specific storage device.", Player));
			}
			StorageDevice.BeginShowSelector(Player, callback, (object)resultCallback);
		}

		protected override void PrepareEventArgs(SaveDeviceEventArgs args)
		{
			base.PrepareEventArgs(args);
			args.PlayerToPrompt = Player;
		}
	}
}
