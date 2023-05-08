using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA
{
	public class ExceptionGame : Game
	{
		private Exception _lastError;

		private string _link;

		private string _gameName;

		private string _version;

		private readonly Exception exception;

		private SpriteBatch batch;

		private SpriteFont font;

		private DateTime _startTime;

		private TimeSpan _runTime;

		public ExceptionGame(Exception e, string urlLink, string gameName, Version version, DateTime startTime)
		{
			_lastError = e;
			_link = urlLink;
			_gameName = gameName;
			_version = version.ToString();
			_startTime = startTime;
			_runTime = DateTime.UtcNow - _startTime;
			GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1280,
				PreferredBackBufferHeight = 720
			};
			exception = e;
			base.Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			batch = new SpriteBatch(base.GraphicsDevice);
			font = base.Content.Load<SpriteFont>("Debug");
		}

		protected override void Draw(GameTime gameTime)
		{
			base.GraphicsDevice.Clear(Color.Black);
			batch.Begin();
			_lastError = _lastError.GetBaseException();
			base.GraphicsDevice.Clear(Color.Black);
			Rectangle titleSafeArea = base.GraphicsDevice.Viewport.TitleSafeArea;
			int top = titleSafeArea.Top;
			string name = _lastError.GetType().Name;
			batch.DrawString(font, "Sorry, this game has encountered an error", new Vector2(titleSafeArea.X, top), Color.White);
			top += font.LineSpacing;
			batch.DrawString(font, "Take a picture of this screen and email it to: " + _link, new Vector2(titleSafeArea.X, top), Color.White);
			top += font.LineSpacing;
			batch.DrawString(font, _gameName + ", Version " + _version + " ", new Vector2(titleSafeArea.X, top), Color.White);
			top += font.LineSpacing;
			batch.DrawString(font, _startTime.ToString("MM/dd/yy HH:mm") + " " + _runTime.ToString(), new Vector2(titleSafeArea.X, top), Color.White);
			top += font.LineSpacing * 2;
			batch.DrawString(font, _lastError.Message, new Vector2(titleSafeArea.X, top), Color.White);
			top += font.LineSpacing;
			batch.DrawString(font, _lastError.GetType().Name, new Vector2(titleSafeArea.X, top), Color.White);
			top += font.LineSpacing * 2;
			string[] array = _lastError.StackTrace.Split('\n');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim();
				batch.DrawString(font, text2, new Vector2(titleSafeArea.X, top), Color.White);
				top += font.LineSpacing;
			}
			batch.End();
			base.Draw(gameTime);
		}
	}
}
