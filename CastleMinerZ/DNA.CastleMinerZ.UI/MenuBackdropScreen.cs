using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class MenuBackdropScreen : Screen
	{
		private CastleMinerZGame _game;

		public MenuBackdropScreen(CastleMinerZGame game)
			: base(false, false)
		{
			_game = game;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Rectangle destinationRectangle = new Rectangle(0, 0, 1280, 720);
			spriteBatch.Begin();
			spriteBatch.Draw(_game.MenuBackdrop, destinationRectangle, Color.White);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
