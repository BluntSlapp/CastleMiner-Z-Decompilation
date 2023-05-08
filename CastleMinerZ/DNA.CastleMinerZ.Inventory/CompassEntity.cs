using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class CompassEntity : Entity
	{
		private ModelEntity modelEnt;

		public bool TrackPosition = true;

		public CompassEntity(Model model)
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
				Vector3 v = -player.WorldPosition;
				Vector3 v2 = Vector3.TransformNormal(Vector3.Forward, player.LocalToWorld);
				v.Y = 0f;
				v2.Y = 0f;
				modelEnt.LocalRotation = v2.RotationBetween(v);
			}
			base.OnUpdate(gameTime);
		}
	}
}
