using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public static class SpriteBatchSpriteExtensions
	{
		public static void Draw(this SpriteBatch batch, Sprite sprite, Rectangle destinationRectangle, Color color)
		{
			sprite.Draw(batch, destinationRectangle, color);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, Color color)
		{
			sprite.Draw(batch, position, color);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, float scale, Color color)
		{
			sprite.Draw(batch, position, scale, color);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, float scale, Color color, SpriteEffects effects)
		{
			sprite.Draw(batch, position, scale, color, effects);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color)
		{
			sprite.Draw(batch, destinationRectangle, sourceRectangle, color);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, Rectangle sourceRectangle, Color color)
		{
			sprite.Draw(batch, position, sourceRectangle, color);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color, Angle rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
		{
			sprite.Draw(batch, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, Color color, Angle rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			sprite.Draw(batch, position, color, rotation, origin, scale, effects, layerDepth);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, Rectangle sourceRectangle, Color color, Angle rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
		{
			sprite.Draw(batch, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}

		public static void Draw(this SpriteBatch batch, Sprite sprite, Vector2 position, Rectangle sourceRectangle, Color color, Angle rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			sprite.Draw(batch, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
		}
	}
}
