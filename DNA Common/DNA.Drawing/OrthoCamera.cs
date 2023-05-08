using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class OrthoCamera : Camera
	{
		public override Matrix View
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Matrix GetProjection(GraphicsDevice device)
		{
			throw new NotImplementedException();
		}
	}
}
