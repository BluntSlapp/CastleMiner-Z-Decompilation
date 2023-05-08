using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public abstract class Camera : Entity
	{
		public abstract Matrix View { get; }

		public abstract Matrix GetProjection(GraphicsDevice device);

		public virtual void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime time)
		{
			Scene scene = base.Scene;
			if (scene != null)
			{
				scene.Draw(device, time, View, GetProjection(device));
			}
		}
	}
}
