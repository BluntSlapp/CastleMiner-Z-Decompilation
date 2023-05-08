using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Achievements
{
	public class DaysPastAchievement : AchievementManager<CastleMinerZPlayerStats>.Achievement
	{
		private int _days;

		private string lastString;

		private int _lastAmount = -1;

		protected override bool IsSastified
		{
			get
			{
				return base.PlayerStats.MaxDaysSurvived >= _days;
			}
		}

		public override float ProgressTowardsUnlock
		{
			get
			{
				return MathHelper.Clamp((float)base.PlayerStats.MaxDaysSurvived / (float)_days, 0f, 1f);
			}
		}

		public override string ProgressTowardsUnlockMessage
		{
			get
			{
				int maxDaysSurvived = base.PlayerStats.MaxDaysSurvived;
				if (_lastAmount != maxDaysSurvived)
				{
					_lastAmount = maxDaysSurvived;
					lastString = "(" + maxDaysSurvived + "/" + _days + ") Days Survived";
				}
				return lastString;
			}
		}

		public DaysPastAchievement(CastleMinerZAchievementManager manager, int days, string name)
			: base((AchievementManager<CastleMinerZPlayerStats>)manager, name, "Survive for " + days + " days")
		{
			_days = days;
		}
	}
}
