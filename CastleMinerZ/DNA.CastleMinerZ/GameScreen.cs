using System;
using System.Collections.ObjectModel;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.UI;
using DNA.Drawing;
using DNA.Drawing.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ
{
	public class GameScreen : ScreenGroup
	{
		public struct LightColorPack
		{
			public Color fog;

			public Color direct;

			public Color ambient;

			public Color specular;

			public LightColorPack(Color f, Color a, Color d, Color s)
			{
				fog = f;
				direct = d;
				ambient = a;
				specular = s;
			}

			public LightColorPack(float lerp, ref LightColorPack fromColor, ref LightColorPack toColor)
			{
				fog = Color.Lerp(fromColor.fog, toColor.fog, lerp);
				direct = Color.Lerp(fromColor.direct, toColor.direct, lerp);
				ambient = Color.Lerp(fromColor.ambient, toColor.ambient, lerp);
				specular = Color.Lerp(fromColor.specular, toColor.specular, lerp);
			}
		}

		public const float MINUTES_PER_DAY = 16f;

		public static readonly TimeSpan LengthOfDay = TimeSpan.FromMinutes(16.0);

		private CastleMinerZGame _game;

		private BlockTerrain _terrain;

		public CastleMinerSky _sky;

		private InGameMenu _inGameMenu;

		private HostOptions _hostOptionsMenu;

		private TeleportMenu _teleportMenu;

		private ControllerScreen _controllerScreen;

		private SettingsMenu _settingsMenu;

		private InGameHUD _inGameUI;

		public ScreenGroup _uiGroup = new ScreenGroup(true);

		private CameraView mainView;

		private EnemyManager _enemyManager;

		private TracerManager _tracerManager;

		private PickupManager _pickupManager;

		private ItemBlockEntityManager _itemBlockManager;

		public BlockPickerScreen _pickerScreen;

		public CraftingUIScreen _craftingScreen;

		public Scene mainScene;

		public Selector SelectorEntity;

		public CrackBoxEntity CrackBox;

		public GPSMarkerEntity GPSMarker;

		private LightColorPack dawnColors = new LightColorPack(new Color(94, 112, 129), new Color(183, 134, 107), new Color(254, 215, 158), new Color(254, 215, 158));

		private LightColorPack duskColors = new LightColorPack(new Color(145, 100, 57), new Color(0.4f, 0.4f, 0.3f), new Color(254, 215, 158), new Color(254, 215, 158));

		private LightColorPack dayColors = new LightColorPack(new Color(74, 78, 74), new Color(0.4f, 0.4f, 0.4f), new Color(1f, 1f, 1f), new Color(1f, 1f, 1f));

		private LightColorPack nightColors = new LightColorPack(new Color(10, 10, 10), new Color(50, 62, 107), new Color(75, 93, 160), new Color(100, 124, 214));

		private Player _localPlayer;

		public Scene _fpsScene;

		private SpriteBatch spriteBatch;

		public int exitCount;

		public InGameHUD HUD
		{
			get
			{
				return _inGameUI;
			}
		}

		public bool IsBlockPickerUp
		{
			get
			{
				return _uiGroup.Contains(_pickerScreen);
			}
		}

		public float TimeOfDay
		{
			get
			{
				return _sky.TimeOfDay;
			}
		}

		public float Day
		{
			get
			{
				return _sky.Day;
			}
			set
			{
				_sky.Day = value;
				float num = 1f;
				float num2 = 0f;
				float num3 = value + 1.625f;
				num3 -= (float)Math.Floor(num3);
				float num4 = num3 * ((float)Math.PI * 2f);
				float num5 = 0.2f + 0.8f * (float)Math.Abs(Math.Sin(num4));
				float num6 = (float)Math.Sqrt(1f - num5 * num5);
				num4 -= 0.236f;
				float x = (0f - (float)Math.Sin(num4)) * num6;
				float z = (float)Math.Cos(num4) * num6;
				Vector3 vectorToSun = new Vector3(x, num5, z);
				_terrain.VectorToSun = vectorToSun;
				float num7 = _sky.TimeOfDay * 24f;
				float num8 = num7 - (float)(int)num7;
				int num9 = (int)num7;
				float num10 = (_sky.TimeOfDay + 0.96f + 0.5f) % 1f * 2f;
				if (num10 > 1f)
				{
					num10 = 2f - num10;
				}
				LightColorPack lightColorPack;
				switch (num9)
				{
				default:
					num = 0f;
					num2 = 1f;
					lightColorPack = nightColors;
					break;
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
					num = 1f;
					num2 = 0f;
					lightColorPack = dayColors;
					break;
				case 6:
				case 7:
				case 8:
				case 18:
				case 19:
				case 20:
					switch (num9)
					{
					case 6:
						num = 0f;
						num2 = 1f - num8;
						lightColorPack = new LightColorPack(num8, ref nightColors, ref dawnColors);
						break;
					case 7:
						num = 0.5f;
						num2 = 0.5f;
						lightColorPack = dawnColors;
						break;
					case 8:
						num = num8;
						num2 = 0f;
						lightColorPack = new LightColorPack(num8, ref dawnColors, ref dayColors);
						break;
					case 18:
						num = 1f - num8;
						num2 = 0f;
						lightColorPack = new LightColorPack(num8, ref dayColors, ref duskColors);
						break;
					case 19:
						num = 0.5f;
						num2 = 0.5f;
						lightColorPack = duskColors;
						break;
					default:
						num = 0f;
						num2 = num8;
						_game.SetAudio(0f, num8, 0f, 0f);
						lightColorPack = new LightColorPack(num8, ref duskColors, ref nightColors);
						break;
					}
					break;
				}
				float num11 = (CastleMinerZGame.Instance.LocalPlayer.LocalPosition.Y + 32f) / 8f;
				if (num11 < 0f)
				{
					num11 = 0f;
				}
				if (num11 > 1f)
				{
					num11 = 1f;
				}
				num11 = 1f - num11;
				_terrain.FogColor = Color.Lerp(lightColorPack.fog, Color.Black, num11);
				_terrain.AmbientSunColor = lightColorPack.ambient;
				_terrain.SunlightColor = lightColorPack.direct;
				_terrain.SunSpecular = lightColorPack.specular;
				_terrain.PercentMidnight = num10;
				int num12 = _terrain.DepthUnderGround(_game.LocalPlayer.LocalPosition);
				float num13 = Math.Min(1f, (float)num12 / 15f);
				float num14 = 0f;
				if (_game.LocalPlayer.LocalPosition.Y <= -37f)
				{
					num14 = Math.Min(1f, (-37f - _game.LocalPlayer.LocalPosition.Y) / 10f);
				}
				_game.SetAudio(num * (1f - num13), num2 * (1f - num13), num13 * (1f - num14), num14);
			}
		}

		public void ShowInGameMenu()
		{
			_uiGroup.PushScreen(_inGameMenu);
		}

		public void ShowBlockPicker()
		{
			_uiGroup.PushScreen(_pickerScreen);
		}

		public void ShowCraftingScreen()
		{
			_inGameUI.PlayerInventory.DiscoverRecipies();
			_game.GameScreen._craftingScreen.Reset();
			_uiGroup.PushScreen(_craftingScreen);
		}

		public GameScreen(CastleMinerZGame game, Player localPlayer)
			: base(false)
		{
			_game = game;
			_terrain = _game._terrain;
			_localPlayer = localPlayer;
		}

		public void Inialize()
		{
			_teleportMenu = new TeleportMenu(_game);
			_teleportMenu.MenuItemSelected += _teleportMenu_MenuItemSelected;
			_inGameUI = new InGameHUD(_game);
			_pickerScreen = new BlockPickerScreen(_game, _inGameUI);
			_craftingScreen = new CraftingUIScreen(_game, _inGameUI);
			_hostOptionsMenu = new HostOptions(_game);
			_hostOptionsMenu.MenuItemSelected += _hostOptionsMenu_MenuItemSelected;
			_controllerScreen = new ControllerScreen(_game, true);
			_settingsMenu = new SettingsMenu(_game);
			_inGameMenu = new InGameMenu(_game);
			_inGameMenu.MenuItemSelected += _inGameMenu_MenuItemSelected;
			SceneScreen sceneScreen = new SceneScreen(false, false);
			PushScreen(sceneScreen);
			PushScreen(_uiGroup);
			_uiGroup.PushScreen(_inGameUI);
			sceneScreen.AfterDraw += gameScreen_AfterDraw;
			mainScene = new Scene();
			sceneScreen.Scenes.Add(mainScene);
			_sky = new CastleMinerSky();
			mainScene.Children.Add(_terrain);
			mainScene.Children.Add(_localPlayer);
			_localPlayer.Children.Add(_sky);
			SelectorEntity = new Selector();
			mainScene.Children.Add(SelectorEntity);
			CrackBox = new CrackBoxEntity();
			mainScene.Children.Add(CrackBox);
			GPSMarker = new GPSMarkerEntity();
			mainScene.Children.Add(GPSMarker);
			GPSMarker.Visible = false;
			_enemyManager = new EnemyManager();
			mainScene.Children.Add(_enemyManager);
			_tracerManager = new TracerManager();
			mainScene.Children.Add(_tracerManager);
			_pickupManager = new PickupManager();
			mainScene.Children.Add(_pickupManager);
			_itemBlockManager = new ItemBlockEntityManager();
			mainScene.Children.Add(_itemBlockManager);
			mainView = new CameraView(null, _localPlayer.FPSCamera);
			sceneScreen.Views.Add(mainView);
			_fpsScene = new Scene();
			sceneScreen.Scenes.Add(_fpsScene);
			_localPlayer.FPSMode = true;
			CameraView item = new CameraView(null, _localPlayer.GunEyePointCamera);
			sceneScreen.Views.Add(item);
		}

		private void gameScreen_AfterDraw(object sender, DrawEventArgs e)
		{
			if (spriteBatch == null)
			{
				spriteBatch = new SpriteBatch(e.Device);
			}
			if (_game.CurrentNetworkSession == null)
			{
				return;
			}
			Matrix view = mainView.Camera.View;
			Matrix projection = mainView.Camera.GetProjection(e.Device);
			Matrix matrix = view * projection;
			spriteBatch.Begin();
			for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)_game.CurrentNetworkSession.AllGamers).Count; i++)
			{
				NetworkGamer networkGamer = ((ReadOnlyCollection<NetworkGamer>)(object)_game.CurrentNetworkSession.AllGamers)[i];
				if (networkGamer.Tag == null || networkGamer.IsLocal)
				{
					continue;
				}
				Player player = (Player)networkGamer.Tag;
				if (player.Visible)
				{
					Vector3 position = player.LocalPosition + new Vector3(0f, 2f, 0f);
					Vector4 vector = Vector4.Transform(position, matrix);
					if (vector.Z > 0f)
					{
						Vector3 vector2 = new Vector3(vector.X / vector.W, vector.Y / vector.W, vector.Z / vector.W);
						vector2 *= new Vector3(0.5f, -0.5f, 1f);
						vector2 += new Vector3(0.5f, 0.5f, 0f);
						vector2 *= new Vector3(e.Device.Viewport.Width, e.Device.Viewport.Height, 1f);
						Vector2 vector3 = _game._nameTagFont.MeasureString(networkGamer.Gamertag);
						spriteBatch.DrawOutlinedText(_game._nameTagFont, networkGamer.Gamertag, new Vector2(vector2.X, vector2.Y) - vector3 / 2f, Color.White, Color.Black, 1);
					}
				}
			}
			spriteBatch.End();
		}

		public void TeleportToLocation(Vector3 Location, bool spawnOnTop)
		{
			_game.LocalPlayer.LocalPosition = Location;
			EnemyManager.Instance.ResetFarthestDistance();
			_terrain.CenterOn(_game.LocalPlayer.LocalPosition, true);
			InGameWaitScreen.ShowScreen(_game, this, "Please Wait...", spawnOnTop, () => _terrain.MinimallyLoaded);
		}

		private void _teleportMenu_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			switch ((TeleportMenuItems)e.MenuItem.Tag)
			{
			case TeleportMenuItems.Quit:
				_uiGroup.PopScreen();
				_uiGroup.PopScreen();
				break;
			case TeleportMenuItems.Origin:
				_uiGroup.PopScreen();
				_uiGroup.PopScreen();
				TeleportToLocation(WorldInfo.DefaultStartLocation, true);
				break;
			case TeleportMenuItems.Player:
				SelectPlayerScreen.SelectPlayer(_game, _uiGroup, false, false, delegate(Player player)
				{
					if (player != null)
					{
						_game.LocalPlayer.LocalPosition = player.LocalPosition;
						_terrain.CenterOn(_game.LocalPlayer.LocalPosition, true);
					}
					_uiGroup.PopScreen();
					_uiGroup.PopScreen();
					InGameWaitScreen.ShowScreen(_game, this, "Please Wait...", false, () => _terrain.MinimallyLoaded);
				});
				break;
			case TeleportMenuItems.Surface:
				_game.MakeAboveGround(true);
				_uiGroup.PopScreen();
				_uiGroup.PopScreen();
				InGameWaitScreen.ShowScreen(_game, this, "Please Wait...", true, () => _terrain.MinimallyLoaded);
				break;
			}
		}

		private void _hostOptionsMenu_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			switch ((HostOptionItems)e.MenuItem.Tag)
			{
			case HostOptionItems.Return:
				_uiGroup.PopScreen();
				break;
			case HostOptionItems.KickPlayer:
				SelectPlayerScreen.SelectPlayer(_game, _uiGroup, false, false, delegate(Player player)
				{
					if (player != null)
					{
						BroadcastTextMessage.Send(_game.MyNetworkGamer, player.Gamer.Gamertag + " has been kicked by the host");
						KickMessage.Send(_game.MyNetworkGamer, player.Gamer, false);
					}
				});
				break;
			case HostOptionItems.BanPlayer:
				SelectPlayerScreen.SelectPlayer(_game, _uiGroup, false, false, delegate(Player player)
				{
					if (player != null)
					{
						BroadcastTextMessage.Send(_game.MyNetworkGamer, player.Gamer.Gamertag + " has been banned by the host");
						KickMessage.Send(_game.MyNetworkGamer, player.Gamer, true);
						_game.PlayerStats.BanList[player.Gamer.Gamertag] = DateTime.UtcNow;
						_game.SaveData();
					}
				});
				break;
			case HostOptionItems.Restart:
				RestartLevelMessage.Send((LocalNetworkGamer)_game.LocalPlayer.Gamer);
				BroadcastTextMessage.Send(_game.MyNetworkGamer, _game.LocalPlayer.Gamer.Gamertag + " Has Restarted The Game");
				break;
			case HostOptionItems.Public:
				_game.IsPublicGame = !_game.IsPublicGame;
				BroadcastTextMessage.Send(_game.MyNetworkGamer, "Server Game Type has been changed to  " + (_game.IsPublicGame ? "Public" : "Private"));
				break;
			case HostOptionItems.ClearBanList:
				_game.PlayerStats.BanList.Clear();
				_game.SaveData();
				break;
			}
		}

		public void AddPlayer(Player player)
		{
			mainScene.Children.Add(player);
		}

		public override void Update(DNAGame game, GameTime gameTime)
		{
			if (exitCount > 0)
			{
				exitCount--;
				if (exitCount <= 0)
				{
					_game.EndGame(true);
					exitCount = 0;
					return;
				}
			}
			float day = Day;
			Day += (float)(gameTime.get_ElapsedGameTime().TotalSeconds / LengthOfDay.TotalSeconds);
			base.Update(game, gameTime);
		}

		private void _inGameMenu_MenuItemSelected(object sender, SelectedMenuItemArgs e)
		{
			InGameMenuItems inGameMenuItems = (InGameMenuItems)e.MenuItem.Tag;
			SignedInGamer currentGamer = Screen.CurrentGamer;
			switch (inGameMenuItems)
			{
			case InGameMenuItems.Quit:
				if (_localPlayer.Gamer.IsHost)
				{
					_localPlayer.SaveInventory(_game.SaveDevice, _game.CurrentWorld.SavePath);
					_localPlayer.FinalSaveRegistered = true;
				}
				else
				{
					InventoryStoreOnServerMessage.Send((LocalNetworkGamer)_localPlayer.Gamer, _localPlayer.PlayerInventory, true);
				}
				exitCount = 2;
				break;
			case InGameMenuItems.Purchase:
				_game.ShowMarketPlace();
				break;
			case InGameMenuItems.Controls:
				_uiGroup.PushScreen(_controllerScreen);
				break;
			case InGameMenuItems.Settings:
				_uiGroup.PushScreen(_settingsMenu);
				break;
			case InGameMenuItems.Invite:
				if (currentGamer.Privileges.AllowCommunication != 0 && !currentGamer.IsGuest)
				{
					_game.ShowInvite(true);
				}
				break;
			case InGameMenuItems.HostOptions:
				_uiGroup.PushScreen(_hostOptionsMenu);
				break;
			case InGameMenuItems.MyBlocks:
				_uiGroup.PopScreen();
				if (!IsBlockPickerUp)
				{
					_uiGroup.PushScreen(_pickerScreen);
				}
				break;
			case InGameMenuItems.Teleport:
				_uiGroup.PushScreen(_teleportMenu);
				break;
			case InGameMenuItems.Return:
				_uiGroup.PopScreen();
				if (IsBlockPickerUp)
				{
					_uiGroup.PopScreen();
				}
				break;
			}
		}
	}
}
