using System;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class ArcherSkeletonTryAttack : EnemyBaseState
	{
		public override void Enter(BaseZombie entity)
		{
			ZeroVelocity(entity);
			entity.HitCount = 0;
			entity.CurrentPlayer = entity.PlayClip("atack_archer1", false, TimeSpan.FromSeconds(0.25));
			entity.SwingCount--;
		}

		public override void Update(BaseZombie entity, float dt)
		{
			if (entity.IsNearAnimationEnd)
			{
				if (entity.SwingCount <= 0)
				{
					entity.StateMachine.ChangeState(entity.EType.GetGiveUpState(entity));
				}
				else
				{
					entity.StateMachine.ChangeState(EnemyStates.ArcherIdle);
				}
				return;
			}
			Vector3 vector = entity.Target.WorldPosition - entity.WorldPosition;
			vector.Y = 0f;
			if (vector.LengthSquared() > 0.2f)
			{
				float desiredHeading = (float)Math.Atan2(0f - vector.Z, vector.X) + (float)Math.PI / 2f;
				entity.LocalRotation = Quaternion.CreateFromYawPitchRoll(MakeHeading(entity, desiredHeading), 0f, 0f);
			}
			if (!(entity.CurrentPlayer.CurrentTime.TotalSeconds > 1.1000000238418579) || entity.HitCount != 0)
			{
				return;
			}
			entity.HitCount = 1;
			Vector3 worldPosition = entity.WorldPosition;
			Vector3 worldPosition2 = entity.Target.WorldPosition;
			worldPosition.Y += 1.5f;
			worldPosition2.Y += 0.5f;
			Vector3 res;
			if (!MathTools.CalculateInitialBallisticVector(worldPosition, worldPosition2, 25f, out res, -10f))
			{
				res = entity.Target.WorldPosition - entity.WorldPosition;
				if (res.LengthSquared() < 0.001f)
				{
					res = Vector3.Up;
				}
				else
				{
					res.Normalize();
				}
				res *= 25f;
			}
			Vector3 worldPosition3 = entity.WorldPosition;
			worldPosition3.Y += 1.5f;
			TracerManager.Instance.AddArrow(worldPosition3, res, entity.Target);
		}
	}
}
