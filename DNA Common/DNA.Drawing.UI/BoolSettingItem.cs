using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class BoolSettingItem : SettingItemElement
	{
		private string[] TextValues = new string[2];

		public bool On;

		public BoolSettingItem(string text, bool isOn)
			: base(text)
		{
			TextValues[0] = "On";
			TextValues[1] = "Off";
			On = isOn;
		}

		public BoolSettingItem(string text, bool isOn, string onText, string offText)
			: base(text)
		{
			TextValues[0] = onText;
			TextValues[1] = offText;
			On = isOn;
		}

		public override void Clicked()
		{
			On = !On;
		}

		public override void Decreased()
		{
			Clicked();
		}

		public override void Increased()
		{
			Clicked();
		}

		public override bool ChangeValue(TimeSpan elapsedGameTime)
		{
			return false;
		}

		public override void OnDraw(DNAGame _game, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font, Color textColor, Color outlineColor, int outlineWidth, float yLoc)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Vector2 vector = font.MeasureString(">");
			float num = (float)(titleSafeArea.Right - titleSafeArea.Center.X - 120) - vector.X * 2f;
			float num2 = titleSafeArea.Center.X + 50;
			string text = (On ? TextValues[0] : TextValues[1]);
			float x = num2 + vector.X + 10f + num / 2f - font.MeasureString(text).X / 2f;
			spriteBatch.DrawOutlinedText(font, text, new Vector2(x, yLoc), textColor, outlineColor, outlineWidth);
			base.OnDraw(_game, device, spriteBatch, font, textColor, outlineColor, outlineWidth, yLoc);
		}
	}
}
