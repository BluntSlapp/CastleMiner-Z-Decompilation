using DNA.Drawing;
using DNA.Drawing.UI;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	internal class ControllerScreen : Screen
	{
		private CastleMinerZGame _game;

		private SpriteFont _UIFont;

		private static Texture2D _controlsScreen;

		private string _invertText;

		private bool inGame;

		static ControllerScreen()
		{
			_controlsScreen = CastleMinerZGame.Instance.Content.Load<Texture2D>("Controls");
		}

		public ControllerScreen(CastleMinerZGame game, bool InGame)
			: base(true, false)
		{
			_game = game;
			_UIFont = _game._largeFont;
			_invertText = (_game.PlayerStats.InvertYAxis ? " Invert Y Axis(Inverted)" : " Invert Y Axis(Regular)");
			inGame = InGame;
		}

		public override void OnPushed()
		{
			_invertText = (_game.PlayerStats.InvertYAxis ? " Invert Y Axis(Inverted)" : " Invert Y Axis(Regular)");
			base.OnPushed();
		}

		protected override void OnPlayerInput(GameController controller, GameTime gameTime)
		{
			if (controller.PressedButtons.Y)
			{
				_game.PlayerStats.InvertYAxis = !_game.PlayerStats.InvertYAxis;
				_invertText = (_game.PlayerStats.InvertYAxis ? " Invert Y Axis(Inverted)" : " Invert Y Axis(Regular)");
			}
			else if (controller.ButtonPressed)
			{
				PopMe();
			}
			base.OnPlayerInput(controller, gameTime);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Viewport viewport = device.Viewport;
			Rectangle titleSafeArea = viewport.TitleSafeArea;
			spriteBatch.Begin();
			if (inGame)
			{
				spriteBatch.Draw(destinationRectangle: new Rectangle(0, 0, viewport.Width, viewport.Height), texture: _game.DummyTexture, color: new Color(0f, 0f, 0f, 0.5f));
			}
			int width = _controlsScreen.Width;
			spriteBatch.Draw(_controlsScreen, new Rectangle((viewport.Width - width) / 2, titleSafeArea.Top, width, titleSafeArea.Height), Color.White);
			Vector2 vector = _UIFont.MeasureString(_invertText);
			float num = vector.Y / (float)ControllerImages.Y.Height;
			int num2 = (int)((float)ControllerImages.Y.Width * num);
			int num3 = (int)((float)(titleSafeArea.Y + titleSafeArea.Height) - vector.Y);
			spriteBatch.Draw(ControllerImages.Y, new Rectangle(titleSafeArea.X, num3, num2, (int)vector.Y), Color.White);
			spriteBatch.DrawOutlinedText(_UIFont, _invertText, new Vector2(titleSafeArea.X + num2, num3), Color.White, Color.Black, 1);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
