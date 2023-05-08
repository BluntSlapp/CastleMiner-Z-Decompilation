using System.Text;
using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class MainMenu : MenuScreen
	{
		private CastleMinerZGame _game;

		private MenuItemElement purchaseControl;

		private MenuItemElement achievementControl;

		private MenuItemElement reedemControl;

		private StringBuilder builder = new StringBuilder();

		public MainMenu(CastleMinerZGame game)
			: base(game._largeFont, Color.White, Color.Red, false)
		{
			SpriteFont largeFont = game._largeFont;
			_game = game;
			ClickSound = "Click";
			SelectSound = "Click";
			MenuStart = 300f;
			AddMenuItem("Play Game", MainMenuItems.Play);
			purchaseControl = AddMenuItem("Purchase", MainMenuItems.Purchase);
			achievementControl = AddMenuItem("Awards", MainMenuItems.Awards);
			reedemControl = AddMenuItem("Redeem Code", MainMenuItems.Redeem);
			AddMenuItem("Options", MainMenuItems.Options);
			AddMenuItem("Exit", MainMenuItems.Quit);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			spriteBatch.Begin();
			_game.Logo.Draw(spriteBatch, new Vector2(titleSafeArea.Center.X - _game.Logo.Width / 2, 0f), Color.White);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			if (Screen.CurrentGamer.Privileges.AllowOnlineSessions)
			{
				bool isTrialMode = Guide.IsTrialMode;
			}
			SignedInGamer currentGamer = Screen.CurrentGamer;
			purchaseControl.Visible = Guide.IsTrialMode;
			achievementControl.Visible = !Guide.IsTrialMode;
			reedemControl.TextColor = (Guide.IsTrialMode ? Color.Gray : Color.White);
			base.OnUpdate(game, gameTime);
		}
	}
}
