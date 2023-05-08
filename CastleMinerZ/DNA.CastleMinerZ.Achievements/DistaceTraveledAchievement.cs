using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Achievements
{
	public class DistaceTraveledAchievement : AchievementManager<CastleMinerZPlayerStats>.Achievement
	{
		private int _distance;

		private string lastString;

		private int _lastAmount = -1;

		protected override bool IsSastified
		{
			get
			{
				return base.PlayerStats.MaxDistanceTraveled >= (float)_distance;
			}
		}

		public override float ProgressTowardsUnlock
		{
			get
			{
				return MathHelper.Clamp(base.PlayerStats.MaxDistanceTraveled / (float)_distance, 0f, 1f);
			}
		}

		public override string ProgressTowardsUnlockMessage
		{
			get
			{
				int num = (int)base.PlayerStats.MaxDistanceTraveled;
				if (_lastAmount != num)
				{
					_lastAmount = num;
					lastString = "(" + num + "/" + _distance + ") Distance Traveled";
				}
				return lastString;
			}
		}

		public DistaceTraveledAchievement(CastleMinerZAchievementManager manager, int distance, string name)
			: base((AchievementManager<CastleMinerZPlayerStats>)manager, name, "Travel At Least " + distance)
		{
			_distance = distance;
		}
	}
}
