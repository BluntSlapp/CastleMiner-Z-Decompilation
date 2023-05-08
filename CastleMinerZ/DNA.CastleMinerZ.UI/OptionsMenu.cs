using DNA.Drawing;
using DNA.Drawing.UI;
using DNA.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.UI
{
	internal class OptionsMenu : MenuScreen
	{
		private CastleMinerZGame _game;

		public DialogScreen _deleteStorageDialog;

		private ControllerScreen _controllerScreen;

		private SettingsMenu _settingsMenu;

		private WaitScreen optimizeStorageWaitScreen;

		private int OriginalWorldsCount;

		private int CurrentWorldsCount;

		private SpriteBatch SpriteBatch;

		private ScreenGroup _uiGroup;

		private bool Cancel;

		public OptionsMenu(CastleMinerZGame game, ScreenGroup uiGroup, SpriteBatch spriteBatch)
			: base(game._largeFont, Color.White, Color.Red, false)
		{
			SpriteBatch = spriteBatch;
			SpriteFont largeFont = game._largeFont;
			_uiGroup = uiGroup;
			_game = game;
			ClickSound = "Click";
			SelectSound = "Click";
			AddMenuItem("Controls", OptionsMenuItems.Controls);
			AddMenuItem("Erase Storage", OptionsMenuItems.EraseStorage);
			AddMenuItem("Optimize Storage", OptionsMenuItems.OptimizeStorage);
			AddMenuItem("Settings", OptionsMenuItems.Settings);
			_deleteStorageDialog = new DialogScreen("Erase Storage", "Are you sure you want to delete everything?", null, true, _game.DialogScreenImage, _game._medFont, true);
			_deleteStorageDialog.TitlePadding = new Vector2(55f, 15f);
			_deleteStorageDialog.DescriptionPadding = new Vector2(25f, 35f);
			_deleteStorageDialog.ButtonsPadding = new Vector2(25f, 20f);
			_deleteStorageDialog.ClickSound = "Click";
			_deleteStorageDialog.OpenSound = "Popup";
			_controllerScreen = new ControllerScreen(_game, false);
			_settingsMenu = new SettingsMenu(_game);
			base.MenuItemSelected += OptionsMenu_MenuItemSelected;
			optimizeStorageWaitScreen = new WaitScreen("Optimizing Storage...", true, DeleteWorlds, null);
			optimizeStorageWaitScreen.Updating += optimizeStorageWaitScreen_Updating;
			optimizeStorageWaitScreen.ProcessingPlayerInput += optimizeStorageWaitScreen_ProcessingPlayerInput;
			optimizeStorageWaitScreen.AfterDraw += optimizeStorageWaitScreen_AfterDraw;
		}

		private void optimizeStorageWaitScreen_AfterDraw(object sender, DrawEventArgs e)
		{
			Vector2 vector = _game._largeFont.MeasureString(" Cancel");
			float num = vector.Y / (float)ControllerImages.B.Height;
			float num2 = (int)((float)ControllerImages.B.Width * num);
			int num3 = (int)((float)e.Device.Viewport.TitleSafeArea.Bottom - vector.Y);
			int num4 = (int)((float)e.Device.Viewport.TitleSafeArea.Right - num2 - vector.X);
			SpriteBatch.Begin();
			SpriteBatch.Draw(ControllerImages.B, new Rectangle(num4, num3, (int)num2, (int)vector.Y), Color.White);
			SpriteBatch.DrawOutlinedText(_game._largeFont, " Cancel", new Vector2((float)num4 + num2, num3), Color.White, Color.Black, 1);
			SpriteBatch.End();
		}

		private void optimizeStorageWaitScreen_ProcessingPlayerInput(object sender, ControllerInputEventArgs e)
		{
			if (e.Controller.PressedButtons.B || e.Controller.PressedButtons.Back)
			{
				Cancel = true;
				optimizeStorageWaitScreen.Message = "Canceling...";
				optimizeStorageWaitScreen._drawProgress = false;
			}
		}

		private void optimizeStorageWaitScreen_Updating(object sender, UpdateEventArgs e)
		{
			float num = ((OriginalWorldsCount <= 0) ? 1f : (1f - (float)CurrentWorldsCount / (float)OriginalWorldsCount));
			optimizeStorageWaitScreen.Progress = (int)(100f * num);
		}

		private void OptionsMenu_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			switch ((OptionsMenuItems)e.MenuItem.Tag)
			{
			case OptionsMenuItems.Controls:
				_uiGroup.PushScreen(_controllerScreen);
				break;
			case OptionsMenuItems.EraseStorage:
				_uiGroup.ShowDialogScreen(_deleteStorageDialog, delegate
				{
					if (_deleteStorageDialog.OptionSelected != -1)
					{
						WaitScreen.DoWait(_uiGroup, "Deleting Storage...", delegate
						{
							_game.SaveDevice.DeleteStorage();
						}, null);
					}
				});
				break;
			case OptionsMenuItems.OptimizeStorage:
			{
				Cancel = false;
				WorldInfo[] worlds = _game.FrontEnd.WorldManager.GetWorlds();
				OriginalWorldsCount = 0;
				for (int i = 0; i < worlds.Length; i++)
				{
					string gamertag = Screen.CurrentGamer.Gamertag;
					if (worlds[i].OwnerGamerTag != gamertag)
					{
						OriginalWorldsCount++;
					}
				}
				CurrentWorldsCount = OriginalWorldsCount;
				optimizeStorageWaitScreen.Progress = 0;
				optimizeStorageWaitScreen.Start(_uiGroup);
				break;
			}
			case OptionsMenuItems.Settings:
				_uiGroup.PushScreen(_settingsMenu);
				break;
			}
		}

		private void DeleteWorlds()
		{
			WorldInfo[] worlds = _game.FrontEnd.WorldManager.GetWorlds();
			for (int i = 0; i < worlds.Length; i++)
			{
				string gamertag = Screen.CurrentGamer.Gamertag;
				if (worlds[i].OwnerGamerTag != gamertag)
				{
					_game.FrontEnd.WorldManager.Delete(worlds[i]);
					CurrentWorldsCount--;
				}
				if (Cancel)
				{
					break;
				}
			}
			_game.SaveDevice.Flush();
		}
	}
}
