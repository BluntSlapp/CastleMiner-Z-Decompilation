using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DragonDefaultState : DragonBaseState
	{
		public override void Enter(DragonEntity entity)
		{
			entity.TimeLeftBeforeNextViewCheck = entity.EType.SlowViewCheckInterval;
			entity.TargetVelocity = entity.EType.Speed;
			Vector3 worldPosition = entity.WorldPosition;
			Vector3 vector = DragonBaseState.MakeYawVector(entity.DefaultHeading);
			float num = entity.EType.SpawnDistance;
			if (entity.FirstTimeForDefaultState)
			{
				num *= 2f;
			}
			entity.FirstTimeForDefaultState = false;
			entity.Target = null;
			entity.TravelTarget = worldPosition + vector * num;
			entity.TargetYaw = entity.DefaultHeading;
			entity.TargetAltitude = entity.EType.CruisingAltitude;
			entity.HadTargetThisPass = false;
			entity.LoitersLeft = 3;
			entity.ChancesToNotAttack = entity.EType.ChancesToNotAttack;
		}

		public override void Update(DragonEntity entity, float dt)
		{
			if (entity.Removed)
			{
				return;
			}
			if (DragonBaseState.SearchForNewTarget(entity, dt))
			{
				entity.StateMachine.ChangeState(DragonBaseState.GetNextAttackType(entity));
				entity.StateMachine.Update(dt);
				return;
			}
			Vector3 dest;
			float num = DragonBaseState.SteerTowardTarget(entity, out dest);
			if (num < 10f)
			{
				entity.Removed = true;
				EnemyManager.Instance.RemoveDragon();
			}
		}
	}
}
