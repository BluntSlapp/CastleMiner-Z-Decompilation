using System;
using System.Collections.ObjectModel;
using DNA.Drawing;
using DNA.Drawing.UI;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.UI
{
	public class ChooseSessionScreen : LongListScreen
	{
		public class SessionSelectMenuSessionItem : MenuItem
		{
			public enum NetworkProps
			{
				Version,
				Public,
				Permission
			}

			private SpriteFont _largeFont;

			private SpriteFont _medFont;

			private SpriteFont _smallFont;

			public Color Color = Color.White;

			public Color SelectedColor = Color.Red;

			private string _name;

			private int _numberPlayers = -1;

			private OneShotTimer flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

			private bool selectedDirection;

			public AvailableNetworkSession session
			{
				get
				{
					return (AvailableNetworkSession)Tag;
				}
			}

			public string HostName
			{
				get
				{
					if (_name == null)
					{
						_name = session.HostGamertag;
					}
					return _name;
				}
			}

			public int NumberPlayers
			{
				get
				{
					if (_numberPlayers == -1)
					{
						_numberPlayers = session.CurrentGamerCount;
					}
					return _numberPlayers;
				}
			}

			public GameDifficultyTypes GameDifficulty
			{
				get
				{
					return (GameDifficultyTypes)session.SessionProperties.get_Item(5).Value;
				}
			}

			public SessionSelectMenuSessionItem(SpriteFont largeFont, SpriteFont medFont, SpriteFont smallFont, AvailableNetworkSession session)
				: base(session)
			{
				_largeFont = largeFont;
				_smallFont = smallFont;
				_medFont = medFont;
			}

			public override Vector2 Measure()
			{
				string hostName = HostName;
				string text = NumberPlayers + " players";
				SpriteFont spriteFont;
				SpriteFont spriteFont2;
				if (Selected)
				{
					spriteFont = _largeFont;
					spriteFont2 = _medFont;
				}
				else
				{
					spriteFont = _medFont;
					spriteFont2 = _smallFont;
				}
				Vector2 vector = spriteFont.MeasureString(hostName);
				Vector2 vector2 = spriteFont2.MeasureString(text);
				return new Vector2((vector2.X > vector.X) ? vector2.X : vector.X, vector.Y + vector2.Y);
			}

			public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, Vector2 pos)
			{
				Color textColor = Color;
				SpriteFont spriteFont;
				SpriteFont spriteFont2;
				if (Selected)
				{
					spriteFont = _largeFont;
					spriteFont2 = _medFont;
					flashTimer.Update(gameTime.get_ElapsedGameTime());
					if (flashTimer.Expired)
					{
						flashTimer.Reset();
						selectedDirection = !selectedDirection;
					}
					textColor = ((!selectedDirection) ? Color.Lerp(SelectedColor, Color, flashTimer.PercentComplete) : Color.Lerp(Color, SelectedColor, flashTimer.PercentComplete));
				}
				else
				{
					spriteFont = _medFont;
					spriteFont2 = _smallFont;
				}
				string hostName = HostName;
				string text = NumberPlayers + " player";
				if (NumberPlayers != 1)
				{
					text += "s";
				}
				Vector2 vector = spriteFont.MeasureString(hostName);
				Vector2 vector2 = spriteFont2.MeasureString(text);
				spriteBatch.DrawOutlinedText(spriteFont, hostName, pos, textColor, Color.Black, 1);
				spriteBatch.DrawOutlinedText(spriteFont2, text, new Vector2(pos.X, pos.Y + vector.Y), textColor, Color.Black, 1);
				if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Survival)
				{
					string text2 = ((GameDifficulty == GameDifficultyTypes.EASY) ? " Difficulty: Easy" : ((GameDifficulty == GameDifficultyTypes.HARD) ? " Difficulty: Normal" : ((GameDifficulty != GameDifficultyTypes.NOENEMIES) ? " Difficulty: Hardcore" : " Difficulty: No Enemies")));
					spriteBatch.DrawOutlinedText(spriteFont2, text2, new Vector2(pos.X + vector2.X, pos.Y + vector.Y), textColor, Color.Black, 1);
				}
			}
		}

		private CastleMinerZGame _game;

		public ChooseSessionScreen(CastleMinerZGame game)
			: base(false)
		{
			_game = game;
			SelectSound = "Click";
			ClickSound = "Click";
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			SpriteFont largeFont = _game._largeFont;
			Viewport viewport = device.Viewport;
			Rectangle rectangle = (destRect = new Rectangle(viewport.TitleSafeArea.Left, viewport.TitleSafeArea.Top + largeFont.LineSpacing * 2, viewport.TitleSafeArea.Width, viewport.TitleSafeArea.Height - largeFont.LineSpacing * 2));
			spriteBatch.Begin();
			if (base.MenuItems.Count == 0)
			{
				string text = "No Worlds Found";
				Vector2 vector = largeFont.MeasureString(text);
				int lineSpacing = largeFont.LineSpacing;
				spriteBatch.DrawOutlinedText(largeFont, text, new Vector2((float)viewport.TitleSafeArea.Center.X - vector.X / 2f, (float)viewport.TitleSafeArea.Center.Y - vector.Y / 2f), Color.White, Color.Black, 2);
			}
			else
			{
				string text = "Choose a World";
				Vector2 vector = largeFont.MeasureString(text);
				int lineSpacing2 = largeFont.LineSpacing;
				spriteBatch.DrawOutlinedText(largeFont, text, new Vector2((float)(viewport.Width / 2) - vector.X / 2f, viewport.TitleSafeArea.Top), Color.White, Color.Black, 2);
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		public void Populate(AvailableNetworkSessionCollection sessions)
		{
			base.MenuItems.Clear();
			if (sessions == null)
			{
				return;
			}
			foreach (AvailableNetworkSession item in (ReadOnlyCollection<AvailableNetworkSession>)(object)sessions)
			{
				base.MenuItems.Add(new SessionSelectMenuSessionItem(_game._largeFont, _game._medFont, _game._smallFont, item));
			}
		}

		public override void OnPoped()
		{
			base.MenuItems.Clear();
			base.OnPoped();
		}

		protected override void OnBack()
		{
			PopMe();
			base.OnBack();
		}
	}
}
