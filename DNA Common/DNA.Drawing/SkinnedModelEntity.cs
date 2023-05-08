using DNA.Drawing.Animation;
using DNA.Profiling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class SkinnedModelEntity : ModelEntity
	{
		protected Matrix[] _skinTransforms;

		private SkinedAnimationData SkinningData
		{
			get
			{
				return (SkinedAnimationData)base.Model.Tag;
			}
		}

		protected override Skeleton GetSkeleton()
		{
			return SkinningData.Skeleton;
		}

		public SkinnedModelEntity(Model model)
			: base(model)
		{
			_skinTransforms = new Matrix[SkinningData.Skeleton.Count];
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			base.OnUpdate(gameTime);
			using (Profiler.TimeSection("UpdateTransform", ProfilerThreadEnum.MAIN))
			{
				UpdateSkinTransforms();
			}
		}

		private void UpdateSkinTransforms()
		{
			for (int i = 0; i < _skinTransforms.Length; i++)
			{
				_skinTransforms[i] = SkinningData.InverseBindPose[i] * _worldBoneTransforms[i];
			}
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
						skinnedEffect.EnableDefaultLighting();
						skinnedEffect.SpecularColor = new Vector3(0.25f);
						skinnedEffect.SpecularPower = 16f;
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
			if (ShowSkeleton)
			{
				DrawWireframeBones(device, view, projection);
			}
		}
	}
}
