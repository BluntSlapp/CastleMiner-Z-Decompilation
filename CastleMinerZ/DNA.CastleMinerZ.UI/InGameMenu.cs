using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class InGameMenu : MenuScreen
	{
		private CastleMinerZGame _game;

		private MenuItemElement purchaseInGameControl;

		private MenuItemElement inviteControl;

		private MenuItemElement hostOptions;

		private MenuItemElement teleport;

		public InGameMenu(CastleMinerZGame game)
			: base(game._largeFont, Color.White, Color.Red, true)
		{
			SpriteFont largeFont = game._largeFont;
			_game = game;
			SelectSound = "Click";
			ClickSound = "Click";
			AddMenuItem("Return To Game", InGameMenuItems.Return);
			purchaseInGameControl = AddMenuItem("Purchase", InGameMenuItems.Purchase);
			AddMenuItem("Inventory", InGameMenuItems.MyBlocks);
			if (_game.GameMode != 0 && _game.Difficulty != GameDifficultyTypes.HARDCORE)
			{
				teleport = AddMenuItem("Teleport", InGameMenuItems.Teleport);
			}
			inviteControl = AddMenuItem("Invite Friends", InGameMenuItems.Invite);
			AddMenuItem("Controls", InGameMenuItems.Controls);
			AddMenuItem("Settings", InGameMenuItems.Settings);
			hostOptions = AddMenuItem("Host Options", InGameMenuItems.HostOptions);
			AddMenuItem("Main Menu", InGameMenuItems.Quit);
		}

		public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			spriteBatch.Begin();
			Rectangle destinationRectangle = new Rectangle(0, 0, device.Viewport.Width, device.Viewport.Height);
			spriteBatch.Draw(_game.DummyTexture, destinationRectangle, new Color(0f, 0f, 0f, 0.5f));
			spriteBatch.End();
			base.Draw(device, spriteBatch, gameTime);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			SignedInGamer currentGamer = Screen.CurrentGamer;
			purchaseInGameControl.Visible = Guide.IsTrialMode;
			hostOptions.Visible = _game.IsOnlineGame && _game.CurrentNetworkSession.IsHost && _game.CurrentWorld.OwnerGamerTag == Screen.CurrentGamer.Gamertag;
			inviteControl.Visible = _game.IsOnlineGame && currentGamer.Privileges.AllowCommunication != 0 && !currentGamer.IsGuest;
			base.OnUpdate(game, gameTime);
		}

		public override void OnPushed()
		{
			if (!CastleMinerZGame.Instance.IsOnlineGame)
			{
				CastleMinerZGame.Instance.GameScreen.mainScene.DoUpdate = false;
				CastleMinerZGame.Instance.GameScreen.DoUpdate = false;
			}
			base.OnPushed();
		}

		public override void OnPoped()
		{
			if (!CastleMinerZGame.Instance.IsOnlineGame)
			{
				CastleMinerZGame.Instance.GameScreen.mainScene.DoUpdate = true;
				CastleMinerZGame.Instance.GameScreen.DoUpdate = true;
			}
			base.OnPoped();
		}
	}
}
