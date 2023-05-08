using System;

namespace DNA.CastleMinerZ.AI
{
	public class FelguardChase : ZombieChase
	{
		protected override void StartMoveAnimation(BaseZombie entity)
		{
			if (entity.CurrentSpeed < 2.7f)
			{
				entity.CurrentPlayer = entity.PlayClip("walk", true, TimeSpan.FromSeconds(0.25));
				entity.CurrentPlayer.Speed = Math.Min(entity.CurrentSpeed / _walkSpeed, 1f);
			}
			else if (entity.CurrentSpeed < 5f || !entity.EType.HasRunFast)
			{
				entity.CurrentPlayer = entity.PlayClip("run", true, TimeSpan.FromSeconds(0.25));
				entity.CurrentPlayer.Speed = Math.Min(entity.CurrentSpeed / _runSpeed, 1f);
			}
		}
	}
}
