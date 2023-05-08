using System;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class BarSettingItem : SettingItemElement
	{
		public float Value;

		public BarSettingItem(string text, float defaultValue)
			: base(text)
		{
			changeValueTimer = new OneShotTimer(TimeSpan.FromSeconds(0.1));
			Value = defaultValue;
		}

		public override void Clicked()
		{
			if ((double)Value < 0.25)
			{
				Value = 0.25f;
			}
			else if ((double)Value < 0.5)
			{
				Value = 0.5f;
			}
			else if ((double)Value < 0.75)
			{
				Value = 0.75f;
			}
			else if (Value < 1f)
			{
				Value = 1f;
			}
			else
			{
				Value = 0f;
			}
		}

		public override void Increased()
		{
			Value += 0.05f;
			if (Value > 1f)
			{
				Value = 1f;
			}
		}

		public override void Decreased()
		{
			Value -= 0.05f;
			if (Value < 0f)
			{
				Value = 0f;
			}
		}

		public override void OnDraw(DNAGame _game, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font, Color textColor, Color outlineColor, int outlineWidth, float yLoc)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Vector2 vector = font.MeasureString(">");
			float num = (float)(titleSafeArea.Right - titleSafeArea.Center.X - 120) - vector.X * 2f;
			float num2 = vector.Y / 2f;
			float num3 = yLoc + (vector.Y - num2) / 2f - 3f;
			spriteBatch.DrawOutlinedText(font, "<", new Vector2(titleSafeArea.Center.X + 50, yLoc), textColor, outlineColor, outlineWidth);
			spriteBatch.DrawOutlinedText(font, ">", new Vector2((float)(titleSafeArea.Right - 50) - vector.X, yLoc), textColor, outlineColor, outlineWidth);
			spriteBatch.Draw(_game.DummyTexture, new Rectangle((int)((float)(titleSafeArea.Center.X + 60) + vector.X), (int)num3, (int)num, (int)num2), Color.Black);
			spriteBatch.Draw(_game.DummyTexture, new Rectangle((int)((float)(titleSafeArea.Center.X + 61) + vector.X), (int)num3 + 1, (int)((num - 2f) * Value), (int)(num2 - 2f)), Color.White);
			base.OnDraw(_game, device, spriteBatch, font, textColor, outlineColor, outlineWidth, yLoc);
		}
	}
}
