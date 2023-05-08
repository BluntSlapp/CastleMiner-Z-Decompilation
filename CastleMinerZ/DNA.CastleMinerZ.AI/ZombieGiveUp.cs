using System;

namespace DNA.CastleMinerZ.AI
{
	public class ZombieGiveUp : EnemyBaseState
	{
		public override void Enter(BaseZombie entity)
		{
			ZeroVelocity(entity);
			entity.CurrentPlayer = entity.PlayClip("eat_start", false, TimeSpan.FromSeconds(0.25));
		}

		public override void Update(BaseZombie entity, float dt)
		{
			if (entity.CurrentPlayer.Finished)
			{
				entity.Remove();
			}
		}
	}
}
