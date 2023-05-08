namespace DNA.CastleMinerZ.AI
{
	public class SkeletonDie : ZombieDie
	{
		public override string GetAnimName(BaseZombie entity)
		{
			return "death" + (entity.Rnd.Next(7) + 1);
		}
	}
}
