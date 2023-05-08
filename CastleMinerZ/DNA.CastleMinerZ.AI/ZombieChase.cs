using System;
using DNA.Audio;
using DNA.CastleMinerZ.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.AI
{
	public class ZombieChase : EnemyBaseState
	{
		protected float _runSpeed = 3f;

		private float _runFastSpeed = 4f;

		protected float _walkSpeed = 1f;

		public override bool IsRestartable()
		{
			return true;
		}

		protected virtual void StartMoveAnimation(BaseZombie entity)
		{
			if (entity.CurrentSpeed < 2.7f)
			{
				entity.CurrentPlayer = entity.PlayClip("walk", true, TimeSpan.FromSeconds(0.25));
				entity.CurrentPlayer.Speed = Math.Min(entity.CurrentSpeed / _walkSpeed, 1f);
			}
			else if (entity.CurrentSpeed < 3.7f)
			{
				entity.CurrentPlayer = entity.PlayClip("walk2", true, TimeSpan.FromSeconds(0.25));
				entity.CurrentPlayer.Speed = Math.Min(entity.CurrentSpeed / _walkSpeed, 1f);
			}
			else if (entity.CurrentSpeed < 5f || !entity.EType.HasRunFast)
			{
				entity.CurrentPlayer = entity.PlayClip("run", true, TimeSpan.FromSeconds(0.25));
				entity.CurrentPlayer.Speed = Math.Min(entity.CurrentSpeed / _runSpeed, 1f);
			}
			else
			{
				entity.CurrentPlayer = entity.PlayClip("run_fast", true, TimeSpan.FromSeconds(0.25));
				entity.CurrentPlayer.Speed = Math.Min(entity.CurrentSpeed / _runFastSpeed, 1f);
			}
		}

		public override void Enter(BaseZombie entity)
		{
			entity.IsBlocking = true;
			entity.IsHittable = true;
			StartMoveAnimation(entity);
			entity.ResetFrustration();
		}

		public override void HandleSpeedUp(BaseZombie entity)
		{
			StartMoveAnimation(entity);
		}

		public override void Update(BaseZombie entity, float dt)
		{
			if (entity.ZombieGrowlCue == null || !entity.ZombieGrowlCue.IsPlaying)
			{
				if (entity.EType.FoundIn == EnemyType.FoundInEnum.ABOVEGROUND)
				{
					entity.ZombieGrowlCue = SoundManager.Instance.PlayInstance("ZombieGrowl", entity.SoundEmitter);
				}
				else if (entity.EType.FoundIn == EnemyType.FoundInEnum.CAVES)
				{
					entity.ZombieGrowlCue = SoundManager.Instance.PlayInstance("Skeleton", entity.SoundEmitter);
				}
				else
				{
					entity.ZombieGrowlCue = SoundManager.Instance.PlayInstance("Felguard", entity.SoundEmitter);
				}
			}
			Vector3 worldVelocity = entity.PlayerPhysics.WorldVelocity;
			worldVelocity.X = 0f;
			worldVelocity.Z = 0f;
			Vector3 vector = entity.Target.WorldPosition - entity.WorldPosition;
			float y = vector.Y;
			vector.Y = 0f;
			float num = vector.Length();
			if (num < 5f)
			{
				float num2 = entity.TimeToIntercept();
				if (num2 < 1f / entity.CurrentSpeed)
				{
					if (Math.Abs(y) > 4f && entity.OnGround && entity.Target.InContact)
					{
						entity.StateMachine.ChangeState(entity.EType.GetGiveUpState(entity));
					}
					else
					{
						entity.StateMachine.ChangeState(entity.EType.GetAttackState(entity));
					}
					return;
				}
			}
			if (entity.FrustrationCount <= 0f)
			{
				entity.StateMachine.ChangeState(entity.EType.GetGiveUpState(entity));
				entity.PlayerPhysics.WorldVelocity = worldVelocity;
				return;
			}
			if (entity.EType.HasRunFast && !entity.IsMovingFast && entity.Target.IsLocal)
			{
				entity.TimeLeftTilFast -= dt;
				if (entity.TimeLeftTilFast <= 0f || EnemyManager.Instance.ZombieFestIsOn)
				{
					SpeedUpEnemyMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, entity.EnemyID, entity.Target.Gamer.Id);
				}
				Vector3 worldVelocity2 = entity.Target.PlayerPhysics.WorldVelocity;
				worldVelocity2.Y = 0f;
				if (worldVelocity2.LengthSquared() > 3.5f)
				{
					entity.TimeLeftTilRunFast -= dt;
					if (entity.TimeLeftTilRunFast < 0f)
					{
						SpeedUpEnemyMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, entity.EnemyID, entity.Target.Gamer.Id);
					}
				}
			}
			if (vector.LengthSquared() < 0.001f)
			{
				vector = Vector3.Zero;
			}
			else
			{
				vector.Normalize();
			}
			float num3 = entity.CurrentSpeed;
			if (!entity.OnGround)
			{
				num3 *= (entity.IsMovingFast ? 1f : 0.5f);
			}
			else if (entity.TouchingWall)
			{
				worldVelocity.Y += (entity.IsMovingFast ? entity.EType.FastJumpSpeed : 10f);
			}
			worldVelocity.X = vector.X * num3;
			worldVelocity.Z = vector.Z * num3;
			entity.PlayerPhysics.WorldVelocity = worldVelocity;
			worldVelocity.Y = 0f;
			if (worldVelocity.LengthSquared() > 0.2f)
			{
				float desiredHeading = (float)Math.Atan2(0f - worldVelocity.Z, worldVelocity.X) + (float)Math.PI / 2f;
				entity.LocalRotation = Quaternion.CreateFromYawPitchRoll(MakeHeading(entity, desiredHeading), 0f, 0f);
			}
			if (!(num >= 5f))
			{
				return;
			}
			if (entity.IsMovingFast)
			{
				float num4 = entity.TimeToIntercept();
				if (num4 > 8f)
				{
					entity.StateMachine.ChangeState(entity.EType.GetGiveUpState(entity));
					entity.PlayerPhysics.WorldVelocity = worldVelocity;
				}
			}
			else if (num > 25f)
			{
				entity.StateMachine.ChangeState(entity.EType.GetGiveUpState(entity));
				entity.PlayerPhysics.WorldVelocity = worldVelocity;
			}
		}
	}
}
