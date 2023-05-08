using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace DNA.Drawing.Particles
{
	public class ParticleEmitter : Entity
	{
		private bool _emitting = true;

		private int MaxParticles;

		private bool _firstUpdate = true;

		private ParticleEffect _particleEffect;

		private Vector3 _previousPosition;

		private float _timeLeftOver;

		private TimeSpan _timeToEmit;

		private Random _rand = new Random();

		public Vector3 EmitterSize = Vector3.Zero;

		private Effect particleEffect;

		private EffectParameter effectWorldParameter;

		private EffectParameter effectViewParameter;

		private EffectParameter effectProjectionParameter;

		private EffectParameter effectViewportScaleParameter;

		private EffectParameter effectTimeParameter;

		private DynamicVertexBuffer vertexBuffer;

		private IndexBuffer indexBuffer;

		private ParticleVertex[] particles;

		private int firstActiveParticle;

		private int firstNewParticle;

		private int firstFreeParticle;

		private int firstRetiredParticle;

		private float currentTime;

		private int drawCounter;

		private static Random random = new Random();

		public DNAGame _game;

		public static Effect _effect;

		public bool Emitting
		{
			get
			{
				return _emitting;
			}
			set
			{
				if (!_emitting && value)
				{
					_timeToEmit = _particleEffect.EmmissionTime;
					_firstUpdate = true;
				}
				_emitting = value;
			}
		}

		public bool HasActiveParticles
		{
			get
			{
				return firstActiveParticle != firstFreeParticle;
			}
		}

		public bool Loaded
		{
			get
			{
				return particleEffect != null;
			}
		}

		private void LoadParticleEffect(GraphicsDevice device)
		{
			if (_effect == null)
			{
				_effect = _game.Content.Load<Effect>("ParticleEffect");
			}
			particleEffect = _effect.Clone();
		}

		public void AdvanceEffect(TimeSpan time)
		{
			int num = (int)(time.TotalSeconds * 60.0);
			TimeSpan zero = TimeSpan.Zero;
			TimeSpan timeSpan = TimeSpan.FromSeconds(1.0 / 60.0);
			for (int i = 0; i < num; i++)
			{
				zero += timeSpan;
				Update(_game, new GameTime(zero, timeSpan));
			}
		}

		private void SetParams()
		{
			if (!Loaded)
			{
				Initialize();
			}
			EffectParameterCollection parameters = particleEffect.Parameters;
			effectWorldParameter = parameters["World"];
			effectViewParameter = parameters["View"];
			effectProjectionParameter = parameters["Projection"];
			effectViewportScaleParameter = parameters["ViewportScale"];
			effectTimeParameter = parameters["CurrentTime"];
			parameters["Duration"].SetValue((float)_particleEffect.ParticleLifeTime.TotalSeconds);
			parameters["DurationRandomness"].SetValue(_particleEffect.LifetimeVariation);
			parameters["Gravity"].SetValue(_particleEffect.Gravity);
			parameters["EndVelocity"].SetValue(_particleEffect.VelocityEnd);
			parameters["FadeOut"].SetValue(_particleEffect.FadeOut);
			parameters["StartRotation"].SetValue(_particleEffect.RandomizeRotations ? 1 : 0);
			Color c = _particleEffect.ColorMin;
			Color c2 = _particleEffect.ColorMax;
			if (EntityColor.HasValue)
			{
				c = DrawingTools.ModulateColors(EntityColor.Value, c);
				c2 = DrawingTools.ModulateColors(EntityColor.Value, c2);
			}
			parameters["MinColor"].SetValue(c.ToVector4());
			parameters["MaxColor"].SetValue(c2.ToVector4());
			parameters["RotateSpeed"].SetValue(new Vector2(_particleEffect.RotateSpeedMin, _particleEffect.RotateSpeedMax));
			parameters["StartSize"].SetValue(new Vector2(_particleEffect.StartSizeMin, _particleEffect.StartSizeMax));
			parameters["EndSize"].SetValue(new Vector2(_particleEffect.EndSizeMin, _particleEffect.EndSizeMax));
			Texture2D texture = _particleEffect.Texture;
			parameters["Texture"].SetValue(texture);
			particleEffect.CurrentTechnique = particleEffect.Techniques["Particles"];
		}

		public void Reset()
		{
			firstRetiredParticle = (firstFreeParticle = (firstNewParticle = (firstActiveParticle = 0)));
			_firstUpdate = true;
			_timeLeftOver = 0f;
			currentTime = 0f;
			drawCounter = 0;
			SetParams();
		}

		public void Initialize()
		{
			switch (_particleEffect.BlendMode)
			{
			case ParticleBlendMode.Inherit:
				base.BlendState = null;
				break;
			case ParticleBlendMode.Additive:
				base.BlendState = BlendState.Additive;
				break;
			case ParticleBlendMode.NonPreMult:
				base.BlendState = BlendState.NonPremultiplied;
				break;
			}
			_timeToEmit = _particleEffect.EmmissionTime;
			MaxParticles = (int)Math.Ceiling((double)_particleEffect.ParticlesPerSecond * _particleEffect.ParticleLifeTime.TotalSeconds);
			particles = new ParticleVertex[MaxParticles * 4];
			for (int i = 0; i < MaxParticles; i++)
			{
				particles[i * 4].Corner = new Short2(-1f, -1f);
				particles[i * 4 + 1].Corner = new Short2(1f, -1f);
				particles[i * 4 + 2].Corner = new Short2(1f, 1f);
				particles[i * 4 + 3].Corner = new Short2(-1f, 1f);
			}
			LoadParticleEffect(_game.GraphicsDevice);
			Reset();
			vertexBuffer = new DynamicVertexBuffer(_game.GraphicsDevice, ParticleVertex.VertexDeclaration, MaxParticles * 4, BufferUsage.WriteOnly);
			uint[] array = new uint[MaxParticles * 6];
			for (int j = 0; j < MaxParticles; j++)
			{
				array[j * 6] = (uint)(j * 4);
				array[j * 6 + 1] = (uint)(j * 4 + 1);
				array[j * 6 + 2] = (uint)(j * 4 + 2);
				array[j * 6 + 3] = (uint)(j * 4);
				array[j * 6 + 4] = (uint)(j * 4 + 2);
				array[j * 6 + 5] = (uint)(j * 4 + 3);
			}
			indexBuffer = new IndexBuffer(_game.GraphicsDevice, typeof(uint), array.Length, BufferUsage.WriteOnly);
			indexBuffer.SetData(array);
		}

		internal ParticleEmitter(DNAGame game, ParticleEffect particleEffect)
		{
			_game = game;
			_particleEffect = particleEffect;
			AlphaSort = true;
			switch (_particleEffect.BlendMode)
			{
			case ParticleBlendMode.Additive:
				base.BlendState = BlendState.Additive;
				break;
			case ParticleBlendMode.NonPreMult:
				base.BlendState = BlendState.NonPremultiplied;
				break;
			}
			base.DepthStencilState = DepthStencilState.DepthRead;
			base.SamplerState = SamplerState.LinearWrap;
		}

		public void SetInitalPosition(Vector3 position)
		{
			_previousPosition = position;
			_firstUpdate = false;
			if (!Loaded)
			{
				Initialize();
			}
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			Vector3 worldPosition = base.WorldPosition;
			if (_firstUpdate)
			{
				SetInitalPosition(worldPosition);
			}
			bool flag = _particleEffect.EmmissionTime > TimeSpan.Zero;
			currentTime += (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			RetireActiveParticles();
			FreeRetiredParticles();
			if (!HasActiveParticles)
			{
				currentTime = 0f;
				if (!Emitting && _particleEffect.DieAfterEmmision)
				{
					RemoveFromParent();
				}
			}
			if (firstRetiredParticle == firstActiveParticle)
			{
				drawCounter = 0;
			}
			float num = 1f / _particleEffect.ParticlesPerSecond;
			float num2 = (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			if (flag)
			{
				_timeToEmit -= gameTime.get_ElapsedGameTime();
				if (_timeToEmit <= TimeSpan.Zero)
				{
					num2 += (float)_timeToEmit.TotalSeconds;
					_timeToEmit = TimeSpan.Zero;
					_emitting = false;
				}
			}
			if (num2 > 0f && Emitting)
			{
				Vector3 velocity = (worldPosition - _previousPosition) / num2;
				float num3 = _timeLeftOver + num2;
				float num4 = 0f - _timeLeftOver;
				if (EmitterSize.LengthSquared() == 0f)
				{
					while (num3 > num)
					{
						num4 += num;
						num3 -= num;
						float amount = num4 / num2;
						Vector3 position = Vector3.Lerp(_previousPosition, worldPosition, amount);
						AddParticle(position, velocity);
					}
				}
				else
				{
					while (num3 > num)
					{
						num4 += num;
						num3 -= num;
						Vector3 position2 = new Vector3((float)((double)worldPosition.X + (_rand.NextDouble() * 2.0 - 1.0) * (double)EmitterSize.X), (float)((double)worldPosition.Y + (_rand.NextDouble() * 2.0 - 1.0) * (double)EmitterSize.Y), (float)((double)worldPosition.Z + (_rand.NextDouble() * 2.0 - 1.0) * (double)EmitterSize.Z));
						AddParticle(position2, velocity);
					}
				}
				_timeLeftOver = num3;
			}
			_previousPosition = worldPosition;
			base.OnUpdate(gameTime);
		}

		private void RetireActiveParticles()
		{
			float num = (float)_particleEffect.ParticleLifeTime.TotalSeconds;
			while (firstActiveParticle != firstNewParticle)
			{
				float num2 = currentTime - particles[firstActiveParticle * 4].Time;
				if (num2 < num)
				{
					break;
				}
				particles[firstActiveParticle * 4].Time = drawCounter;
				firstActiveParticle++;
				if (firstActiveParticle >= MaxParticles)
				{
					firstActiveParticle = 0;
				}
			}
		}

		private void FreeRetiredParticles()
		{
			while (firstRetiredParticle != firstActiveParticle)
			{
				int num = drawCounter - (int)particles[firstRetiredParticle * 4].Time;
				if (num < 3)
				{
					break;
				}
				firstRetiredParticle++;
				if (firstRetiredParticle >= MaxParticles)
				{
					firstRetiredParticle = 0;
				}
			}
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			if (_firstUpdate)
			{
				return;
			}
			if (_particleEffect.LocalSpace)
			{
				effectWorldParameter.SetValue(base.LocalToWorld);
			}
			else
			{
				effectWorldParameter.SetValue(Matrix.Identity);
			}
			effectViewParameter.SetValue(view);
			effectProjectionParameter.SetValue(projection);
			if (vertexBuffer.IsContentLost)
			{
				vertexBuffer.SetData(particles);
			}
			if (firstNewParticle != firstFreeParticle)
			{
				AddNewParticlesToVertexBuffer();
			}
			if (firstActiveParticle != firstFreeParticle)
			{
				effectViewportScaleParameter.SetValue(new Vector2(0.5f / device.Viewport.AspectRatio, -0.5f));
				effectTimeParameter.SetValue(currentTime);
				device.SetVertexBuffer(vertexBuffer);
				device.Indices = indexBuffer;
				for (int i = 0; i < particleEffect.CurrentTechnique.Passes.Count; i++)
				{
					EffectPass effectPass = particleEffect.CurrentTechnique.Passes[i];
					effectPass.Apply();
					if (firstActiveParticle < firstFreeParticle)
					{
						device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, firstActiveParticle * 4, (firstFreeParticle - firstActiveParticle) * 4, firstActiveParticle * 6, (firstFreeParticle - firstActiveParticle) * 2);
						continue;
					}
					device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, firstActiveParticle * 4, (MaxParticles - firstActiveParticle) * 4, firstActiveParticle * 6, (MaxParticles - firstActiveParticle) * 2);
					if (firstFreeParticle > 0)
					{
						device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, firstFreeParticle * 4, 0, firstFreeParticle * 2);
					}
				}
			}
			drawCounter++;
		}

		private void AddNewParticlesToVertexBuffer()
		{
			int num = 36;
			if (firstNewParticle < firstFreeParticle)
			{
				vertexBuffer.SetData(firstNewParticle * num * 4, particles, firstNewParticle * 4, (firstFreeParticle - firstNewParticle) * 4, num, SetDataOptions.NoOverwrite);
			}
			else
			{
				vertexBuffer.SetData(firstNewParticle * num * 4, particles, firstNewParticle * 4, (MaxParticles - firstNewParticle) * 4, num, SetDataOptions.NoOverwrite);
				if (firstFreeParticle > 0)
				{
					vertexBuffer.SetData(0, particles, 0, firstFreeParticle * 4, num, SetDataOptions.NoOverwrite);
				}
			}
			firstNewParticle = firstFreeParticle;
		}

		public void AddParticle(Vector3 position, Vector3 velocity)
		{
			if (_particleEffect.LocalSpace)
			{
				position = Vector3.Zero;
			}
			int num = firstFreeParticle + 1;
			if (num >= MaxParticles)
			{
				num = 0;
			}
			if (num != firstRetiredParticle)
			{
				velocity *= _particleEffect.EmitterVelocitySensitivity;
				float num2 = MathHelper.Lerp(_particleEffect.HorizontalVelocityMin, _particleEffect.HorizontalVelocityMax, (float)random.NextDouble());
				double num3 = random.NextDouble() * 6.2831854820251465;
				velocity.X += num2 * (float)Math.Cos(num3);
				velocity.Y += num2 * (float)Math.Sin(num3);
				velocity.Z += MathHelper.Lerp(_particleEffect.VerticalVelocityMin, _particleEffect.VerticalVelocityMax, (float)random.NextDouble());
				velocity = Vector3.TransformNormal(velocity, base.LocalToWorld);
				Color color = new Color((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));
				for (int i = 0; i < 4; i++)
				{
					particles[firstFreeParticle * 4 + i].Position = position;
					particles[firstFreeParticle * 4 + i].Velocity = velocity;
					particles[firstFreeParticle * 4 + i].Random = color;
					particles[firstFreeParticle * 4 + i].Time = currentTime;
				}
				firstFreeParticle = num;
			}
		}
	}
}
