using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using DNA.CastleMinerZ.UI;
using DNA.Drawing;
using DNA.Drawing.UI;
using DNA.Input;
using DNA.IO.Storage;
using DNA.Security.Cryptography;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ
{
	public class FrontEndScreen : ScreenGroup
	{
		public CastleMinerZGame _game;

		public ScreenGroup _uiGroup = new ScreenGroup(true);

		public Screen _adScreen = new Screen(true, false);

		public Screen _upSellScreen = new Screen(true, false);

		private WorldPickerScreen _worldPickerScreen;

		private Screen _connectingScreen = new Screen(true, false);

		private Screen _loadingScreen = new Screen(true, false);

		private MainMenu _mainMenu;

		private PlayGameTypeMenu _playGameTypeMenu;

		private OptionsMenu _optionsMenu;

		private AchievementScreen<CastleMinerZPlayerStats> _achievementScreen;

		private ConnectMenu _chooseModeMenu;

		private GameModeMenu _gameModeMenu;

		private DialogScreen _undeadNotKilledDialog;

		private DialogScreen _creativeNotUnlockedDialog;

		private DifficultyLevelScreen _difficultyLevelScreen;

		private ChooseSessionScreen _chooseSessionScreen;

		private SinglePlayerStartScreen _startScreen = new SinglePlayerStartScreen(false);

		private Screen _chooseAnotherGameScreen = new Screen(true, false);

		private SpriteBatch SpriteBatch;

		public SpriteFont _largeFont;

		private PromoCode.PromoCodeManager _promoManager;

		private CheatCode.CheatCodeManager _cheatcodeManager;

		public WorldManager WorldManager;

		private Texture2D _UpSellImg;

		private Texture2D _Advert;

		private bool _localGame;

		private string _versionString;

		private OneShotTimer textFlashTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5), true);

		public PromoCode CastleMinerPromoCode;

		public PromoCode[] PromoCodes = new PromoCode[9];

		public CheatCode[] CheatCodes = new CheatCode[1];

		public FrontEndScreen(CastleMinerZGame game)
			: base(false)
		{
			_versionString = "Version " + game.Version.ToString();
			_Advert = game.Content.Load<Texture2D>("Advert");
			_UpSellImg = game.Content.Load<Texture2D>("Upsell");
			_game = game;
			_largeFont = game._largeFont;
			SpriteBatch = new SpriteBatch(game.GraphicsDevice);
			MenuBackdropScreen screen = new MenuBackdropScreen(game);
			PushScreen(screen);
			PushScreen(_uiGroup);
			_uiGroup.PushScreen(_adScreen);
			_uiGroup.PushScreen(_upSellScreen);
			_uiGroup.PushScreen(_startScreen);
			_startScreen.ClickSound = "Click";
			_startScreen.OnStartPressed += _startScreen_OnStartPressed;
			_startScreen.AfterDraw += _startScreen_AfterDraw;
			_mainMenu = new MainMenu(game);
			_mainMenu.MenuItemSelected += _mainMenu_MenuItemSelected;
			_playGameTypeMenu = new PlayGameTypeMenu(game);
			_playGameTypeMenu.MenuItemSelected += _playGameTypeMenu_MenuItemSelected;
			_optionsMenu = new OptionsMenu(game, _uiGroup, SpriteBatch);
			_adScreen.AfterDraw += _adScreen_AfterDraw;
			_adScreen.ProcessingInput += _adScreen_ProcessingInput;
			_upSellScreen.AfterDraw += _upSellScreen_AfterDraw;
			_upSellScreen.ProcessingInput += _upSellScreen_ProcessingInput;
			_upSellScreen.Updating += _upSellScreen_Updating;
			_chooseModeMenu = new ConnectMenu(game);
			_chooseModeMenu.MenuItemSelected += _chooseModeMenu_MenuItemSelected;
			_gameModeMenu = new GameModeMenu(game);
			_gameModeMenu.MenuItemSelected += _gameModeMenu_MenuItemSelected;
			_undeadNotKilledDialog = new DialogScreen("Kill Undead Dragon", "Unlock this game mode by killing the Undead Dragon in Endurance Mode", null, false, _game.DialogScreenImage, _game._medFont, true);
			_undeadNotKilledDialog.TitlePadding = new Vector2(55f, 15f);
			_undeadNotKilledDialog.DescriptionPadding = new Vector2(25f, 35f);
			_undeadNotKilledDialog.ButtonsPadding = new Vector2(25f, 20f);
			_undeadNotKilledDialog.ClickSound = "Click";
			_undeadNotKilledDialog.OpenSound = "Popup";
			_creativeNotUnlockedDialog = new DialogScreen("Unlock Creative Mode", "Unlock this mode with the promo code from the original CastleMiner game", null, false, _game.DialogScreenImage, _game._medFont, true);
			_creativeNotUnlockedDialog.TitlePadding = new Vector2(55f, 15f);
			_creativeNotUnlockedDialog.DescriptionPadding = new Vector2(25f, 35f);
			_creativeNotUnlockedDialog.ButtonsPadding = new Vector2(25f, 20f);
			_creativeNotUnlockedDialog.ClickSound = "Click";
			_creativeNotUnlockedDialog.OpenSound = "Popup";
			_difficultyLevelScreen = new DifficultyLevelScreen(game);
			_difficultyLevelScreen.MenuItemSelected += _difficultyLevelScreen_MenuItemSelected;
			_connectingScreen.BeforeDraw += _connectingScreen_BeforeDraw;
			_chooseSessionScreen = new ChooseSessionScreen(game);
			_chooseSessionScreen.Clicked += _chooseSessionScreen_Clicked;
			_worldPickerScreen = new WorldPickerScreen(game, _uiGroup);
			_worldPickerScreen.Clicked += _worldPickerScreen_Clicked;
			_chooseAnotherGameScreen.BeforeDraw += _chooseAnotherGameScreen_BeforeDraw;
			_chooseAnotherGameScreen.ProcessingPlayerInput += _chooseAnotherGameScreen_ProcessingPlayerInput;
			_loadingScreen.BeforeDraw += _loadingScreen_BeforeDraw;
		}

		private void _playGameTypeMenu_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			switch ((PlayGameTypeMenuItem)e.MenuItem.Tag)
			{
			case PlayGameTypeMenuItem.Online:
				if (Guide.IsTrialMode)
				{
					_game.ShowMarketPlace(Screen.SelectedPlayerIndex.Value);
				}
				else if (Screen.CurrentGamer.Privileges.AllowOnlineSessions)
				{
					_localGame = false;
					_uiGroup.PushScreen(_gameModeMenu);
				}
				else
				{
					ShowUIDialog("XBox Live Gold Account Required", "This gamer is not permitted to play online games, or is not signed into Xbox Live.\n\n Please sign in with a gamer that has privilages to play online, or play a local game.", true);
				}
				break;
			case PlayGameTypeMenuItem.Offline:
				_localGame = true;
				_uiGroup.PushScreen(_gameModeMenu);
				break;
			}
		}

		private void _difficultyLevelScreen_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			GameDifficultyTypes difficulty = (GameDifficultyTypes)e.MenuItem.Tag;
			_game.Difficulty = difficulty;
			_uiGroup.PushScreen(_worldPickerScreen);
		}

		private void _gameModeMenu_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			GameModeTypes gameModeTypes = (GameModeTypes)e.MenuItem.Tag;
			_game.GameMode = gameModeTypes;
			_game.InfiniteResourceMode = false;
			_game.Difficulty = GameDifficultyTypes.EASY;
			if (_localGame)
			{
				switch (gameModeTypes)
				{
				case GameModeTypes.Endurance:
					startWorld();
					break;
				case GameModeTypes.DragonEndurance:
					if (Guide.IsTrialMode)
					{
						_game.ShowMarketPlace(Screen.SelectedPlayerIndex.Value);
					}
					else if (_game.PlayerStats.UndeadDragonKills > 0 || PromoCodes[4].Redeemed || _game.PlayerStats.v1Player)
					{
						_uiGroup.PushScreen(_worldPickerScreen);
					}
					else
					{
						_uiGroup.ShowDialogScreen(_undeadNotKilledDialog, delegate
						{
						});
					}
					break;
				case GameModeTypes.Survival:
					if (Guide.IsTrialMode)
					{
						_game.ShowMarketPlace(Screen.SelectedPlayerIndex.Value);
					}
					else
					{
						_uiGroup.PushScreen(_difficultyLevelScreen);
					}
					break;
				case GameModeTypes.Creative:
					_game.GameMode = GameModeTypes.Survival;
					_game.InfiniteResourceMode = true;
					if (Guide.IsTrialMode)
					{
						_game.ShowMarketPlace();
					}
					else if (PromoCodes[5].Redeemed)
					{
						_uiGroup.PushScreen(_difficultyLevelScreen);
					}
					else
					{
						_uiGroup.ShowDialogScreen(_creativeNotUnlockedDialog, delegate
						{
						});
					}
					break;
				}
			}
			else if (gameModeTypes == GameModeTypes.Creative)
			{
				_game.GameMode = GameModeTypes.Survival;
				_game.InfiniteResourceMode = true;
				if (Guide.IsTrialMode)
				{
					_game.ShowMarketPlace();
				}
				else if (PromoCodes[5].Redeemed)
				{
					_uiGroup.PushScreen(_chooseModeMenu);
				}
				else
				{
					_uiGroup.ShowDialogScreen(_creativeNotUnlockedDialog, delegate
					{
					});
				}
			}
			else if (_game.GameMode == GameModeTypes.DragonEndurance)
			{
				if (_game.PlayerStats.UndeadDragonKills > 0 || PromoCodes[4].Redeemed || _game.PlayerStats.v1Player)
				{
					_uiGroup.PushScreen(_chooseModeMenu);
				}
				else
				{
					_uiGroup.ShowDialogScreen(_undeadNotKilledDialog, delegate
					{
					});
				}
			}
			else
			{
				_uiGroup.PushScreen(_chooseModeMenu);
			}
		}

		private void startWorld()
		{
			WorldTypeIDs terrainVersion = _game.CurrentWorld._terrainVersion;
			if (!Guide.IsTrialMode)
			{
				WorldManager.TakeOwnership(_game.CurrentWorld);
			}
			_game.CurrentWorld._terrainVersion = WorldTypeIDs.CastleMinerZ;
			if (terrainVersion != _game.CurrentWorld._terrainVersion)
			{
				_game.BeginLoadTerrain(_game.CurrentWorld, true);
			}
			HostGame(_localGame);
		}

		public void ShowUIDialog(string title, string message, bool drawbehind)
		{
			DialogScreen dialogScreen = new DialogScreen(title, message, null, false, _game.DialogScreenImage, _game._medFont, drawbehind);
			dialogScreen.TitlePadding = new Vector2(55f, 15f);
			dialogScreen.DescriptionPadding = new Vector2(25f, 35f);
			dialogScreen.ButtonsPadding = new Vector2(25f, 20f);
			dialogScreen.ClickSound = "Click";
			dialogScreen.OpenSound = "Popup";
			_uiGroup.ShowDialogScreen(dialogScreen, null);
		}

		private void JoinCallback(bool success)
		{
			if (success)
			{
				_game.GetWorldInfo(delegate(WorldInfo worldInfo)
				{
					_uiGroup.PopScreen();
					WorldManager.RegisterNetworkWorld(worldInfo);
					_game.BeginLoadTerrain(worldInfo, false);
					_uiGroup.PushScreen(_loadingScreen);
					_game.WaitForTerrainLoad(delegate
					{
						_uiGroup.PopScreen();
						_game.StartGame();
					});
				});
			}
			else
			{
				PopToMainMenu(Screen.CurrentGamer, null);
				ShowUIDialog("Connection Error", "There was an error connecting.", false);
			}
		}

		public void JoinInvitedGame()
		{
			_uiGroup.PushScreen(_connectingScreen);
			_game.JoinInvitedGame(new SignedInGamer[1] { Screen.CurrentGamer }, JoinCallback);
		}

		private void JoinGame(AvailableNetworkSession session)
		{
			_uiGroup.PushScreen(_connectingScreen);
			_game.JoinGame(session, JoinCallback);
		}

		private void _chooseSessionScreen_Clicked(object sender, SelectedEventArgs e)
		{
			AvailableNetworkSession session = (AvailableNetworkSession)e.Tag;
			JoinGame(session);
		}

		private void _startScreen_AfterDraw(object sender, DrawEventArgs e)
		{
			Rectangle titleSafeArea = e.Device.Viewport.TitleSafeArea;
			float num = (float)e.Device.Viewport.Height / 1080f;
			SpriteBatch.Begin();
			_game.Logo.Draw(SpriteBatch, new Vector2(titleSafeArea.Center.X - _game.Logo.Width / 2, titleSafeArea.Center.Y - _game.Logo.Height / 2), Color.White);
			string text = "www.CastleMinerZ.com";
			Vector2 vector = _game._medFont.MeasureString(text);
			SpriteBatch.DrawOutlinedText(_game._medFont, text, new Vector2((float)titleSafeArea.Center.X - vector.X / 2f, (float)titleSafeArea.Bottom - vector.Y), Color.White, Color.Black, 1);
			vector = _largeFont.MeasureString("Press Start");
			SpriteBatch.DrawOutlinedText(_largeFont, "Press Start", new Vector2((float)titleSafeArea.Center.X - vector.X / 2f, titleSafeArea.Center.Y + _game.Logo.Height / 2), Color.White, Color.Black, 1);
			SpriteBatch.DrawOutlinedText(_game._consoleFont, _versionString, new Vector2(titleSafeArea.Left, titleSafeArea.Top), Color.White, Color.Black, 1);
			SpriteBatch.End();
		}

		private void _startScreen_OnStartPressed(object sender, EventArgs e)
		{
			SignedInGamer currentGamer = Screen.CurrentGamer;
			if (currentGamer == null)
			{
				if (!Guide.IsVisible)
				{
					Guide.ShowSignIn(1, false);
				}
				return;
			}
			SetupSaveDevice(delegate(bool success)
			{
				if (success)
				{
					WaitScreen.DoWait(_uiGroup, "Loading Player Info...", delegate
					{
						if (Screen.CurrentGamer != null)
						{
							SetupNewGamer(Screen.CurrentGamer, _game.SaveDevice);
							if (Screen.CurrentGamer != null)
							{
								_uiGroup.PushScreen(_mainMenu);
							}
						}
					}, null);
				}
			});
		}

		private void CloseSaveDevice()
		{
			if (_game.SaveDevice != null)
			{
				((Collection<IGameComponent>)(object)_game.Components).Remove((IGameComponent)_game.SaveDevice);
				_game.SaveDevice.Dispose();
				_game.SaveDevice = null;
			}
		}

		private void SetupSaveDevice(SuccessCallback callback)
		{
			WaitScreen waitScreen = new WaitScreen("Opening Storage Device");
			_uiGroup.PushScreen(waitScreen);
			CloseSaveDevice();
			SHA256HashProvider sHA256HashProvider = new SHA256HashProvider();
			byte[] data = sHA256HashProvider.Compute(Encoding.UTF8.GetBytes(Screen.CurrentGamer.Gamertag + "CMZ777")).Data;
			MD5HashProvider mD5HashProvider = new MD5HashProvider();
			byte[] data2 = mD5HashProvider.Compute(Encoding.UTF8.GetBytes(Screen.CurrentGamer.Gamertag + "CMZ778")).Data;
			_game.SaveDevice = new PlayerSaveDevice(Screen.SelectedPlayerIndex.Value, "CastleMiner Z Save", data, data2);
			_game.SaveDevice.PromptForReselect = false;
			_game.SaveDevice.DeviceDisconnected += SaveDevice_DeviceDisconnected;
			((Collection<IGameComponent>)(object)_game.Components).Add((IGameComponent)_game.SaveDevice);
			_game.SaveDevice.PromptForDevice(delegate(bool success)
			{
				callback(success);
				waitScreen.PopMe();
			});
		}

		private void SaveDevice_DeviceDisconnected(object sender, SaveDeviceEventArgs e)
		{
			_game.EndGame(true);
			_game.FrontEnd.PopToStartScreen();
			_uiGroup.PushScreen(_mainMenu);
			WaitScreen.DoWait(_uiGroup, "Loading Player Info...", delegate
			{
				SetupNewGamer(Screen.CurrentGamer, _game.SaveDevice);
			}, null);
		}

		public void BeginSetupNewGamer(SignedInGamer gamer)
		{
			WorldManager = null;
			_game.SetupNewGamer(gamer);
		}

		public void EndSetupNewGamer(SignedInGamer gamer, SaveDevice saveDevice)
		{
			InitPromoCodes(gamer, saveDevice);
			WorldManager = new WorldManager(gamer, saveDevice);
		}

		public void SetupNewGamer(SignedInGamer gamer, SaveDevice saveDevice)
		{
			BeginSetupNewGamer(gamer);
			EndSetupNewGamer(gamer, saveDevice);
		}

		private void HostGame(bool local)
		{
			_uiGroup.PushScreen(_loadingScreen);
			_game.WaitForTerrainLoad(delegate
			{
				_uiGroup.PopScreen();
				_uiGroup.PushScreen(_connectingScreen);
				_game.HostGame(local, delegate(bool result)
				{
					if (result)
					{
						_game.TerrainServerID = _game.MyNetworkGamer.Id;
						_game.StartGame();
					}
					else
					{
						_uiGroup.PopScreen();
						ShowUIDialog("Hosting Error", "There was an error hosting the game.", false);
					}
				});
			});
		}

		private void _mainMenu_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			if (WorldManager == null || !_game.IsAvatarLoaded)
			{
				return;
			}
			switch ((MainMenuItems)e.MenuItem.Tag)
			{
			case MainMenuItems.Play:
				_uiGroup.PushScreen(_playGameTypeMenu);
				break;
			case MainMenuItems.Redeem:
				if (Guide.IsTrialMode)
				{
					_game.ShowMarketPlace();
					break;
				}
				Guide.BeginShowKeyboardInput(Screen.SelectedPlayerIndex.Value, "Enter Promo Code", "Enter Your Promotional Code...", "", (AsyncCallback)delegate(IAsyncResult result)
				{
					string text = Guide.EndShowKeyboardInput(result);
					if (text != null)
					{
						string reason;
						CheatCode cheatCode = _cheatcodeManager.Redeem(text, out reason);
						while (Guide.IsVisible)
						{
							Thread.Sleep(250);
						}
						if (cheatCode != null)
						{
							Guide.BeginShowMessageBox("Code Redeemed", "You Have Unlocked:\n\n" + cheatCode.Reward, (IEnumerable<string>)new string[1] { "Ok" }, 0, MessageBoxIcon.None, (AsyncCallback)delegate
							{
							}, (object)null);
						}
						else
						{
							PromoCode promoCode = _promoManager.Redeem(text, out reason);
							while (Guide.IsVisible)
							{
								Thread.Sleep(250);
							}
							if (promoCode != null)
							{
								CodeRedeemed((PromoCodeIds)promoCode.Tag);
								Guide.BeginShowMessageBox("Code Redeemed", "You Have Unlocked:\n\n" + promoCode.Reward, (IEnumerable<string>)new string[1] { "Ok" }, 0, MessageBoxIcon.None, (AsyncCallback)delegate
								{
								}, (object)null);
							}
							else
							{
								Guide.BeginShowMessageBox("Invalid Code", "The code you entered is invalid.\n\n" + reason, (IEnumerable<string>)new string[1] { "Ok" }, 0, MessageBoxIcon.None, (AsyncCallback)delegate
								{
								}, (object)null);
							}
						}
					}
				}, (object)null);
				break;
			case MainMenuItems.Purchase:
				_game.ShowMarketPlace();
				break;
			case MainMenuItems.Awards:
				_achievementScreen = new AchievementScreen<CastleMinerZPlayerStats>(CastleMinerZGame.Instance.AcheivmentManager, CastleMinerZGame.Instance._myriadLarge, CastleMinerZGame.Instance._myriadMed, CastleMinerZGame.Instance.DummyTexture);
				_achievementScreen.ClickSound = "Click";
				_uiGroup.PushScreen(_achievementScreen);
				break;
			case MainMenuItems.Options:
				_uiGroup.PushScreen(_optionsMenu);
				break;
			case MainMenuItems.Quit:
				_uiGroup.PopScreen();
				break;
			}
		}

		private void _chooseModeMenu_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			switch ((ConnectMenuItems)e.MenuItem.Tag)
			{
			case ConnectMenuItems.Host:
				_localGame = false;
				if (_game.GameMode == GameModeTypes.Endurance)
				{
					startWorld();
				}
				else if (_game.GameMode == GameModeTypes.DragonEndurance)
				{
					_uiGroup.PushScreen(_worldPickerScreen);
				}
				else if (_game.GameMode == GameModeTypes.Survival)
				{
					_game.Difficulty = GameDifficultyTypes.EASY;
					_uiGroup.PushScreen(_difficultyLevelScreen);
				}
				else
				{
					_uiGroup.PushScreen(_worldPickerScreen);
				}
				break;
			case ConnectMenuItems.Connect:
				_game.GetNetworkSessions(delegate(AvailableNetworkSessionCollection result)
				{
					_chooseSessionScreen.Populate(result);
					_uiGroup.PopScreen();
					_uiGroup.PushScreen(_chooseSessionScreen);
				});
				_uiGroup.PushScreen(_connectingScreen);
				break;
			}
		}

		private void _worldPickerScreen_Clicked(object sender, SelectedEventArgs e)
		{
			WorldInfo info = (WorldInfo)e.Tag;
			if (info == null)
			{
				startWorld();
			}
			else if (info.OwnerGamerTag != Screen.CurrentGamer.Gamertag)
			{
				_uiGroup.ShowDialogScreen(_worldPickerScreen._takeOverTerrain, delegate
				{
					if (_worldPickerScreen._takeOverTerrain.OptionSelected != -1)
					{
						WorldManager.TakeOwnership(info);
						_game.BeginLoadTerrain(info, true);
						HostGame(_localGame);
					}
				});
			}
			else if (info.InfiniteResourceMode != _game.InfiniteResourceMode)
			{
				_uiGroup.ShowDialogScreen(_worldPickerScreen._infiniteModeConversion, delegate
				{
					if (_worldPickerScreen._infiniteModeConversion.OptionSelected != -1)
					{
						WorldManager.TakeOwnership(info);
						_game.BeginLoadTerrain(info, true);
						HostGame(_localGame);
					}
				});
			}
			else
			{
				_game.BeginLoadTerrain(info, true);
				HostGame(_localGame);
			}
		}

		private void _loadingScreen_BeforeDraw(object sender, DrawEventArgs e)
		{
			Viewport viewport = e.Device.Viewport;
			Rectangle titleSafeArea = e.Device.Viewport.TitleSafeArea;
			float num = (float)viewport.Height / 1080f;
			float loadProgress = _game.LoadProgress;
			string text = "Loading The World... Please Wait";
			float num2 = (float)titleSafeArea.Width * 0.8f;
			float num3 = (float)titleSafeArea.Left + ((float)titleSafeArea.Width - num2) / 2f;
			Sprite sprite = _game._uiSprites["Bar"];
			Vector2 vector = _largeFont.MeasureString(text);
			Vector2 location = new Vector2(num3, (float)(titleSafeArea.Height / 2) + vector.Y);
			float num4 = location.Y + (float)_largeFont.LineSpacing + 10f * num;
			Rectangle rectangle = new Rectangle((int)num3, (int)num4, (int)num2, _largeFont.LineSpacing);
			int left = rectangle.Left;
			int top = rectangle.Top;
			float num6 = (float)rectangle.Width / (float)sprite.Width;
			SpriteBatch.Begin();
			_game.Logo.Draw(SpriteBatch, new Vector2(titleSafeArea.Center.X - _game.Logo.Width / 2, 0f), Color.White);
			SpriteBatch.DrawOutlinedText(_largeFont, text, location, Color.White, Color.Black, 1);
			SpriteBatch.Draw(_game.DummyTexture, new Rectangle(left - 2, top - 2, rectangle.Width + 4, rectangle.Height + 4), Color.White);
			SpriteBatch.Draw(_game.DummyTexture, new Rectangle(left, top, rectangle.Width, rectangle.Height), Color.Black);
			int num5 = (int)((float)sprite.Width * loadProgress);
			sprite.Draw(SpriteBatch, new Rectangle(left, top, (int)((float)rectangle.Width * loadProgress), rectangle.Height), new Rectangle(sprite.Width - num5, 0, num5, sprite.Height), Color.White);
			textFlashTimer.Update(e.GameTime.get_ElapsedGameTime());
			Color.Lerp(Color.Red, Color.White, textFlashTimer.PercentComplete);
			if (textFlashTimer.Expired)
			{
				textFlashTimer.Reset();
			}
			SpriteBatch.End();
		}

		private void _connectingScreen_BeforeDraw(object sender, DrawEventArgs e)
		{
			Viewport viewport = e.Device.Viewport;
			Rectangle titleSafeArea = e.Device.Viewport.TitleSafeArea;
			float num = (float)viewport.Height / 1080f;
			string text = "Connecting... Please Wait";
			Vector2 vector = _largeFont.MeasureString(text);
			Vector2 location = new Vector2((float)(titleSafeArea.Width / 2) - vector.X / 2f, (float)(titleSafeArea.Height / 2) + vector.Y);
			textFlashTimer.Update(e.GameTime.get_ElapsedGameTime());
			Color textColor = Color.Lerp(Color.Red, Color.White, textFlashTimer.PercentComplete);
			if (textFlashTimer.Expired)
			{
				textFlashTimer.Reset();
			}
			SpriteBatch.Begin();
			_game.Logo.Draw(SpriteBatch, new Vector2(titleSafeArea.Center.X - _game.Logo.Width / 2, 0f), Color.White);
			SpriteBatch.DrawOutlinedText(_largeFont, text, location, textColor, Color.Black, 1);
			SpriteBatch.End();
		}

		private void CodeRedeemed(PromoCodeIds promoCode)
		{
			int num = 0;
		}

		private void _adScreen_ProcessingInput(object sender, InputEventArgs e)
		{
			if (e.InputManager.ButtonsPressed.Start)
			{
				_game.Exit();
			}
			if (e.InputManager.ButtonsPressed.Back)
			{
				_uiGroup.PushScreen(_upSellScreen);
				_uiGroup.PushScreen(_startScreen);
			}
		}

		private void _adScreen_AfterDraw(object sender, DrawEventArgs e)
		{
			Rectangle titleSafeArea = e.Device.Viewport.TitleSafeArea;
			float num = _Advert.Width;
			float num2 = _Advert.Height;
			Rectangle destinationRectangle = new Rectangle((int)((float)titleSafeArea.Center.X - num / 2f), (int)((float)titleSafeArea.Center.Y - num2 / 2f), (int)num, (int)num2);
			SpriteBatch.Begin();
			SpriteBatch.Draw(_Advert, destinationRectangle, Color.White);
			SpriteBatch.End();
		}

		private void _upSellScreen_Updating(object sender, UpdateEventArgs e)
		{
			if (!Guide.IsTrialMode)
			{
				_uiGroup.PopScreen();
			}
		}

		private void _upSellScreen_ProcessingInput(object sender, InputEventArgs e)
		{
			if (e.InputManager.ButtonsPressed.Start)
			{
				_game.ShowMarketPlace();
			}
			if (e.InputManager.ButtonsPressed.Back)
			{
				_uiGroup.PushScreen(_startScreen);
			}
			if (e.InputManager.ButtonsPressed.B)
			{
				_uiGroup.PopScreen();
			}
		}

		private void _upSellScreen_AfterDraw(object sender, DrawEventArgs e)
		{
			Rectangle titleSafeArea = e.Device.Viewport.TitleSafeArea;
			float num = _UpSellImg.Width;
			float num2 = _UpSellImg.Height;
			Rectangle destinationRectangle = new Rectangle((int)((float)titleSafeArea.Center.X - num / 2f), (int)((float)titleSafeArea.Center.Y - num2 / 2f), (int)num, (int)num2);
			SpriteBatch.Begin();
			SpriteBatch.Draw(_UpSellImg, destinationRectangle, Color.White);
			SpriteBatch.End();
		}

		private void _chooseAnotherGameScreen_ProcessingPlayerInput(object sender, ControllerInputEventArgs e)
		{
			if (e.Controller.PressedButtons.A || e.Controller.PressedButtons.B || e.Controller.PressedButtons.Back)
			{
				_uiGroup.PopScreen();
			}
		}

		private void _chooseAnotherGameScreen_BeforeDraw(object sender, DrawEventArgs e)
		{
			Viewport viewport = e.Device.Viewport;
			SpriteBatch.Begin();
			string text = "Session Has Ended";
			Vector2 vector = _largeFont.MeasureString(text);
			int lineSpacing = _largeFont.LineSpacing;
			SpriteBatch.DrawOutlinedText(_largeFont, text, new Vector2((float)viewport.TitleSafeArea.Center.X - vector.X / 2f, (float)viewport.TitleSafeArea.Center.Y - vector.Y / 2f), Color.White, Color.Black, 2);
			SpriteBatch.End();
		}

		public void PopToStartScreen()
		{
			while (_uiGroup.CurrentScreen != _startScreen && _uiGroup.CurrentScreen != null)
			{
				_uiGroup.PopScreen();
			}
			if (_uiGroup.CurrentScreen == null)
			{
				_uiGroup.PushScreen(_adScreen);
				_uiGroup.PushScreen(_upSellScreen);
				_uiGroup.PushScreen(_startScreen);
			}
			_game.SetAudio(1f, 0f, 0f, 0f);
			_game.PlayMusic("Theme");
		}

		public void PopToMainMenu(SignedInGamer gamer, SuccessCallback callback)
		{
			while (_uiGroup.CurrentScreen != _mainMenu && _uiGroup.CurrentScreen != null)
			{
				_uiGroup.PopScreen();
			}
			Screen.SelectedPlayerIndex = gamer.PlayerIndex;
			if (_uiGroup.CurrentScreen == null && _game.SaveDevice != null)
			{
				CloseSaveDevice();
			}
			_game.SetAudio(1f, 0f, 0f, 0f);
			_game.PlayMusic("Theme");
			if (_uiGroup.CurrentScreen == null)
			{
				_uiGroup.PushScreen(_adScreen);
				_uiGroup.PushScreen(_upSellScreen);
				_uiGroup.PushScreen(_startScreen);
			}
			if (_game.SaveDevice == null)
			{
				SetupSaveDevice(delegate(bool success)
				{
					_uiGroup.PushScreen(_mainMenu);
					callback(success);
				});
			}
			else if (callback != null)
			{
				callback(true);
			}
		}

		public void AcceptInvite()
		{
		}

		private void InitPromoCodes(SignedInGamer gamer, SaveDevice saveDevice)
		{
			_promoManager = new PromoCode.PromoCodeManager(gamer, saveDevice);
			PromoCodes[0] = _promoManager.RegisterCode("CastleMinerZUnlockAll", "Everything", PromoCodeIds.UnlockAll);
			PromoCodes[1] = _promoManager.RegisterCode("CastleMinerZStartWithAssault", "Start With an Assault Rifle", PromoCodeIds.StartWithAssault);
			PromoCodes[2] = _promoManager.RegisterCode("CastleMinerZStartWithGoldPick", "Start With a Gold Pick", PromoCodeIds.StartWithGoldPick);
			PromoCodes[3] = _promoManager.RegisterCode("CastleMinerZStartWithMoreAmmo", "Start With more Ammo", PromoCodeIds.StartWithMoreAmmo);
			PromoCodes[4] = _promoManager.RegisterCode("CastleMinerZUnlockDragonEndurance", "Dragon Endurance Mode", PromoCodeIds.UnlockDragonEndurance);
			PromoCodes[5] = _promoManager.RegisterCode("CastleMinerZUnlockCreative", "Creative Mode", PromoCodeIds.UnlockCreativeMode);
			PromoCodes[6] = _promoManager.RegisterCode("CastleMinerZStartWithBloodstoneAssault", "Start With a Bloodstone Assault Rifle", PromoCodeIds.StartWithBloodstoneAssault);
			PromoCodes[7] = _promoManager.RegisterCode("CastleMinerZStartWithBloodstonePick", "Start With a Bloodstone Pick", PromoCodeIds.StartWithBloodstonePick);
			PromoCodes[8] = _promoManager.RegisterCode("CastleMinerZStartWithTeleporter", "Start With a Teleporter", PromoCodeIds.StartWithTeleporter);
			_promoManager.LoadCodes();
			_cheatcodeManager = new CheatCode.CheatCodeManager(saveDevice);
			CheatCodes[0] = _cheatcodeManager.RegisterCode("CastleMinerZInfiniteResourceMode", "Creative Mode", "cmzinfinity", CheatCodeIds.InfinteResources);
			_cheatcodeManager.LoadCodes();
			List<PromoCode> redeemedCodes = _promoManager.GetRedeemedCodes();
			foreach (PromoCode item in redeemedCodes)
			{
				CodeRedeemed((PromoCodeIds)item.Tag);
			}
			CastleMinerPromoCode = _promoManager.GetDisplayCode("CastleCraftUnlockAll", "Unlock Everything In CastleMiner", null);
		}

		public void RedeemCode(PromoCodeIds id)
		{
			PromoCode promoCode = PromoCodes[(int)id];
			if (!promoCode.Redeemed)
			{
				_promoManager.Redeem(PromoCodes[(int)id]);
			}
		}
	}
}
