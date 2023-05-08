using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class MenuTextItem : LongListScreen.TextItem
	{
		private SpriteFont _largeFont;

		private SpriteFont _smallFont;

		public MenuTextItem(SpriteFont largeFont, SpriteFont smallFont, string text, object tag)
			: base(text, largeFont, tag)
		{
			_largeFont = largeFont;
			_smallFont = smallFont;
		}

		public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, Vector2 pos)
		{
			Font = (Selected ? _largeFont : _smallFont);
			base.Draw(device, spriteBatch, gameTime, pos);
		}

		public override Vector2 Measure()
		{
			Font = (Selected ? _largeFont : _smallFont);
			return base.Measure();
		}
	}
}
