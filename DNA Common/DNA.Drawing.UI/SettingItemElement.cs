using System;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class SettingItemElement
	{
		public string Text;

		public Color? OutlineColor;

		public int? OnlineWidth;

		protected OneShotTimer changeValueTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		protected OneShotTimer delayTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		private Color? _textColor = Color.White;

		private Color? _selectedColor = Color.Red;

		public bool Visible = true;

		private SpriteFont _font;

		public Color? TextColor
		{
			get
			{
				return _textColor;
			}
			set
			{
				_textColor = value;
			}
		}

		public Color? SelectedColor
		{
			get
			{
				return _selectedColor;
			}
			set
			{
				_selectedColor = value;
			}
		}

		public SpriteFont Font
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;
			}
		}

		public SettingItemElement(string text)
		{
			Text = text;
		}

		public virtual void Clicked()
		{
		}

		public virtual void Increased()
		{
		}

		public virtual void Decreased()
		{
		}

		public virtual void OnDraw(DNAGame _game, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font, Color textColor, Color outlineColor, int outlineWidth, float yLoc)
		{
			Vector2 location = new Vector2(device.Viewport.TitleSafeArea.Left, yLoc);
			spriteBatch.DrawOutlinedText(font, Text, location, textColor, outlineColor, outlineWidth);
		}

		public void ResetTimer()
		{
			changeValueTimer.Reset();
			delayTimer.Reset();
		}

		public virtual bool ChangeValue(TimeSpan elapsedGameTime)
		{
			delayTimer.Update(elapsedGameTime);
			if (delayTimer.Expired)
			{
				changeValueTimer.Update(elapsedGameTime);
				if (changeValueTimer.Expired)
				{
					changeValueTimer.Reset();
					return true;
				}
			}
			return false;
		}
	}
}
