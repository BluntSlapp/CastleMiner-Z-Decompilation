using System;
using System.Collections.Generic;
using DNA.Audio;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.Utils.Trace;
using DNA.Drawing;
using DNA.Drawing.Animation;
using DNA.Drawing.Particles;
using DNA.Profiling;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.AI
{
	public class DragonClientEntity : Entity, IShootableEnemy
	{
		private enum DeathAnimationState
		{
			STILL_ALIVE,
			FIRST_REACTION,
			FALLING,
			POSTCOLLISION,
			PAUSE
		}

		private struct ParticlePackage
		{
			public ParticleEffect _flashEffect;

			public ParticleEffect _firePuffEffect;

			public ParticleEffect _smokePuffEffect;
		}

		private struct DeathAnimationPackage
		{
			public string Name;

			public float BeginPositionChangeTime;

			public float PauseTime;

			public float TimeToWaitAfterStop;

			public Vector3 BoxOffset;
		}

		private class DragonTraceProbe : AABBTraceProbe
		{
			public override bool TestThisType(BlockTypeEnum e)
			{
				if (e != BlockTypeEnum.NumberOfBlocks && e != BlockTypeEnum.Log)
				{
					return BlockType.GetType(e).BlockPlayer;
				}
				return false;
			}
		}

		private const int NUM_DEATH_ANIMATIONS = 2;

		private const float ANIM_BLEND_TIME = 0.5f;

		public const float TIMEOUT_MAX_TIME = 10f;

		public const int NUM_WAYPOINTS = 20;

		private static ParticlePackage[] ParticlePackages;

		private static DeathAnimationPackage[] DeathAnimationPackages;

		private static Model DragonBody;

		public static Model DragonFeet;

		private static DragonTraceProbe MovementProbe = new DragonTraceProbe();

		private static BoundingBox HeadBox = new BoundingBox(new Vector3(-1.5f, 0f, -9.625f), new Vector3(1.5f, 2.5f, -4.125f));

		private static BoundingBox BodyBox = new BoundingBox(new Vector3(-1.75f, -1.05f, -4.2f), new Vector3(1.75f, 2.45f, 4.8f));

		private static BoundingBox DeadBodyBox = new BoundingBox(new Vector3(-2.5f, 0f, -2.5f), new Vector3(2.5f, 10f, 2.5f));

		public Plane[] HeadHitVolume;

		public Plane[] BodyHitVolume;

		public DragonPartEntity[] _dragonModel = new DragonPartEntity[2];

		public AnimationPlayer[] CurrentPlayer = new AnimationPlayer[2];

		public DragonType EType;

		public List<ActionDragonWaypoint> Waypoints = new List<ActionDragonWaypoint>();

		public List<int> NextFireballIndex = new List<int>();

		public float CurrentInterpolationTime;

		public Vector3 CurrentVelocity;

		public Vector3 TargetPosition;

		public DragonAnimEnum _currentAnimation;

		public DragonAnimEnum NextAnimation;

		public float TimeLeftBeforeFlap;

		public bool GotInitialWaypoints;

		public float TimeoutTimer;

		public float TargetRoll;

		public float Roll;

		public bool WaitingToShoot;

		public float Health;

		public bool SpawnPickups;

		public AudioEmitter SoundEmitter = new AudioEmitter();

		public SoundCue3D DragonCryCue;

		public SoundCue3D DragonFlapCue;

		public OneShotTimer FlapTimer = new OneShotTimer(TimeSpan.FromSeconds(1.0));

		private int DeathPackageIndex;

		private DeathAnimationState DeathState;

		private float DeathTimer;

		private bool OnGround;

		public TraceProbe _dragonProbe = new TraceProbe();

		public bool Dead
		{
			get
			{
				return DeathState != DeathAnimationState.STILL_ALIVE;
			}
		}

		public DragonAnimEnum CurrentAnimation
		{
			get
			{
				return _currentAnimation;
			}
			set
			{
				_currentAnimation = value;
				NextAnimation = value;
				PlaySingleClip(DragonEntity.AnimNames[(int)value], false, 0.5f);
				if (_currentAnimation == DragonAnimEnum.FLAP)
				{
					TimeLeftBeforeFlap = 11f;
				}
			}
		}

		public float ClipSpeed
		{
			get
			{
				if (CurrentPlayer[0] != null)
				{
					return CurrentPlayer[0].Speed;
				}
				return 1f;
			}
			set
			{
				for (int i = 0; i < 2; i++)
				{
					if (CurrentPlayer[i] != null)
					{
						CurrentPlayer[i].Speed = value;
					}
				}
			}
		}

		public bool ClipFinished
		{
			get
			{
				if (CurrentPlayer[0] != null)
				{
					return (CurrentPlayer[0].Duration - CurrentPlayer[0].CurrentTime).TotalSeconds < 0.5;
				}
				return true;
			}
		}

		public TimeSpan ClipCurrentTime
		{
			get
			{
				if (CurrentPlayer[0] != null)
				{
					return CurrentPlayer[0].CurrentTime;
				}
				return TimeSpan.FromSeconds(0.0);
			}
		}

		public static void Init()
		{
			ParticlePackages = new ParticlePackage[2];
			ParticlePackages[0]._flashEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("FlashEffect");
			ParticlePackages[0]._firePuffEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("FirePuff");
			ParticlePackages[0]._smokePuffEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("BigSmokePuff");
			ParticlePackages[1]._flashEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("IceFlash");
			ParticlePackages[1]._firePuffEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("IcePuff");
			ParticlePackages[1]._smokePuffEffect = CastleMinerZGame.Instance.Content.Load<ParticleEffect>("BigPuff");
			DeathAnimationPackages = new DeathAnimationPackage[2];
			DeathAnimationPackages[0].Name = "death_air_1";
			DeathAnimationPackages[0].BeginPositionChangeTime = 1f;
			DeathAnimationPackages[0].PauseTime = 1.36666667f;
			DeathAnimationPackages[0].TimeToWaitAfterStop = 5f;
			DeathAnimationPackages[0].BoxOffset = new Vector3(0f, 0f, 7f);
			DeathAnimationPackages[1].Name = "death_air_2";
			DeathAnimationPackages[1].BeginPositionChangeTime = 0.9f;
			DeathAnimationPackages[1].PauseTime = 1.4f;
			DeathAnimationPackages[1].TimeToWaitAfterStop = 5f;
			DeathAnimationPackages[1].BoxOffset = new Vector3(0f, 0f, -3.5f);
			DragonBody = CastleMinerZGame.Instance.Content.Load<Model>("Enemies\\Dragon\\DragonBodyHigh");
			DragonFeet = CastleMinerZGame.Instance.Content.Load<Model>("Enemies\\Dragon\\DragonFeetHigh");
		}

		public DragonClientEntity(DragonTypeEnum type, float health)
		{
			EType = DragonType.GetDragonType(type);
			base.LocalRotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, 0f);
			base.LocalPosition = Vector3.Zero;
			Health = ((health == -1f) ? EType.StartingHealth : health);
			DeathState = DeathAnimationState.STILL_ALIVE;
			DrawPriority = (int)(515 + type);
			_dragonModel[0] = new DragonPartEntity(EType, DragonBody);
			_dragonModel[1] = new DragonPartEntity(EType, DragonFeet);
			CurrentInterpolationTime = 0f;
			base.Children.Add(_dragonModel[0]);
			base.Children.Add(_dragonModel[1]);
			OnGround = false;
			GotInitialWaypoints = false;
			DrawPriority = (int)(515 + type);
			Collider = false;
			Visible = false;
			HeadHitVolume = new Plane[6];
			BodyHitVolume = new Plane[6];
			TraceProbe.MakeOrientedBox(base.LocalToWorld, HeadBox, HeadHitVolume);
			TraceProbe.MakeOrientedBox(base.LocalToWorld, BodyBox, BodyHitVolume);
			TimeoutTimer = 0f;
			WaitingToShoot = false;
			SpawnPickups = false;
		}

		public void AddActionWaypoint(ActionDragonWaypoint inwpt)
		{
			TimeoutTimer = 0f;
			for (int i = 0; i < Waypoints.Count; i++)
			{
				if (Waypoints[i].BaseWpt.HostTime > inwpt.BaseWpt.HostTime)
				{
					if (i >= 2)
					{
						Waypoints.Insert(i, inwpt);
					}
					return;
				}
			}
			if (Waypoints.Count > 0 && CurrentInterpolationTime > Waypoints[Waypoints.Count - 1].BaseWpt.HostTime)
			{
				Waypoints.Clear();
				ActionDragonWaypoint item = default(ActionDragonWaypoint);
				item.Action = DragonWaypointActionEnum.GOTO;
				item.BaseWpt.Position = base.WorldPosition;
				item.BaseWpt.Velocity = CurrentVelocity;
				item.BaseWpt.HostTime = CurrentInterpolationTime;
				item.BaseWpt.Animation = CurrentAnimation;
				item.BaseWpt.Sound = DragonSoundEnum.NONE;
				item.FireballIndex = 0;
				Waypoints.Add(item);
			}
			Waypoints.Add(inwpt);
		}

		public bool Trace(TraceProbe tp)
		{
			_dragonProbe.Init(tp._start, tp._end);
			_dragonProbe.TestShape(HeadHitVolume, IntVector3.Zero);
			if (_dragonProbe._collides)
			{
				tp._collides = true;
				tp._end = _dragonProbe._end;
				tp._inT = _dragonProbe._inT;
				tp._inNormal = _dragonProbe._inNormal;
				tp._inFace = _dragonProbe._inFace;
			}
			_dragonProbe.Reset();
			_dragonProbe.TestShape(BodyHitVolume, IntVector3.Zero);
			if (_dragonProbe._collides && _dragonProbe._inT < tp._inT)
			{
				tp._collides = true;
				tp._end = _dragonProbe._end;
				tp._inT = _dragonProbe._inT;
				tp._inNormal = _dragonProbe._inNormal;
				tp._inFace = _dragonProbe._inFace;
			}
			return tp._collides;
		}

		public bool IsHeadshot(Vector3 position)
		{
			return BodyHitVolume[1].DotCoordinate(position) > 0f;
		}

		public void TakeDamage(Vector3 damagePosition, Vector3 damageDirection, InventoryItem.InventoryItemClass itemClass, byte shooterID)
		{
			DamageType enemyDamageType = itemClass.EnemyDamageType;
			float enemyDamage = itemClass.EnemyDamage;
			if (CastleMinerZGame.Instance.IsLocalPlayerId(shooterID))
			{
				CastleMinerZPlayerStats.ItemStats itemStats = CastleMinerZGame.Instance.PlayerStats.GetItemStats(itemClass.ID);
				itemStats.Hits++;
			}
			if (!Dead)
			{
				float num = (IsHeadshot(damagePosition) ? 1f : 2.5f);
				enemyDamage *= num;
				Health -= enemyDamage;
				EnemyManager.Instance.DragonHasBeenHit();
				if (Health <= 0f)
				{
					KillDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, base.WorldPosition, shooterID, itemClass.ID);
				}
			}
		}

		public void Kill(bool spawnPickups)
		{
			if (!Dead)
			{
				DeathPackageIndex = 0;
				SpawnPickups = spawnPickups;
				PlaySingleClip(DeathAnimationPackages[DeathPackageIndex].Name, false, 0.25f);
				DeathState = DeathAnimationState.FIRST_REACTION;
				DeathTimer = DeathAnimationPackages[DeathPackageIndex].PauseTime;
				float heading = DragonBaseState.GetHeading(base.LocalToWorld.Forward, 0f);
				base.LocalRotation = Quaternion.CreateFromYawPitchRoll(heading, 0f, 0f);
				int damageType = (int)EType.DamageType;
				ParticleEmitter particleEmitter = ParticlePackages[damageType]._flashEffect.CreateEmitter(CastleMinerZGame.Instance);
				particleEmitter.Reset();
				particleEmitter.Emitting = true;
				particleEmitter.LocalPosition = Vector3.Zero;
				particleEmitter.DrawPriority = 800;
				base.Children.Add(particleEmitter);
				particleEmitter = ParticlePackages[damageType]._firePuffEffect.CreateEmitter(CastleMinerZGame.Instance);
				particleEmitter.Reset();
				particleEmitter.Emitting = true;
				particleEmitter.LocalPosition = Vector3.Zero;
				particleEmitter.DrawPriority = 800;
				base.Children.Add(particleEmitter);
				particleEmitter = ParticlePackages[damageType]._smokePuffEffect.CreateEmitter(CastleMinerZGame.Instance);
				particleEmitter.Reset();
				particleEmitter.Emitting = true;
				particleEmitter.LocalPosition = Vector3.Zero;
				particleEmitter.DrawPriority = 800;
				base.Children.Add(particleEmitter);
			}
		}

		public void HandleUpdateDragonMessage(UpdateDragonMessage msg)
		{
			AddActionWaypoint(ActionDragonWaypoint.CreateFromBase(msg.Waypoint));
		}

		public void HandleDragonAttackMessage(DragonAttackMessage msg)
		{
			DragonWaypointActionEnum action = (msg.AnimatedAttack ? DragonWaypointActionEnum.ANIMSHOOT : DragonWaypointActionEnum.QUICKSHOOT);
			AddActionWaypoint(ActionDragonWaypoint.Create(msg.Waypoint, msg.Target, action, msg.FireballIndex));
		}

		public void ShootFireball(Vector3 targetPosition)
		{
			Bone bone = _dragonModel[0].Skeleton["Bip01_Ponytail1"];
			Matrix matrix = _dragonModel[0].WorldBoneTransforms[bone.Index];
			Vector3 translation = matrix.Translation;
			FireballEntity t = new FireballEntity(translation, targetPosition, NextFireballIndex[0], EType, EnemyManager.Instance.DragonControlledLocally);
			NextFireballIndex.RemoveAt(0);
			base.Scene.Children.Add(t);
		}

		public void ProcessWaypoint(ActionDragonWaypoint waypoint)
		{
			switch (waypoint.Action)
			{
			case DragonWaypointActionEnum.ANIMSHOOT:
				TargetPosition = waypoint.ActionPosition;
				WaitingToShoot = true;
				CurrentAnimation = DragonAnimEnum.ATTACK;
				NextFireballIndex.Add(waypoint.FireballIndex);
				break;
			case DragonWaypointActionEnum.QUICKSHOOT:
				NextFireballIndex.Add(waypoint.FireballIndex);
				ShootFireball(waypoint.ActionPosition);
				break;
			}
			if (waypoint.BaseWpt.Sound != 0)
			{
				DragonSoundEnum sound = waypoint.BaseWpt.Sound;
				if (sound == DragonSoundEnum.CRY)
				{
					SoundManager.Instance.PlayInstance("DragonScream", SoundEmitter);
				}
			}
		}

		private void UpdateWhileDead(DNAGame game, GameTime gameTime)
		{
			float num = (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			if (DeathState < DeathAnimationState.POSTCOLLISION)
			{
				CurrentVelocity.Y -= 10f * num;
				if (DeathState == DeathAnimationState.FALLING)
				{
					MoveDragonWithCollision(num);
					if (OnGround)
					{
						if (!ClipFinished && !CurrentPlayer[0].Playing)
						{
							SoundManager.Instance.PlayInstance("DragonFall", SoundEmitter);
							for (int i = 0; i < 2; i++)
							{
								CurrentPlayer[i].Play();
							}
						}
						CurrentVelocity.X *= 0.7f;
						CurrentVelocity.Z *= 0.7f;
						if (CurrentVelocity.LengthSquared() < 0.5f)
						{
							DeathState = DeathAnimationState.POSTCOLLISION;
							DeathTimer = DeathAnimationPackages[DeathPackageIndex].TimeToWaitAfterStop;
						}
					}
				}
				else
				{
					base.LocalPosition += CurrentVelocity * num;
				}
			}
			switch (DeathState)
			{
			case DeathAnimationState.FIRST_REACTION:
			{
				float num2 = (float)ClipCurrentTime.TotalSeconds;
				float y = MathTools.MapAndLerp(num2, DeathAnimationPackages[DeathPackageIndex].BeginPositionChangeTime, DeathAnimationPackages[DeathPackageIndex].PauseTime, -11.75f, 0f);
				bool flag = num2 >= DeathAnimationPackages[DeathPackageIndex].PauseTime;
				Vector3 localPosition = _dragonModel[0].LocalPosition;
				localPosition.Y = y;
				for (int j = 0; j < 2; j++)
				{
					_dragonModel[j].LocalPosition = localPosition;
					if (flag)
					{
						CurrentPlayer[j].Pause();
					}
				}
				if (flag)
				{
					DeathState = DeathAnimationState.FALLING;
				}
				break;
			}
			case DeathAnimationState.POSTCOLLISION:
				if (ClipFinished)
				{
					DeathTimer -= num;
				}
				break;
			}
			for (int k = 0; k < base.Children.Count; k++)
			{
				base.Children[k].Update(game, gameTime);
			}
			if (DeathState == DeathAnimationState.POSTCOLLISION && DeathTimer < 0f)
			{
				if (SpawnPickups)
				{
					Vector3 boxOffset = DeathAnimationPackages[DeathPackageIndex].BoxOffset;
					boxOffset = Vector3.TransformNormal(boxOffset, base.LocalToWorld);
					Vector3 location = base.WorldPosition + boxOffset;
					EnemyManager.Instance.SpawnDragonPickups(location);
				}
				EnemyManager.Instance.RemoveDragonEntity();
			}
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			if (Dead)
			{
				UpdateWhileDead(game, gameTime);
				return;
			}
			float num = (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			TimeoutTimer += num;
			if (TimeoutTimer > 10f)
			{
				TimeoutTimer = -3600f;
				RemoveDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer);
			}
			if (Waypoints.Count < 3 && !GotInitialWaypoints)
			{
				return;
			}
			GotInitialWaypoints = true;
			Visible = true;
			if (CurrentInterpolationTime == 0f)
			{
				CurrentAnimation = DragonAnimEnum.SOAR;
				CurrentInterpolationTime = Waypoints[0].BaseWpt.HostTime;
			}
			else if (Waypoints[Waypoints.Count - 1].BaseWpt.HostTime - CurrentInterpolationTime < 0.1f)
			{
				CurrentInterpolationTime += (float)gameTime.get_ElapsedGameTime().TotalSeconds * 0.8f;
			}
			else
			{
				CurrentInterpolationTime += (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			}
			while (Waypoints.Count > 1 && CurrentInterpolationTime >= Waypoints[1].BaseWpt.HostTime)
			{
				Waypoints.RemoveAt(0);
				ProcessWaypoint(Waypoints[0]);
			}
			NextAnimation = Waypoints[0].BaseWpt.Animation;
			Vector3 outpos;
			Vector3 outvel;
			if (Waypoints.Count >= 2)
			{
				ActionDragonWaypoint.InterpolatePositionVelocity(CurrentInterpolationTime, Waypoints[0], Waypoints[1], out outpos, out outvel);
			}
			else
			{
				ActionDragonWaypoint.Extrapolate(CurrentInterpolationTime, Waypoints[0], out outpos, out outvel);
			}
			Roll = MathTools.MoveTowardTarget(Roll, Waypoints[0].BaseWpt.TargetRoll, EType.RollRate, (float)gameTime.get_ElapsedGameTime().TotalSeconds);
			CurrentVelocity = outvel;
			Vector3 vector = Vector3.Normalize(outvel);
			Vector3 vector2 = vector;
			Vector3 forward = base.LocalToWorld.Forward;
			vector2.Y = 0f;
			forward.Y = 0f;
			vector2.Normalize();
			forward.Normalize();
			Vector3.Cross(vector2, forward);
			Vector3 up = Vector3.Up;
			Vector3 vector3 = Vector3.Normalize(Vector3.Cross(vector, up));
			up = Vector3.Cross(vector3, vector);
			Matrix identity = Matrix.Identity;
			identity.Forward = vector;
			identity.Right = vector3;
			identity.Up = up;
			identity = Matrix.Multiply(Matrix.CreateFromYawPitchRoll(0f, 0f, Roll), identity);
			identity.Translation = outpos;
			base.LocalToParent = identity;
			SoundEmitter.Position = identity.Translation;
			SoundEmitter.Forward = identity.Forward;
			SoundEmitter.Up = identity.Up;
			SoundEmitter.Velocity = Vector3.Zero;
			TraceProbe.MakeOrientedBox(base.LocalToWorld, HeadBox, HeadHitVolume);
			TraceProbe.MakeOrientedBox(base.LocalToWorld, BodyBox, BodyHitVolume);
			for (int i = 0; i < base.Children.Count; i++)
			{
				base.Children[i].Update(game, gameTime);
			}
			if (WaitingToShoot)
			{
				if (CurrentAnimation == DragonAnimEnum.ATTACK)
				{
					if (ClipCurrentTime.TotalSeconds > 1.1333333333333333)
					{
						ShootFireball(TargetPosition);
						WaitingToShoot = false;
					}
				}
				else
				{
					WaitingToShoot = false;
				}
			}
			if (ClipFinished)
			{
				CurrentAnimation = NextAnimation;
			}
			FlapTimer.Update(gameTime.get_ElapsedGameTime());
			if (FlapTimer.Expired && CurrentAnimation != 0)
			{
				SoundManager.Instance.PlayInstance("WingFlap", SoundEmitter);
				FlapTimer = new OneShotTimer(TimeSpan.FromSeconds(1.0));
			}
		}

		public void PlaySingleClip(string name, bool loop, float blendTime)
		{
			for (int i = 0; i < 2; i++)
			{
				CurrentPlayer[i] = _dragonModel[i].PlayClip(name, loop, TimeSpan.FromSeconds(blendTime));
			}
		}

		public bool MoveDragonWithCollision(float dt)
		{
			using (Profiler.TimeSection("Pickup Collision", ProfilerThreadEnum.MAIN))
			{
				bool result = false;
				Vector3 boxOffset = DeathAnimationPackages[DeathPackageIndex].BoxOffset;
				boxOffset = Vector3.TransformNormal(boxOffset, base.LocalToWorld);
				float num = dt;
				Vector3 vector = base.WorldPosition + boxOffset;
				Vector3 vector2 = vector;
				Vector3 vector3 = CurrentVelocity;
				OnGround = false;
				MovementProbe.SkipEmbedded = true;
				int num2 = 0;
				do
				{
					Vector3 vector4 = vector2;
					Vector3 vector5 = Vector3.Multiply(vector3, num);
					vector2 += vector5;
					MovementProbe.Init(vector4, vector2, DeadBodyBox);
					BlockTerrain.Instance.Trace(MovementProbe);
					if (MovementProbe._collides)
					{
						result = true;
						if (MovementProbe._inFace == BlockFace.POSY)
						{
							OnGround = true;
						}
						if (MovementProbe._startsIn)
						{
							break;
						}
						float num3 = Math.Max(MovementProbe._inT - 0.001f, 0f);
						vector2 = vector4 + vector5 * num3;
						vector3 -= Vector3.Multiply(MovementProbe._inNormal, Vector3.Dot(MovementProbe._inNormal, vector3));
						num *= 1f - num3;
						if (num <= 1E-07f)
						{
							break;
						}
						if (vector3.LengthSquared() <= 1E-06f || Vector3.Dot(CurrentVelocity, vector3) <= 1E-06f)
						{
							vector3 = Vector3.Zero;
							break;
						}
					}
					num2++;
				}
				while (MovementProbe._collides && num2 < 4);
				if (num2 == 4)
				{
					vector3 = Vector3.Zero;
				}
				vector2 -= boxOffset;
				if (vector2.Y < -64f)
				{
					vector2.Y = -64f;
					vector3.Y = 0f;
					OnGround = true;
				}
				base.LocalPosition = vector2;
				CurrentVelocity = vector3;
				return result;
			}
		}
	}
}
