using System;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class Screen
	{
		public Color? BackgroundColor = null;

		public Texture2D BackgroundImage;

		private static PlayerIndex? _selectedPlayerIndex;

		private bool _drawBehind;

		private bool _acceptInput = true;

		private bool _doUpdate = true;

		private UpdateEventArgs _updateEventArgs = new UpdateEventArgs();

		private ControllerInputEventArgs _controllerEventArgs = new ControllerInputEventArgs();

		private DrawEventArgs args = new DrawEventArgs();

		public static PlayerIndex? SelectedPlayerIndex
		{
			get
			{
				return _selectedPlayerIndex;
			}
			set
			{
				_selectedPlayerIndex = value;
			}
		}

		public static SignedInGamer CurrentGamer
		{
			get
			{
				if (!_selectedPlayerIndex.HasValue)
				{
					return null;
				}
				return Gamer.SignedInGamers[_selectedPlayerIndex.Value];
			}
		}

		public virtual bool Exiting { get; set; }

		public bool DrawBehind
		{
			get
			{
				if (BackgroundColor.HasValue)
				{
					return false;
				}
				return _drawBehind;
			}
		}

		public virtual bool AcceptInput
		{
			get
			{
				return _acceptInput;
			}
			set
			{
				_acceptInput = value;
			}
		}

		public virtual bool DoUpdate
		{
			get
			{
				return _doUpdate;
			}
			set
			{
				_doUpdate = value;
			}
		}

		public event EventHandler<UpdateEventArgs> Updating;

		public event EventHandler<InputEventArgs> ProcessingInput;

		public event EventHandler<ControllerInputEventArgs> ProcessingPlayerInput;

		public event EventHandler<DrawEventArgs> BeforeDraw;

		public event EventHandler<DrawEventArgs> AfterDraw;

		public event EventHandler Pushed;

		public event EventHandler Poped;

		public static event EventHandler<SignedOutEventArgs> PlayerSignedOut;

		public static event EventHandler<SignedInEventArgs> PlayerSignedIn;

		static Screen()
		{
			_selectedPlayerIndex = null;
			SignedInGamer.add_SignedIn((EventHandler<SignedInEventArgs>)SignedInGamer_SignedIn);
			SignedInGamer.add_SignedOut((EventHandler<SignedOutEventArgs>)SignedInGamer_SignedOut);
		}

		public Screen(bool acceptInput, bool drawBehind)
		{
			_acceptInput = acceptInput;
			_drawBehind = drawBehind;
		}

		public virtual void OnLostFocus()
		{
		}

		protected virtual void OnUpdate(DNAGame game, GameTime gameTime)
		{
		}

		public virtual void Update(DNAGame game, GameTime gameTime)
		{
			if (DoUpdate)
			{
				if (this.Updating != null)
				{
					_updateEventArgs.GameTime = gameTime;
					this.Updating(this, _updateEventArgs);
				}
				OnUpdate(game, gameTime);
			}
		}

		protected virtual void OnInput(InputManager inputManager, GameTime gameTime)
		{
		}

		public virtual void ProcessInput(InputManager inputManager, GameTime gameTime)
		{
			if (_selectedPlayerIndex.HasValue)
			{
				ProcessInput(inputManager.Controllers[(int)_selectedPlayerIndex.Value], gameTime);
			}
			if (this.ProcessingInput != null)
			{
				this.ProcessingInput(this, new InputEventArgs(inputManager, gameTime));
			}
			OnInput(inputManager, gameTime);
		}

		protected virtual void OnPlayerInput(GameController controller, GameTime gameTime)
		{
		}

		protected virtual void ProcessInput(GameController controller, GameTime gameTime)
		{
			if (this.ProcessingPlayerInput != null)
			{
				_controllerEventArgs.Controller = controller;
				_controllerEventArgs.GameTime = gameTime;
				this.ProcessingPlayerInput(this, _controllerEventArgs);
			}
			OnPlayerInput(controller, gameTime);
		}

		protected virtual void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
		}

		public virtual void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			args.Device = device;
			args.GameTime = gameTime;
			if (this.BeforeDraw != null)
			{
				this.BeforeDraw(this, args);
			}
			if (BackgroundImage != null)
			{
				device.Clear(ClearOptions.DepthBuffer, Color.Red, 1f, 0);
				int width = device.Viewport.Width;
				int height = device.Viewport.Height;
				int num = width * BackgroundImage.Height / height;
				int num2 = num - width;
				int num3 = num2 / 2;
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);
				spriteBatch.Draw(BackgroundImage, new Rectangle(-num3, 0, num, height), (Rectangle?)new Rectangle(0, 0, BackgroundImage.Width, BackgroundImage.Height), Color.White);
				spriteBatch.End();
			}
			else if (BackgroundColor.HasValue)
			{
				device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, BackgroundColor.Value, 1f, 0);
			}
			else
			{
				device.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
			}
			OnDraw(device, spriteBatch, gameTime);
			if (this.AfterDraw != null)
			{
				this.AfterDraw(this, args);
			}
		}

		public virtual void OnPushed()
		{
			Exiting = false;
			if (this.Pushed != null)
			{
				this.Pushed(this, new EventArgs());
			}
		}

		public virtual void OnPoped()
		{
			if (this.Poped != null)
			{
				this.Poped(this, new EventArgs());
			}
		}

		public void PopMe()
		{
			Exiting = true;
		}

		private static void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
		{
			if (_selectedPlayerIndex.HasValue && e.Gamer.PlayerIndex == _selectedPlayerIndex.Value && Screen.PlayerSignedOut != null)
			{
				Screen.PlayerSignedOut(sender, e);
			}
		}

		private static void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
		{
			if (_selectedPlayerIndex.HasValue && e.Gamer.PlayerIndex == _selectedPlayerIndex.Value && Screen.PlayerSignedIn != null)
			{
				Screen.PlayerSignedIn(sender, e);
			}
		}
	}
}
