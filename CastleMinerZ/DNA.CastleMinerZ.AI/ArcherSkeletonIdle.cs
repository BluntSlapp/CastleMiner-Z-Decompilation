using System;

namespace DNA.CastleMinerZ.AI
{
	public class ArcherSkeletonIdle : EnemyBaseState
	{
		public override void Enter(BaseZombie entity)
		{
			ZeroVelocity(entity);
			entity.CurrentPlayer = entity.PlayClip("idle_archer1", true, TimeSpan.FromSeconds(0.25));
			entity.StateTimer = 1f + (float)entity.Rnd.NextDouble() * 2f;
		}

		public override void Update(BaseZombie entity, float dt)
		{
			entity.StateTimer -= dt;
			if (entity.StateTimer <= 0f)
			{
				entity.CurrentPlayer.Stop();
				entity.StateMachine.ChangeState(entity.EType.GetChaseState(entity));
			}
		}
	}
}
