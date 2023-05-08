using System.Collections.ObjectModel;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class CrackBoxEntity : ModelEntity
	{
		public float CrackAmount = 0.9f;

		private static Model _model;

		static CrackBoxEntity()
		{
			_model = CastleMinerZGame.Instance.Content.Load<Model>("CrackBox");
		}

		public CrackBoxEntity()
			: base(_model)
		{
			base.BlendState = new BlendState();
			base.BlendState.ColorSourceBlend = Blend.Zero;
			base.BlendState.ColorDestinationBlend = Blend.SourceColor;
			base.DepthStencilState = DepthStencilState.DepthRead;
			DrawPriority = 300;
		}

		protected override void DrawMesh(ModelMesh mesh, GameTime gameTime, Matrix world, Matrix view, Matrix projection)
		{
			if (!(CrackAmount <= 0.1f))
			{
				float num = CrackAmount;
				if (num >= 1f)
				{
					num = 0.99f;
				}
				((ReadOnlyCollection<Effect>)(object)mesh.Effects)[0].Parameters["crackAmount"].SetValue(num);
				base.DrawMesh(mesh, gameTime, world, view, projection);
			}
		}
	}
}
