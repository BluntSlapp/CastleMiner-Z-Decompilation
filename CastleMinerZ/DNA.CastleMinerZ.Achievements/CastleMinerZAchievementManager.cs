namespace DNA.CastleMinerZ.Achievements
{
	public class CastleMinerZAchievementManager : AchievementManager<CastleMinerZPlayerStats>
	{
		private CastleMinerZGame _game;

		public Achievement[] Achievements = new Achievement[25];

		public CastleMinerZAchievementManager(CastleMinerZGame game)
			: base(game.PlayerStats)
		{
			_game = game;
		}

		public override void CreateAcheivements()
		{
			AddAcheivement(Achievements[0] = new PlayTimeAchievement(this, 1, "Short Timer"));
			AddAcheivement(Achievements[1] = new PlayTimeAchievement(this, 10, "Veteren MinerZ"));
			AddAcheivement(Achievements[2] = new PlayTimeAchievement(this, 100, "MinerZ Potato"));
			AddAcheivement(Achievements[3] = new DistaceTraveledAchievement(this, 50, "First Contact"));
			AddAcheivement(Achievements[4] = new DistaceTraveledAchievement(this, 200, "Leaving Home"));
			AddAcheivement(Achievements[5] = new DistaceTraveledAchievement(this, 1000, "Desert Crawler"));
			AddAcheivement(Achievements[6] = new DistaceTraveledAchievement(this, 2300, "Mountain Man"));
			AddAcheivement(Achievements[7] = new DistaceTraveledAchievement(this, 3000, "Deep Freeze"));
			AddAcheivement(Achievements[8] = new DistaceTraveledAchievement(this, 3600, "Hell On Earth"));
			AddAcheivement(Achievements[9] = new DistaceTraveledAchievement(this, 5000, "Around the World"));
			AddAcheivement(Achievements[10] = new DepthTraveledAchievement(this, -20f, "Deep Digger"));
			AddAcheivement(Achievements[11] = new DepthTraveledAchievement(this, -40f, "Welcome To Hell"));
			AddAcheivement(Achievements[12] = new DaysPastAchievement(this, 1, "Survived The Night"));
			AddAcheivement(Achievements[13] = new DaysPastAchievement(this, 7, "A Week Later"));
			AddAcheivement(Achievements[14] = new DaysPastAchievement(this, 28, "28 Days Later"));
			AddAcheivement(Achievements[15] = new DaysPastAchievement(this, 100, "Survivor"));
			AddAcheivement(Achievements[16] = new DaysPastAchievement(this, 196, "28 Weeks Later"));
			AddAcheivement(Achievements[17] = new DaysPastAchievement(this, 365, "Anniversary"));
			AddAcheivement(Achievements[18] = new TotalCraftedAchievement(this, 1, "Tinkerer"));
			AddAcheivement(Achievements[19] = new TotalCraftedAchievement(this, 100, "Crafter"));
			AddAcheivement(Achievements[20] = new TotalCraftedAchievement(this, 1000, "Master Craftsman"));
			AddAcheivement(Achievements[21] = new TotalKillsAchievement(this, 1, "Self Defense"));
			AddAcheivement(Achievements[22] = new TotalKillsAchievement(this, 100, "No Fear"));
			AddAcheivement(Achievements[23] = new TotalKillsAchievement(this, 1000, "Zombie Slayer"));
			AddAcheivement(Achievements[24] = new UndeadKilledAchievement(this, "Dragon Slayer"));
		}

		public override void OnAchieved(Achievement acheivement)
		{
			if (_game.GameScreen != null)
			{
				_game.GameScreen.HUD.DisplayAcheivement(acheivement);
			}
			base.OnAchieved(acheivement);
		}
	}
}
