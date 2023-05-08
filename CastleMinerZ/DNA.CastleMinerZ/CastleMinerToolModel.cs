using System.Collections.ObjectModel;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class CastleMinerToolModel : ModelEntity
	{
		public Color ToolColor = Color.White;

		public Color ToolColor2 = Color.White;

		public CastleMinerToolModel(Model model)
			: base(model)
		{
		}

		protected override void DrawMesh(ModelMesh mesh, GameTime gameTime, Matrix world, Matrix view, Matrix projection)
		{
			BasicEffect basicEffect = ((ReadOnlyCollection<Effect>)(object)mesh.Effects)[0] as BasicEffect;
			if (basicEffect != null)
			{
				if (mesh.Name.Contains("recolor_"))
				{
					if (ToolColor.A == 0)
					{
						return;
					}
					basicEffect.DiffuseColor = ToolColor.ToVector3();
				}
				else if (mesh.Name.Contains("recolor2_"))
				{
					if (ToolColor2.A == 0)
					{
						return;
					}
					basicEffect.DiffuseColor = ToolColor2.ToVector3();
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
