using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class CameraView : View
	{
		private Camera _camera;

		public Camera Camera
		{
			get
			{
				return _camera;
			}
			set
			{
				_camera = value;
				if (_camera != null && _camera.Scene == null)
				{
					throw new Exception("Camera Must Be in Scene");
				}
			}
		}

		public CameraView(RenderTarget2D target, Camera camera)
			: base(target)
		{
			if (camera != null && camera.Scene == null)
			{
				throw new Exception("Camera Must Be in Scene");
			}
			_camera = camera;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			base.OnDraw(device, spriteBatch, gameTime);
			if (Camera != null)
			{
				Camera.Draw(device, spriteBatch, gameTime);
			}
		}
	}
}
