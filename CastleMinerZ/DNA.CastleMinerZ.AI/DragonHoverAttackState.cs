using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DragonHoverAttackState : DragonBaseState
	{
		public override void Enter(DragonEntity entity)
		{
			entity.TimeLeftBeforeNextViewCheck = entity.EType.FastViewCheckInterval;
			entity.TargetAltitude = entity.EType.HuntingAltitude;
			entity.TargetVelocity = entity.EType.Speed;
		}

		public override void Update(DragonEntity entity, float dt)
		{
			if (entity.Target != null && !entity.Target.ValidLivingGamer)
			{
				entity.Target = null;
				entity.MigrateDragonTo = null;
				entity.MigrateDragon = false;
			}
			if (entity.Target == null)
			{
				DragonBaseState.SearchForNewTarget(entity, dt);
			}
			Vector3 dest;
			float num = DragonBaseState.SteerTowardTarget(entity, out dest);
			if (num < entity.EType.HoverDistance)
			{
				entity.StateMachine.ChangeState(DragonStates.Hover);
			}
		}
	}
}
