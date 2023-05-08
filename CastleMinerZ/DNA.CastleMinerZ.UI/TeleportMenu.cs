using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class TeleportMenu : MenuScreen
	{
		private MenuItemElement toPLayer;

		private CastleMinerZGame _game;

		public TeleportMenu(CastleMinerZGame game)
			: base(game._largeFont, false)
		{
			_game = game;
			ClickSound = "Click";
			SelectSound = "Click";
			AddMenuItem("Return To Game", TeleportMenuItems.Quit);
			AddMenuItem("Teleport To Surface", TeleportMenuItems.Surface);
			AddMenuItem("Teleport To Start", TeleportMenuItems.Origin);
			toPLayer = AddMenuItem("Teleport To Player", TeleportMenuItems.Player);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			toPLayer.Visible = _game.IsOnlineGame;
			base.OnUpdate(game, gameTime);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Begin();
			Rectangle destinationRectangle = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);
			spriteBatch.Draw(_game.DummyTexture, destinationRectangle, new Color(0f, 0f, 0f, 0.5f));
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
