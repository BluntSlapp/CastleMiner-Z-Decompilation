using System.Collections.Generic;
using DNA.Audio;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.UI;
using DNA.CastleMinerZ.Utils.Trace;
using DNA.Drawing;
using DNA.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ
{
	public class TracerManager : Entity
	{
		public struct TracerVertex : IVertexType
		{
			public Vector3 Position;

			public Vector4 Color;

			public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.Color, 0));

			VertexDeclaration IVertexType.VertexDeclaration
			{
				get
				{
					return VertexDeclaration;
				}
			}

			public TracerVertex(Vector3 position, Vector4 color)
			{
				Position = position;
				Color = color;
			}
		}

		public class TracerProbe : TraceProbe
		{
			public override bool TestThisType(BlockTypeEnum e)
			{
				BlockType type = BlockType.GetType(e);
				if (type.CanBeTouched)
				{
					return type.BlockPlayer;
				}
				return false;
			}
		}

		public class Tracer
		{
			private static TracerProbe tp = new TracerProbe();

			private static Vector4 TailColor = new Vector4(1f, 1f, 1f, 0f);

			private Vector3 Head;

			private Vector3 Tail;

			private Vector3 HeadVelocity;

			private Vector3 TailVelocity;

			private Vector4 TracerColor;

			private Player Target;

			private GunInventoryItemClass ItemClass;

			private AudioEmitter SoundEmitter = new AudioEmitter();

			private float TimeLeft;

			private float TailTime;

			private byte ShooterID;

			private bool PlayedSound;

			public bool RemoveAfterDraw;

			public InventoryItemIDs itemID;

			public void Init(Vector3 pos, Vector3 dir, InventoryItemIDs item, byte shooterID)
			{
				ItemClass = (GunInventoryItemClass)InventoryItem.GetClass(item);
				Head = (Tail = pos + dir * 0.5f);
				float velocity = ItemClass.Velocity;
				TracerColor = ItemClass.TracerColor;
				HeadVelocity = (TailVelocity = dir * velocity);
				itemID = item;
				TailTime = ItemClass.FlightTime - 0.2f;
				TimeLeft = ItemClass.FlightTime;
				RemoveAfterDraw = false;
				Target = null;
				PlayedSound = false;
				ShooterID = shooterID;
			}

			public void Init(Vector3 pos, Vector3 vel, Player target, Color color)
			{
				Vector3 vector = ((!(vel.LengthSquared() < 0.001f)) ? Vector3.Up : Vector3.Normalize(vel));
				Head = (Tail = pos + vector * 0.5f);
				TracerColor = color.ToVector4();
				HeadVelocity = (TailVelocity = vel);
				itemID = InventoryItemIDs.Coal;
				TailTime = 1.8f;
				TimeLeft = 2f;
				RemoveAfterDraw = false;
				Target = target;
				ItemClass = null;
				PlayedSound = false;
				ShooterID = byte.MaxValue;
			}

			public void Update(float dt)
			{
				TimeLeft -= dt;
				if (TimeLeft < 0f)
				{
					RemoveAfterDraw = true;
				}
				Head += HeadVelocity * dt;
				HeadVelocity.Y -= 10f * dt;
				if (TimeLeft < TailTime - 0.2f)
				{
					Tail += TailVelocity * dt;
					TailVelocity.Y -= 10f * dt;
				}
				tp.Init(Tail, Head);
				if (Target != null)
				{
					bool flag = false;
					bool flag2 = false;
					float num = 2f;
					Vector3 vector = Vector3.Zero;
					if (Target.ValidLivingGamer)
					{
						Vector3 worldPosition = Target.WorldPosition;
						BoundingBox playerAABB = Target.PlayerAABB;
						playerAABB.Min += worldPosition;
						playerAABB.Max += worldPosition;
						tp.TestBoundBox(playerAABB);
					}
					if (tp._collides)
					{
						vector = tp.GetIntersection();
						flag = true;
						flag2 = true;
						num = tp._inT;
					}
					tp.Reset();
					BlockTerrain.Instance.Trace(tp);
					if (tp._collides && tp._inT < num)
					{
						flag = false;
						flag2 = true;
						vector = tp.GetIntersection();
					}
					if (flag2)
					{
						Head = vector;
						RemoveAfterDraw = true;
						if (flag)
						{
							if (Target.IsLocal)
							{
								InGameHUD.Instance.ApplyDamage(0.35f, Head);
							}
							SoundManager.Instance.PlayInstance("BulletHitHuman", Target.SoundEmitter);
						}
						else
						{
							SoundManager.Instance.PlayInstance("BulletHitDirt", SoundEmitter);
						}
						ParticleEmitter particleEmitter = _smokeEffect.CreateEmitter(CastleMinerZGame.Instance);
						particleEmitter.Reset();
						particleEmitter.Emitting = true;
						Instance.Scene.Children.Add(particleEmitter);
						particleEmitter.LocalPosition = vector;
						particleEmitter.DrawPriority = 800;
					}
					else if (!PlayedSound)
					{
						float num2 = Vector3.DistanceSquared(Head, Target.WorldPosition);
						if (num2 < 25f)
						{
							PlayedSound = true;
							SoundManager.Instance.PlayInstance("arrow", SoundEmitter);
						}
					}
				}
				else
				{
					IShootableEnemy shootableEnemy = EnemyManager.Instance.Trace(tp, false);
					if (tp._collides)
					{
						Head = tp.GetIntersection();
						RemoveAfterDraw = true;
						if (shootableEnemy != null)
						{
							shootableEnemy.TakeDamage(Head, Vector3.Normalize(HeadVelocity), ItemClass, ShooterID);
							if (shootableEnemy is BaseZombie)
							{
								SoundManager.Instance.PlayInstance("BulletHitHuman", ((BaseZombie)shootableEnemy).SoundEmitter);
							}
						}
						else
						{
							SoundManager.Instance.PlayInstance("BulletHitDirt", SoundEmitter);
						}
						Vector3 intersection = tp.GetIntersection();
						if (shootableEnemy is DragonClientEntity)
						{
							ParticleEmitter particleEmitter2 = _dragonFlashEffect.CreateEmitter(CastleMinerZGame.Instance);
							particleEmitter2.Reset();
							particleEmitter2.Emitting = true;
							Instance.Scene.Children.Add(particleEmitter2);
							particleEmitter2.LocalPosition = intersection;
							particleEmitter2.DrawPriority = 800;
						}
						ParticleEmitter particleEmitter3 = _smokeEffect.CreateEmitter(CastleMinerZGame.Instance);
						particleEmitter3.Reset();
						particleEmitter3.Emitting = true;
						Instance.Scene.Children.Add(particleEmitter3);
						particleEmitter3.LocalPosition = intersection;
						particleEmitter3.DrawPriority = 800;
						if (shootableEnemy == null)
						{
							ParticleEmitter particleEmitter4 = _sparkEffect.CreateEmitter(CastleMinerZGame.Instance);
							particleEmitter4.Reset();
							particleEmitter4.Emitting = true;
							Instance.Scene.Children.Add(particleEmitter4);
							particleEmitter4.LocalPosition = intersection;
							particleEmitter4.DrawPriority = 800;
						}
					}
				}
				Vector3 vector2 = Vector3.Normalize(HeadVelocity);
				Vector3 vector3 = ((vector2.Y != 1f) ? Vector3.UnitY : Vector3.UnitX);
				Vector3 vector4 = Vector3.Cross(vector3, vector2);
				vector3 = Vector3.Cross(vector2, vector4);
				SoundEmitter.Position = Head;
				SoundEmitter.Forward = vector2;
				SoundEmitter.Up = Vector3.Normalize(vector3);
				SoundEmitter.Velocity = HeadVelocity;
				if (!tp._collides && !BlockTerrain.Instance.IsTracerStillInWorld(Head))
				{
					RemoveAfterDraw = true;
				}
			}

			public int AddToDrawCache(TracerVertex[] vertexCache, int index)
			{
				Vector3 vector = Tail - Head;
				if (vector.LengthSquared() < 0.01f)
				{
					return index;
				}
				vector.Normalize();
				int num = index * 5;
				vertexCache[num].Position = Head;
				vertexCache[num++].Color = TracerColor;
				Vector3 vector2 = ((vector.Y != 1f) ? Vector3.UnitY : Vector3.UnitX);
				Vector3 vector3 = Vector3.Cross(vector2, vector);
				vector3.Normalize();
				vector2 = Vector3.Cross(vector, vector3);
				Vector3 vector4 = Vector3.Lerp(Head, Tail, 0.1f);
				vertexCache[num].Position = vector4 + vector2 * 0.025f;
				vertexCache[num++].Color = TracerColor;
				vertexCache[num].Position = vector4 - vector2 * 0.025f * 0.5f - vector3 * 0.025f * 0.866f;
				vertexCache[num++].Color = TracerColor;
				vertexCache[num].Position = vector4 - vector2 * 0.025f * 0.5f + vector3 * 0.025f * 0.866f;
				vertexCache[num++].Color = TracerColor;
				vertexCache[num].Position = Tail;
				vertexCache[num].Color = TailColor;
				return ++index;
			}
		}

		private const float TRACER_TIME = 2f;

		private const float TRACER_WIDTH = 0.025f;

		private const int MAX_TRACERS_PER_DRAW = 20;

		private const int VXS_PER_TRACE = 5;

		private const int POLYS_PER_TRACE = 6;

		private const int VERTEX_BUFFER_SIZE = 100;

		private const float ARROW_SOUND_CUE_DISTANCE = 5f;

		private const float ARROW_SOUND_CUE_DISTANCE_SQ = 25f;

		private static Effect _effect;

		public static ParticleEffect _smokeEffect;

		public static ParticleEffect _sparkEffect;

		public static ParticleEffect _dragonFlashEffect;

		private static DynamicVertexBuffer _currentVb;

		private static DynamicVertexBuffer _vb1;

		private static DynamicVertexBuffer _vb2;

		private static IndexBuffer _ib;

		public static TracerManager Instance;

		private List<Tracer> _tracers = new List<Tracer>();

		private List<Tracer> _unusedTracers = new List<Tracer>();

		private TracerVertex[] _vertexCache = new TracerVertex[100];

		public static void Initialize()
		{
			_effect = CastleMinerZGame.Instance.Content.Load<Effect>("Effects/Tracer");
			_smokeEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("SmokeEffect");
			_sparkEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("SparksEffect");
			_dragonFlashEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("FlashEffect");
			_vb1 = new DynamicVertexBuffer(CastleMinerZGame.Instance.GraphicsDevice, typeof(TracerVertex), 100, BufferUsage.WriteOnly);
			_vb2 = new DynamicVertexBuffer(CastleMinerZGame.Instance.GraphicsDevice, typeof(TracerVertex), 100, BufferUsage.WriteOnly);
			_ib = new IndexBuffer(CastleMinerZGame.Instance.GraphicsDevice, IndexElementSize.SixteenBits, 360, BufferUsage.WriteOnly);
			short[] array = new short[18]
			{
				1, 0, 2, 3, 0, 1, 2, 0, 3, 1,
				4, 3, 2, 4, 1, 3, 4, 2
			};
			short[] array2 = new short[360];
			short num = 0;
			int num2 = 0;
			int num3 = 0;
			while (num3 < 20)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array2[num2++] = (short)(array[i] + num);
				}
				num3++;
				num = (short)(num + 5);
			}
			_ib.SetData(array2);
		}

		public TracerManager()
		{
			Instance = this;
			Collidee = false;
			Collider = false;
			for (int i = 0; i < 100; i++)
			{
				_vertexCache[i] = new TracerVertex(Vector3.Zero, (i % 5 == 4) ? Color.Red.ToVector4() : Color.Red.ToVector4());
			}
			DrawPriority = 700;
		}

		private Tracer GetTracer()
		{
			int count = _unusedTracers.Count;
			Tracer result;
			if (count != 0)
			{
				count--;
				result = _unusedTracers[count];
				_unusedTracers.RemoveAt(count);
			}
			else
			{
				_unusedTracers.Add(new Tracer());
				result = new Tracer();
			}
			return result;
		}

		public void AddTracer(Vector3 position, Vector3 velocity, InventoryItemIDs item, byte shooterID)
		{
			Tracer tracer = GetTracer();
			tracer.Init(position, velocity, item, shooterID);
			_tracers.Add(tracer);
			EnemyManager.Instance.RegisterGunShot(position);
		}

		public void AddArrow(Vector3 position, Vector3 velocity, Player target)
		{
			Tracer tracer = GetTracer();
			tracer.Init(position, velocity, target, Color.Pink);
			_tracers.Add(tracer);
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			int count = _tracers.Count;
			float dt = (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			for (int i = 0; i < count; i++)
			{
				_tracers[i].Update(dt);
			}
			base.OnUpdate(gameTime);
		}

		private void FlushTracers(TracerVertex[] vertexCache, int numtracers, bool firsttime, GraphicsDevice g, Matrix view, Matrix projection)
		{
			if (firsttime)
			{
				g.BlendState = BlendState.NonPremultiplied;
				g.DepthStencilState = DepthStencilState.DepthRead;
				_effect.Parameters["Projection"].SetValue(projection);
				_effect.Parameters["View"].SetValue(view);
				_effect.Parameters["World"].SetValue(Matrix.Identity);
				g.Indices = _ib;
				_effect.CurrentTechnique = _effect.Techniques[0];
				_effect.CurrentTechnique.Passes[0].Apply();
				_currentVb = _vb1;
			}
			else if (_currentVb == _vb1)
			{
				_currentVb = _vb2;
			}
			else
			{
				_currentVb = _vb1;
			}
			_currentVb.SetData(vertexCache, 0, numtracers * 5);
			g.SetVertexBuffer(_currentVb);
			g.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numtracers * 5, 0, numtracers * 6);
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			int num = _tracers.Count;
			double totalSecond = gameTime.get_ElapsedGameTime().TotalSeconds;
			int num2 = 0;
			bool flag = true;
			BlendState blendState = device.BlendState;
			RasterizerState rasterizerState = device.RasterizerState;
			DepthStencilState depthStencilState = device.DepthStencilState;
			int num3 = 0;
			while (num3 < num)
			{
				num2 = _tracers[num3].AddToDrawCache(_vertexCache, num2);
				if (num2 == 20)
				{
					FlushTracers(_vertexCache, num2, flag, device, view, projection);
					flag = false;
					num2 = 0;
				}
				if (_tracers[num3].RemoveAfterDraw)
				{
					_unusedTracers.Add(_tracers[num3]);
					num--;
					if (num != num3)
					{
						_tracers[num3] = _tracers[num];
					}
					_tracers.RemoveAt(num);
				}
				else
				{
					num3++;
				}
			}
			if (num2 != 0)
			{
				FlushTracers(_vertexCache, num2, flag, device, view, projection);
				flag = false;
			}
			if (!flag)
			{
				device.RasterizerState = rasterizerState;
				device.BlendState = blendState;
			}
			base.Draw(device, gameTime, view, projection);
		}
	}
}
