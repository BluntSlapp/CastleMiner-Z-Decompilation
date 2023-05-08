using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class ClockEntity : Entity
	{
		private ModelEntity modelEnt;

		public bool TrackPosition = true;

		public ClockEntity(Model model)
		{
			modelEnt = new ModelEntity(model);
			base.Children.Add(modelEnt);
			modelEnt.EnableDefaultLighting();
			modelEnt.EnablePerPixelLighting();
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			if (TrackPosition)
			{
				modelEnt.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.Down, (float)Math.PI * 2f * CastleMinerZGame.Instance.GameScreen.TimeOfDay);
			}
			base.OnUpdate(gameTime);
		}
	}
}
