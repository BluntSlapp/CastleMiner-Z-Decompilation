using System;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.AI
{
	public class DragonPartEntity : SkinnedModelEntity
	{
		public const float HEIGHT_OFFSET = -23.5f;

		public const float SUB_PART_SCALE = 0.5f;

		public Texture2D DragonTexture;

		public DragonPartEntity(DragonType type, Model model)
			: base(model)
		{
			base.LocalRotation = Quaternion.CreateFromYawPitchRoll((float)Math.PI, 0f, 0f);
			base.LocalPosition = new Vector3(0f, -23.5f, 4f) * 0.5f;
			base.LocalScale = new Vector3(0.5f);
			DragonTexture = type.Texture;
			DrawPriority = (int)(515 + type.EType);
			Collider = false;
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			foreach (ModelMesh mesh in base.Model.Meshes)
			{
				foreach (Effect effect in mesh.Effects)
				{
					if (effect is SkinnedEffect)
					{
						SkinnedEffect skinnedEffect = (SkinnedEffect)effect;
						skinnedEffect.SetBoneTransforms(_skinTransforms);
						skinnedEffect.View = view;
						skinnedEffect.Projection = projection;
						skinnedEffect.SpecularColor = new Vector3(0.25f);
						skinnedEffect.SpecularPower = 16f;
						skinnedEffect.EmissiveColor = Vector3.Zero;
						skinnedEffect.DiffuseColor = Vector3.One;
						skinnedEffect.AmbientLightColor = BlockTerrain.Instance.AmbientSunColor.ToVector3() * 0.5f;
						skinnedEffect.DirectionalLight0.Enabled = true;
						skinnedEffect.DirectionalLight0.DiffuseColor = BlockTerrain.Instance.SunlightColor.ToVector3();
						skinnedEffect.DirectionalLight0.SpecularColor = BlockTerrain.Instance.SunSpecular.ToVector3();
						skinnedEffect.DirectionalLight0.Direction = Vector3.Negate(BlockTerrain.Instance.VectorToSun);
						skinnedEffect.DirectionalLight1.Enabled = false;
						skinnedEffect.DirectionalLight2.Enabled = false;
						skinnedEffect.Texture = DragonTexture;
						continue;
					}
					if (effect.Parameters["Bones"] != null)
					{
						effect.Parameters["Bones"].SetValue(_skinTransforms);
					}
					if (effect.Parameters["World"] != null)
					{
						effect.Parameters["World"].SetValue(base.LocalToWorld);
					}
					if (effect.Parameters["View"] != null)
					{
						effect.Parameters["View"].SetValue(view);
					}
					if (effect.Parameters["Projection"] != null)
					{
						effect.Parameters["Projection"].SetValue(projection);
					}
					if (effect.Parameters["WorldView"] != null)
					{
						effect.Parameters["WorldView"].SetValue(base.LocalToWorld * view);
					}
					if (effect.Parameters["WorldViewProj"] != null)
					{
						effect.Parameters["WorldViewProj"].SetValue(base.LocalToWorld * view * projection);
					}
				}
				mesh.Draw();
			}
		}
	}
}
