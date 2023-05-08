using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class MenuItemElement
	{
		public object Tag;

		public string Text;

		public Color? OutlineColor;

		public int? OnlineWidth;

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

		public MenuItemElement(string text, object tag)
		{
			Text = text;
			Tag = tag;
		}

		public MenuItemElement(string text, Color textColor, Color selectedColor, object tag)
		{
			Text = text;
			Tag = tag;
			_textColor = textColor;
			_selectedColor = selectedColor;
		}

		public MenuItemElement(string text, Color textColor, Color selectedColor, SpriteFont font, object tag)
		{
			Text = text;
			Tag = tag;
			_textColor = textColor;
			_selectedColor = selectedColor;
			Font = font;
		}
	}
}
