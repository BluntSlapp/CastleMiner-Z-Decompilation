using System;
using System.Collections.ObjectModel;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class GPSMarkerEntity : ModelEntity
	{
		private static Model MarkerModel;

		public Color color = Color.White;

		public GPSMarkerEntity()
			: base(MarkerModel)
		{
			EnableDefaultLighting();
			EnablePerPixelLighting();
		}

		static GPSMarkerEntity()
		{
			MarkerModel = CastleMinerZGame.Instance.Content.Load<Model>("Marker");
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			base.LocalRotation = Quaternion.CreateFromYawPitchRoll((float)gameTime.get_TotalGameTime().TotalSeconds * 2f % ((float)Math.PI * 2f), 0f, 0f);
			base.Update(game, gameTime);
		}

		protected override void DrawMesh(ModelMesh mesh, GameTime gameTime, Matrix world, Matrix view, Matrix projection)
		{
			BasicEffect basicEffect = ((ReadOnlyCollection<Effect>)(object)mesh.Effects)[0] as BasicEffect;
			if (basicEffect != null)
			{
				if (mesh.Name.Contains("recolor_"))
				{
					if (color.A == 0)
					{
						return;
					}
					basicEffect.DiffuseColor = color.ToVector3();
				}
				else
				{
					basicEffect.DiffuseColor = Color.White.ToVector3();
				}
			}
			base.DrawMesh(mesh, gameTime, world, view, projection);
		}
	}
}
