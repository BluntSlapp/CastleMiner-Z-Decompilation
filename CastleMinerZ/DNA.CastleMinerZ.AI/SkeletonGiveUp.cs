using System;

namespace DNA.CastleMinerZ.AI
{
	public class SkeletonGiveUp : ZombieGiveUp
	{
		public override void Enter(BaseZombie entity)
		{
			ZeroVelocity(entity);
			entity.CurrentPlayer = entity.PlayClip("enraged", false, TimeSpan.FromSeconds(0.25));
		}
	}
}
