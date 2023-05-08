using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class Sprite
	{
		private Texture2D _texture;

		private Rectangle _sourceRectangle;

		public Texture2D Texture
		{
			get
			{
				return _texture;
			}
		}

		public Rectangle SourceRectangle
		{
			get
			{
				return _sourceRectangle;
			}
		}

		public RectangleF UVRectangle
		{
			get
			{
				return new RectangleF((float)_sourceRectangle.Left / (float)_texture.Width, (float)_sourceRectangle.Top / (float)_texture.Height, (float)_sourceRectangle.Width / (float)_texture.Width, (float)_sourceRectangle.Height / (float)_texture.Height);
			}
		}

		public int Width
		{
			get
			{
				return _sourceRectangle.Width;
			}
		}

		public int Height
		{
			get
			{
				return _sourceRectangle.Height;
			}
		}

		public Sprite(Texture2D texture, Rectangle sourceRectangle)
		{
			_texture = texture;
			_sourceRectangle = sourceRectangle;
		}

		public void Draw(SpriteBatch batch, Rectangle destinationRectangle, Color color)
		{
			batch.Draw(_texture, destinationRectangle, (Rectangle?)_sourceRectangle, color);
		}

		public void Draw(SpriteBatch batch, Vector2 position, Color color)
		{
			batch.Draw(_texture, position, (Rectangle?)_sourceRectangle, color);
		}

		public void Draw(SpriteBatch batch, Vector2 position, float scale, Color color)
		{
			batch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, (int)((float)_sourceRectangle.Width * scale), (int)((float)_sourceRectangle.Height * scale)), (Rectangle?)_sourceRectangle, color);
		}

		public void Draw(SpriteBatch batch, Vector2 position, float scale, Color color, SpriteEffects effects)
		{
			Rectangle rectangle = new Rectangle((int)position.X, (int)position.Y, (int)((float)_sourceRectangle.Width * scale), (int)((float)_sourceRectangle.Height * scale));
			batch.Draw(_texture, rectangle, (Rectangle?)_sourceRectangle, color, 0f, Vector2.Zero, effects, 1f);
		}

		public void Draw(SpriteBatch batch, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color)
		{
			Rectangle value = new Rectangle(sourceRectangle.Left + _sourceRectangle.Left, sourceRectangle.Top + _sourceRectangle.Top, sourceRectangle.Width, sourceRectangle.Height);
			batch.Draw(_texture, destinationRectangle, (Rectangle?)value, color);
		}

		public void Draw(SpriteBatch batch, Vector2 position, Rectangle sourceRectangle, Color color)
		{
			Draw(batch, new Rectangle((int)position.X, (int)position.Y, sourceRectangle.Width, sourceRectangle.Height), sourceRectangle, color);
		}

		public void Draw(SpriteBatch batch, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color, Angle rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
		{
			Rectangle value = new Rectangle(sourceRectangle.Left + _sourceRectangle.Left, sourceRectangle.Top + _sourceRectangle.Top, sourceRectangle.Width, sourceRectangle.Height);
			batch.Draw(_texture, destinationRectangle, (Rectangle?)value, color, rotation.Radians, origin, effects, layerDepth);
		}

		public void Draw(SpriteBatch batch, Vector2 position, Color color, Angle rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			batch.Draw(_texture, position, (Rectangle?)_sourceRectangle, color, rotation.Radians, origin, scale, effects, layerDepth);
		}

		public void Draw(SpriteBatch batch, Vector2 position, Rectangle sourceRectangle, Color color, Angle rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			Rectangle value = new Rectangle(sourceRectangle.Left + _sourceRectangle.Left, sourceRectangle.Top + _sourceRectangle.Top, sourceRectangle.Width, sourceRectangle.Height);
			batch.Draw(_texture, position, (Rectangle?)value, color, rotation.Radians, origin, scale, effects, layerDepth);
		}

		public void Draw(SpriteBatch batch, Vector2 position, Rectangle sourceRectangle, Color color, Angle rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			Rectangle value = new Rectangle(sourceRectangle.Left + _sourceRectangle.Left, sourceRectangle.Top + _sourceRectangle.Top, sourceRectangle.Width, sourceRectangle.Height);
			batch.Draw(_texture, position, (Rectangle?)value, color, rotation.Radians, origin, scale, effects, layerDepth);
		}
	}
}
