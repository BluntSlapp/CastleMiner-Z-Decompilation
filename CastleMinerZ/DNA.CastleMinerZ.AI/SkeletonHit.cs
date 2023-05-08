namespace DNA.CastleMinerZ.AI
{
	public class SkeletonHit : ZombieHit
	{
		public override string GetAnimName(BaseZombie entity)
		{
			return "gethit" + (entity.Rnd.Next(3) + 1);
		}
	}
}
