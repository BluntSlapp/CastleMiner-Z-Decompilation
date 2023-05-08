using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public abstract class UIElement
	{
		private object _tag;

		private Color _color = Color.White;

		private Vector2 _location;

		private bool _visible = true;

		public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		public Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}

		public RectangleF Bounds
		{
			get
			{
				return new RectangleF(Location, Size);
			}
		}

		public Vector2 Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				OnVisibilityChanged(_visible);
			}
		}

		public abstract Vector2 Size { get; set; }

		protected virtual void OnVisibilityChanged(bool visibility)
		{
		}

		protected abstract void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, bool selected);

		public void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, bool selected)
		{
			if (_visible)
			{
				RectangleF rectangleF = (RectangleF)device.Viewport.TitleSafeArea;
				OnDraw(device, spriteBatch, gameTime, selected);
			}
		}
	}
}
