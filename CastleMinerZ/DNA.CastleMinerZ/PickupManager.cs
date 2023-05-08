using System.Collections.Generic;
using System.Collections.ObjectModel;
using DNA.Audio;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Net;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ
{
	public class PickupManager : Entity
	{
		public static PickupManager Instance;

		private int _nextPickupID;

		public List<PickupEntity> Pickups = new List<PickupEntity>();

		public List<PickupEntity> PendingPickupList = new List<PickupEntity>();

		public PickupManager()
		{
			Visible = false;
			Collidee = false;
			Collider = false;
			Instance = this;
		}

		public void HandleMessage(CastleMinerZMessage message)
		{
			if (message is CreatePickupMessage)
			{
				HandleCreatePickupMessage((CreatePickupMessage)message);
			}
			else if (message is ConsumePickupMessage)
			{
				HandleConsumePickupMessage((ConsumePickupMessage)message);
			}
			else if (message is RequestPickupMessage)
			{
				HandleRequestPickupMessage((RequestPickupMessage)message);
			}
		}

		public void CreateUpwardPickup(InventoryItem item, Vector3 location, float vel)
		{
			Vector3 vec = new Vector3(MathTools.RandomFloat(-0.5f, 0.501f), 0.1f, MathTools.RandomFloat(-0.5f, 0.501f));
			vec.Normalize();
			vec *= vel;
			CreatePickupMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, location, vec, _nextPickupID++, item, false);
		}

		public void CreatePickup(InventoryItem item, Vector3 location, bool dropped)
		{
			float num = 0f;
			if (dropped)
			{
				Player localPlayer = CastleMinerZGame.Instance.LocalPlayer;
				Matrix localToWorld = localPlayer.FPSCamera.LocalToWorld;
				Vector3 forward = localToWorld.Forward;
				forward.Y = 0f;
				forward.Normalize();
				forward.Y = 0.1f;
				forward += localToWorld.Left * (MathTools.RandomFloat() * 0.25f - 0.12f);
				num = 4f;
				forward.Normalize();
				forward *= num;
				CreatePickupMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, location, forward, _nextPickupID++, item, dropped);
			}
			else
			{
				CreateUpwardPickup(item, location, 1.5f);
			}
		}

		public void PlayerTouchedPickup(PickupEntity pickup)
		{
			RequestPickupMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, pickup.SpawnerID, pickup.PickupID);
		}

		private void HandleCreatePickupMessage(CreatePickupMessage msg)
		{
			int id = msg.Sender.Id;
			PickupEntity pickupEntity = new PickupEntity(msg.Item, msg.PickupID, id, msg.Dropped);
			pickupEntity.LocalPosition = msg.SpawnPosition;
			pickupEntity.PlayerPhysics.LocalVelocity = msg.SpawnVector;
			Pickups.Add(pickupEntity);
			base.Scene.Children.Add(pickupEntity);
		}

		public void RemovePickup(PickupEntity pe)
		{
			Pickups.Remove(pe);
			pe.RemoveFromParent();
			if (CastleMinerZGame.Instance.IsGameHost)
			{
				PendingPickupList.Remove(pe);
			}
		}

		private void HandleRequestPickupMessage(RequestPickupMessage msg)
		{
			if (!CastleMinerZGame.Instance.IsGameHost)
			{
				return;
			}
			for (int i = 0; i < Pickups.Count; i++)
			{
				if (Pickups[i].PickupID == msg.PickupID && Pickups[i].SpawnerID == msg.SpawnerID)
				{
					PickupEntity pickupEntity = Pickups[i];
					if (!PendingPickupList.Contains(pickupEntity))
					{
						PendingPickupList.Add(pickupEntity);
						ConsumePickupMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, msg.Sender.Id, pickupEntity.GetActualGraphicPos(), pickupEntity.SpawnerID, pickupEntity.PickupID, pickupEntity.Item);
						break;
					}
				}
			}
		}

		private void HandleConsumePickupMessage(ConsumePickupMessage msg)
		{
			Vector3 zero = Vector3.Zero;
			PickupEntity pickupEntity = null;
			Player player = null;
			if (CastleMinerZGame.Instance.CurrentNetworkSession != null)
			{
				for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)CastleMinerZGame.Instance.CurrentNetworkSession.AllGamers).Count; i++)
				{
					NetworkGamer networkGamer = ((ReadOnlyCollection<NetworkGamer>)(object)CastleMinerZGame.Instance.CurrentNetworkSession.AllGamers)[i];
					if (networkGamer != null && networkGamer.Id == msg.PickerUpper)
					{
						Player player2 = (Player)networkGamer.Tag;
						if (player2 != null)
						{
							player = player2;
						}
					}
				}
			}
			for (int j = 0; j < Pickups.Count; j++)
			{
				if (Pickups[j].PickupID == msg.PickupID && Pickups[j].SpawnerID == msg.SpawnerID)
				{
					pickupEntity = Pickups[j];
					RemovePickup(pickupEntity);
				}
			}
			zero = ((pickupEntity == null) ? msg.PickupPosition : pickupEntity.GetActualGraphicPos());
			if (player != null)
			{
				if (player == CastleMinerZGame.Instance.LocalPlayer)
				{
					CastleMinerZGame.Instance.GameScreen.HUD.PlayerInventory.AddInventoryItem(msg.Item);
					SoundManager.Instance.PlayInstance("pickupitem");
				}
				FlyingPickupEntity t = new FlyingPickupEntity(msg.Item, player, zero);
				base.Scene.Children.Add(t);
			}
		}
	}
}
