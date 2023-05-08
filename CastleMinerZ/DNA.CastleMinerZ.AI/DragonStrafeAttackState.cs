using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DragonStrafeAttackState : DragonBaseState
	{
		public override void Enter(DragonEntity entity)
		{
			entity.TimeLeftBeforeNextViewCheck = entity.EType.FastViewCheckInterval;
			entity.TargetAltitude = entity.EType.HuntingAltitude;
			entity.TimeLeftBeforeNextShot = entity.EType.StrafeFireRate;
			entity.TargetVelocity = entity.EType.Speed;
		}

		public override void Update(DragonEntity entity, float dt)
		{
			if (entity.Target != null && !entity.Target.ValidLivingGamer)
			{
				entity.Target = null;
				entity.MigrateDragon = false;
				entity.MigrateDragonTo = null;
			}
			if (entity.Target == null)
			{
				DragonBaseState.SearchForNewTarget(entity, dt);
			}
			Vector3 dest;
			float num = DragonBaseState.SteerTowardTarget(entity, out dest);
			if (num < entity.EType.BreakOffStrafeDistance)
			{
				entity.StateMachine.ChangeState(DragonStates.Loiter);
			}
			else
			{
				if (!(num > entity.EType.MinAttackDistance))
				{
					return;
				}
				Vector3 forward = entity.LocalToWorld.Forward;
				forward.Y = 0f;
				forward.Normalize();
				if (!(num < entity.EType.MaxAttackDistance) || !(Vector3.Dot(dest, forward) / num > 0.95f))
				{
					return;
				}
				entity.ShootTarget = entity.TravelTarget;
				if (entity.ShotPending)
				{
					return;
				}
				entity.TimeLeftBeforeNextShot -= dt;
				if (entity.TimeLeftBeforeNextShot < 0f)
				{
					entity.TimeLeftBeforeNextShot += entity.EType.StrafeFireRate;
					if (entity.ChancesToNotAttack == 0)
					{
						entity.ShotPending = true;
					}
				}
			}
		}
	}
}
