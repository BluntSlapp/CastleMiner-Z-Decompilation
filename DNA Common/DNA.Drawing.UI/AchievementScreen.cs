using System;
using System.Text;
using DNA.Audio;
using DNA.Input;
using DNA.Text;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class AchievementScreen<T> : Screen where T : PlayerStats
	{
		public const float deadZone = 0.25f;

		private AchievementManager<T> _achievementManager;

		private int MaxAchievementsToDisplay = 6;

		private int TopDisplayIndex;

		private SpriteFont _largeFont;

		private SpriteFont _smallFont;

		private Color _mainTextColor = new Color(225, 229, 220);

		private Color _otherTextColor = new Color(115, 131, 136);

		private Color _backColor = new Color(26, 27, 26);

		private Color _progressBackColor = new Color(38, 38, 38);

		private Color _progressColor = new Color(68, 68, 67);

		private Color _progressOutlineColor = new Color(60, 57, 52);

		private Texture2D _dummyTexture;

		private StringBuilder sbuilder = new StringBuilder();

		private OneShotTimer holdTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5));

		private OneShotTimer scrollTimer = new OneShotTimer(TimeSpan.FromSeconds(0.10000000149011612));

		private bool lastselectup;

		private bool lastselectdown;

		public string ClickSound;

		public AchievementScreen(AchievementManager<T> achievementManager, SpriteFont largeFont, SpriteFont smallFont, Texture2D dummyTexture)
			: base(true, false)
		{
			_achievementManager = achievementManager;
			_largeFont = largeFont;
			_smallFont = smallFont;
			_dummyTexture = dummyTexture;
		}

		public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Vector2 location = new Vector2(titleSafeArea.X + 110, titleSafeArea.Y + 15);
			spriteBatch.Begin();
			spriteBatch.Draw(_dummyTexture, new Rectangle(titleSafeArea.X + 100, titleSafeArea.Top, titleSafeArea.Width - 200, titleSafeArea.Height), Color.Black);
			spriteBatch.Draw(_dummyTexture, new Rectangle(titleSafeArea.X + 105, titleSafeArea.Top + 5, titleSafeArea.Width - 210, titleSafeArea.Height - 10), _backColor);
			spriteBatch.DrawOutlinedText(_largeFont, "Awards", location, _mainTextColor, _progressOutlineColor, 2);
			int num = 0;
			for (int i = 0; i < _achievementManager.Count; i++)
			{
				if (_achievementManager[i].Acheived)
				{
					num++;
				}
			}
			sbuilder.Length = 0;
			sbuilder.Concat(num);
			sbuilder.Append("/");
			sbuilder.Concat(_achievementManager.Count);
			spriteBatch.DrawOutlinedText(_largeFont, sbuilder, new Vector2((float)(titleSafeArea.X + titleSafeArea.Width - 110) - _largeFont.MeasureString(sbuilder).X, location.Y), _mainTextColor, _progressOutlineColor, 2);
			location.X += 65f;
			location.Y += 75f;
			float num2 = _smallFont.MeasureString("OK").Y - 5f;
			for (int j = 0; j < MaxAchievementsToDisplay; j++)
			{
				spriteBatch.Draw(_dummyTexture, new Rectangle((int)location.X, (int)location.Y, 700, 70), _progressOutlineColor);
				spriteBatch.Draw(_dummyTexture, new Rectangle((int)location.X + 2, (int)location.Y + 2, 696, 66), _progressBackColor);
				spriteBatch.Draw(_dummyTexture, new Rectangle((int)location.X + 2, (int)location.Y + 2, (int)(696f * _achievementManager[j + TopDisplayIndex].ProgressTowardsUnlock), 66), _progressColor);
				spriteBatch.DrawOutlinedText(_smallFont, _achievementManager[j + TopDisplayIndex].Name, new Vector2(location.X + 10f, location.Y + 35f - num2), _mainTextColor, Color.Black, 1);
				spriteBatch.DrawOutlinedText(_smallFont, _achievementManager[j + TopDisplayIndex].HowToUnlock, new Vector2(location.X + 10f, location.Y + 35f), _otherTextColor, Color.Black, 1);
				spriteBatch.DrawOutlinedText(_smallFont, _achievementManager[j + TopDisplayIndex].ProgressTowardsUnlockMessage, new Vector2(location.X + 690f - _smallFont.MeasureString(_achievementManager[j + TopDisplayIndex].ProgressTowardsUnlockMessage).X, location.Y + 35f - num2 / 2f), _mainTextColor, Color.Black, 1);
				location.Y += 80f;
			}
			spriteBatch.End();
			base.Draw(device, spriteBatch, gameTime);
		}

		private void PlayClickSound()
		{
			if (ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(ClickSound);
			}
		}

		protected override void OnPlayerInput(GameController controller, GameTime gameTime)
		{
			bool flag = false;
			bool flag2 = false;
			if (controller.CurrentState.ThumbSticks.Left.Y < -0.25f || controller.CurrentState.DPad.Down == ButtonState.Pressed || controller.CurrentState.Triggers.Right > 0.25f)
			{
				flag2 = true;
			}
			if (controller.CurrentState.ThumbSticks.Left.Y > 0.25f || controller.CurrentState.DPad.Up == ButtonState.Pressed || controller.CurrentState.Triggers.Left > 0.25f)
			{
				flag = true;
			}
			if ((controller.CurrentState.ThumbSticks.Left.Y < -0.25f && controller.LastState.ThumbSticks.Left.Y > -0.25f) || controller.PressedDPad.Down || (controller.CurrentState.Triggers.Right > 0.25f && controller.LastState.Triggers.Right < 0.25f))
			{
				if (TopDisplayIndex < _achievementManager.Count - MaxAchievementsToDisplay)
				{
					PlayClickSound();
				}
				TopDisplayIndex++;
				flag2 = false;
			}
			if ((controller.CurrentState.ThumbSticks.Left.Y > 0.25f && controller.LastState.ThumbSticks.Left.Y < 0.25f) || controller.PressedDPad.Up || (controller.CurrentState.Triggers.Left > 0.25f && controller.LastState.Triggers.Left < 0.25f))
			{
				if (TopDisplayIndex > 0)
				{
					PlayClickSound();
				}
				TopDisplayIndex--;
				flag = false;
			}
			if (flag2)
			{
				holdTimer.Update(gameTime.get_ElapsedGameTime());
				if (holdTimer.Expired)
				{
					scrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (scrollTimer.Expired)
					{
						if (TopDisplayIndex < _achievementManager.Count - MaxAchievementsToDisplay)
						{
							PlayClickSound();
						}
						scrollTimer.Reset();
						TopDisplayIndex++;
					}
				}
			}
			else if (flag)
			{
				holdTimer.Update(gameTime.get_ElapsedGameTime());
				if (holdTimer.Expired)
				{
					scrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (scrollTimer.Expired)
					{
						if (TopDisplayIndex > 0)
						{
							PlayClickSound();
						}
						scrollTimer.Reset();
						TopDisplayIndex--;
					}
				}
			}
			if (TopDisplayIndex < 0)
			{
				TopDisplayIndex = 0;
			}
			if (TopDisplayIndex > _achievementManager.Count - MaxAchievementsToDisplay)
			{
				TopDisplayIndex = _achievementManager.Count - MaxAchievementsToDisplay;
			}
			if (controller.PressedButtons.A || controller.PressedButtons.B || controller.PressedButtons.Back)
			{
				PlayClickSound();
				PopMe();
			}
			base.OnPlayerInput(controller, gameTime);
			if ((!flag2 && lastselectdown) || (!flag && lastselectup))
			{
				holdTimer.Reset();
			}
			lastselectdown = flag2;
			lastselectup = flag;
		}
	}
}
