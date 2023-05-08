using System;

namespace DNA.CastleMinerZ.AI
{
	public class FelguardGiveUp : ZombieGiveUp
	{
		public override void Enter(BaseZombie entity)
		{
			ZeroVelocity(entity);
			entity.CurrentPlayer = entity.PlayClip("Idle", false, TimeSpan.FromSeconds(0.25));
		}
	}
}
