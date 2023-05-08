using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Achievements
{
	public class TotalKillsAchievement : AchievementManager<CastleMinerZPlayerStats>.Achievement
	{
		private int _kills;

		private string lastString;

		private int _lastAmount = -1;

		protected override bool IsSastified
		{
			get
			{
				return base.PlayerStats.TotalKills >= _kills;
			}
		}

		public override float ProgressTowardsUnlock
		{
			get
			{
				return MathHelper.Clamp((float)base.PlayerStats.TotalKills / (float)_kills, 0f, 1f);
			}
		}

		public override string ProgressTowardsUnlockMessage
		{
			get
			{
				int totalKills = base.PlayerStats.TotalKills;
				if (_lastAmount != totalKills)
				{
					_lastAmount = totalKills;
					lastString = "(" + totalKills + "/" + _kills + ") Enemies Killed";
				}
				return lastString;
			}
		}

		public TotalKillsAchievement(CastleMinerZAchievementManager manager, int kills, string name)
			: base((AchievementManager<CastleMinerZPlayerStats>)manager, name, "Kill " + kills + " Enemies")
		{
			_kills = kills;
		}
	}
}
