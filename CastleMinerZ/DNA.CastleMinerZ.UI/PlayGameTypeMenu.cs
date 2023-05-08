using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	internal class PlayGameTypeMenu : MenuScreen
	{
		private CastleMinerZGame _game;

		private MenuItemElement onlineControl;

		public PlayGameTypeMenu(CastleMinerZGame game)
			: base(game._largeFont, Color.White, Color.Red, false)
		{
			SpriteFont largeFont = game._largeFont;
			_game = game;
			ClickSound = "Click";
			SelectSound = "Click";
			MenuStart = 300f;
			onlineControl = AddMenuItem("Play Online", PlayGameTypeMenuItem.Online);
			AddMenuItem("Play Offline", PlayGameTypeMenuItem.Offline);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			spriteBatch.Begin();
			_game.Logo.Draw(spriteBatch, new Vector2(titleSafeArea.Center.X - _game.Logo.Width / 2, 0f), Color.White);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			onlineControl.TextColor = (Guide.IsTrialMode ? Color.Gray : Color.White);
			base.Update(game, gameTime);
		}
	}
}
