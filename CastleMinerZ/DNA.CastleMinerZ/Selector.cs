using System.Collections.ObjectModel;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class Selector : ModelEntity
	{
		public static Model _selectorModel;

		private float u1;

		private float u2;

		static Selector()
		{
			_selectorModel = CastleMinerZGame.Instance.Content.Load<Model>("Selector");
		}

		public Selector()
			: base(_selectorModel)
		{
			base.RasterizerState = RasterizerState.CullNone;
			DepthStencilState depthStencilState = new DepthStencilState
			{
				DepthBufferFunction = CompareFunction.Less,
				DepthBufferWriteEnable = false
			};
			base.BlendState = BlendState.AlphaBlend;
			base.DepthStencilState = depthStencilState;
			DrawPriority = 400;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			u1 += (float)gameTime.get_ElapsedGameTime().TotalSeconds / 5f;
			u2 -= (float)gameTime.get_ElapsedGameTime().TotalSeconds / 5f;
			u1 -= (int)u1;
			u2 -= (int)u2;
			base.OnUpdate(gameTime);
		}

		protected override void DrawMesh(ModelMesh mesh, GameTime gameTime, Matrix world, Matrix view, Matrix projection)
		{
			if (!CastleMinerZGame.Instance.DrawingReflection)
			{
				if (mesh.Name == "Box001")
				{
					((ReadOnlyCollection<Effect>)(object)mesh.Effects)[0].Parameters["uvOffset"].SetValue(new Vector2(u1, 0f));
				}
				if (mesh.Name == "Box002")
				{
					((ReadOnlyCollection<Effect>)(object)mesh.Effects)[0].Parameters["uvOffset"].SetValue(new Vector2(u2, 0f));
				}
				base.DrawMesh(mesh, gameTime, world, view, projection);
			}
		}
	}
}
