using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Achievements
{
	public class PlayTimeAchievement : AchievementManager<CastleMinerZPlayerStats>.Achievement
	{
		private int _hours;

		private string lastString;

		private int _lastAmount = -1;

		protected override bool IsSastified
		{
			get
			{
				return base.PlayerStats.TimeOnline.TotalHours >= (double)_hours;
			}
		}

		public override float ProgressTowardsUnlock
		{
			get
			{
				return MathHelper.Clamp((float)base.PlayerStats.TimeOnline.TotalHours / (float)_hours, 0f, 1f);
			}
		}

		public override string ProgressTowardsUnlockMessage
		{
			get
			{
				int num = (int)base.PlayerStats.TimeOnline.TotalHours;
				if (_lastAmount != num)
				{
					_lastAmount = num;
					lastString = "(" + num + "/" + _hours + ") hours played";
				}
				return lastString;
			}
		}

		public PlayTimeAchievement(CastleMinerZAchievementManager manager, int hours, string name)
			: base((AchievementManager<CastleMinerZPlayerStats>)manager, name, "Play Online For  " + hours + " Hours.")
		{
			_hours = hours;
		}
	}
}
