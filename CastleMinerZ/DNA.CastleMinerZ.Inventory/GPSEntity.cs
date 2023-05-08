using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class GPSEntity : Entity
	{
		private ModelEntity modelEnt;

		public bool TrackPosition = true;

		public GPSEntity(Model model)
		{
			modelEnt = new ModelEntity(model);
			base.Children.Add(modelEnt);
			modelEnt.EnableDefaultLighting();
			modelEnt.EnablePerPixelLighting();
		}

		public Player GetPlayer()
		{
			for (Entity parent = base.Parent; parent != null; parent = parent.Parent)
			{
				if (parent is Player)
				{
					return (Player)parent;
				}
			}
			return null;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			if (TrackPosition)
			{
				Player player = GetPlayer();
				if (player == null)
				{
					player = CastleMinerZGame.Instance.LocalPlayer;
				}
				Vector3 v;
				Vector3 v2;
				if (player == CastleMinerZGame.Instance.LocalPlayer && CastleMinerZGame.Instance.GameScreen.HUD.ActiveInventoryItem is GPSItem)
				{
					GPSItem gPSItem = (GPSItem)CastleMinerZGame.Instance.GameScreen.HUD.ActiveInventoryItem;
					v = gPSItem.PointToLocation - player.WorldPosition;
					v2 = Vector3.TransformNormal(Vector3.Forward, player.LocalToWorld);
				}
				else
				{
					v = -player.WorldPosition;
					v2 = Vector3.TransformNormal(Vector3.Forward, player.LocalToWorld);
				}
				v.Y = 0f;
				v2.Y = 0f;
				modelEnt.LocalRotation = v2.RotationBetween(v);
			}
			base.OnUpdate(gameTime);
		}
	}
}
