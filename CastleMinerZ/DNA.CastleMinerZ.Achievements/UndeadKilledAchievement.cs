namespace DNA.CastleMinerZ.Achievements
{
	public class UndeadKilledAchievement : AchievementManager<CastleMinerZPlayerStats>.Achievement
	{
		protected override bool IsSastified
		{
			get
			{
				return base.PlayerStats.UndeadDragonKills > 0;
			}
		}

		public override float ProgressTowardsUnlock
		{
			get
			{
				if (base.PlayerStats.UndeadDragonKills > 0)
				{
					return 1f;
				}
				return 0f;
			}
		}

		public override string ProgressTowardsUnlockMessage
		{
			get
			{
				if (base.PlayerStats.UndeadDragonKills > 0)
				{
					return "Undead Dragon Killed";
				}
				return "Undead Dragon Not Killed";
			}
		}

		public UndeadKilledAchievement(CastleMinerZAchievementManager manager, string name)
			: base((AchievementManager<CastleMinerZPlayerStats>)manager, name, "Kill The Undead Dragon")
		{
		}
	}
}
