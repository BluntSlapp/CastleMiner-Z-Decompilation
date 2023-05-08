using System;
using System.Collections.Generic;
using System.Threading;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class ScreenGroup : Screen
	{
		private Stack<Screen> _screens = new Stack<Screen>();

		private Screen[] screensList = new Screen[0];

		public override bool AcceptInput
		{
			get
			{
				for (int i = 0; i < screensList.Length; i++)
				{
					Screen screen = screensList[i];
					if (screen.AcceptInput)
					{
						return true;
					}
					if (!screen.DrawBehind)
					{
						break;
					}
				}
				return false;
			}
			set
			{
				throw new Exception("Cannot explictly set Accept Input on a Screen Group");
			}
		}

		public override bool Exiting
		{
			get
			{
				if (_screens.Count == 0)
				{
					return true;
				}
				return base.Exiting;
			}
			set
			{
				base.Exiting = value;
			}
		}

		public Screen CurrentScreen
		{
			get
			{
				if (_screens.Count == 0)
				{
					return null;
				}
				return _screens.Peek();
			}
		}

		public ScreenGroup(bool drawBehind)
			: base(false, drawBehind)
		{
		}

		public void ShowDialogScreen(DialogScreen screen, ThreadStart callback)
		{
			screen.Callback = callback;
			PushScreen(screen);
		}

		public void Clear()
		{
			while (PopScreen() != null)
			{
			}
		}

		public void PushScreen(Screen screen)
		{
			lock (this)
			{
				if (_screens.Count > 0)
				{
					Screen screen2 = _screens.Peek();
					screen2.OnLostFocus();
				}
				_screens.Push(screen);
				screen.OnPushed();
				screensList = _screens.ToArray();
			}
		}

		public Screen PopScreen()
		{
			lock (this)
			{
				if (_screens.Count == 0)
				{
					return null;
				}
				Screen screen = _screens.Pop();
				screen.OnPoped();
				screen.OnLostFocus();
				screensList = _screens.ToArray();
				return screen;
			}
		}

		public bool Contains(Screen screen)
		{
			return _screens.Contains(screen);
		}

		public override void ProcessInput(InputManager inputManager, GameTime gameTime)
		{
			lock (this)
			{
				for (int i = 0; i < screensList.Length; i++)
				{
					Screen screen = screensList[i];
					if (screen.AcceptInput)
					{
						screen.ProcessInput(inputManager, gameTime);
						break;
					}
				}
			}
			base.ProcessInput(inputManager, gameTime);
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			base.Update(game, gameTime);
			lock (this)
			{
				while (_screens.Count != 0 && _screens.Peek().Exiting)
				{
					Screen screen = PopScreen();
					screen.Exiting = false;
				}
				for (int i = 0; i < screensList.Length; i++)
				{
					Screen screen2 = screensList[i];
					screen2.Update(game, gameTime);
					if (!screen2.DrawBehind)
					{
						break;
					}
				}
			}
		}

		public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			base.Draw(device, spriteBatch, gameTime);
			if (_screens.Count == 0)
			{
				return;
			}
			int num = screensList.Length - 1;
			for (int i = 0; i < screensList.Length; i++)
			{
				Screen screen = screensList[i];
				if (!screen.DrawBehind)
				{
					num = i;
					break;
				}
			}
			for (int num2 = num; num2 >= 0; num2--)
			{
				Screen screen2 = screensList[num2];
				screen2.Draw(device, spriteBatch, gameTime);
			}
		}
	}
}
