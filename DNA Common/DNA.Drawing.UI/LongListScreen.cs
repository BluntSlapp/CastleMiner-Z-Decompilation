using System;
using System.Collections.Generic;
using DNA.Audio;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DNA.Drawing.UI
{
	public class LongListScreen : Screen
	{
		public abstract class MenuItem
		{
			public object Tag;

			public bool Selected;

			public MenuItem(object tag)
			{
				Tag = tag;
			}

			public abstract void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, Vector2 pos);

			public abstract Vector2 Measure();
		}

		public class TextItem : MenuItem
		{
			public string Text;

			public SpriteFont Font;

			public Color Color = Color.White;

			public Color SelectedColor = Color.Red;

			private OneShotTimer flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

			private bool selectedDirection;

			public TextItem(string text, SpriteFont font, object tag)
				: base(tag)
			{
				Text = text;
				Font = font;
			}

			public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, Vector2 pos)
			{
				Color textColor = Color;
				if (Selected)
				{
					flashTimer.Update(gameTime.get_ElapsedGameTime());
					if (flashTimer.Expired)
					{
						flashTimer.Reset();
						selectedDirection = !selectedDirection;
					}
					textColor = ((!selectedDirection) ? Color.Lerp(SelectedColor, Color, flashTimer.PercentComplete) : Color.Lerp(Color, SelectedColor, flashTimer.PercentComplete));
				}
				spriteBatch.DrawOutlinedText(Font, Text, pos, textColor, Color.Black, 1);
			}

			public override Vector2 Measure()
			{
				return Font.MeasureString(Text);
			}
		}

		public const float deadZone = 0.25f;

		private List<MenuItem> _menuItems = new List<MenuItem>();

		private int _selectedIndex;

		private OneShotTimer holdTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		private OneShotTimer scrollTimer = new OneShotTimer(TimeSpan.FromSeconds(0.10000000149011612));

		private OneShotTimer accelerateDelayTimer = new OneShotTimer(TimeSpan.FromSeconds(3.0));

		private OneShotTimer accelerateTimer = new OneShotTimer(TimeSpan.FromSeconds(5.0));

		private TimeSpan scrollRate = TimeSpan.FromSeconds(0.05000000074505806);

		private bool lastselectup;

		private bool lastselectdown;

		public string SelectSound;

		public string ClickSound;

		public Rectangle destRect;

		public List<MenuItem> MenuItems
		{
			get
			{
				return _menuItems;
			}
		}

		public MenuItem SelectedItem
		{
			get
			{
				if (_selectedIndex < 0 || _selectedIndex >= _menuItems.Count)
				{
					return null;
				}
				return _menuItems[_selectedIndex];
			}
		}

		public event EventHandler<SelectedEventArgs> Clicked;

		public event EventHandler<SelectedEventArgs> BackClicked;

		public LongListScreen(bool drawBehind)
			: base(true, drawBehind)
		{
		}

		protected virtual void OnClicked(MenuItem selectedItem)
		{
		}

		public void Click()
		{
			OnClicked(SelectedItem);
			if (this.Clicked != null)
			{
				this.Clicked(this, new SelectedEventArgs(SelectedItem.Tag));
			}
			if (ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(ClickSound);
			}
		}

		protected virtual void OnBack()
		{
		}

		public void Back()
		{
			OnBack();
			if (this.BackClicked != null)
			{
				this.BackClicked(this, null);
			}
			if (ClickSound != null)
			{
				SoundManager.Instance.PlayInstance(ClickSound);
			}
		}

		protected override void OnPlayerInput(GameController controller, GameTime gameTime)
		{
			bool flag = false;
			bool flag2 = false;
			if (controller.CurrentState.ThumbSticks.Left.Y < -0.25f || controller.CurrentState.DPad.Down == ButtonState.Pressed || controller.CurrentState.Triggers.Right > 0.25f)
			{
				flag2 = true;
				if (!lastselectdown)
				{
					accelerateDelayTimer.Reset();
					accelerateTimer.Reset();
				}
			}
			if (controller.CurrentState.ThumbSticks.Left.Y > 0.25f || controller.CurrentState.DPad.Up == ButtonState.Pressed || controller.CurrentState.Triggers.Left > 0.25f)
			{
				flag = true;
				if (!lastselectup)
				{
					accelerateDelayTimer.Reset();
					accelerateTimer.Reset();
				}
			}
			if (Math.Abs(controller.CurrentState.ThumbSticks.Left.Y) > 0.8f)
			{
				accelerateDelayTimer.Update(gameTime.get_ElapsedGameTime());
			}
			if (accelerateDelayTimer.Expired)
			{
				accelerateTimer.Update(gameTime.get_ElapsedGameTime());
			}
			float num = accelerateTimer.PercentComplete * (float)scrollRate.TotalSeconds / 2f;
			float num2 = Math.Abs(controller.CurrentState.ThumbSticks.Left.Y);
			if (num2 > 0f)
			{
				scrollTimer.MaxTime = TimeSpan.FromSeconds(scrollRate.TotalSeconds / (double)num2 - (double)num);
			}
			if ((controller.CurrentState.ThumbSticks.Left.Y < -0.25f && controller.LastState.ThumbSticks.Left.Y > -0.25f) || controller.PressedDPad.Down || (controller.CurrentState.Triggers.Right > 0.25f && controller.LastState.Triggers.Right < 0.25f))
			{
				if (_selectedIndex < _menuItems.Count - 1 && SelectSound != null)
				{
					SoundManager.Instance.PlayInstance(SelectSound);
				}
				_selectedIndex++;
			}
			if ((controller.CurrentState.ThumbSticks.Left.Y > 0.25f && controller.LastState.ThumbSticks.Left.Y < 0.25f) || controller.PressedDPad.Up || (controller.CurrentState.Triggers.Left > 0.25f && controller.LastState.Triggers.Left < 0.25f))
			{
				if (_selectedIndex > 0 && SelectSound != null)
				{
					SoundManager.Instance.PlayInstance(SelectSound);
				}
				_selectedIndex--;
			}
			if (flag2)
			{
				holdTimer.Update(gameTime.get_ElapsedGameTime());
				if (holdTimer.Expired)
				{
					scrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (scrollTimer.Expired)
					{
						scrollTimer.Reset();
						if (accelerateTimer.Expired)
						{
							_selectedIndex += 10;
						}
						else
						{
							_selectedIndex++;
						}
						if (_selectedIndex < _menuItems.Count && SelectSound != null)
						{
							SoundManager.Instance.PlayInstance(SelectSound);
						}
					}
				}
			}
			else if (flag)
			{
				holdTimer.Update(gameTime.get_ElapsedGameTime());
				if (holdTimer.Expired)
				{
					scrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (scrollTimer.Expired)
					{
						scrollTimer.Reset();
						if (accelerateTimer.Expired)
						{
							_selectedIndex -= 10;
						}
						else
						{
							_selectedIndex--;
						}
						if (_selectedIndex >= 0 && SelectSound != null)
						{
							SoundManager.Instance.PlayInstance(SelectSound);
						}
					}
				}
			}
			if (_selectedIndex < 0)
			{
				_selectedIndex = 0;
			}
			if (_selectedIndex >= _menuItems.Count)
			{
				_selectedIndex = _menuItems.Count - 1;
			}
			if (controller.PressedButtons.A || controller.PressedButtons.Start)
			{
				if (MenuItems.Count > 0)
				{
					Click();
				}
				else
				{
					Back();
				}
			}
			if (controller.PressedButtons.B || controller.PressedButtons.Back)
			{
				Back();
			}
			base.OnPlayerInput(controller, gameTime);
			if ((!flag2 && lastselectdown) || (!flag && lastselectup))
			{
				holdTimer.Reset();
			}
			lastselectdown = flag2;
			lastselectup = flag;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Viewport viewport = device.Viewport;
			if (_selectedIndex < 0 || _selectedIndex >= _menuItems.Count)
			{
				return;
			}
			MenuItem menuItem = _menuItems[_selectedIndex];
			menuItem.Selected = true;
			Vector2 vector = menuItem.Measure();
			float num = (float)destRect.Center.Y - vector.Y / 2f;
			spriteBatch.Begin();
			menuItem.Draw(device, spriteBatch, gameTime, new Vector2(destRect.Left, num));
			float num2 = num;
			int num3;
			for (num3 = _selectedIndex - 1; num3 >= 0; num3--)
			{
				menuItem = _menuItems[num3];
				menuItem.Selected = false;
				Vector2 vector2 = menuItem.Measure();
				num2 -= vector2.Y;
				if (vector2.Y + num2 < (float)destRect.Top)
				{
					break;
				}
				menuItem.Draw(device, spriteBatch, gameTime, new Vector2(destRect.Left, num2));
			}
			num2 = num + vector.Y;
			num3 = _selectedIndex + 1;
			while (num3 < _menuItems.Count)
			{
				menuItem = _menuItems[num3];
				menuItem.Selected = false;
				Vector2 vector3 = menuItem.Measure();
				if (num2 > (float)destRect.Bottom)
				{
					break;
				}
				menuItem.Draw(device, spriteBatch, gameTime, new Vector2(destRect.Left, num2));
				num3++;
				num2 += vector3.Y;
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
