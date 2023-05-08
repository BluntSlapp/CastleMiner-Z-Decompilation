using System;
using System.Text;
using System.Threading;
using DNA.Drawing;
using DNA.Drawing.UI;
using DNA.Text;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class WaitScreen : Screen
	{
		private ProgressCallback _progress;

		private ThreadStart _operation;

		private ThreadStart _complete;

		private string _message;

		public bool _drawProgress;

		public int Progress;

		private OneShotTimer textFlashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5), true);

		private StringBuilder sbuilder = new StringBuilder();

		public string Message
		{
			set
			{
				_message = value;
			}
		}

		public static void DoWait(ScreenGroup group, string message)
		{
			WaitScreen waitScreen = new WaitScreen(message, null);
			waitScreen.Start(group);
		}

		public static void DoWait(ScreenGroup group, string message, ProgressCallback progress)
		{
			if (!progress())
			{
				WaitScreen waitScreen = new WaitScreen(message, progress);
				waitScreen.Start(group);
			}
		}

		public static void DoWait(ScreenGroup group, string message, ThreadStart longOperation, ThreadStart onComplete)
		{
			WaitScreen waitScreen = new WaitScreen(message, longOperation, onComplete);
			waitScreen.Start(group);
		}

		private WaitScreen(string message, ThreadStart operation, ThreadStart onComplete)
			: base(true, false)
		{
			_operation = operation;
			_message = message;
			_complete = onComplete;
		}

		public WaitScreen(string message, bool drawProgress, ThreadStart operation, ThreadStart onComplete)
			: base(true, false)
		{
			_operation = operation;
			_message = message;
			_complete = onComplete;
			_drawProgress = drawProgress;
		}

		private WaitScreen(string message, ProgressCallback progress)
			: base(true, false)
		{
			_progress = progress;
			_message = message;
		}

		public WaitScreen(string message)
			: base(true, false)
		{
			_message = message;
		}

		public void Start(ScreenGroup group)
		{
			group.PushScreen(this);
			if (_operation != null)
			{
				CastleMinerZGame.Instance.TaskScheduler.DoUserWorkItem(DoOperation, null);
			}
		}

		public void DoOperation(object state)
		{
			_operation();
			PopMe();
			if (_complete != null)
			{
				_complete();
			}
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			if (_progress != null && _progress())
			{
				PopMe();
				if (_complete != null)
				{
					_complete();
				}
			}
			base.OnUpdate(game, gameTime);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			SpriteFont largeFont = CastleMinerZGame.Instance._largeFont;
			Vector2 vector = largeFont.MeasureString(_message);
			Vector2 location = new Vector2(640f - vector.X / 2f, 360f + vector.Y / 2f);
			textFlashTimer.Update(gameTime.get_ElapsedGameTime());
			Color textColor = Color.Lerp(Color.Red, Color.White, textFlashTimer.PercentComplete);
			if (textFlashTimer.Expired)
			{
				textFlashTimer.Reset();
			}
			spriteBatch.Begin();
			spriteBatch.DrawOutlinedText(largeFont, _message, location, textColor, Color.Black, 1);
			if (_drawProgress)
			{
				sbuilder.Length = 0;
				sbuilder.Concat(Progress);
				sbuilder.Append("%");
				float x = location.X + largeFont.MeasureString(_message).X + largeFont.MeasureString(" 100%").X - largeFont.MeasureString(sbuilder).X;
				spriteBatch.DrawOutlinedText(largeFont, sbuilder, new Vector2(x, location.Y), textColor, Color.Black, 1);
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
