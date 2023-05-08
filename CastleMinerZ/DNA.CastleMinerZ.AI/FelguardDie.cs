namespace DNA.CastleMinerZ.AI
{
	public class FelguardDie : ZombieDie
	{
		public override string GetAnimName(BaseZombie entity)
		{
			if (entity.Rnd.Next(2) != 0)
			{
				return "death2";
			}
			return "death1";
		}
	}
}
