using System.Collections.Generic;
using DNA.Profiling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class SceneScreen : Screen
	{
		private List<Scene> _scenes = new List<Scene>();

		private List<View> _views = new List<View>();

		private Dictionary<Scene, int> scenes = new Dictionary<Scene, int>();

		public List<Scene> Scenes
		{
			get
			{
				return _scenes;
			}
		}

		public List<View> Views
		{
			get
			{
				return _views;
			}
		}

		public SceneScreen(bool acceptInput, bool drawBehind)
			: base(acceptInput, drawBehind)
		{
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			using (Profiler.TimeSection("Update", ProfilerThreadEnum.MAIN))
			{
				for (int i = 0; i < _scenes.Count; i++)
				{
					_scenes[i].Update(game, gameTime);
				}
				base.OnUpdate(game, gameTime);
			}
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			using (Profiler.TimeSection("Drawing", ProfilerThreadEnum.MAIN))
			{
				for (int i = 0; i < _views.Count; i++)
				{
					_views[i].Draw(device, spriteBatch, gameTime);
				}
				base.OnDraw(device, spriteBatch, gameTime);
			}
		}
	}
}
