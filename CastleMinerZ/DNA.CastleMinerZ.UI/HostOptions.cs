using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class HostOptions : MenuScreen
	{
		private CastleMinerZGame _game;

		private MenuItemElement pubItem;

		private MenuItemElement banListItem;

		public HostOptions(CastleMinerZGame game)
			: base(game._largeFont, false)
		{
			_game = game;
			ClickSound = "Click";
			SelectSound = "Click";
			AddMenuItem("Return To Game", HostOptionItems.Return);
			AddMenuItem("Kick Player", HostOptionItems.KickPlayer);
			AddMenuItem("Ban Player", HostOptionItems.BanPlayer);
			AddMenuItem("Restart", HostOptionItems.Restart);
			pubItem = AddMenuItem("Visibility:", HostOptionItems.Public);
			banListItem = AddMenuItem("Clear Ban List", HostOptionItems.ClearBanList);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			pubItem.Text = (_game.IsPublicGame ? "Visibility: Public" : "Visibility: Private");
			banListItem.Visible = _game.PlayerStats.BanList.Count > 0;
			base.OnUpdate(game, gameTime);
		}

		protected override void OnMenuItemSelected(UIElement selectedControl)
		{
			base.OnMenuItemSelected(selectedControl);
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
