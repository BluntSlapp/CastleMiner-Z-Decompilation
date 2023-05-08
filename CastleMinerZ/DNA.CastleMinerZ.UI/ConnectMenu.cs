using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class ConnectMenu : MenuScreen
	{
		private CastleMinerZGame _game;

		public ConnectMenu(CastleMinerZGame game)
			: base(game._largeFont, Color.White, Color.Red, false)
		{
			_game = game;
			SpriteFont largeFont = _game._largeFont;
			SelectSound = "Click";
			ClickSound = "Click";
			MenuStart = 330f;
			AddMenuItem("Host Game", ConnectMenuItems.Host);
			AddMenuItem("Join Game", ConnectMenuItems.Connect);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			float num = (float)device.Viewport.Height / 1080f;
			spriteBatch.Begin();
			_game.Logo.Draw(spriteBatch, new Vector2(titleSafeArea.Center.X - _game.Logo.Width / 2, 0f), Color.White);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
