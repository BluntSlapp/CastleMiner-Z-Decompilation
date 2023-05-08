namespace DNA.CastleMinerZ.AI
{
	public class ArcherSkeletonEmerge : ZombieEmerge
	{
		public override string GetClipName(BaseZombie entity)
		{
			if (entity.Rnd.Next(2) != 0)
			{
				return "standup2";
			}
			return "standup";
		}

		public override void Enter(BaseZombie entity)
		{
			entity.SwingCount = 5 + entity.Rnd.Next(5);
			base.Enter(entity);
		}
	}
}
