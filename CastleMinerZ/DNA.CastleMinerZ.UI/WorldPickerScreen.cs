using System;
using System.Text;
using DNA.Drawing;
using DNA.Drawing.UI;
using DNA.Input;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	public class WorldPickerScreen : LongListScreen
	{
		public class WorldPickerMenuItem : MenuItem
		{
			private SpriteFont _largeFont;

			private SpriteFont _medFont;

			private SpriteFont _smallFont;

			public Color Color = Color.White;

			public Color SelectedColor = Color.Red;

			private StringBuilder sbuilder = new StringBuilder();

			private OneShotTimer flashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.25));

			private bool selectedDirection;

			public WorldInfo world
			{
				get
				{
					return (WorldInfo)Tag;
				}
			}

			private string Name
			{
				get
				{
					return world.Name;
				}
			}

			private string Creator
			{
				get
				{
					return world.CreatorGamerTag;
				}
			}

			public DateTime LastPlayed
			{
				get
				{
					return world.LastPlayedDate;
				}
			}

			public WorldPickerMenuItem(SpriteFont largeFont, SpriteFont medFont, SpriteFont smallFont, string gamerTag, WorldInfo world)
				: base(world)
			{
				_largeFont = largeFont;
				_smallFont = smallFont;
				_medFont = medFont;
				if (world.OwnerGamerTag != gamerTag)
				{
					Color = Color.Gray;
				}
			}

			public override Vector2 Measure()
			{
				string name = Name;
				sbuilder.Length = 0;
				sbuilder.Append(LastPlayed.ToString("g")).Append(" - Created By: ").Append(Creator);
				SpriteFont spriteFont;
				SpriteFont spriteFont2;
				if (Selected)
				{
					spriteFont = _largeFont;
					spriteFont2 = _medFont;
				}
				else
				{
					spriteFont = _medFont;
					spriteFont2 = _smallFont;
				}
				Vector2 vector = spriteFont.MeasureString(name);
				Vector2 vector2 = spriteFont2.MeasureString(sbuilder);
				return new Vector2((vector2.X > vector.X) ? vector2.X : vector.X, vector.Y + vector2.Y);
			}

			public override void Draw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime, Vector2 pos)
			{
				Color textColor = Color;
				SpriteFont spriteFont;
				SpriteFont spriteFont2;
				if (Selected)
				{
					spriteFont = _largeFont;
					spriteFont2 = _medFont;
					flashTimer.Update(gameTime.get_ElapsedGameTime());
					if (flashTimer.Expired)
					{
						flashTimer.Reset();
						selectedDirection = !selectedDirection;
					}
					textColor = ((!selectedDirection) ? Color.Lerp(SelectedColor, Color, flashTimer.PercentComplete) : Color.Lerp(Color, SelectedColor, flashTimer.PercentComplete));
				}
				else
				{
					spriteFont = _medFont;
					spriteFont2 = _smallFont;
				}
				string name = Name;
				sbuilder.Length = 0;
				sbuilder.Append(LastPlayed.ToString("g")).Append(" - Created By: ").Append(Creator);
				Vector2 vector = spriteFont.MeasureString(name);
				spriteFont2.MeasureString(sbuilder);
				spriteBatch.DrawOutlinedText(spriteFont, name, pos, textColor, Color.Black, 1);
				spriteBatch.DrawOutlinedText(spriteFont2, sbuilder, new Vector2(pos.X, pos.Y + vector.Y), textColor, Color.Black, 1);
			}
		}

		private CastleMinerZGame _game;

		private DialogScreen _deleteWorldDialog;

		public DialogScreen _takeOverTerrain;

		public DialogScreen _infiniteModeConversion;

		private ScreenGroup _UiGroup;

		private WorldManager WorldManager
		{
			get
			{
				return _game.FrontEnd.WorldManager;
			}
		}

		public WorldPickerScreen(CastleMinerZGame game, ScreenGroup uiGroup)
			: base(false)
		{
			_game = game;
			SelectSound = "Click";
			ClickSound = "Click";
			_deleteWorldDialog = new DialogScreen("Delete World", "Are you sure you want to delete this world?", null, true, _game.DialogScreenImage, _game._medFont, true);
			_deleteWorldDialog.TitlePadding = new Vector2(55f, 15f);
			_deleteWorldDialog.DescriptionPadding = new Vector2(25f, 35f);
			_deleteWorldDialog.ButtonsPadding = new Vector2(25f, 20f);
			_deleteWorldDialog.ClickSound = "Click";
			_deleteWorldDialog.OpenSound = "Popup";
			_takeOverTerrain = new DialogScreen("Take Over World", "This world belongs to someone else. Would you like to make your own copy?\n\nYou will be able to make changes locally and host it yourself.", null, true, _game.DialogScreenImage, _game._medFont, true);
			_takeOverTerrain.TitlePadding = new Vector2(55f, 15f);
			_takeOverTerrain.DescriptionPadding = new Vector2(25f, 35f);
			_takeOverTerrain.ButtonsPadding = new Vector2(25f, 20f);
			_takeOverTerrain.ClickSound = "Click";
			_takeOverTerrain.OpenSound = "Popup";
			_infiniteModeConversion = new DialogScreen("Creative Mode", "Are you sure you want to play this world in Creative Mode? You will not be able to load it in normal mode again.", null, true, _game.DialogScreenImage, _game._medFont, true);
			_infiniteModeConversion.TitlePadding = new Vector2(55f, 15f);
			_infiniteModeConversion.DescriptionPadding = new Vector2(25f, 35f);
			_infiniteModeConversion.ButtonsPadding = new Vector2(25f, 20f);
			_infiniteModeConversion.ClickSound = "Click";
			_infiniteModeConversion.OpenSound = "Popup";
			_UiGroup = uiGroup;
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			SpriteFont largeFont = _game._largeFont;
			Viewport viewport = device.Viewport;
			Rectangle rectangle = (destRect = new Rectangle(viewport.TitleSafeArea.Left, viewport.TitleSafeArea.Top + largeFont.LineSpacing * 2, viewport.TitleSafeArea.Width, viewport.TitleSafeArea.Height - largeFont.LineSpacing * 2));
			spriteBatch.Begin();
			if (base.MenuItems.Count == 0)
			{
				string text = "No Worlds Found";
				Vector2 vector = largeFont.MeasureString(text);
				int lineSpacing = largeFont.LineSpacing;
				spriteBatch.DrawOutlinedText(largeFont, text, new Vector2((float)viewport.TitleSafeArea.Center.X - vector.X / 2f, (float)viewport.TitleSafeArea.Center.Y - vector.Y / 2f), Color.White, Color.Black, 2);
			}
			else
			{
				string text = "Choose a World";
				Vector2 vector = largeFont.MeasureString(text);
				int lineSpacing2 = largeFont.LineSpacing;
				spriteBatch.DrawOutlinedText(largeFont, text, new Vector2((float)(viewport.Width / 2) - vector.X / 2f, viewport.TitleSafeArea.Top), Color.White, Color.Black, 2);
			}
			Vector2 vector2 = _game._medFont.MeasureString(" Delete World");
			Vector2 vector3 = _game._medFont.MeasureString(" Rename World");
			float num = vector2.Y / (float)ControllerImages.X.Height;
			int num2 = (int)((float)ControllerImages.X.Width * num);
			int num3 = (int)((float)viewport.TitleSafeArea.Bottom - vector2.Y - vector3.Y);
			int num4 = (int)((float)(viewport.TitleSafeArea.Right - num2) - vector3.X);
			spriteBatch.Draw(ControllerImages.X, new Rectangle(num4, num3, num2, (int)vector2.Y), Color.White);
			spriteBatch.DrawOutlinedText(_game._medFont, " Delete World", new Vector2(num4 + num2, num3), Color.White, Color.Black, 1);
			num = vector3.Y / (float)ControllerImages.Y.Height;
			num2 = (int)((float)ControllerImages.Y.Width * num);
			num3 = (int)((float)viewport.TitleSafeArea.Bottom - vector3.Y);
			num4 = (int)((float)(viewport.TitleSafeArea.Right - num2) - vector3.X);
			spriteBatch.Draw(ControllerImages.Y, new Rectangle(num4, num3, num2, (int)vector2.Y), Color.White);
			spriteBatch.DrawOutlinedText(_game._medFont, " Rename World", new Vector2(num4 + num2, num3), Color.White, Color.Black, 1);
			spriteBatch.End();
			base.OnDraw(device, spriteBatch, gameTime);
		}

		protected override void OnPlayerInput(GameController controller, GameTime gameTime)
		{
			if (controller.PressedButtons.X && base.SelectedItem is WorldPickerMenuItem)
			{
				_UiGroup.ShowDialogScreen(_deleteWorldDialog, delegate
				{
					if (_deleteWorldDialog.OptionSelected != -1)
					{
						WorldPickerMenuItem selected = (WorldPickerMenuItem)base.SelectedItem;
						WaitScreen.DoWait(_UiGroup, "Deleting World...", delegate
						{
							WorldManager.Delete(selected.world);
							base.MenuItems.Remove(selected);
							_game.SaveDevice.Flush();
						}, null);
					}
				});
			}
			else if (controller.PressedButtons.Y && base.SelectedItem is WorldPickerMenuItem)
			{
				WorldPickerMenuItem worldPickerMenuItem = (WorldPickerMenuItem)base.SelectedItem;
				RenameWorld(worldPickerMenuItem.world);
			}
			base.OnPlayerInput(controller, gameTime);
		}

		public override void OnPoped()
		{
			base.MenuItems.Clear();
			base.OnPoped();
		}

		protected override void OnBack()
		{
			PopMe();
			base.OnBack();
		}

		private void Populate()
		{
			base.MenuItems.Clear();
			TextItem textItem = new TextItem("Create World", _game._largeFont, null);
			textItem.Color = new Color(294, 190, 88);
			base.MenuItems.Add(textItem);
			WorldInfo[] worlds = WorldManager.GetWorlds();
			WorldInfo[] array = worlds;
			foreach (WorldInfo worldInfo in array)
			{
				if (worldInfo.InfiniteResourceMode == _game.InfiniteResourceMode || _game.InfiniteResourceMode)
				{
					base.MenuItems.Add(new WorldPickerMenuItem(_game._largeFont, _game._medFont, _game._smallFont, Screen.CurrentGamer.Gamertag, worldInfo));
				}
			}
			base.MenuItems.Sort(SortWorlds);
		}

		public static int SortWorlds(MenuItem a, MenuItem b)
		{
			if (a == null)
			{
				if (b == null)
				{
					return 0;
				}
				return -1;
			}
			if (b == null)
			{
				return 1;
			}
			if (a is WorldPickerMenuItem && b is WorldPickerMenuItem)
			{
				WorldPickerMenuItem worldPickerMenuItem = (WorldPickerMenuItem)a;
				WorldPickerMenuItem worldPickerMenuItem2 = (WorldPickerMenuItem)b;
				if (worldPickerMenuItem.Color == Color.White && worldPickerMenuItem2.Color == Color.Gray)
				{
					return -1;
				}
				if (worldPickerMenuItem.Color == Color.Gray && worldPickerMenuItem2.Color == Color.White)
				{
					return 1;
				}
				if (worldPickerMenuItem.LastPlayed > worldPickerMenuItem2.LastPlayed)
				{
					return -1;
				}
				if (worldPickerMenuItem.LastPlayed < worldPickerMenuItem2.LastPlayed)
				{
					return 1;
				}
				return 0;
			}
			if (a is WorldPickerMenuItem && !(b is WorldPickerMenuItem))
			{
				return 1;
			}
			if (!(a is WorldPickerMenuItem) && b is WorldPickerMenuItem)
			{
				return -1;
			}
			return 0;
		}

		public override void OnPushed()
		{
			Populate();
			base.OnPushed();
		}

		private void RenameWorld(WorldInfo world)
		{
			Guide.BeginShowKeyboardInput(Screen.SelectedPlayerIndex.Value, "Rename " + world.Name, "Enter a New Name", world.Name, (AsyncCallback)delegate(IAsyncResult result)
			{
				string text = Guide.EndShowKeyboardInput(result);
				if (text != null)
				{
					if (text.Length > 25)
					{
						world.Name = text.Substring(0, 25);
					}
					else
					{
						world.Name = text;
					}
					world.SaveToStorage(Screen.CurrentGamer, CastleMinerZGame.Instance.SaveDevice);
					Populate();
				}
			}, (object)null);
		}
	}
}
