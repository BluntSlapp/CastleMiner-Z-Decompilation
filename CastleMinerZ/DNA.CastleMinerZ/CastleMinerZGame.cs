using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using DNA.Audio;
using DNA.Avatars;
using DNA.CastleMinerZ.Achievements;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.UI;
using DNA.CastleMinerZ.Utils.Threading;
using DNA.Drawing;
using DNA.Drawing.Animation;
using DNA.Drawing.UI;
using DNA.IO.Storage;
using DNA.Net;
using DNA.Profiling;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ
{
	public class CastleMinerZGame : DNAGame
	{
		public enum NetworkProps
		{
			Version,
			Public,
			Permission,
			GameMode,
			JoinGame,
			Difficulty,
			InfiniteResources
		}

		public delegate void WorldInfoCallback(WorldInfo info);

		public delegate void GotSessionsCallback(AvailableNetworkSessionCollection sessions);

		private class SaveDataInfo
		{
			public WorldInfo Worldinfo;

			public PlayerInventory Inventory;

			public CastleMinerZPlayerStats PlayerStats;
		}

		public const int NetworkVersion = 12;

		private static Version GameVersion = new Version(1, 4, 4);

		public static CastleMinerZGame Instance;

		private Player _localPlayer;

		public AudioListener Listener = new AudioListener();

		public CastleMinerZAchievementManager AcheivmentManager;

		public BlockTerrain _terrain;

		public WaterPlane _waterPlane;

		public SpriteFont _nameTagFont;

		public SpriteFont _largeFont;

		public SpriteFont _medFont;

		public SpriteFont _smallFont;

		public SpriteFont _systemFont;

		public SpriteFont _consoleFont;

		public SpriteFont _myriadLarge;

		public SpriteFont _myriadMed;

		private AvatarDescription _myAvatarDescription;

		public CastleMinerZPlayerStats PlayerStats = new CastleMinerZPlayerStats();

		public SpriteManager _uiSprites;

		public CastleMinerZControllerMapping _controllerMapping = new CastleMinerZControllerMapping();

		public WorldInfo CurrentWorld;

		public FrontEndScreen FrontEnd;

		public GameScreen GameScreen;

		public bool IsPurchased;

		public Texture2D DialogScreenImage;

		public Texture2D MenuBackdrop;

		public byte TerrainServerID;

		public bool DrawingReflection;

		public LocalNetworkGamer MyNetworkGamer;

		public PlayerSaveDevice SaveDevice;

		public GameModeTypes GameMode;

		public bool InfiniteResourceMode;

		public GameDifficultyTypes Difficulty;

		public bool RequestEndGame;

		public AudioCategory MusicSounds;

		public AudioCategory DaySounds;

		public AudioCategory NightSounds;

		public AudioCategory CaveSounds;

		public AudioCategory HellSounds;

		public Cue MusicCue;

		public Cue DayCue;

		public Cue NightCue;

		public Cue CaveCue;

		public Cue HellCue;

		public Sprite Logo;

		private ThreadStart _waitForTerrainCallback;

		private WorldInfoCallback _waitForWorldInfo;

		private bool _saving;

		private object saveLock = new object();

		public static readonly int[] SaveProcessorAffinity = new int[1] { 5 };

		private bool _savingTerrain;

		private int _currentFrameNumber;

		private OneShotTimer _worldUpdateTimer = new OneShotTimer(TimeSpan.FromSeconds(5.0));

		private bool _fadeMusic;

		private OneShotTimer musicFadeTimer = new OneShotTimer(TimeSpan.FromSeconds(3.0));

		private Entity holdingGround = new Entity();

		public bool IsPublicGame
		{
			get
			{
				if (base.CurrentNetworkSession.SessionProperties.get_Item(1).HasValue)
				{
					return base.CurrentNetworkSession.SessionProperties.get_Item(1).Value != 0;
				}
				return false;
			}
			set
			{
				base.CurrentNetworkSession.SessionProperties.set_Item(1, (int?)(value ? 1 : 0));
			}
		}

		public bool IsAvatarLoaded
		{
			get
			{
				return _myAvatarDescription != null;
			}
		}

		public float LoadProgress
		{
			get
			{
				return (float)_terrain.LoadingProgress / 100f;
			}
		}

		public bool IsOnlineGame
		{
			get
			{
				if (base.CurrentNetworkSession != null)
				{
					return base.CurrentNetworkSession.SessionType == NetworkSessionType.PlayerMatch;
				}
				return false;
			}
		}

		public bool IsGameHost
		{
			get
			{
				if (MyNetworkGamer != null)
				{
					return MyNetworkGamer.Id == TerrainServerID;
				}
				return false;
			}
		}

		public Player LocalPlayer
		{
			get
			{
				return _localPlayer;
			}
		}

		public CastleMinerZGame()
			: base(false, GameVersion)
		{
			Instance = this;
			if (Debugger.IsAttached)
			{
				WantProfiling(false, true);
			}
			Profiler.Profiling = false;
			Profiler.SetColor("Zombie Update", Color.Blue);
			Profiler.SetColor("Zombie Collision", Color.Red);
			Profiler.SetColor("Drawing Terrain", Color.Green);
			base.IsFixedTimeStep = false;
			PauseDuringGuide = false;
			StartGamerServices();
			TaskDispatcher.Create();
		}

		public void ShowInvite(bool full)
		{
			SignedInGamer currentGamer = Screen.CurrentGamer;
			if (currentGamer.Privileges.AllowCommunication == GamerPrivilegeSetting.Blocked || currentGamer.IsGuest)
			{
				return;
			}
			try
			{
				if (full)
				{
					FriendCollection friends = Screen.CurrentGamer.GetFriends();
					Guide.ShowGameInvite(Screen.SelectedPlayerIndex.Value, (IEnumerable<Gamer>)(object)friends);
				}
				else
				{
					Guide.ShowGameInvite(Screen.SelectedPlayerIndex.Value, (IEnumerable<Gamer>)null);
				}
			}
			catch
			{
			}
		}

		public bool IsLocalPlayerId(byte id)
		{
			if (LocalPlayer != null && LocalPlayer.Gamer != null)
			{
				return LocalPlayer.Gamer.Id == id;
			}
			return false;
		}

		protected override void SecondaryLoad()
		{
			Guide.NotificationPosition = NotificationPosition.TopLeft;
			SoundManager.ActiveListener = Listener;
			Texture2D texture2D = base.Content.Load<Texture2D>("LoadScreen");
			LoadScreen loadScreen = new LoadScreen(texture2D, TimeSpan.FromSeconds(10.300000190734863));
			MainThreadMessageSender.Init();
			ScreenManager.PushScreen(loadScreen);
			SoundManager.Instance.Load("Sounds");
			DaySounds = SoundManager.Instance.GetCatagory("AmbientDay");
			NightSounds = SoundManager.Instance.GetCatagory("AmbientNight");
			CaveSounds = SoundManager.Instance.GetCatagory("AmbientCave");
			MusicSounds = SoundManager.Instance.GetCatagory("Music");
			HellSounds = SoundManager.Instance.GetCatagory("AmbientHell");
			PlayMusic("Theme");
			SetAudio(1f, 0f, 0f, 0f);
			ControllerImages.Load(base.Content);
			MenuBackdrop = base.Content.Load<Texture2D>("MenuBack");
			_terrain = new BlockTerrain(base.GraphicsDevice, base.Content);
			_waterPlane = new WaterPlane(base.GraphicsDevice, base.Content);
			InventoryItem.Initalize(base.Content);
			BlockEntity.Initialize();
			TracerManager.Initialize();
			string text = "720\\";
			_consoleFont = base.Content.Load<SpriteFont>(text + "ConsoleFont");
			_largeFont = base.Content.Load<SpriteFont>(text + "LargeFont");
			_medFont = base.Content.Load<SpriteFont>(text + "MedFont");
			_smallFont = base.Content.Load<SpriteFont>(text + "SmallFont");
			_systemFont = base.Content.Load<SpriteFont>(text + "System");
			_nameTagFont = base.Content.Load<SpriteFont>(text + "NameTagFont");
			_myriadLarge = base.Content.Load<SpriteFont>(text + "MyriadLarge");
			_myriadMed = base.Content.Load<SpriteFont>(text + "MyriadMedium");
			_uiSprites = base.Content.Load<SpriteManager>("SpriteSheet");
			DialogScreenImage = base.Content.Load<Texture2D>("DialogBack");
			Logo = _uiSprites["Logo"];
			ProfilerUtils.SystemFont = _systemFont;
			EnemyType.Init();
			DragonType.Init();
			FireballEntity.Init();
			DragonClientEntity.Init();
			AvatarAnimationManager.Instance.RegisterAnimation("Swim", base.Content.Load<AnimationClip>("AvatarAnimation\\Swim Underwater"), true);
			AvatarAnimationManager.Instance.RegisterAnimation("Wave", base.Content.Load<AnimationClip>("AvatarAnimation\\Wave"), true);
			AvatarAnimationManager.Instance.RegisterAnimation("Run", base.Content.Load<AnimationClip>("AvatarAnimation\\Run"), true);
			AvatarAnimationManager.Instance.RegisterAnimation("Walk", base.Content.Load<AnimationClip>("AvatarAnimation\\Walk"), true);
			AvatarAnimationManager.Instance.RegisterAnimation("Die", base.Content.Load<AnimationClip>("AvatarAnimation\\Faint"), false);
			AvatarAnimationManager.Instance.RegisterAnimation("GunReload", base.Content.Load<AnimationClip>("AvatarAnimation\\AssaultReload"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GunShoulderIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\AssaultShoulderIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GunShoulderWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\AssaultShoulderWalk"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GunShoulderShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\AssaultShoulderShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GunShoulder", base.Content.Load<AnimationClip>("AvatarAnimation\\AssaultShoulder"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GunShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\AssaultShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GunIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\HoldAssultIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GunRun", base.Content.Load<AnimationClip>("AvatarAnimation\\AKRun"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PistolIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\HoldPistolIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PistolWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\PistolWalk"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PistolShoulder", base.Content.Load<AnimationClip>("AvatarAnimation\\PistolShoulder"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PistolShoulderWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\PistolShoulderWalk"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PistolReload", base.Content.Load<AnimationClip>("AvatarAnimation\\PistolReload"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PistolShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\PistolShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PistolShoulderIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\PistolShoulderIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PistolShoulderShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\PistolShoulderShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PumpShotgunShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\PumpShotgunShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PumpShotgunReload", base.Content.Load<AnimationClip>("AvatarAnimation\\PumpShotgunReload"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PumpShotgunShoulderShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\PumpShotgunShoulderShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("RifleIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\RifleIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("RifleWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\RifleWalk"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("RifleShoulder", base.Content.Load<AnimationClip>("AvatarAnimation\\RifleShoulder"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("RifleShoulderWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\RifleShoulderWalk"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("RifleReload", base.Content.Load<AnimationClip>("AvatarAnimation\\RifleReload"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("RifleShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\RifleShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("RifleShoulderIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\RifleShoulderIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("RifleShoulderShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\RifleShoulderShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("SMGIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\SMGIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("SMGWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\SMGWalk"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("SMGShoulder", base.Content.Load<AnimationClip>("AvatarAnimation\\SMGShoulder"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("SMGShoulderWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\SMGShoulderWalk"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("SMGReload", base.Content.Load<AnimationClip>("AvatarAnimation\\SMGReload"), false, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("SMGShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\SMGShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("SMGShoulderIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\SMGShoulderIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("SMGShoulderShoot", base.Content.Load<AnimationClip>("AvatarAnimation\\SMGShoulderShoot"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GenericIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\GenericIdle"), true, new AvatarBone[1] { AvatarBone.CollarRight });
			AvatarAnimationManager.Instance.RegisterAnimation("GenericUse", base.Content.Load<AnimationClip>("AvatarAnimation\\GenericUse"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("GenericWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\GenericWalk"), true, new AvatarBone[1] { AvatarBone.CollarRight });
			AvatarAnimationManager.Instance.RegisterAnimation("FistIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\FPSIdle"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("FistUse", base.Content.Load<AnimationClip>("AvatarAnimation\\FPSPick"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("FistWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\FPSWalk"), true, new AvatarBone[1] { AvatarBone.BackUpper });
			AvatarAnimationManager.Instance.RegisterAnimation("PickIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\FPSIdle"), true, new AvatarBone[1] { AvatarBone.CollarRight });
			AvatarAnimationManager.Instance.RegisterAnimation("PickUse", base.Content.Load<AnimationClip>("AvatarAnimation\\FPSPick"), true, new AvatarBone[1] { AvatarBone.CollarRight });
			AvatarAnimationManager.Instance.RegisterAnimation("PickWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\FPSWalk"), true, new AvatarBone[1] { AvatarBone.CollarRight });
			AvatarAnimationManager.Instance.RegisterAnimation("BlockIdle", base.Content.Load<AnimationClip>("AvatarAnimation\\BlockHoldIdle"), true, new AvatarBone[1] { AvatarBone.CollarRight });
			AvatarAnimationManager.Instance.RegisterAnimation("BlockUse", base.Content.Load<AnimationClip>("AvatarAnimation\\BlockUse"), true, new AvatarBone[1] { AvatarBone.CollarRight });
			AvatarAnimationManager.Instance.RegisterAnimation("BlockWalk", base.Content.Load<AnimationClip>("AvatarAnimation\\BlockHoldWalk"), true, new AvatarBone[1] { AvatarBone.CollarRight });
			AvatarAnimationManager.Instance.RegisterAnimation("Stand", base.Content.Load<AnimationClip>("AvatarAnimation\\Stand0"), true);
			AvatarAnimationManager.Instance.RegisterAnimation("IdleHead", base.Content.Load<AnimationClip>("AvatarAnimation\\MaleIdleLookAround"), true, new AvatarBone[1] { AvatarBone.Neck });
			AvatarAnimationManager.Instance.RegisterAnimation("Tilt", base.Content.Load<AnimationClip>("AvatarAnimation\\Tilt"), true, new AvatarBone[1] { AvatarBone.BackLower }, new AvatarBone[2]
			{
				AvatarBone.CollarRight,
				AvatarBone.CollarLeft
			});
			FrontEnd = new FrontEndScreen(this);
			BeginLoadTerrain(null, true);
			while (!loadScreen.Finished)
			{
				Thread.Sleep(50);
			}
			ScreenManager.PopScreen();
			ScreenManager.PushScreen(FrontEnd);
			texture2D.Dispose();
			IsPurchased = !Guide.IsTrialMode;
			NetworkSession.add_InviteAccepted((EventHandler<InviteAcceptedEventArgs>)NetworkSession_InviteAccepted);
			base.SecondaryLoad();
		}

		private void NetworkSession_InviteAccepted(object sender, InviteAcceptedEventArgs e)
		{
			Screen.SelectedPlayerIndex = e.Gamer.PlayerIndex;
			if (Guide.IsTrialMode)
			{
				ShowMarketPlace();
				return;
			}
			if (base.CurrentNetworkSession != null)
			{
				EndGame(true);
			}
			FrontEnd.PopToMainMenu(e.Gamer, delegate(bool success)
			{
				if (success)
				{
					WaitScreen.DoWait(FrontEnd._uiGroup, "Loading Player Info...", delegate
					{
						FrontEnd.SetupNewGamer(e.Gamer, SaveDevice);
					}, FrontEnd.JoinInvitedGame);
				}
			});
		}

		public void BeginLoadTerrain(WorldInfo info, bool host)
		{
			if (info == null)
			{
				CurrentWorld = WorldInfo.CreateNewWorld(null);
			}
			else
			{
				CurrentWorld = info;
			}
			TaskDispatcher.Instance.CanUseMainCore = false;
			_terrain.AsyncInit(CurrentWorld, host, delegate
			{
				_savingTerrain = false;
			});
		}

		public void WaitForTerrainLoad(ThreadStart callback)
		{
			_waitForTerrainCallback = callback;
		}

		public void GetWorldInfo(WorldInfoCallback callback)
		{
			_waitForWorldInfo = callback;
			RequestWorldInfoMessage.Send(MyNetworkGamer);
		}

		public void HostGame(bool local, SuccessCallback callback)
		{
			NetworkSessionProperties networkSessionProperties = new NetworkSessionProperties();
			networkSessionProperties.set_Item(0, (int?)12);
			networkSessionProperties.set_Item(1, (int?)1);
			networkSessionProperties.set_Item(3, (int?)(int)GameMode);
			networkSessionProperties.set_Item(4, (int?)0);
			networkSessionProperties.set_Item(5, (int?)(int)Difficulty);
			if (InfiniteResourceMode)
			{
				networkSessionProperties.set_Item(6, (int?)1);
			}
			else
			{
				networkSessionProperties.set_Item(6, (int?)0);
			}
			if (local)
			{
				HostGame(NetworkSessionType.Local, networkSessionProperties, new SignedInGamer[1] { Screen.CurrentGamer }, 2, false, true, callback);
			}
			else
			{
				HostGame(NetworkSessionType.PlayerMatch, networkSessionProperties, new SignedInGamer[1] { Screen.CurrentGamer }, 4, false, true, callback);
			}
		}

		public void GetNetworkSessions(GotSessionsCallback callback)
		{
			NetworkSessionProperties networkSessionProperties = new NetworkSessionProperties();
			networkSessionProperties.set_Item(0, (int?)12);
			networkSessionProperties.set_Item(1, (int?)1);
			networkSessionProperties.set_Item(4, (int?)0);
			if (InfiniteResourceMode)
			{
				networkSessionProperties.set_Item(6, (int?)1);
			}
			else
			{
				networkSessionProperties.set_Item(6, (int?)0);
				networkSessionProperties.set_Item(3, (int?)(int)GameMode);
			}
			NetworkSession.BeginFind(NetworkSessionType.PlayerMatch, (IEnumerable<SignedInGamer>)new SignedInGamer[1] { Screen.CurrentGamer }, networkSessionProperties, (AsyncCallback)delegate(IAsyncResult result)
			{
				AvailableNetworkSessionCollection sessions = null;
				try
				{
					sessions = NetworkSession.EndFind(result);
				}
				catch
				{
				}
				try
				{
					GotSessionsCallback gotSessionsCallback = (GotSessionsCallback)result.AsyncState;
					if (gotSessionsCallback != null)
					{
						gotSessionsCallback(sessions);
					}
				}
				catch (Exception e)
				{
					CrashGame(e);
				}
			}, (object)callback);
		}

		public void StartGame()
		{
			PlayerStats.GamesPlayed++;
			PlayerExistsMessage.Send(MyNetworkGamer, true);
			Difficulty = (GameDifficultyTypes)base.CurrentNetworkSession.SessionProperties.get_Item(5).Value;
		}

		public void SaveData()
		{
			if (!_saving)
			{
				SaveDataInfo saveDataInfo = new SaveDataInfo();
				if (GameScreen != null && GameScreen.HUD != null)
				{
					saveDataInfo.Inventory = GameScreen.HUD.PlayerInventory;
					saveDataInfo.Worldinfo = CurrentWorld;
					saveDataInfo.PlayerStats = PlayerStats;
					TaskScheduler.QueueUserWorkItem(SaveDataInternal, saveDataInfo);
				}
			}
		}

		public void SavePlayerStats(CastleMinerZPlayerStats playerStats)
		{
			if (Screen.CurrentGamer != null && !Screen.CurrentGamer.IsGuest)
			{
				SaveDevice.Save("stats.sav", true, true, delegate(Stream stream)
				{
					BinaryWriter binaryWriter = new BinaryWriter(stream);
					playerStats.Save(binaryWriter);
					binaryWriter.Flush();
				});
			}
		}

		public void SaveDataInternal(object state)
		{
			SaveDataInfo saveDataInfo = (SaveDataInfo)state;
			lock (saveLock)
			{
				try
				{
					_saving = true;
					Thread.CurrentThread.SetProcessorAffinity(SaveProcessorAffinity);
					SavePlayerStats(saveDataInfo.PlayerStats);
					if (saveDataInfo.Worldinfo.OwnerGamerTag != null)
					{
						saveDataInfo.Worldinfo.LastPlayedDate = DateTime.Now;
						saveDataInfo.Worldinfo.LastPosition = LocalPlayer.LocalPosition;
						saveDataInfo.Worldinfo.SaveToStorage(Screen.CurrentGamer, SaveDevice);
					}
					if (!LocalPlayer.FinalSaveRegistered)
					{
						if (LocalPlayer.Gamer.IsHost)
						{
							LocalPlayer.SaveInventory(SaveDevice, CurrentWorld.SavePath);
						}
						else
						{
							InventoryStoreOnServerMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, saveDataInfo.Inventory, false);
						}
					}
					if (GameMode != 0)
					{
						ChunkCache.Instance.Flush(true);
					}
					SaveDevice.Flush();
				}
				catch
				{
				}
				finally
				{
					_saving = false;
				}
			}
		}

		public void EndGame(bool saveData)
		{
			if (LocalPlayer != null && LocalPlayer.UnderwaterCue != null && !LocalPlayer.UnderwaterCue.IsPaused)
			{
				LocalPlayer.UnderwaterCue.Pause();
			}
			LeaveGame();
			if (saveData && LocalPlayer != null)
			{
				SaveData();
			}
			if (ScreenManager.CurrentScreen == GameScreen)
			{
				ScreenManager.PopScreen();
			}
			GameScreen = null;
			if (_terrain.Parent != null)
			{
				_terrain.RemoveFromParent();
			}
			if (WaterPlane.Instance.Parent != null)
			{
				WaterPlane.Instance.RemoveFromParent();
			}
			if (Screen.CurrentGamer == null)
			{
				FrontEnd.PopToStartScreen();
			}
			else
			{
				FrontEnd.PopToMainMenu(Screen.CurrentGamer, null);
			}
			if (GameMode == GameModeTypes.Endurance && FrontEnd.WorldManager != null)
			{
				FrontEnd.WorldManager.Delete(CurrentWorld);
				SaveDevice.Flush();
			}
			_waitForTerrainCallback = null;
			_savingTerrain = true;
			BeginLoadTerrain(null, true);
			WaitScreen.DoWait(FrontEnd._uiGroup, "Please Wait...", IsSavingProgress);
		}

		public override void OnSessionEnded(NetworkSessionEndReason reason)
		{
			EndGame(true);
			FrontEnd.ShowUIDialog("Session Ended", "You have been disconnected from the network session.", false);
			base.OnSessionEnded(reason);
		}

		private bool IsSavingProgress()
		{
			return !_savingTerrain;
		}

		protected override void Update(GameTime gameTime)
		{
			UpdateMusic(gameTime);
			if (_terrain != null)
			{
				_terrain.GlobalUpdate(gameTime);
				if (_terrain.MinimallyLoaded && _waitForTerrainCallback != null)
				{
					_waitForTerrainCallback();
					_waitForTerrainCallback = null;
				}
			}
			if (PlayerStats != null)
			{
				if (Guide.IsTrialMode)
				{
					PlayerStats.TimeInTrial += gameTime.get_ElapsedGameTime();
				}
				else
				{
					PlayerStats.TimeInFull += gameTime.get_ElapsedGameTime();
					if (!IsPurchased)
					{
						IsPurchased = true;
						PlayerStats.TimeOfPurchase = DateTime.UtcNow;
						if (GameMode == GameModeTypes.Endurance)
						{
							Console.WriteLine("Thank you for purchasing");
						}
						else
						{
							Console.WriteLine("Thank you for purchasing, World Saved");
						}
					}
				}
				if (FrontEnd != null && ScreenManager.CurrentScreen == FrontEnd)
				{
					PlayerStats.TimeInMenu += gameTime.get_ElapsedGameTime();
				}
				if (base.CurrentNetworkSession != null && base.CurrentNetworkSession.SessionType == NetworkSessionType.PlayerMatch && Instance.GameMode == GameModeTypes.Endurance)
				{
					PlayerStats.TimeOnline += gameTime.get_ElapsedGameTime();
				}
			}
			if (RequestEndGame)
			{
				RequestEndGame = false;
				EndGame(true);
			}
			base.Update(gameTime);
		}

		public void CheaterFound()
		{
			SaveDevice.DeleteStorage();
			Exit();
		}

		protected override void OnPlayerSignedOut(SignedInGamer gamer)
		{
			Screen.SelectedPlayerIndex = null;
			EndGame(true);
			FrontEnd.PopToStartScreen();
			base.OnPlayerSignedOut(gamer);
		}

		protected override void SendNetworkUpdates(NetworkSession session, GameTime gameTime)
		{
			if (session == null || ((ReadOnlyCollection<LocalNetworkGamer>)(object)session.LocalGamers).Count == 0)
			{
				return;
			}
			if (((ReadOnlyCollection<LocalNetworkGamer>)(object)session.LocalGamers)[0].IsHost && GameScreen != null)
			{
				_worldUpdateTimer.Update(gameTime.get_ElapsedGameTime());
				if (_worldUpdateTimer.Expired)
				{
					TimeOfDayMessage.Send(((ReadOnlyCollection<LocalNetworkGamer>)(object)session.LocalGamers)[0], GameScreen.Day);
					_worldUpdateTimer.Reset();
				}
			}
			_currentFrameNumber++;
			if (_currentFrameNumber > ((ReadOnlyCollection<NetworkGamer>)(object)session.AllGamers).Count)
			{
				_currentFrameNumber = 0;
			}
			if (_localPlayer != null)
			{
				for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)session.AllGamers).Count; i++)
				{
					if (((ReadOnlyCollection<NetworkGamer>)(object)session.AllGamers)[i].IsLocal && i != _currentFrameNumber && !_localPlayer.UsingTool && !_localPlayer.Reloading)
					{
						return;
					}
				}
				if (((ReadOnlyCollection<LocalNetworkGamer>)(object)session.LocalGamers).Count > 0)
				{
					PlayerUpdateMessage.Send(((ReadOnlyCollection<LocalNetworkGamer>)(object)session.LocalGamers)[0], _localPlayer, _controllerMapping);
				}
			}
			MainThreadMessageSender.Instance.DrainQueue();
		}

		public void SetupNewGamer(SignedInGamer gamer)
		{
			PlayerStats = new CastleMinerZPlayerStats();
			PlayerStats.GamerTag = gamer.Gamertag;
			PlayerStats.InvertYAxis = gamer.GameDefaults.InvertYAxis;
			LoadPlayerData();
			Brightness = PlayerStats.brightness;
			MusicSounds.SetVolume(PlayerStats.musicVolume);
			AcheivmentManager = new CastleMinerZAchievementManager(this);
			IAsyncResult asyncResult = AvatarDescription.BeginGetFromGamer((Gamer)gamer, (AsyncCallback)null, (object)null);
			_myAvatarDescription = AvatarDescription.EndGetFromGamer(asyncResult);
		}

		public void MakeAboveGround(bool spawnontop)
		{
			if (spawnontop)
			{
				_localPlayer.LocalPosition = _terrain.FindTopmostGroundLocation(_localPlayer.LocalPosition);
			}
			else
			{
				_localPlayer.LocalPosition = _terrain.FindSafeStartLocation(_localPlayer.LocalPosition);
			}
		}

		public void PlayMusic(string cueName)
		{
			_fadeMusic = false;
			if (MusicCue != null && MusicCue.IsPlaying)
			{
				MusicCue.Stop(AudioStopOptions.Immediate);
				MusicCue = null;
			}
			if (MusicCue == null || !MusicCue.IsPlaying)
			{
				MusicCue = SoundManager.Instance.PlayInstance(cueName);
			}
			MusicSounds.SetVolume(PlayerStats.musicVolume);
		}

		public void FadeMusic()
		{
			_fadeMusic = true;
			musicFadeTimer.Reset();
		}

		public void SetAudio(float day, float night, float cave, float hell)
		{
			if (DayCue == null)
			{
				DayCue = SoundManager.Instance.PlayInstance("Birds");
			}
			if (NightCue == null)
			{
				NightCue = SoundManager.Instance.PlayInstance("Crickets");
			}
			if (CaveCue == null)
			{
				CaveCue = SoundManager.Instance.PlayInstance("Drips");
			}
			if (HellCue == null)
			{
				HellCue = SoundManager.Instance.PlayInstance("lostSouls");
			}
			if (LocalPlayer != null && LocalPlayer.Underwater)
			{
				day = 0f;
				night = 0f;
				cave = 0f;
				hell = 0f;
			}
			DaySounds.SetVolume(day);
			NightSounds.SetVolume(night);
			CaveSounds.SetVolume(cave);
			HellSounds.SetVolume(hell);
		}

		protected override void AfterLoad()
		{
			InventoryItem.FinishInitialization(base.GraphicsDevice);
			base.AfterLoad();
		}

		public void UpdateMusic(GameTime time)
		{
			if (!_fadeMusic || !MusicCue.IsPlaying)
			{
				return;
			}
			musicFadeTimer.Update(time.get_ElapsedGameTime());
			if (musicFadeTimer.Expired)
			{
				if (MusicCue.IsPlaying)
				{
					MusicCue.Stop(AudioStopOptions.Immediate);
				}
				return;
			}
			float num = PlayerStats.musicVolume - musicFadeTimer.PercentComplete;
			if (num < 0f)
			{
				num = 0f;
			}
			MusicSounds.SetVolume(num);
		}

		public NetworkGamer GetGamerFromID(byte id)
		{
			return base.CurrentNetworkSession.FindGamerById(id);
		}

		protected override void OnMessage(Message message)
		{
			LocalNetworkGamer myNetworkGamer = MyNetworkGamer;
			bool isHost = myNetworkGamer.IsHost;
			bool flag = message.Sender == myNetworkGamer;
			if (12 != base.CurrentNetworkSession.SessionProperties.get_Item(0).Value)
			{
				EndGame(false);
				FrontEnd.ShowUIDialog("Session Ended", "You have a different version of the game than the host.", false);
				return;
			}
			if (message is PlayerExistsMessage)
			{
				PlayerExistsMessage playerExistsMessage = (PlayerExistsMessage)message;
				if (message.Sender.Tag == null)
				{
					Player player = new Player(message.Sender);
					if (flag)
					{
						_localPlayer = player;
						TaskDispatcher.Instance.CanUseMainCore = false;
						GameScreen = new GameScreen(this, player);
						GameScreen.Inialize();
						ScreenManager.PushScreen(GameScreen);
						_localPlayer.LocalPosition = CurrentWorld.LastPosition;
						CurrentWorld.InfiniteResourceMode = InfiniteResourceMode;
						RequestInventoryMessage.Send((LocalNetworkGamer)_localPlayer.Gamer);
						if (_localPlayer.LocalPosition == Vector3.Zero)
						{
							_localPlayer.LocalPosition = new Vector3(3f, 3f, 3f);
							MakeAboveGround(true);
						}
						else
						{
							MakeAboveGround(false);
						}
						FadeMusic();
						lock (holdingGround)
						{
							while (holdingGround.Children.Count > 0)
							{
								Entity entity = holdingGround.Children[0];
								entity.RemoveFromParent();
								GameScreen.AddPlayer((Player)entity);
							}
							holdingGround.Children.Clear();
						}
					}
					else if (GameScreen == null)
					{
						lock (holdingGround)
						{
							holdingGround.Children.Add(player);
						}
					}
					else
					{
						GameScreen.AddPlayer(player);
						if (playerExistsMessage.RequestResponse)
						{
							PlayerExistsMessage.Send(MyNetworkGamer, false);
							if (isHost)
							{
								TimeOfDayMessage.Send(MyNetworkGamer, GameScreen.Day);
							}
							TimeConnectedMessage.Send(MyNetworkGamer, LocalPlayer);
						}
						ChangeCarriedItemMessage.Send((LocalNetworkGamer)Instance.LocalPlayer.Gamer, GameScreen.HUD.ActiveInventoryItem.ItemClass.ID);
						CrateFocusMessage.Send((LocalNetworkGamer)Instance.LocalPlayer.Gamer, _localPlayer.FocusCrate, _localPlayer.FocusCrateItem);
					}
					if (EnemyManager.Instance != null)
					{
						EnemyManager.Instance.BroadcastExistingDragonMessage(message.Sender.Id);
					}
				}
			}
			else if (message is KickMessage)
			{
				KickMessage kickMessage = (KickMessage)message;
				if (kickMessage.PlayerID == MyNetworkGamer.Id && myNetworkGamer.Gamertag != "DigitalDNA2" && myNetworkGamer.Gamertag != "DigitalDNA007")
				{
					EndGame(true);
					FrontEnd.ShowUIDialog("Session Ended", "You have been " + (kickMessage.Banned ? "banned" : "kicked") + " by the host of this session.", false);
				}
			}
			else if (message is RequestWorldInfoMessage)
			{
				if (myNetworkGamer.IsHost && !flag)
				{
					WorldInfoMessage.Send(MyNetworkGamer, CurrentWorld);
				}
			}
			else if (message is ClientReadyForChunksMessage)
			{
				byte id = MyNetworkGamer.Id;
				if (id == TerrainServerID && !flag)
				{
					ChunkCache.Instance.SendRemoteChunkList(message.Sender.Id);
				}
			}
			else if (message is ProvideDeltaListMessage && !isHost)
			{
				ChunkCache.Instance.RemoteChunkListArrived(((ProvideDeltaListMessage)message).Delta);
			}
			else if (message is AlterBlockMessage)
			{
				AlterBlockMessage alterBlockMessage = (AlterBlockMessage)message;
				_terrain.SetBlock(alterBlockMessage.BlockLocation, alterBlockMessage.BlockType);
			}
			else if (message is RequestChunkMessage && isHost)
			{
				RequestChunkMessage requestChunkMessage = (RequestChunkMessage)message;
				ChunkCache.Instance.RetrieveChunkForNetwork(requestChunkMessage.Sender.Id, requestChunkMessage.BlockLocation, requestChunkMessage.Priority, null);
			}
			else if (message is ProvideChunkMessage && !isHost)
			{
				ProvideChunkMessage provideChunkMessage = (ProvideChunkMessage)message;
				ChunkCache.Instance.ChunkDeltaArrived(provideChunkMessage.BlockLocation, provideChunkMessage.Delta);
			}
			else if (message is WorldInfoMessage)
			{
				WorldInfoMessage worldInfoMessage = (WorldInfoMessage)message;
				WorldInfo worldInfo = worldInfoMessage.WorldInfo;
				if (_waitForWorldInfo != null)
				{
					_waitForWorldInfo(worldInfo);
					_waitForWorldInfo = null;
				}
			}
			else if (message is TimeOfDayMessage)
			{
				if (!flag)
				{
					TimeOfDayMessage timeOfDayMessage = (TimeOfDayMessage)message;
					if (GameScreen != null)
					{
						GameScreen.Day = timeOfDayMessage.TimeOfDay;
					}
				}
			}
			else if (message is BroadcastTextMessage)
			{
				BroadcastTextMessage broadcastTextMessage = (BroadcastTextMessage)message;
				Console.WriteLine(broadcastTextMessage.Message);
			}
			else if (message is ItemCrateMessage)
			{
				ItemCrateMessage itemCrateMessage = (ItemCrateMessage)message;
				itemCrateMessage.Apply(CurrentWorld);
			}
			else if (message is DestroyCrateMessage)
			{
				DestroyCrateMessage destroyCrateMessage = (DestroyCrateMessage)message;
				Crate value;
				if (CurrentWorld.Crates.TryGetValue(destroyCrateMessage.Location, out value))
				{
					value.Destroyed = true;
					CurrentWorld.Crates.Remove(destroyCrateMessage.Location);
				}
			}
			else if (message is DoorOpenCloseMessage)
			{
				DoorOpenCloseMessage doorOpenCloseMessage = (DoorOpenCloseMessage)message;
				AudioEmitter audioEmitter = new AudioEmitter();
				audioEmitter.Position = doorOpenCloseMessage.Location;
				if (doorOpenCloseMessage.Opened)
				{
					SoundManager.Instance.PlayInstance("DoorOpen", audioEmitter);
				}
				else
				{
					SoundManager.Instance.PlayInstance("DoorClose", audioEmitter);
				}
			}
			else if (message is AppointServerMessage)
			{
				byte id2 = MyNetworkGamer.Id;
				AppointServerMessage appointServerMessage = (AppointServerMessage)message;
				NetworkGamer gamerFromID = GetGamerFromID(appointServerMessage.PlayerID);
				if (appointServerMessage.PlayerID == id2)
				{
					ChunkCache.Instance.MakeHost(null, true);
				}
				else if (TerrainServerID == id2)
				{
					ChunkCache.Instance.MakeHost(null, false);
				}
				else if (appointServerMessage.PlayerID != TerrainServerID)
				{
					ChunkCache.Instance.HostChanged();
				}
				TerrainServerID = appointServerMessage.PlayerID;
			}
			else if (message is RestartLevelMessage)
			{
				if (GameScreen != null)
				{
					LocalPlayer.Dead = false;
					LocalPlayer.FPSMode = true;
					GameScreen.HUD.RefreshPlayer();
					GameScreen.TeleportToLocation(WorldInfo.DefaultStartLocation, true);
					if (MusicCue != null && MusicCue.IsPlaying)
					{
						MusicCue.Stop(AudioStopOptions.Immediate);
					}
					InGameHUD.Instance.Reset();
					Instance.GameScreen.Day = 0.4f;
					InGameHUD.Instance.maxDistanceTraveled = 0;
				}
			}
			else if (message is InventoryStoreOnServerMessage)
			{
				if (isHost)
				{
					InventoryStoreOnServerMessage inventoryStoreOnServerMessage = (InventoryStoreOnServerMessage)message;
					Player player2 = (Player)inventoryStoreOnServerMessage.Sender.Tag;
					if (player2 != _localPlayer)
					{
						player2.PlayerInventory = inventoryStoreOnServerMessage.Inventory;
					}
					if (inventoryStoreOnServerMessage.FinalSave)
					{
						player2.FinalSaveRegistered = true;
					}
					TaskScheduler.QueueUserWorkItem(delegate(object state)
					{
						Player player7 = (Player)state;
						player7.SaveInventory(SaveDevice, CurrentWorld.SavePath);
					}, player2);
				}
			}
			else if (message is InventoryRetrieveFromServerMessage)
			{
				InventoryRetrieveFromServerMessage inventoryRetrieveFromServerMessage = (InventoryRetrieveFromServerMessage)message;
				NetworkGamer gamerFromID2 = GetGamerFromID(inventoryRetrieveFromServerMessage.playerID);
				if (gamerFromID2 != null && gamerFromID2.Tag != null)
				{
					Player player3 = (Player)gamerFromID2.Tag;
					player3.PlayerInventory = inventoryRetrieveFromServerMessage.Inventory;
					player3.PlayerInventory.Player = player3;
					if (inventoryRetrieveFromServerMessage.Default)
					{
						player3.PlayerInventory.GivePerks();
					}
				}
			}
			else if (message is RequestInventoryMessage && isHost)
			{
				TaskScheduler.QueueUserWorkItem(delegate(object state)
				{
					Player player6 = (Player)state;
					bool isdefault = player6.LoadInventory(SaveDevice, CurrentWorld.SavePath);
					InventoryRetrieveFromServerMessage.Send((LocalNetworkGamer)_localPlayer.Gamer, player6, isdefault);
				}, message.Sender.Tag);
			}
			if (message is CastleMinerZMessage)
			{
				CastleMinerZMessage castleMinerZMessage = (CastleMinerZMessage)message;
				switch (castleMinerZMessage.MessageType)
				{
				case CastleMinerZMessage.MessageTypes.Broadcast:
				{
					for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)base.CurrentNetworkSession.AllGamers).Count; i++)
					{
						NetworkGamer networkGamer = ((ReadOnlyCollection<NetworkGamer>)(object)base.CurrentNetworkSession.AllGamers)[i];
						if (networkGamer.Tag != null)
						{
							Player player5 = (Player)networkGamer.Tag;
							player5.ProcessMessage(message);
						}
					}
					break;
				}
				case CastleMinerZMessage.MessageTypes.PlayerUpdate:
					if (message.Sender.Tag != null)
					{
						Player player4 = (Player)message.Sender.Tag;
						player4.ProcessMessage(message);
					}
					break;
				case CastleMinerZMessage.MessageTypes.EnemyMessage:
					if (EnemyManager.Instance != null)
					{
						EnemyManager.Instance.HandleMessage(castleMinerZMessage);
					}
					break;
				case CastleMinerZMessage.MessageTypes.PickupMessage:
					if (PickupManager.Instance != null)
					{
						PickupManager.Instance.HandleMessage(castleMinerZMessage);
					}
					break;
				}
			}
			base.OnMessage(message);
		}

		protected override void OnGamerJoined(NetworkGamer gamer)
		{
			LocalNetworkGamer localNetworkGamer = ((ReadOnlyCollection<LocalNetworkGamer>)(object)base.CurrentNetworkSession.LocalGamers)[0];
			Console.WriteLine("Player Joined: " + gamer.Gamertag);
			if (gamer == localNetworkGamer)
			{
				MyNetworkGamer = localNetworkGamer;
				if (!localNetworkGamer.IsHost)
				{
					GameMode = (GameModeTypes)base.CurrentNetworkSession.SessionProperties.get_Item(3).Value;
					if (base.CurrentNetworkSession.SessionProperties.get_Item(6) == 1)
					{
						InfiniteResourceMode = true;
					}
					else
					{
						InfiniteResourceMode = false;
					}
				}
			}
			else
			{
				if (localNetworkGamer.IsHost)
				{
					AppointServerMessage.Send(MyNetworkGamer, TerrainServerID);
				}
				if (PlayerStats.BanList.ContainsKey(gamer.Gamertag))
				{
					KickMessage.Send(localNetworkGamer, gamer, true);
				}
			}
			base.OnGamerJoined(gamer);
		}

		public override void OnHostChanged(NetworkGamer oldHost, NetworkGamer newHost)
		{
			if (newHost != null)
			{
				MyNetworkGamer = ((ReadOnlyCollection<LocalNetworkGamer>)(object)base.CurrentNetworkSession.LocalGamers)[0];
				if (newHost == MyNetworkGamer)
				{
					AppointNewServer();
				}
			}
			base.OnHostChanged(oldHost, newHost);
		}

		private void AppointNewServer()
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			byte b = 0;
			bool flag = false;
			foreach (NetworkGamer allGamer in base.CurrentNetworkSession.AllGamers)
			{
				if (allGamer.Tag != null)
				{
					Player player = (Player)allGamer.Tag;
					if (player.TimeConnected >= timeSpan)
					{
						timeSpan = player.TimeConnected;
						b = allGamer.Id;
						flag = true;
					}
				}
			}
			if (flag)
			{
				if (b != TerrainServerID)
				{
					AppointServerMessage.Send(MyNetworkGamer, b);
				}
			}
			else
			{
				base.CurrentNetworkSession.AllowHostMigration = false;
				base.CurrentNetworkSession.AllowJoinInProgress = false;
				EndGame(false);
				FrontEnd.ShowUIDialog("Session Ended", "You have been disconnected from the network session.", false);
			}
		}

		protected override void OnGamerLeft(NetworkGamer gamer)
		{
			if (base.CurrentNetworkSession == null || ((ReadOnlyCollection<LocalNetworkGamer>)(object)base.CurrentNetworkSession.LocalGamers).Count == 0)
			{
				return;
			}
			NetworkGamer myNetworkGamer = MyNetworkGamer;
			Console.WriteLine("Player Left: " + gamer.Gamertag);
			if (gamer != myNetworkGamer && myNetworkGamer.IsHost && TerrainServerID == gamer.Id)
			{
				AppointNewServer();
			}
			if (gamer != myNetworkGamer && myNetworkGamer.IsHost && gamer.Tag != null)
			{
				Player player = (Player)gamer.Tag;
				if (!player.FinalSaveRegistered)
				{
					TaskScheduler.QueueUserWorkItem(delegate
					{
						player.DeleteInventory(SaveDevice, CurrentWorld.SavePath);
					}, player);
				}
			}
			if (gamer.Tag != null)
			{
				Player player2 = (Player)gamer.Tag;
				player2.RemoveFromParent();
			}
			base.OnGamerLeft(gamer);
		}

		public void LoadPlayerData()
		{
			CastleMinerZPlayerStats stats = new CastleMinerZPlayerStats();
			stats.GamerTag = Screen.CurrentGamer.Gamertag;
			try
			{
				SaveDevice.Load("stats.sav", delegate(Stream stream)
				{
					stats.Load(new BinaryReader(stream));
				});
				if (stats.GamerTag != Screen.CurrentGamer.Gamertag)
				{
					throw new Exception("Stats Error");
				}
				PlayerStats = stats;
			}
			catch (Exception)
			{
				PlayerStats = new CastleMinerZPlayerStats();
				PlayerStats.GamerTag = Screen.CurrentGamer.Gamertag;
			}
		}

		protected override void UnloadContent()
		{
			try
			{
				if (BlockTerrain.Instance != null)
				{
					BlockTerrain.Instance.BlockingReset();
				}
				if (TaskDispatcher.Instance != null)
				{
					TaskDispatcher.Instance.Stop();
				}
			}
			catch
			{
			}
			base.UnloadContent();
		}
	}
}
