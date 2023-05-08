using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class SpriteManager
	{
		public class SpriteManagerReader : ContentTypeReader<SpriteManager>
		{
			protected override SpriteManager Read(ContentReader input, SpriteManager existingInstance)
			{
				SpriteManager spriteManager = new SpriteManager();
				Texture2D texture = input.ReadObject<Texture2D>();
				int num = ((BinaryReader)(object)input).ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string key = ((BinaryReader)(object)input).ReadString();
					Rectangle sourceRectangle = new Rectangle(((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32());
					spriteManager._sprites[key] = new Sprite(texture, sourceRectangle);
				}
				return spriteManager;
			}
		}

		private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

		public Sprite this[string name]
		{
			get
			{
				return _sprites[name];
			}
		}
	}
}
