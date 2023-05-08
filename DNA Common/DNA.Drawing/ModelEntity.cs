using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DNA.Drawing.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class ModelEntity : Entity
	{
		protected AnimationData _animationData;

		private LayeredAnimationPlayer _animations = new LayeredAnimationPlayer(16);

		public bool ShowSkeleton;

		private BasicEffect _wireFrameEffect;

		private VertexPositionColor[] _wireFrameVerts;

		protected Matrix[] _worldBoneTransforms;

		protected Matrix[] _bindPose;

		private Skeleton _skeleton;

		private bool _lighting = true;

		private Model _model;

		public LayeredAnimationPlayer Animations
		{
			get
			{
				return _animations;
			}
		}

		public Matrix[] WorldBoneTransforms
		{
			get
			{
				return _worldBoneTransforms;
			}
		}

		public Skeleton Skeleton
		{
			get
			{
				return _skeleton;
			}
		}

		public bool Lighting
		{
			get
			{
				return _lighting;
			}
			set
			{
				_lighting = value;
			}
		}

		protected Model Model
		{
			get
			{
				return _model;
			}
			set
			{
				SetupModel(value);
			}
		}

		public void GetBindPose(Matrix[] bindPose)
		{
			Skeleton.CopyTransformsTo(bindPose);
		}

		public void SetDefaultPose()
		{
			Skeleton skeleton = Skeleton;
			for (int i = 0; i < skeleton.Count; i++)
			{
				skeleton[i].SetTransform(_bindPose[i]);
			}
		}

		protected static void ChangeEffectUsedByMesh(ModelMesh mesh, Effect replacementEffect)
		{
			Dictionary<Effect, Effect> dictionary = new Dictionary<Effect, Effect>();
			foreach (Effect effect3 in mesh.Effects)
			{
				if (!dictionary.ContainsKey(effect3))
				{
					Effect effect2 = (dictionary[effect3] = replacementEffect.Clone());
				}
			}
			foreach (ModelMeshPart meshPart in mesh.MeshParts)
			{
				meshPart.Effect = dictionary[meshPart.Effect];
			}
		}

		protected static void ChangeEffectUsedByModel(Model model, Effect replacementEffect)
		{
			new Dictionary<Effect, Effect>();
			foreach (ModelMesh mesh in model.Meshes)
			{
				ChangeEffectUsedByMesh(mesh, replacementEffect);
			}
		}

		protected virtual Skeleton GetSkeleton()
		{
			return Bone.BuildSkeleton(_model);
		}

		protected override void OnApplyEffect(Effect sourceEffect)
		{
			ChangeEffectUsedByModel(_model, sourceEffect);
		}

		public void EnableDefaultLighting()
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes).Count; i++)
			{
				ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[i];
				for (int j = 0; j < ((ReadOnlyCollection<Effect>)(object)modelMesh.Effects).Count; j++)
				{
					if (((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j] is BasicEffect)
					{
						BasicEffect basicEffect = (BasicEffect)((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j];
						basicEffect.EnableDefaultLighting();
						basicEffect.LightingEnabled = true;
					}
				}
			}
		}

		public void SetLighting(Vector3 ambient, Vector3 Direction0, Vector3 DColor0, Vector3 SColor0)
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes).Count; i++)
			{
				ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[i];
				for (int j = 0; j < ((ReadOnlyCollection<Effect>)(object)modelMesh.Effects).Count; j++)
				{
					if (((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j] is BasicEffect)
					{
						BasicEffect basicEffect = (BasicEffect)((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j];
						basicEffect.AmbientLightColor = ambient;
						DirectionalLight directionalLight = basicEffect.DirectionalLight0;
						directionalLight.DiffuseColor = DColor0;
						directionalLight.SpecularColor = SColor0;
						directionalLight.Direction = Direction0;
						basicEffect.DirectionalLight1.Enabled = false;
						basicEffect.DirectionalLight2.Enabled = false;
						basicEffect.LightingEnabled = true;
					}
				}
			}
		}

		public void SetLighting(Vector3 ambient, Vector3 Direction0, Vector3 DColor0, Vector3 SColor0, Vector3 Direction1, Vector3 DColor1, Vector3 SColor1)
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes).Count; i++)
			{
				ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[i];
				for (int j = 0; j < ((ReadOnlyCollection<Effect>)(object)modelMesh.Effects).Count; j++)
				{
					if (((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j] is BasicEffect)
					{
						BasicEffect basicEffect = (BasicEffect)((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j];
						basicEffect.AmbientLightColor = ambient;
						DirectionalLight directionalLight = basicEffect.DirectionalLight0;
						directionalLight.DiffuseColor = DColor0;
						directionalLight.SpecularColor = SColor0;
						directionalLight.Direction = Direction0;
						directionalLight.Enabled = false;
						directionalLight = basicEffect.DirectionalLight1;
						directionalLight.DiffuseColor = DColor1;
						directionalLight.SpecularColor = SColor1;
						directionalLight.Direction = Direction1;
						directionalLight.Enabled = true;
						basicEffect.DirectionalLight2.Enabled = false;
						basicEffect.LightingEnabled = true;
					}
				}
			}
		}

		public void SetLighting(Vector3 ambient, Vector3 Direction0, Vector3 DColor0, Vector3 SColor0, Vector3 Direction1, Vector3 DColor1, Vector3 SColor1, Vector3 Direction2, Vector3 DColor2, Vector3 SColor2)
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes).Count; i++)
			{
				ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[i];
				for (int j = 0; j < ((ReadOnlyCollection<Effect>)(object)modelMesh.Effects).Count; j++)
				{
					if (((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j] is BasicEffect)
					{
						BasicEffect basicEffect = (BasicEffect)((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j];
						basicEffect.AmbientLightColor = ambient;
						DirectionalLight directionalLight = basicEffect.DirectionalLight0;
						directionalLight.DiffuseColor = DColor0;
						directionalLight.SpecularColor = SColor0;
						directionalLight.Direction = Direction0;
						directionalLight.Enabled = false;
						directionalLight = basicEffect.DirectionalLight1;
						directionalLight.DiffuseColor = DColor1;
						directionalLight.SpecularColor = SColor1;
						directionalLight.Direction = Direction1;
						directionalLight.Enabled = true;
						directionalLight = basicEffect.DirectionalLight2;
						directionalLight.DiffuseColor = DColor2;
						directionalLight.SpecularColor = SColor2;
						directionalLight.Direction = Direction2;
						directionalLight.Enabled = true;
						basicEffect.LightingEnabled = true;
					}
				}
			}
		}

		public void SetAlphaTest(int referenceAlpha, CompareFunction compareFunction)
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes).Count; i++)
			{
				ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[i];
				for (int j = 0; j < ((ReadOnlyCollection<Effect>)(object)modelMesh.Effects).Count; j++)
				{
					AlphaTestEffect alphaTestEffect = (AlphaTestEffect)((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j];
					alphaTestEffect.ReferenceAlpha = referenceAlpha;
					alphaTestEffect.AlphaFunction = compareFunction;
				}
			}
		}

		public void EnablePerPixelLighting()
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes).Count; i++)
			{
				ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[i];
				for (int j = 0; j < ((ReadOnlyCollection<Effect>)(object)modelMesh.Effects).Count; j++)
				{
					if (((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j] is BasicEffect)
					{
						BasicEffect basicEffect = (BasicEffect)((ReadOnlyCollection<Effect>)(object)modelMesh.Effects)[j];
						basicEffect.PreferPerPixelLighting = true;
						basicEffect.LightingEnabled = true;
					}
				}
			}
		}

		protected void AllocateBoneTransforms()
		{
			_worldBoneTransforms = new Matrix[Skeleton.Count];
			_bindPose = new Matrix[Skeleton.Count];
		}

		public ModelEntity(Model model)
		{
			SetupModel(model);
		}

		private void SetupModel(Model model)
		{
			_model = model;
			_animationData = (AnimationData)model.Tag;
			_skeleton = GetSkeleton();
			AllocateBoneTransforms();
			GetBindPose(_bindPose);
			Skeleton.CopyAbsoluteBoneTransformsTo(_worldBoneTransforms, base.LocalToWorld);
		}

		public AnimationPlayer PlayClip(string clipName, bool looping, IList<string> influenceBoneNames, TimeSpan blendTime)
		{
			return PlayClip(0, clipName, looping, influenceBoneNames, blendTime);
		}

		public AnimationPlayer PlayClip(string clipName, bool looping, IList<Bone> influenceBones, TimeSpan blendTime)
		{
			return PlayClip(0, clipName, looping, influenceBones, blendTime);
		}

		public AnimationPlayer PlayClip(string clipName, bool looping, TimeSpan blendTime)
		{
			return PlayClip(0, clipName, looping, blendTime);
		}

		public AnimationPlayer PlayClip(int channel, string clipName, bool looping, IList<string> influenceBoneNames, TimeSpan blendTime)
		{
			AnimationClip clip = _animationData.AnimationClips[clipName];
			AnimationPlayer animationPlayer = new AnimationPlayer(clip, Skeleton.BonesFromNames(influenceBoneNames));
			animationPlayer.Looping = looping;
			animationPlayer.Play();
			_animations.PlayAnimation(channel, animationPlayer, blendTime);
			return animationPlayer;
		}

		public AnimationPlayer PlayClip(int channel, string clipName, bool looping, IList<Bone> influenceBones, TimeSpan blendTime)
		{
			AnimationClip clip = _animationData.AnimationClips[clipName];
			AnimationPlayer animationPlayer = new AnimationPlayer(clip, influenceBones);
			animationPlayer.Looping = looping;
			animationPlayer.Play();
			_animations.PlayAnimation(channel, animationPlayer, blendTime);
			return animationPlayer;
		}

		public AnimationPlayer PlayClip(int channel, string clipName, bool looping, TimeSpan blendTime)
		{
			AnimationClip clip = _animationData.AnimationClips[clipName];
			AnimationPlayer animationPlayer = new AnimationPlayer(clip);
			animationPlayer.Looping = looping;
			animationPlayer.Play();
			_animations.PlayAnimation(channel, animationPlayer, blendTime);
			return animationPlayer;
		}

		public void DumpAnimationNames()
		{
			foreach (string key in _animationData.AnimationClips.Keys)
			{
				string text = key;
			}
		}

		public override BoundingSphere GetLocalBoundingSphere()
		{
			BoundingSphere boundingSphere = ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[0].BoundingSphere;
			for (int i = 1; i < ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes).Count; i++)
			{
				boundingSphere = BoundingSphere.CreateMerged(boundingSphere, ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[i].BoundingSphere);
			}
			return boundingSphere;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			SetDefaultPose();
			_animations.Update(gameTime.get_ElapsedGameTime(), Skeleton);
			Skeleton.CopyAbsoluteBoneTransformsTo(_worldBoneTransforms, base.LocalToWorld);
			base.OnUpdate(gameTime);
		}

		protected virtual void DrawMesh(ModelMesh mesh, GameTime gameTime, Matrix world, Matrix view, Matrix projection)
		{
			for (int i = 0; i < ((ReadOnlyCollection<Effect>)(object)mesh.Effects).Count; i++)
			{
				Effect effect = ((ReadOnlyCollection<Effect>)(object)mesh.Effects)[i];
				effect.CurrentTechnique = effect.Techniques[0];
				if (effect is BasicEffect)
				{
					BasicEffect basicEffect = (BasicEffect)effect;
					basicEffect.World = world;
					basicEffect.View = view;
					basicEffect.Projection = projection;
					if (EntityColor.HasValue)
					{
						Color value = EntityColor.Value;
						basicEffect.DiffuseColor = new Vector3((float)(int)value.R / 255f, (float)(int)value.G / 255f, (float)(int)value.B / 255f);
						basicEffect.Alpha = (float)(int)value.A / 255f;
					}
					continue;
				}
				if (effect is AlphaTestEffect)
				{
					AlphaTestEffect alphaTestEffect = (AlphaTestEffect)effect;
					alphaTestEffect.World = world;
					alphaTestEffect.View = view;
					alphaTestEffect.Projection = projection;
					if (EntityColor.HasValue)
					{
						Color value2 = EntityColor.Value;
						alphaTestEffect.DiffuseColor = new Vector3((float)(int)value2.R / 255f, (float)(int)value2.G / 255f, (float)(int)value2.B / 255f);
						alphaTestEffect.Alpha = (float)(int)value2.A / 255f;
					}
					continue;
				}
				if (effect.Parameters["World"] != null)
				{
					effect.Parameters["World"].SetValue(world);
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
					effect.Parameters["WorldView"].SetValue(world * view);
				}
				if (effect.Parameters["WorldViewProj"] != null)
				{
					effect.Parameters["WorldViewProj"].SetValue(world * view * projection);
				}
				if (effect.Parameters["mvpMatrix"] != null)
				{
					effect.Parameters["mvpMatrix"].SetValue(world * view * projection);
				}
			}
			mesh.Draw();
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes).Count; i++)
			{
				ModelMesh modelMesh = ((ReadOnlyCollection<ModelMesh>)(object)_model.Meshes)[i];
				Matrix world = _worldBoneTransforms[modelMesh.ParentBone.Index];
				DrawMesh(modelMesh, gameTime, world, view, projection);
			}
			base.Draw(device, gameTime, view, projection);
		}

		protected void DrawWireframeBones(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
		{
			Matrix[] worldBoneTransforms = _worldBoneTransforms;
			if (_wireFrameVerts == null)
			{
				_wireFrameVerts = new VertexPositionColor[worldBoneTransforms.Length * 2];
			}
			_wireFrameVerts[0].Color = Color.Blue;
			_wireFrameVerts[0].Position = worldBoneTransforms[0].Translation;
			_wireFrameVerts[1] = _wireFrameVerts[0];
			for (int i = 2; i < worldBoneTransforms.Length * 2; i += 2)
			{
				_wireFrameVerts[i].Position = worldBoneTransforms[i / 2].Translation;
				_wireFrameVerts[i].Color = Color.Red;
				_wireFrameVerts[i + 1].Position = worldBoneTransforms[Skeleton[i / 2].Parent.Index].Translation;
				_wireFrameVerts[i + 1].Color = Color.Green;
			}
			if (_wireFrameEffect == null)
			{
				_wireFrameEffect = new BasicEffect(graphicsDevice);
			}
			_wireFrameEffect.LightingEnabled = false;
			_wireFrameEffect.TextureEnabled = false;
			_wireFrameEffect.VertexColorEnabled = true;
			_wireFrameEffect.Projection = projection;
			_wireFrameEffect.View = view;
			_wireFrameEffect.World = Matrix.Identity;
			for (int j = 0; j < _wireFrameEffect.CurrentTechnique.Passes.Count; j++)
			{
				EffectPass effectPass = _wireFrameEffect.CurrentTechnique.Passes[j];
				effectPass.Apply();
				graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, _wireFrameVerts, 0, worldBoneTransforms.Length);
			}
		}
	}
}
