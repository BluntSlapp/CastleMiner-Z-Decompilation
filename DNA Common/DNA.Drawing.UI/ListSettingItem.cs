using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class ListSettingItem : SettingItemElement
	{
		private int Index;

		private List<object> Items;

		public object CurrentItem
		{
			get
			{
				return Items[Index];
			}
		}

		public ListSettingItem(string text, List<object> items, int defaultIndex)
			: base(text)
		{
			Index = defaultIndex;
			Items = items;
		}

		public override void Clicked()
		{
			Index++;
			if (Index >= Items.Count)
			{
				Index = 0;
			}
		}

		public override void Decreased()
		{
			Index--;
			if (Index < 0)
			{
				Index = 0;
			}
		}

		public override void Increased()
		{
			Index++;
			if (Index >= Items.Count)
			{
				Index = Items.Count - 1;
			}
		}

		public override void OnDraw(DNAGame _game, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont font, Color textColor, Color outlineColor, int outlineWidth, float yLoc)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Vector2 vector = font.MeasureString(">");
			float num = (float)(titleSafeArea.Right - titleSafeArea.Center.X - 120) - vector.X * 2f;
			float num2 = titleSafeArea.Center.X + 50;
			string text = CurrentItem.ToString();
			float x = num2 + vector.X + 10f + num / 2f - font.MeasureString(text).X / 2f;
			spriteBatch.DrawOutlinedText(font, "<", new Vector2(num2, yLoc), textColor, outlineColor, outlineWidth);
			spriteBatch.DrawOutlinedText(font, ">", new Vector2((float)(titleSafeArea.Right - 50) - vector.X, yLoc), textColor, outlineColor, outlineWidth);
			spriteBatch.DrawOutlinedText(font, text, new Vector2(x, yLoc), textColor, outlineColor, outlineWidth);
			base.OnDraw(_game, device, spriteBatch, font, textColor, outlineColor, outlineWidth, yLoc);
		}
	}
}
