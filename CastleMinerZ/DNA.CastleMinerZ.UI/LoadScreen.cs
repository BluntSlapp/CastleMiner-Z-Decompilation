using System;
using DNA.Drawing.UI;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class LoadScreen : Screen
	{
		private OneShotTimer preBlackness = new OneShotTimer(TimeSpan.FromSeconds(2.0));

		private OneShotTimer fadeIn = new OneShotTimer(TimeSpan.FromSeconds(2.0));

		private OneShotTimer display = new OneShotTimer(TimeSpan.FromSeconds(2.0));

		private OneShotTimer fadeOut = new OneShotTimer(TimeSpan.FromSeconds(2.0));

		private OneShotTimer postBlackness = new OneShotTimer(TimeSpan.FromSeconds(2.0));

		private Texture2D _image;

		public bool Finished;

		public LoadScreen(Texture2D loadScreen, TimeSpan totalTime)
			: base(true, false)
		{
			display.MaxTime = totalTime - TimeSpan.FromSeconds(8.0);
			_image = loadScreen;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Rectangle destinationRectangle = new Rectangle(0, 0, 1280, 720);
			spriteBatch.Begin();
			if (preBlackness.Expired)
			{
				if (fadeIn.Expired)
				{
					if (display.Expired)
					{
						if (fadeOut.Expired)
						{
							if (postBlackness.Expired)
							{
								Finished = true;
							}
							else
							{
								spriteBatch.Draw(_image, destinationRectangle, Color.Black);
							}
						}
						else
						{
							spriteBatch.Draw(_image, destinationRectangle, Color.Lerp(Color.White, Color.Black, fadeOut.PercentComplete));
						}
					}
					else
					{
						spriteBatch.Draw(_image, destinationRectangle, Color.White);
					}
				}
				else
				{
					spriteBatch.Draw(_image, destinationRectangle, Color.Lerp(Color.Black, Color.White, fadeIn.PercentComplete));
				}
			}
			else
			{
				spriteBatch.Draw(_image, destinationRectangle, Color.Black);
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			if (preBlackness.Expired)
			{
				if (fadeIn.Expired)
				{
					if (display.Expired)
					{
						if (fadeOut.Expired)
						{
							if (postBlackness.Expired)
							{
								Finished = true;
							}
							else
							{
								postBlackness.Update(gameTime.get_ElapsedGameTime());
							}
						}
						else
						{
							fadeOut.Update(gameTime.get_ElapsedGameTime());
						}
					}
					else
					{
						display.Update(gameTime.get_ElapsedGameTime());
					}
				}
				else
				{
					fadeIn.Update(gameTime.get_ElapsedGameTime());
				}
			}
			else
			{
				preBlackness.Update(gameTime.get_ElapsedGameTime());
			}
			base.OnUpdate(game, gameTime);
		}
	}
}
