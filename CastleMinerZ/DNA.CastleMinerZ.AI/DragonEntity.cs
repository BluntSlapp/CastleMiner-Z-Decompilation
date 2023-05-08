using System;
using System.Collections.Generic;
using DNA.CastleMinerZ.Net;
using DNA.Drawing;
using DNA.Drawing.Animation;
using DNA.Timers;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DragonEntity : Entity
	{
		public const float UPDATE_INTERVAL = 0.2f;

		public const float DEBT_BEFORE_FLAPPING = 1f;

		public const float DEBT_ASCEND_MULTIPLIER = 2f;

		public const float DEBT_DESCEND_MULTIPLIER = 0.5f;

		public const float DEBT_YAW_MULTIPLIER = 1.5f;

		public const float DEBT_REMOVAL_RATE = 0.5f;

		public const float DEBT_AMBIENT = 0.2f;

		public const float GUESS_AT_LATENCY = 0.4f;

		public const float DISTANCE_FROM_LP_TO_MIGRATE_DRAGON = 150f;

		public static readonly string[] AnimNames = new string[4] { "flying_idle", "fly_forward", "gethit", "Idle" };

		public static int NextFireballIndex = 0;

		public DragonPartEntity _dragonModel;

		public AnimationPlayer CurrentPlayer;

		public StateMachine<DragonEntity> StateMachine;

		public List<Vector3> Gunshots = new List<Vector3>();

		public DragonType EType;

		public float _velocity;

		public float TargetVelocity;

		public bool FirstTimeForDefaultState;

		public bool HadTargetThisPass;

		public int LoiterCount;

		public float LoiterTimer;

		public float NextUpdateTime;

		public Player Target;

		public Vector3 TravelTarget;

		public bool ShotPending;

		public Vector3 ShootTarget;

		public DragonAnimEnum _currentAnimation;

		public DragonAnimEnum NextAnimation;

		public float FlapDebt;

		public int ShotsLeft;

		public float TimeLeftBeforeNextShot;

		public float TimeLeftTilShotsHeard;

		public float TimeLeftBeforeNextViewCheck;

		public float DefaultHeading;

		public float TargetAltitude;

		public float _yaw;

		public float TargetYaw;

		public float _roll;

		public float TargetRoll;

		public float _pitch;

		public float TargetPitch;

		public int UpdatesSent;

		public int AnimationChangeUpdate;

		public int FlapChangeUpdate;

		public int LoitersLeft;

		public bool Removed;

		public DragonSoundEnum NextSound;

		public int ChancesToNotAttack;

		public float DragonTime;

		public bool ForBiome;

		public bool MigrateDragon;

		public Player MigrateDragonTo;

		public OneShotTimer CryTimer = new OneShotTimer(TimeSpan.FromSeconds(5.0));

		public float Velocity
		{
			get
			{
				return _velocity;
			}
			set
			{
				_velocity = value;
				TargetVelocity = value;
			}
		}

		public float Yaw
		{
			get
			{
				return _yaw;
			}
			set
			{
				_yaw = value;
				TargetYaw = value;
			}
		}

		public float Roll
		{
			get
			{
				return _roll;
			}
			set
			{
				_roll = value;
				TargetRoll = value;
			}
		}

		public float Pitch
		{
			get
			{
				return _pitch;
			}
			set
			{
				_pitch = value;
				TargetPitch = value;
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
				PlaySingleClip(AnimNames[(int)value], false);
			}
		}

		public float ClipSpeed
		{
			get
			{
				if (CurrentPlayer != null)
				{
					return CurrentPlayer.Speed;
				}
				return 1f;
			}
			set
			{
				if (CurrentPlayer != null)
				{
					CurrentPlayer.Speed = value;
				}
			}
		}

		public bool ClipFinished
		{
			get
			{
				if (CurrentPlayer != null)
				{
					return CurrentPlayer.Finished;
				}
				return true;
			}
		}

		public TimeSpan ClipCurrentTime
		{
			get
			{
				if (CurrentPlayer != null)
				{
					return CurrentPlayer.CurrentTime;
				}
				return TimeSpan.FromSeconds(0.0);
			}
		}

		public DragonEntity(DragonTypeEnum type, bool forBiome, DragonHostMigrationInfo miginfo)
		{
			EType = DragonType.GetDragonType(type);
			base.LocalRotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, 0f);
			base.LocalPosition = Vector3.Zero;
			Target = null;
			TimeLeftBeforeNextShot = 0f;
			ShotsLeft = 0;
			FirstTimeForDefaultState = true;
			FlapDebt = 0f;
			UpdatesSent = 0;
			FlapChangeUpdate = 0;
			AnimationChangeUpdate = 0;
			Removed = false;
			NextSound = DragonSoundEnum.NONE;
			Velocity = EType.Speed;
			ShotsLeft = 0;
			ShotPending = false;
			ShootTarget = Vector3.Zero;
			ChancesToNotAttack = EType.ChancesToNotAttack;
			TimeLeftTilShotsHeard = 0f;
			DrawPriority = (int)(515 + type);
			_dragonModel = new DragonPartEntity(EType, DragonClientEntity.DragonFeet);
			base.Children.Add(_dragonModel);
			MigrateDragon = false;
			MigrateDragonTo = null;
			Collider = false;
			HadTargetThisPass = false;
			StateMachine = new StateMachine<DragonEntity>(this);
			Visible = false;
			NextUpdateTime = -1f;
			LoiterCount = 0;
			DragonTime = 0f;
			ForBiome = forBiome;
			if (miginfo == null)
			{
				InitSpawnState();
			}
			else
			{
				InitAfterHostChange(miginfo);
			}
		}

		public void InitAfterHostChange(DragonHostMigrationInfo miginfo)
		{
			DragonTime = miginfo.NextDragonTime;
			base.LocalPosition = miginfo.Position;
			Yaw = miginfo.Yaw;
			TargetYaw = miginfo.TargetYaw;
			Roll = miginfo.Roll;
			TargetRoll = miginfo.TargetRoll;
			Pitch = miginfo.Pitch;
			TargetPitch = miginfo.TargetPitch;
			TargetVelocity = miginfo.TargetVelocity;
			DefaultHeading = miginfo.DefaultHeading;
			Velocity = miginfo.Velocity;
			TargetVelocity = miginfo.TargetVelocity;
			NextUpdateTime = miginfo.NextUpdateTime;
			NextFireballIndex = miginfo.NextFireballIndex;
			ForBiome = miginfo.ForBiome;
			Target = CastleMinerZGame.Instance.LocalPlayer;
			TravelTarget = miginfo.Target;
			FirstTimeForDefaultState = false;
			ChancesToNotAttack = 0;
			LoitersLeft = 0;
			CurrentAnimation = miginfo.Animation;
			FlapDebt = miginfo.FlapDebt;
			StateMachine.ChangeState(DragonBaseState.GetNextAttackType(this));
		}

		public void InitSpawnState()
		{
			Vector3 worldPosition = CastleMinerZGame.Instance.LocalPlayer.WorldPosition;
			float num = MathTools.RandomFloat(-(float)Math.PI / 2f, (float)Math.PI / 2f);
			Vector3 vector = DragonBaseState.MakeYawVector(num);
			worldPosition -= vector * EType.SpawnDistance;
			worldPosition.Y = EType.CruisingAltitude;
			TargetAltitude = EType.CruisingAltitude;
			Yaw = (DefaultHeading = num);
			Pitch = 0f;
			CurrentAnimation = DragonAnimEnum.FLAP;
			Velocity = EType.Speed;
			Target = null;
			base.LocalRotation = Quaternion.CreateFromYawPitchRoll(Yaw, 0f, 0f);
			base.LocalPosition = worldPosition;
			StateMachine.ChangeState(DragonStates.Default);
		}

		public void DoMigration()
		{
			DragonHostMigrationInfo dragonHostMigrationInfo = new DragonHostMigrationInfo();
			dragonHostMigrationInfo.Yaw = Yaw;
			dragonHostMigrationInfo.TargetYaw = TargetYaw;
			dragonHostMigrationInfo.Roll = Roll;
			dragonHostMigrationInfo.TargetRoll = TargetRoll;
			dragonHostMigrationInfo.Pitch = Pitch;
			dragonHostMigrationInfo.TargetPitch = TargetPitch;
			dragonHostMigrationInfo.NextDragonTime = DragonTime + 0.4f;
			dragonHostMigrationInfo.NextUpdateTime = NextUpdateTime + 0.4f;
			dragonHostMigrationInfo.Position = base.LocalPosition;
			dragonHostMigrationInfo.Velocity = Velocity;
			dragonHostMigrationInfo.TargetVelocity = TargetVelocity;
			dragonHostMigrationInfo.DefaultHeading = DefaultHeading;
			dragonHostMigrationInfo.NextFireballIndex = NextFireballIndex;
			dragonHostMigrationInfo.ForBiome = ForBiome;
			dragonHostMigrationInfo.Target = TravelTarget;
			dragonHostMigrationInfo.EType = EType.EType;
			EnemyManager.Instance.MigrateDragon(MigrateDragonTo, dragonHostMigrationInfo);
		}

		public int GetNextFireballIndex()
		{
			return (CastleMinerZGame.Instance.LocalPlayer.Gamer.Id << 23) | NextFireballIndex++;
		}

		public void UpdateSounds(TimeSpan ElapsedGameTime)
		{
			CryTimer.Update(ElapsedGameTime);
			if (NextSound == DragonSoundEnum.NONE && CryTimer.Expired)
			{
				NextSound = DragonSoundEnum.CRY;
				CryTimer = new OneShotTimer(TimeSpan.FromSeconds(MathTools.RandomInt(6, 14)));
			}
		}

		public void RegisterGunshot(Vector3 position)
		{
			Gunshots.Add(position);
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			if (MigrateDragon)
			{
				if (Target != null)
				{
					MigrateDragonTo = Target;
				}
				if (MigrateDragonTo != null && MigrateDragonTo.ValidLivingGamer)
				{
					DoMigration();
					return;
				}
				MigrateDragonTo = null;
				MigrateDragon = false;
			}
			float num = (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			DragonTime += num;
			StateMachine._currentState.Update(this, num);
			UpdateSounds(gameTime.get_ElapsedGameTime());
			float value = MathHelper.WrapAngle(TargetYaw - _yaw);
			if (CurrentAnimation == DragonAnimEnum.HOVER)
			{
				TargetRoll = 0f;
			}
			else
			{
				TargetRoll = MathHelper.Clamp(value, 0f - EType.MaxRoll, EType.MaxRoll);
			}
			float y = base.WorldPosition.Y;
			if (y > TargetAltitude - 2f && y < TargetAltitude + 2f)
			{
				TargetPitch = 0f;
			}
			else
			{
				float num2 = TargetAltitude - y;
				TargetPitch = (float)Math.Sign(num2) * Math.Min(num2 * num2 / 30f, EType.MaxPitch);
			}
			float yaw = _yaw;
			_yaw = MathTools.MoveTowardTargetAngle(_yaw, TargetYaw, EType.YawRate, num);
			_velocity = MathTools.MoveTowardTarget(_velocity, TargetVelocity, EType.MaxAccel, num);
			_roll = MathTools.MoveTowardTarget(_roll, TargetRoll, EType.RollRate, num);
			_pitch = MathTools.MoveTowardTarget(_pitch, TargetPitch, EType.PitchRate, num);
			if (AnimationChangeUpdate >= FlapChangeUpdate && UpdatesSent > AnimationChangeUpdate)
			{
				float num3 = ((_pitch > 0f) ? 2f : 0.5f);
				FlapDebt += _pitch * num * num3;
				FlapDebt += Math.Abs(MathHelper.WrapAngle(yaw - Yaw)) * 1.5f;
				if (Math.Abs(_pitch) < 0.01f)
				{
					FlapDebt += 0.2f * num;
				}
				if (CurrentAnimation != 0)
				{
					FlapDebt -= 0.5f * num;
					if (FlapDebt < -1f && NextAnimation == DragonAnimEnum.FLAP)
					{
						NextAnimation = DragonAnimEnum.SOAR;
						FlapChangeUpdate = UpdatesSent;
					}
				}
				else if (NextAnimation == DragonAnimEnum.SOAR && FlapDebt > 1f)
				{
					NextAnimation = DragonAnimEnum.FLAP;
					FlapChangeUpdate = UpdatesSent;
				}
				FlapDebt = FlapDebt.Clamp(-1f, 1f);
			}
			base.LocalRotation = Quaternion.CreateFromYawPitchRoll(_yaw, _pitch, _roll);
			base.LocalPosition += base.LocalToWorld.Forward * (_velocity * num);
			for (int i = 0; i < base.Children.Count; i++)
			{
				base.Children[i].Update(game, gameTime);
			}
			if (ClipFinished)
			{
				CurrentAnimation = NextAnimation;
				AnimationChangeUpdate = UpdatesSent;
			}
			if (DragonTime > NextUpdateTime)
			{
				if (NextUpdateTime == -1f)
				{
					UpdateDragonMessage.UpdateCount = 0;
				}
				((DragonBaseState)StateMachine._currentState).SendUpdateMessage(this);
				NextUpdateTime = DragonTime + 0.2f;
			}
			Gunshots.Clear();
		}

		public void PlaySingleClip(string name, bool loop)
		{
			CurrentPlayer = _dragonModel.PlayClip(name, loop, TimeSpan.FromSeconds(0.25));
		}
	}
}
