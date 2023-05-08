using System;
using System.Text;
using DNA.Audio;
using DNA.CastleMinerZ.Inventory;
using DNA.Drawing;
using DNA.Drawing.UI;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class BlockPickerScreen : Screen
	{
		private const int Columns = 8;

		private const int Rows = 4;

		private const int ItemSize = 59;

		private Sprite _background;

		private Sprite _gridSelector;

		private CastleMinerZGame _game;

		private SpriteFont _bigFont;

		private SpriteFont _smallFont;

		private InGameHUD _hud;

		private Point _selectedLocation = new Point(0, 0);

		private InventoryItem _holdingItem;

		private StringBuilder stringBuilder = new StringBuilder();

		private OneShotTimer waitScrollTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5));

		private OneShotTimer autoScrollTimer = new OneShotTimer(TimeSpan.FromSeconds(0.10000000149011612));

		private OneShotTimer itemCountWaitScrollTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5));

		private OneShotTimer itemCountAutoScrollTimer = new OneShotTimer(TimeSpan.FromSeconds(0.10000000149011612));

		public InventoryItem SelectedItem
		{
			get
			{
				if (_selectedLocation.Y < 4)
				{
					return _hud.PlayerInventory.Inventory[_selectedLocation.X + _selectedLocation.Y * 8];
				}
				if (_selectedLocation.Y == 4)
				{
					return _hud.PlayerInventory.InventoryTray[_selectedLocation.X];
				}
				return null;
			}
			set
			{
				if (_selectedLocation.Y < 4)
				{
					_hud.PlayerInventory.Inventory[_selectedLocation.X + _selectedLocation.Y * 8] = value;
				}
				if (_selectedLocation.Y == 4)
				{
					_hud.PlayerInventory.InventoryTray[_selectedLocation.X] = value;
				}
			}
		}

		public override void OnPushed()
		{
			_selectedLocation = new Point(_game.GameScreen.HUD.PlayerInventory.SelectedInventoryIndex, 4);
			base.OnPushed();
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			if (_hud.LocalPlayer.Dead)
			{
				PopMe();
			}
			base.Update(game, gameTime);
		}

		public BlockPickerScreen(CastleMinerZGame game, InGameHUD hud)
			: base(true, false)
		{
			_hud = hud;
			_game = game;
			_bigFont = _game._medFont;
			_smallFont = _game._smallFont;
			_background = _game._uiSprites["BlockUIBack"];
			_gridSelector = _game._uiSprites["Selector"];
		}

		public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			spriteBatch.Begin();
			Rectangle destinationRectangle = new Rectangle(titleSafeArea.Center.X - _background.Width / 2, titleSafeArea.Center.Y - _background.Height / 2, _background.Width, _background.Height);
			SpriteFont smallFont = CastleMinerZGame.Instance._smallFont;
			_background.Draw(spriteBatch, destinationRectangle, Color.White);
			Vector2 vector = new Vector2(destinationRectangle.Left, destinationRectangle.Top);
			PlayerInventory playerInventory = _hud.PlayerInventory;
			Vector2 vector2 = new Vector2(404f, 334f);
			Vector2 vector3 = _bigFont.MeasureString("Split Items ");
			float y = vector3.Y;
			float num = y / (float)ControllerImages.Y.Height * (float)ControllerImages.A.Width;
			spriteBatch.Draw(ControllerImages.Y, new Rectangle((int)vector2.X, 160, (int)num, (int)y), Color.White);
			spriteBatch.DrawOutlinedText(_bigFont, " To Craft", new Vector2(vector2.X + num, 160f), Color.White, Color.Black, 2);
			spriteBatch.Draw(ControllerImages.X, new Rectangle((int)vector2.X, (int)(vector2.Y - vector3.Y - 10f), (int)num, (int)y), Color.White);
			spriteBatch.DrawOutlinedText(_bigFont, " Drop Item", new Vector2(vector2.X + num, vector2.Y - vector3.Y - 10f), Color.White, Color.Black, 2);
			spriteBatch.DrawOutlinedText(_bigFont, "Split Items ", new Vector2(vector2.X + 325f, vector2.Y - vector3.Y - 10f), Color.White, Color.Black, 2);
			spriteBatch.Draw(ControllerImages.RightThumstick, new Rectangle((int)(vector2.X + 325f + vector3.X), (int)(vector2.Y - vector3.Y - 10f), (int)num, (int)y), Color.White);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					InventoryItem inventoryItem = playerInventory.Inventory[i * 8 + j];
					if (inventoryItem != null && inventoryItem != _holdingItem && (_holdingItem == null || SelectedItem != inventoryItem))
					{
						Vector2 vector4 = vector2 + 59f * new Vector2(j, i);
						inventoryItem.Draw2D(spriteBatch, new Rectangle((int)vector4.X, (int)vector4.Y, 59, 59));
					}
				}
			}
			Vector2 vector5 = new Vector2(404f, 584f);
			for (int k = 0; k < 8; k++)
			{
				InventoryItem inventoryItem2 = playerInventory.InventoryTray[k];
				if (inventoryItem2 != null && inventoryItem2 != _holdingItem && (_holdingItem == null || SelectedItem != inventoryItem2))
				{
					Vector2 vector6 = vector5 + 59f * new Vector2(k, 0f);
					inventoryItem2.Draw2D(spriteBatch, new Rectangle((int)vector6.X, (int)vector6.Y, 59, 59));
				}
			}
			Vector2 position = ((_selectedLocation.Y >= 4) ? (vector5 + 59f * new Vector2(_selectedLocation.X, 0f)) : (vector2 + 59f * new Vector2(_selectedLocation.X, _selectedLocation.Y)));
			if (_holdingItem != null)
			{
				_holdingItem.Draw2D(spriteBatch, new Rectangle((int)position.X, (int)position.Y, 59, 59));
			}
			position -= new Vector2(4f, 4f);
			_gridSelector.Draw(spriteBatch, position, (_holdingItem == null) ? Color.White : Color.Red);
			Vector2 location = new Vector2(404f, vector.Y + 20f);
			InventoryItem inventoryItem3 = SelectedItem;
			if (_holdingItem != null)
			{
				inventoryItem3 = _holdingItem;
			}
			if (inventoryItem3 != null)
			{
				spriteBatch.DrawOutlinedText(_bigFont, inventoryItem3.Name, location, Color.White, Color.Black, 2);
				Vector2 location2 = new Vector2(location.X, location.Y + _bigFont.MeasureString(inventoryItem3.Name).Y);
				spriteBatch.DrawOutlinedText(_smallFont, inventoryItem3.Description1, location2, Color.White, Color.Black, 1);
				location2.Y += _smallFont.MeasureString(inventoryItem3.Description1).Y;
				spriteBatch.DrawOutlinedText(_smallFont, inventoryItem3.Description2, location2, Color.White, Color.Black, 1);
			}
			spriteBatch.End();
			base.Draw(device, spriteBatch, gameTime);
		}

		protected override void OnPlayerInput(GameController controller, GameTime gameTime)
		{
			if (controller.PressedButtons.A)
			{
				if (_holdingItem == null)
				{
					if (SelectedItem != null)
					{
						_holdingItem = SelectedItem;
						_hud.PlayerInventory.Remove(_holdingItem);
					}
					SoundManager.Instance.PlayInstance("Click");
				}
				else
				{
					InventoryItem selectedItem = SelectedItem;
					if (selectedItem != null && selectedItem.CanStack(_holdingItem))
					{
						selectedItem.Stack(_holdingItem);
						SelectedItem = selectedItem;
						if (_holdingItem.StackCount == 0)
						{
							_holdingItem = null;
						}
					}
					else
					{
						SelectedItem = _holdingItem;
						_holdingItem = selectedItem;
					}
					SoundManager.Instance.PlayInstance("Click");
				}
			}
			else if (controller.PressedButtons.RightStick)
			{
				if (_holdingItem == null)
				{
					if (SelectedItem != null)
					{
						SoundManager.Instance.PlayInstance("Click");
						if (SelectedItem.StackCount == 1)
						{
							_holdingItem = SelectedItem;
							_hud.PlayerInventory.Remove(_holdingItem);
						}
						else
						{
							_holdingItem = SelectedItem.Split();
						}
					}
				}
				else if (_holdingItem != null)
				{
					SoundManager.Instance.PlayInstance("Click");
					if (SelectedItem != null)
					{
						if (_holdingItem.ItemClass == SelectedItem.ItemClass)
						{
							if (SelectedItem.StackCount > 1)
							{
								InventoryItem item = SelectedItem.Split();
								_holdingItem.Stack(item);
								SelectedItem.Stack(item);
							}
							else if (_holdingItem.StackCount < _holdingItem.MaxStackCount)
							{
								_holdingItem.Stack(SelectedItem);
								SelectedItem = null;
							}
						}
						else
						{
							InventoryItem selectedItem2 = SelectedItem;
							SelectedItem = _holdingItem;
							_holdingItem = selectedItem2;
						}
					}
					else if (_holdingItem.StackCount > 1)
					{
						SelectedItem = _holdingItem.Split();
					}
					else
					{
						SelectedItem = _holdingItem;
						_holdingItem = null;
					}
				}
			}
			else if (controller.PressedButtons.Start)
			{
				_game.GameScreen.ShowInGameMenu();
				SoundManager.Instance.PlayInstance("Click");
			}
			else if (controller.PressedButtons.Back)
			{
				SoundManager.Instance.PlayInstance("Click");
				_holdingItem = null;
				PopMe();
			}
			else if (controller.PressedButtons.B)
			{
				SoundManager.Instance.PlayInstance("Click");
				if (_holdingItem != null)
				{
					_hud.PlayerInventory.AddInventoryItem(_holdingItem);
					_holdingItem = null;
				}
				else
				{
					PopMe();
				}
			}
			else if (controller.PressedButtons.X)
			{
				SoundManager.Instance.PlayInstance("Click");
				if (_holdingItem != null)
				{
					Vector3 localPosition = _game.LocalPlayer.LocalPosition;
					localPosition.Y += 1f;
					PickupManager.Instance.CreatePickup(_holdingItem, localPosition, true);
					SoundManager.Instance.PlayInstance("dropitem");
					_holdingItem = null;
				}
				else if (SelectedItem != null)
				{
					_hud.PlayerInventory.DropItem(SelectedItem);
				}
			}
			else if (controller.PressedButtons.Y)
			{
				if (_holdingItem != null)
				{
					_hud.PlayerInventory.AddInventoryItem(_holdingItem);
					_holdingItem = null;
				}
				SoundManager.Instance.PlayInstance("Click");
				_game.GameScreen.ShowCraftingScreen();
			}
			else if (controller.CurrentState.ThumbSticks.Right.Y < -0.2f && controller.LastState.ThumbSticks.Right.Y >= -0.2f)
			{
				itemCountWaitScrollTimer.Reset();
				itemCountAutoScrollTimer.Reset();
				if (ItemCountDown())
				{
					SoundManager.Instance.PlayInstance("Click");
				}
			}
			else if (controller.CurrentState.ThumbSticks.Right.Y > 0.2f && controller.LastState.ThumbSticks.Right.Y <= 0.2f)
			{
				itemCountWaitScrollTimer.Reset();
				itemCountAutoScrollTimer.Reset();
				if (ItemCountUp())
				{
					SoundManager.Instance.PlayInstance("Click");
				}
			}
			if (controller.PressedDPad.Down || (controller.CurrentState.ThumbSticks.Left.Y < -0.2f && controller.LastState.ThumbSticks.Left.Y >= -0.2f))
			{
				waitScrollTimer.Reset();
				autoScrollTimer.Reset();
				if (SelectDown())
				{
					SoundManager.Instance.PlayInstance("Click");
				}
			}
			if (controller.PressedDPad.Up || (controller.CurrentState.ThumbSticks.Left.Y > 0.2f && controller.LastState.ThumbSticks.Left.Y <= 0.2f))
			{
				waitScrollTimer.Reset();
				autoScrollTimer.Reset();
				if (SelectUp())
				{
					SoundManager.Instance.PlayInstance("Click");
				}
			}
			if (controller.PressedButtons.LeftShoulder || controller.PressedDPad.Left || (controller.CurrentState.ThumbSticks.Left.X < -0.2f && controller.LastState.ThumbSticks.Left.X >= -0.2f))
			{
				waitScrollTimer.Reset();
				autoScrollTimer.Reset();
				if (SelectLeft())
				{
					SoundManager.Instance.PlayInstance("Click");
				}
			}
			if (controller.PressedButtons.RightShoulder || controller.PressedDPad.Right || (controller.CurrentState.ThumbSticks.Left.X > 0.2f && controller.LastState.ThumbSticks.Left.X <= 0.2f))
			{
				waitScrollTimer.Reset();
				autoScrollTimer.Reset();
				if (SelectRight())
				{
					SoundManager.Instance.PlayInstance("Click");
				}
			}
			itemCountWaitScrollTimer.Update(gameTime.get_ElapsedGameTime());
			if (itemCountWaitScrollTimer.Expired)
			{
				if (controller.CurrentState.ThumbSticks.Right.Y < -0.2f)
				{
					itemCountAutoScrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (itemCountAutoScrollTimer.Expired)
					{
						itemCountAutoScrollTimer.Reset();
						if (ItemCountDown())
						{
							SoundManager.Instance.PlayInstance("Click");
						}
					}
				}
				else if (controller.CurrentState.ThumbSticks.Right.Y > 0.2f)
				{
					itemCountAutoScrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (itemCountAutoScrollTimer.Expired)
					{
						itemCountAutoScrollTimer.Reset();
						if (ItemCountUp())
						{
							SoundManager.Instance.PlayInstance("Click");
						}
					}
				}
			}
			waitScrollTimer.Update(gameTime.get_ElapsedGameTime());
			if (waitScrollTimer.Expired)
			{
				if (controller.CurrentState.ThumbSticks.Left.Y < -0.2f)
				{
					autoScrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (autoScrollTimer.Expired)
					{
						autoScrollTimer.Reset();
						if (SelectDown())
						{
							SoundManager.Instance.PlayInstance("Click");
						}
					}
				}
				else if (controller.CurrentState.ThumbSticks.Left.Y > 0.2f)
				{
					autoScrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (autoScrollTimer.Expired)
					{
						autoScrollTimer.Reset();
						if (SelectUp())
						{
							SoundManager.Instance.PlayInstance("Click");
						}
					}
				}
				else if (controller.CurrentState.ThumbSticks.Left.X < -0.2f)
				{
					autoScrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (autoScrollTimer.Expired)
					{
						autoScrollTimer.Reset();
						if (SelectLeft())
						{
							SoundManager.Instance.PlayInstance("Click");
						}
					}
				}
				else if (controller.CurrentState.ThumbSticks.Left.X > 0.2f)
				{
					autoScrollTimer.Update(gameTime.get_ElapsedGameTime());
					if (autoScrollTimer.Expired)
					{
						autoScrollTimer.Reset();
						if (SelectRight())
						{
							SoundManager.Instance.PlayInstance("Click");
						}
					}
				}
			}
			base.OnPlayerInput(controller, gameTime);
		}

		public override void OnPoped()
		{
			if (_holdingItem != null)
			{
				_hud.PlayerInventory.AddInventoryItem(_holdingItem);
				_holdingItem = null;
			}
			base.OnPoped();
		}

		public bool ItemCountDown()
		{
			if (_holdingItem == null)
			{
				return false;
			}
			if (SelectedItem == null)
			{
				if (_holdingItem.StackCount == 1)
				{
					SelectedItem = _holdingItem;
					_holdingItem = null;
				}
				else
				{
					SelectedItem = _holdingItem.PopOneItem();
				}
			}
			else
			{
				if (_holdingItem.ItemClass != SelectedItem.ItemClass || SelectedItem.StackCount >= SelectedItem.MaxStackCount)
				{
					return false;
				}
				if (_holdingItem.StackCount == 1)
				{
					SelectedItem.Stack(_holdingItem);
					_holdingItem = null;
				}
				else
				{
					SelectedItem.Stack(_holdingItem.PopOneItem());
				}
			}
			return true;
		}

		public bool ItemCountUp()
		{
			if (_holdingItem == null)
			{
				if (SelectedItem == null)
				{
					return false;
				}
				if (SelectedItem.StackCount == 1)
				{
					_holdingItem = SelectedItem;
					_hud.PlayerInventory.Remove(_holdingItem);
				}
				else
				{
					_holdingItem = SelectedItem.PopOneItem();
				}
			}
			else
			{
				if (SelectedItem == null)
				{
					return false;
				}
				if (_holdingItem.ItemClass != SelectedItem.ItemClass || _holdingItem.StackCount >= _holdingItem.MaxStackCount)
				{
					return false;
				}
				if (SelectedItem.StackCount == 1)
				{
					_holdingItem.Stack(SelectedItem);
					SelectedItem = null;
				}
				else
				{
					_holdingItem.Stack(SelectedItem.PopOneItem());
				}
			}
			return true;
		}

		public bool SelectDown()
		{
			_selectedLocation.Y++;
			if (_selectedLocation.Y > 4)
			{
				_selectedLocation.Y = 0;
			}
			return true;
		}

		public bool SelectUp()
		{
			_selectedLocation.Y--;
			if (_selectedLocation.Y < 0)
			{
				_selectedLocation.Y = 4;
			}
			return true;
		}

		public bool SelectLeft()
		{
			_selectedLocation.X--;
			if (_selectedLocation.X < 0)
			{
				_selectedLocation.X = 7;
			}
			return true;
		}

		public bool SelectRight()
		{
			_selectedLocation.X++;
			if (_selectedLocation.X > 7)
			{
				_selectedLocation.X = 0;
			}
			return true;
		}
	}
}
