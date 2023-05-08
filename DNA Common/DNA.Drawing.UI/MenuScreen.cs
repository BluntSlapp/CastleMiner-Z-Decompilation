using System;
using System.Collections.Generic;
using DNA.Audio;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.UI
{
	public class MenuScreen : Screen
	{
		private bool _flashDir;

		private OneShotTimer _flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

		public string SelectSound;

		public string ClickSound;

		private List<MenuItemElement> _menuItems = new List<MenuItemElement>();

		private int _selectedIndex;

		public float? MenuStart = null;

		public SpriteFont Font;

		public Color TextColor = Color.White;

		public Color SelectedColor = Color.Red;

		public Color OutlineColor = Color.Black;

		public int OnlineWidth = 2;

		public List<MenuItemElement> MenuItems
		{
			get
			{
				return _menuItems;
			}
		}

		public TimeSpan FlashTime
		{
			get
			{
				return _flashTimer.MaxTime;
			}
			set
			{
				_flashTimer.MaxTime = value;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return _selectedIndex;
			}
			set
			{
				_selectedIndex = value;
			}
		}

		public event EventHandler<SelectedMenuItemArgs> MenuItemSelected;

		public MenuItemElement AddMenuItem(string text, object tag)
		{
			MenuItemElement menuItemElement = new MenuItemElement(text, tag);
			MenuItems.Add(menuItemElement);
			return menuItemElement;
		}

		public MenuScreen(SpriteFont font, bool drawBehind)
			: base(true, drawBehind)
		{
			Font = font;
		}

		public MenuScreen(SpriteFont font, Color textColor, Color selectedColor, bool drawBehind)
			: base(true, drawBehind)
		{
			TextColor = textColor;
			SelectedColor = selectedColor;
			Font = font;
		}

		protected override void OnPlayerInput(GameController controller, GameTime gameTime)
		{
			float num = 0.25f;
			if (controller.PressedDPad.Down || controller.PressedButtons.RightShoulder || (controller.CurrentState.ThumbSticks.Left.Y < 0f - num && controller.LastState.ThumbSticks.Left.Y > 0f - num) || (controller.CurrentState.ThumbSticks.Right.Y < 0f - num && controller.LastState.ThumbSticks.Right.Y > 0f - num))
			{
				if (SelectSound != null)
				{
					SoundManager.Instance.PlayInstance(SelectSound);
				}
				SelectNext();
			}
			if (controller.PressedDPad.Up || controller.PressedButtons.LeftShoulder || (controller.CurrentState.ThumbSticks.Left.Y > num && controller.LastState.ThumbSticks.Left.Y < num) || (controller.CurrentState.ThumbSticks.Right.Y > num && controller.LastState.ThumbSticks.Right.Y < num))
			{
				if (SelectSound != null)
				{
					SoundManager.Instance.PlayInstance(SelectSound);
				}
				SelectPrevious();
			}
			if (controller.PressedButtons.B || controller.PressedButtons.Back)
			{
				if (ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(ClickSound);
				}
				PopMe();
			}
			if (controller.PressedButtons.A || controller.PressedButtons.Start)
			{
				if (ClickSound != null)
				{
					SoundManager.Instance.PlayInstance(ClickSound);
				}
				SelectMenuItem();
			}
			base.OnPlayerInput(controller, gameTime);
		}

		protected virtual void OnMenuItemSelected(UIElement selectedControl)
		{
		}

		private void SelectMenuItem()
		{
			if (_selectedIndex >= 0)
			{
				MenuItemElement control = _menuItems[_selectedIndex];
				if (this.MenuItemSelected != null)
				{
					this.MenuItemSelected(this, new SelectedMenuItemArgs(control));
				}
			}
		}

		private void SelectFirst()
		{
			if (_menuItems.Count == 0)
			{
				_selectedIndex = -1;
			}
			else
			{
				_selectedIndex = 0;
			}
		}

		private void SelectNext()
		{
			if (_menuItems.Count == 0)
			{
				_selectedIndex = -1;
				return;
			}
			int selectedIndex = _selectedIndex;
			do
			{
				_selectedIndex++;
				if (_selectedIndex >= _menuItems.Count)
				{
					_selectedIndex = 0;
				}
				if (_selectedIndex == selectedIndex && !_menuItems[_selectedIndex].Visible)
				{
					_selectedIndex = -1;
					break;
				}
			}
			while (!_menuItems[_selectedIndex].Visible);
		}

		private void SelectPrevious()
		{
			if (_menuItems.Count == 0)
			{
				_selectedIndex = -1;
				return;
			}
			int selectedIndex = _selectedIndex;
			do
			{
				_selectedIndex--;
				if (_selectedIndex < 0)
				{
					_selectedIndex = _menuItems.Count - 1;
				}
				if (_selectedIndex == selectedIndex && !_menuItems[_selectedIndex].Visible)
				{
					_selectedIndex = -1;
					break;
				}
			}
			while (!_menuItems[_selectedIndex].Visible);
		}

		private Vector2 MeasureItem(MenuItemElement item)
		{
			SpriteFont spriteFont = ((item.Font == null) ? Font : item.Font);
			return spriteFont.MeasureString(item.Text);
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			_flashTimer.Update(gameTime.get_ElapsedGameTime());
			if (_flashTimer.Expired)
			{
				_flashTimer.Reset();
				_flashDir = !_flashDir;
			}
			while (_selectedIndex >= _menuItems.Count || !MenuItems[_selectedIndex].Visible)
			{
				_selectedIndex++;
				_selectedIndex %= _menuItems.Count;
			}
			spriteBatch.Begin();
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < _menuItems.Count; i++)
			{
				if (_menuItems[i].Visible)
				{
					num += MeasureItem(_menuItems[i]).Y + num2;
				}
			}
			num -= num2;
			float num3 = device.Viewport.Height;
			float num4 = device.Viewport.Width;
			float num5 = (num3 - num) / 2f;
			if (MenuStart.HasValue)
			{
				num5 = MenuStart.Value;
			}
			for (int j = 0; j < _menuItems.Count; j++)
			{
				MenuItemElement menuItemElement = _menuItems[j];
				if (menuItemElement.Visible)
				{
					SpriteFont font = ((menuItemElement.Font == null) ? Font : menuItemElement.Font);
					Color color = (menuItemElement.TextColor.HasValue ? menuItemElement.TextColor.Value : TextColor);
					Color outlineColor = (menuItemElement.OutlineColor.HasValue ? menuItemElement.OutlineColor.Value : OutlineColor);
					int outlineWidth = (menuItemElement.OnlineWidth.HasValue ? menuItemElement.OnlineWidth.Value : OnlineWidth);
					Vector2 vector = MeasureItem(menuItemElement);
					Vector2 location = new Vector2((num4 - vector.X) / 2f, num5);
					num5 += vector.Y + num2;
					Color textColor = color;
					if (j == _selectedIndex)
					{
						Color value = (menuItemElement.SelectedColor.HasValue ? menuItemElement.SelectedColor.Value : SelectedColor);
						float amount = (_flashDir ? _flashTimer.PercentComplete : (1f - _flashTimer.PercentComplete));
						textColor = Color.Lerp(color, value, amount);
					}
					spriteBatch.DrawOutlinedText(font, menuItemElement.Text, location, textColor, outlineColor, outlineWidth);
				}
			}
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}
	}
}
