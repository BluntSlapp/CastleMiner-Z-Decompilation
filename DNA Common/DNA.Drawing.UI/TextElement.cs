using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class TextElement : UIElement
	{
		private bool _pulseDir;

		private TimeSpan _currenPulseTime = TimeSpan.FromSeconds(0.0);

		private TimeSpan _pulseTime = TimeSpan.FromSeconds(0.0);

		private float _pulseSize = 0.1f;

		public string Text = "<Text>";

		public SpriteFont Font;

		private Color _outLineColor = Color.Black;

		private int _outLineWidth = 2;

		public TimeSpan PulseTime
		{
			get
			{
				return _pulseTime;
			}
			set
			{
				_pulseTime = value;
			}
		}

		public float PulseSize
		{
			get
			{
				return _pulseSize;
			}
			set
			{
				_pulseSize = value;
			}
		}

		public Color OutlineColor
		{
			get
			{
				return _outLineColor;
			}
			set
			{
				_outLineColor = value;
			}
		}

		public int OutlineWidth
		{
			get
			{
				return _outLineWidth;
			}
			set
			{
				_outLineWidth = value;
			}
		}

		public override Vector2 Size
		{
			get
			{
				return Font.MeasureString(Text);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public TextElement(string text, SpriteFont font)
		{
			Text = text;
			Font = font;
		}

		public TextElement(SpriteFont font)
		{
			Font = font;
		}

		protected virtual Color GetForColor(bool selected)
		{
			return base.Color;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, bool selected)
		{
			if (_pulseTime > TimeSpan.Zero)
			{
				if (_pulseDir)
				{
					_currenPulseTime += gameTime.get_ElapsedGameTime();
					if (_currenPulseTime > _pulseTime)
					{
						_currenPulseTime = _pulseTime;
						_pulseDir = !_pulseDir;
					}
				}
				else
				{
					_currenPulseTime -= gameTime.get_ElapsedGameTime();
					if (_currenPulseTime < TimeSpan.Zero)
					{
						_currenPulseTime = TimeSpan.Zero;
						_pulseDir = !_pulseDir;
					}
				}
				float scale = (float)(1.0 + (double)PulseSize * _currenPulseTime.TotalSeconds / _pulseTime.TotalSeconds);
				Vector2 vector = new Vector2(Size.X / 2f, Size.Y / 2f);
				spriteBatch.DrawOutlinedText(Font, Text, base.Location + vector, GetForColor(selected), OutlineColor, OutlineWidth, scale, 0f, vector);
			}
			else
			{
				spriteBatch.DrawOutlinedText(Font, Text, base.Location, GetForColor(selected), OutlineColor, OutlineWidth);
			}
		}
	}
}
