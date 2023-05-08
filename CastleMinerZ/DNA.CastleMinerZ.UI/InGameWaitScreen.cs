using System;
using DNA.Drawing;
using DNA.Drawing.UI;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class InGameWaitScreen : Screen
	{
		private SpriteFont _largeFont;

		private string _text;

		private ProgressCallback _callback;

		private CastleMinerZGame _game;

		private bool _spawnOnTop;

		private OneShotTimer textFlashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5));

		public static void ShowScreen(CastleMinerZGame game, ScreenGroup group, string text, bool spawnontop, ProgressCallback callback)
		{
			if (!callback())
			{
				group.PushScreen(new InGameWaitScreen(game, text, callback, spawnontop));
			}
			else
			{
				game.MakeAboveGround(spawnontop);
			}
		}

		public InGameWaitScreen(CastleMinerZGame game, string text, ProgressCallback callback, bool spawnontop)
			: base(true, false)
		{
			_largeFont = game._largeFont;
			_text = text;
			_callback = callback;
			_game = game;
			_spawnOnTop = spawnontop;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Viewport viewport = device.Viewport;
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			float num = (float)viewport.Height / 1080f;
			float loadProgress = _game.LoadProgress;
			string text = _text;
			float num2 = (float)titleSafeArea.Width * 0.8f;
			float num3 = (float)titleSafeArea.Left + ((float)titleSafeArea.Width - num2) / 2f;
			Sprite sprite = _game._uiSprites["Bar"];
			Vector2 vector = _largeFont.MeasureString(text);
			Vector2 location = new Vector2(num3, (float)(titleSafeArea.Height / 2) + vector.Y / 2f);
			float num4 = location.Y + (float)_largeFont.LineSpacing + 10f * num;
			Rectangle rectangle = new Rectangle((int)num3, (int)num4, (int)num2, _largeFont.LineSpacing);
			int left = rectangle.Left;
			int top = rectangle.Top;
			float num6 = (float)rectangle.Width / (float)sprite.Width;
			spriteBatch.Begin();
			spriteBatch.Draw(destinationRectangle: new Rectangle(0, 0, 1280, 720), texture: _game.MenuBackdrop, color: Color.White);
			_game.Logo.Draw(spriteBatch, new Vector2(titleSafeArea.Center.X - _game.Logo.Width / 2, 0f), Color.White);
			spriteBatch.DrawOutlinedText(_largeFont, text, location, Color.White, Color.Black, 1);
			spriteBatch.Draw(_game.DummyTexture, new Rectangle(left - 2, top - 2, rectangle.Width + 4, rectangle.Height + 4), Color.White);
			spriteBatch.Draw(_game.DummyTexture, new Rectangle(left, top, rectangle.Width, rectangle.Height), Color.Black);
			int num5 = (int)((float)sprite.Width * loadProgress);
			sprite.Draw(spriteBatch, new Rectangle(left, top, (int)((float)rectangle.Width * loadProgress), rectangle.Height), new Rectangle(sprite.Width - num5, 0, num5, sprite.Height), Color.White);
			textFlashTimer.Update(gameTime.get_ElapsedGameTime());
			Color.Lerp(Color.Red, Color.White, textFlashTimer.PercentComplete);
			if (textFlashTimer.Expired)
			{
				textFlashTimer.Reset();
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			if (_callback())
			{
				_game.MakeAboveGround(_spawnOnTop);
				PopMe();
			}
			base.OnUpdate(game, gameTime);
		}
	}
}
