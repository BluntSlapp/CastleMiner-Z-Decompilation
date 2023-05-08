using DNA.Drawing;
using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	internal class GameModeMenu : MenuScreen
	{
		private CastleMinerZGame _game;

		private MenuItemElement SurvivalControl;

		private MenuItemElement DragonEnduranceControl;

		private MenuItemElement CreativeControl;

		public GameModeMenu(CastleMinerZGame game)
			: base(game._largeFont, Color.White, Color.Red, false)
		{
			_game = game;
			SpriteFont largeFont = _game._largeFont;
			SelectSound = "Click";
			ClickSound = "Click";
			AddMenuItem("Endurance", GameModeTypes.Endurance);
			SurvivalControl = AddMenuItem("Survival", GameModeTypes.Survival);
			DragonEnduranceControl = AddMenuItem("Dragon Endurance", GameModeTypes.DragonEndurance);
			CreativeControl = AddMenuItem("Creative", GameModeTypes.Creative);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			string text = "Choose a Game Mode";
			spriteBatch.Begin();
			spriteBatch.DrawOutlinedText(_game._largeFont, text, new Vector2((float)titleSafeArea.Center.X - _game._largeFont.MeasureString(text).X / 2f, titleSafeArea.Y), Color.White, Color.Black, 1);
			string text2 = ((base.SelectedIndex == 0) ? "See How Far You Can Travel" : ((base.SelectedIndex == 1) ? "Build And Mine Resources" : ((base.SelectedIndex != 2) ? "Build With Unlimited Resources" : "Fend Off Wave After Wave Of Dragons")));
			spriteBatch.DrawOutlinedText(_game._largeFont, text2, new Vector2((float)titleSafeArea.Center.X - _game._largeFont.MeasureString(text2).X / 2f, (float)titleSafeArea.Bottom - _game._largeFont.MeasureString(text2).Y), Color.White, Color.Black, 1);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			SignedInGamer currentGamer = Screen.CurrentGamer;
			SurvivalControl.TextColor = ((!Guide.IsTrialMode) ? Color.White : Color.Gray);
			CreativeControl.TextColor = ((!Guide.IsTrialMode && CastleMinerZGame.Instance.FrontEnd.PromoCodes[5].Redeemed) ? Color.White : Color.Gray);
			DragonEnduranceControl.TextColor = ((!Guide.IsTrialMode && (CastleMinerZGame.Instance.PlayerStats.UndeadDragonKills > 0 || _game.PlayerStats.v1Player || CastleMinerZGame.Instance.FrontEnd.PromoCodes[4].Redeemed)) ? Color.White : Color.Gray);
			base.OnUpdate(game, gameTime);
		}
	}
}
