using System;
using DNA.Audio;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.Utils.Trace;
using DNA.Drawing;
using DNA.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.AI
{
	public class FireballEntity : Entity
	{
		private class FireballTraceProbe : AABBTraceProbe
		{
			public override bool TestThisType(BlockTypeEnum e)
			{
				if (e != BlockTypeEnum.NumberOfBlocks)
				{
					return BlockType.GetType(e).BlockPlayer;
				}
				return false;
			}
		}

		private struct ParticlePackage
		{
			public ParticleEffect _fireBallEffect;

			public ParticleEffect _smokeTrailEffect;

			public ParticleEffect _fireTrailEffect;

			public ParticleEffect _fireGlowEffect;

			public ParticleEffect _flashEffect;

			public ParticleEffect _firePuffEffect;

			public ParticleEffect _smokePuffEffect;

			public ParticleEffect _rockBlastEffect;

			public Model _fireballModel;

			public string _flightSoundName;

			public string _detonateSoundName;
		}

		public AudioEmitter SoundEmitter = new AudioEmitter();

		private SoundCue3D FireballCue;

		public FireballModelEntity model;

		private FireballTraceProbe TraceProbe = new FireballTraceProbe();

		public BoundingBox FireballAABB = new BoundingBox(new Vector3(-0.57f), new Vector3(0.57f));

		public Vector3 Target;

		public Vector3 Velocity;

		public ParticleEmitter SmokeEmitter;

		public ParticleEmitter FireEmitter;

		public ParticleEmitter FireBallEmitter;

		public ParticleEmitter FireGlowEmitter;

		public int FireballIndex;

		public bool SpawnedLocally;

		public bool WasInLoadedArea;

		public bool Detonated;

		public DragonType EType;

		private ParticlePackage ParticleDef;

		private static ParticlePackage[] ParticlePackages;

		public static void Init()
		{
			ParticlePackages = new ParticlePackage[2];
			ParticlePackages[0]._fireTrailEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("FireTrail");
			ParticlePackages[0]._smokeTrailEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("SmokeTrail");
			ParticlePackages[0]._fireGlowEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("FireGlow");
			ParticlePackages[0]._fireBallEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("FireBallEffect");
			ParticlePackages[0]._flashEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("FlashEffect");
			ParticlePackages[0]._firePuffEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("FirePuff");
			ParticlePackages[0]._smokePuffEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("BigSmokePuff");
			ParticlePackages[0]._rockBlastEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("BigRockBlast");
			ParticlePackages[0]._fireballModel = CastleMinerZGame.Instance.Content.Load<Model>("FireBall");
			ParticlePackages[0]._flightSoundName = "Fireball";
			ParticlePackages[0]._detonateSoundName = "Explosion";
			ParticlePackages[1]._fireTrailEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("IceTrail");
			ParticlePackages[1]._smokeTrailEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("PuffTrail");
			ParticlePackages[1]._fireGlowEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("IceGlow");
			ParticlePackages[1]._fireBallEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("IceBallEffect");
			ParticlePackages[1]._flashEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("IceFlash");
			ParticlePackages[1]._firePuffEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("IcePuff");
			ParticlePackages[1]._smokePuffEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("BigPuff");
			ParticlePackages[1]._rockBlastEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("BigSnowBlast");
			ParticlePackages[1]._fireballModel = CastleMinerZGame.Instance.Content.Load<Model>("Snowball");
			ParticlePackages[1]._flightSoundName = "Iceball";
			ParticlePackages[1]._detonateSoundName = "Freeze";
		}

		public FireballEntity(Vector3 spawnposition, Vector3 target, int index, DragonType dragonType, bool spawnedLocally)
		{
			EType = dragonType;
			ParticleDef = ParticlePackages[(int)EType.DamageType];
			model = new FireballModelEntity(ParticleDef._fireballModel);
			model.EnableDefaultLighting();
			base.Children.Add(model);
			FireballIndex = index;
			SpawnedLocally = spawnedLocally;
			WasInLoadedArea = false;
			SmokeEmitter = ParticleDef._smokeTrailEffect.CreateEmitter(CastleMinerZGame.Instance);
			SmokeEmitter.Emitting = true;
			SmokeEmitter.DrawPriority = 800;
			base.Children.Add(SmokeEmitter);
			FireEmitter = ParticleDef._fireTrailEffect.CreateEmitter(CastleMinerZGame.Instance);
			FireEmitter.Emitting = true;
			FireEmitter.DrawPriority = 800;
			base.Children.Add(FireEmitter);
			FireGlowEmitter = ParticleDef._fireGlowEffect.CreateEmitter(CastleMinerZGame.Instance);
			FireGlowEmitter.Emitting = true;
			FireGlowEmitter.DrawPriority = 800;
			base.Children.Add(FireGlowEmitter);
			FireBallEmitter = ParticleDef._fireBallEffect.CreateEmitter(CastleMinerZGame.Instance);
			FireBallEmitter.Emitting = true;
			FireBallEmitter.DrawPriority = 800;
			base.Children.Add(FireBallEmitter);
			base.LocalPosition = spawnposition;
			Vector3 vector = target - spawnposition;
			Velocity = vector * (EType.FireballVelocity / vector.Length());
			float yaw = (float)Math.Atan2(0f - Velocity.X, 0f - Velocity.Z);
			base.LocalRotation = Quaternion.CreateFromYawPitchRoll(yaw, 0f, 0f);
			Detonated = false;
			EnemyManager.Instance.AddFireball(this);
			FireballCue = SoundManager.Instance.PlayInstance(ParticleDef._flightSoundName, SoundEmitter);
		}

		public void Detonate(Vector3 position)
		{
			if (!Detonated)
			{
				Detonated = true;
				model.RemoveFromParent();
				FireGlowEmitter.Emitting = false;
				FireBallEmitter.Emitting = false;
				FireEmitter.Emitting = false;
				SmokeEmitter.Emitting = false;
				FireballCue.Stop(AudioStopOptions.Immediate);
				SoundManager.Instance.PlayInstance(ParticleDef._detonateSoundName, SoundEmitter);
				ParticleEmitter particleEmitter = ParticleDef._flashEffect.CreateEmitter(CastleMinerZGame.Instance);
				particleEmitter.Reset();
				particleEmitter.Emitting = true;
				particleEmitter.LocalPosition = position;
				particleEmitter.DrawPriority = 800;
				base.Scene.Children.Add(particleEmitter);
				particleEmitter = ParticleDef._firePuffEffect.CreateEmitter(CastleMinerZGame.Instance);
				particleEmitter.Reset();
				particleEmitter.Emitting = true;
				particleEmitter.LocalPosition = position;
				particleEmitter.DrawPriority = 800;
				base.Scene.Children.Add(particleEmitter);
				particleEmitter = ParticleDef._smokePuffEffect.CreateEmitter(CastleMinerZGame.Instance);
				particleEmitter.Reset();
				particleEmitter.Emitting = true;
				particleEmitter.LocalPosition = position;
				particleEmitter.DrawPriority = 800;
				base.Scene.Children.Add(particleEmitter);
				particleEmitter = ParticleDef._rockBlastEffect.CreateEmitter(CastleMinerZGame.Instance);
				particleEmitter.Reset();
				particleEmitter.Emitting = true;
				particleEmitter.LocalPosition = position;
				particleEmitter.DrawPriority = 800;
				base.Scene.Children.Add(particleEmitter);
			}
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			SoundEmitter.Position = base.LocalPosition;
			SoundEmitter.Forward = base.LocalToWorld.Forward;
			SoundEmitter.Up = Vector3.Up;
			SoundEmitter.Velocity = Velocity;
			if (Detonated)
			{
				if (!FireEmitter.HasActiveParticles && !SmokeEmitter.HasActiveParticles)
				{
					EnemyManager.Instance.RemoveFireball(this);
				}
			}
			else
			{
				Vector3 vector = base.LocalPosition + Velocity * (float)gameTime.get_ElapsedGameTime().TotalSeconds;
				if (vector.Y < -66f)
				{
					EnemyManager.Instance.RemoveFireball(this);
					return;
				}
				if (!BlockTerrain.Instance.IsInsideWorld(vector))
				{
					if (WasInLoadedArea)
					{
						EnemyManager.Instance.RemoveFireball(this);
						return;
					}
					base.LocalPosition = vector;
					model.LocalRotation = Quaternion.CreateFromYawPitchRoll(0f, (float)gameTime.get_TotalGameTime().TotalSeconds * 3f % ((float)Math.PI * 2f), 0f);
					base.Update(game, gameTime);
					return;
				}
				WasInLoadedArea = true;
				TraceProbe.Init(base.LocalPosition, vector, FireballAABB);
				Vector3 worldPosition = CastleMinerZGame.Instance.LocalPlayer.WorldPosition;
				BoundingBox playerAABB = CastleMinerZGame.Instance.LocalPlayer.PlayerAABB;
				playerAABB.Min += worldPosition;
				playerAABB.Max += worldPosition;
				TraceProbe.TestBoundBox(playerAABB);
				if (!TraceProbe._collides)
				{
					TraceProbe.Reset();
					BlockTerrain.Instance.Trace(TraceProbe);
				}
				if (TraceProbe._collides)
				{
					Vector3 intersection = TraceProbe.GetIntersection();
					Detonate(intersection);
					if (SpawnedLocally)
					{
						EnemyManager.Instance.DetonateFireball(intersection, FireballIndex, EType);
					}
				}
				else
				{
					base.LocalPosition = vector;
					model.LocalRotation = Quaternion.CreateFromYawPitchRoll(0f, (float)gameTime.get_TotalGameTime().TotalSeconds * 3f % ((float)Math.PI * 2f), 0f);
				}
			}
			base.Update(game, gameTime);
		}
	}
}
