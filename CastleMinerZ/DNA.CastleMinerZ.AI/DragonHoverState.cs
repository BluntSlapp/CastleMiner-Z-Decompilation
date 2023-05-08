using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DragonHoverState : DragonBaseState
	{
		public override void Enter(DragonEntity entity)
		{
			entity.ShotsLeft = MathTools.RandomInt(entity.EType.MinHoverShots, entity.EType.MaxHoverShots);
			entity.TimeLeftBeforeNextShot = entity.EType.HoverFireRate + entity.EType.HoverFireRate * MathTools.RandomFloat();
			entity.TargetVelocity = 0.25f;
			entity.NextAnimation = DragonAnimEnum.HOVER;
		}

		public override void Exit(DragonEntity entity)
		{
			entity.TargetVelocity = entity.EType.Speed;
			entity.NextAnimation = DragonAnimEnum.FLAP;
		}

		public override void Update(DragonEntity entity, float dt)
		{
			if (entity.Target != null && !entity.Target.ValidLivingGamer)
			{
				entity.Target = null;
			}
			if (DragonBaseState.DoViewCheck(entity, dt, entity.EType.FastViewCheckInterval) && entity.Target != null)
			{
				Vector3 worldPosition = entity.Target.WorldPosition;
				worldPosition.Y += 1.5f;
				if (DragonBaseState.CanSeePosition(entity, worldPosition))
				{
					entity.TravelTarget = entity.Target.WorldPosition;
				}
			}
			Vector3 worldPosition2 = entity.WorldPosition;
			entity.TargetAltitude = entity.EType.HuntingAltitude;
			Vector3 forward = entity.TravelTarget - worldPosition2;
			entity.TargetYaw = MathHelper.WrapAngle(DragonBaseState.GetHeading(forward, entity.TargetYaw));
			entity.ShootTarget = entity.TravelTarget;
			if (entity.ShotPending)
			{
				return;
			}
			entity.TimeLeftBeforeNextShot -= dt;
			if (entity.TimeLeftBeforeNextShot < 0f)
			{
				if (entity.ShotsLeft == 0)
				{
					entity.StateMachine.ChangeState(DragonStates.Loiter);
					entity.StateMachine.Update(dt);
				}
				else
				{
					entity.TimeLeftBeforeNextShot += entity.EType.HoverFireRate;
					entity.ShotPending = true;
					entity.ShotsLeft--;
				}
			}
		}
	}
}
