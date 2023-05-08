using DNA.Drawing;
using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	internal class DifficultyLevelScreen : MenuScreen
	{
		private CastleMinerZGame _game;

		private MenuItemElement HardcoreControl;

		public DifficultyLevelScreen(CastleMinerZGame game)
			: base(game._largeFont, Color.White, Color.Red, false)
		{
			_game = game;
			SpriteFont largeFont = _game._largeFont;
			SelectSound = "Click";
			ClickSound = "Click";
			AddMenuItem("No Enemies", GameDifficultyTypes.NOENEMIES);
			AddMenuItem("Easy", GameDifficultyTypes.EASY);
			AddMenuItem("Normal", GameDifficultyTypes.HARD);
			HardcoreControl = AddMenuItem("Hardcore", GameDifficultyTypes.HARDCORE);
			base.SelectedIndex = 2;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			string text = "Choose a Difficulty Level";
			spriteBatch.Begin();
			spriteBatch.DrawOutlinedText(_game._largeFont, text, new Vector2((float)titleSafeArea.Center.X - _game._largeFont.MeasureString(text).X / 2f, titleSafeArea.Y), Color.White, Color.Black, 1);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			HardcoreControl.Visible = !_game.InfiniteResourceMode;
			base.OnUpdate(game, gameTime);
		}
	}
}
