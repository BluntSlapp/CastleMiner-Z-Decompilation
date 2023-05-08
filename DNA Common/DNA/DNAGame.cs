using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using DNA.Audio;
using DNA.Drawing.UI;
using DNA.Input;
using DNA.Net;
using DNA.Profiling;
using DNA.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA
{
	public class DNAGame : Game
	{
		private enum LoadStatus
		{
			NotStarted,
			InProcess,
			Complete
		}

		public enum ScreenModes
		{
			Mode1080,
			Mode720
		}

		private enum CodeVal
		{
			None = 0,
			Up = 1,
			Down = 2,
			Left = 4,
			Right = 8,
			A = 0x10,
			B = 0x20,
			X = 0x40,
			Y = 0x80
		}

		private bool processMessages;

		public static Random Random = new Random();

		private Version _version;

		private static DateTime _gameStartTime = DateTime.UtcNow;

		public bool PauseDuringGuide = true;

		public bool Stop;

		public DialogManager DialogManager;

		protected GraphicsDeviceManager Graphics;

		protected ScreenGroup ScreenManager = new ScreenGroup(false);

		protected SpriteBatch SpriteBatch;

		protected InputManager InputManager = new InputManager();

		private bool _firstFrame = true;

		private NetworkSession _networkSession;

		private VoiceChat _voiceChat;

		public Texture2D DummyTexture;

		private GameTime _currentGameTime = new GameTime();

		public bool LimitElapsedGameTime = true;

		public TaskScheduler TaskScheduler = new TaskScheduler();

		private LoadStatus _loadStatus;

		public bool ShowTitleSafeArea = true;

		public SpriteFont DebugFont;

		public bool CheatsEnabled;

		private CodeVal[] konamiCode = new CodeVal[10]
		{
			CodeVal.Up,
			CodeVal.Up,
			CodeVal.Down,
			CodeVal.Down,
			CodeVal.Left,
			CodeVal.Right,
			CodeVal.Left,
			CodeVal.Right,
			CodeVal.B,
			CodeVal.A
		};

		private int CodeLimit = 10;

		private Queue<CodeVal> recentCodes = new Queue<CodeVal>();

		private bool _doAfterLoad;

		private Exception LastException;

		private bool _doSystemUpdates = true;

		public float Brightness;

		public Version Version
		{
			get
			{
				return _version;
			}
		}

		public bool Loading
		{
			get
			{
				return _loadStatus != LoadStatus.Complete;
			}
		}

		public GameTime CurrentGameTime
		{
			get
			{
				return _currentGameTime;
			}
			set
			{
				if (!LimitElapsedGameTime || value.get_ElapsedGameTime() <= TimeSpan.FromSeconds(0.1))
				{
					_currentGameTime = value;
				}
				else
				{
					_currentGameTime = new GameTime(value.get_TotalGameTime(), TimeSpan.FromSeconds(0.1), true);
				}
			}
		}

		public NetworkSession CurrentNetworkSession
		{
			get
			{
				return _networkSession;
			}
		}

		public void WaitforSave()
		{
		}

		protected void StartVoiceChat(LocalNetworkGamer gamer)
		{
			_voiceChat = new VoiceChat(gamer);
		}

		protected void ShowSignIn()
		{
			DialogManager.ShowSignIn(false);
		}

		public void ShowMarketPlace()
		{
			DialogManager.ShowMarketPlace(Screen.SelectedPlayerIndex.Value);
		}

		public void ShowMarketPlace(PlayerIndex player)
		{
			DialogManager.ShowMarketPlace(player);
		}

		private string GetLocalizedAssetName(string assetName)
		{
			string[] array = new string[2]
			{
				CultureInfo.CurrentCulture.Name,
				CultureInfo.CurrentCulture.TwoLetterISOLanguageName
			};
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = assetName + '.' + text;
				string path = Path.Combine(base.Content.RootDirectory, text2 + ".xnb");
				if (File.Exists(path))
				{
					return text2;
				}
			}
			return assetName;
		}

		public Texture2D LoadLocalizedImage(string name)
		{
			string localizedAssetName = GetLocalizedAssetName(name);
			return base.Content.Load<Texture2D>(localizedAssetName);
		}

		public void StartGamerServices()
		{
			((Collection<IGameComponent>)(object)base.Components).Add((IGameComponent)new GamerServicesComponent(this));
		}

		public DNAGame(bool PreferMultiSampling, Version version)
		{
			_version = version;
			TaskScheduler.ThreadException += TaskScheduler_ThreadException;
			DialogManager = new DialogManager(this);
			Graphics = new GraphicsDeviceManager(this);
			base.Content.RootDirectory = "Content";
			Screen.PlayerSignedIn += Screen_PlayerSignedIn;
			Screen.PlayerSignedOut += Screen_PlayerSignedOut;
			Graphics.PreferredBackBufferWidth = 1280;
			Graphics.PreferredBackBufferHeight = 720;
			Graphics.PreferMultiSampling = PreferMultiSampling;
		}

		private void TaskScheduler_ThreadException(object sender, TaskScheduler.ExceptionEventArgs e)
		{
			CrashGame(e.InnerException);
		}

		protected void WantProfiling(bool fixTimeStep, bool syncRetrace)
		{
			Profiler.CreateComponent(this);
			base.IsFixedTimeStep = fixTimeStep;
			Graphics.SynchronizeWithVerticalRetrace = syncRetrace;
			Profiler.Profiling = true;
			Profiler.SetColor("Update", Color.DarkBlue);
			Profiler.SetColor("Physics", Color.DarkRed);
			Profiler.SetColor("Collision", Color.Chocolate);
			Profiler.SetColor("Drawing", Color.DarkGreen);
			Profiler.SetColor("UpdateTransform", Color.DarkGoldenrod);
			Profiler.SetColor("SetDefPose", Color.DarkGray);
			Profiler.SetColor("AnimPlrUpdate", Color.DarkOrange);
			Profiler.SetColor("CopyTforms", Color.DarkSlateBlue);
		}

		private void Screen_PlayerSignedOut(object sender, SignedOutEventArgs e)
		{
			OnPlayerSignedOut(e.Gamer);
		}

		private void Screen_PlayerSignedIn(object sender, SignedInEventArgs e)
		{
			OnPlayerSignedIn(e.Gamer);
		}

		protected virtual void OnPlayerSignedIn(SignedInGamer gamer)
		{
		}

		protected virtual void OnPlayerSignedOut(SignedInGamer gamer)
		{
		}

		public void LeaveGame()
		{
			if (_networkSession != null)
			{
				_networkSession.Dispose();
			}
			_networkSession = null;
		}

		private void RegisterNetworkCallbacks(NetworkSession session)
		{
			session.add_GamerJoined((EventHandler<GamerJoinedEventArgs>)_networkSession_GamerJoined);
			session.add_GamerLeft((EventHandler<GamerLeftEventArgs>)_networkSession_GamerLeft);
			session.add_GameEnded((EventHandler<GameEndedEventArgs>)_networkSession_GameEnded);
			session.add_GameStarted((EventHandler<GameStartedEventArgs>)_networkSession_GameStarted);
			session.add_HostChanged((EventHandler<HostChangedEventArgs>)_networkSession_HostChanged);
			session.add_SessionEnded((EventHandler<NetworkSessionEndedEventArgs>)_networkSession_SessionEnded);
		}

		public void HostGame(NetworkSessionType sessionType, NetworkSessionProperties properties, IList<SignedInGamer> gamers, int maxPlayers, bool hostMigration, bool joinInprogress, SuccessCallback callback)
		{
			processMessages = false;
			NetworkSession.BeginCreate(sessionType, (IEnumerable<SignedInGamer>)gamers, maxPlayers, 0, properties, (AsyncCallback)delegate(IAsyncResult result)
			{
				SuccessCallback successCallback = (SuccessCallback)result.AsyncState;
				try
				{
					_networkSession = NetworkSession.EndCreate(result);
					_networkSession.AllowHostMigration = hostMigration;
					_networkSession.AllowJoinInProgress = joinInprogress;
					RegisterNetworkCallbacks(_networkSession);
				}
				catch (Exception)
				{
					if (successCallback != null)
					{
						successCallback(false);
					}
					processMessages = true;
					return;
				}
				if (successCallback != null)
				{
					successCallback(true);
				}
				processMessages = true;
			}, (object)callback);
		}

		public void JoinInvitedGame(IList<SignedInGamer> gamers, SuccessCallback callback)
		{
			processMessages = false;
			try
			{
				NetworkSession.BeginJoinInvited((IEnumerable<SignedInGamer>)gamers, (AsyncCallback)delegate(IAsyncResult result)
				{
					SuccessCallback successCallback = (SuccessCallback)result.AsyncState;
					bool success = true;
					try
					{
						_networkSession = NetworkSession.EndJoinInvited(result);
						RegisterNetworkCallbacks(_networkSession);
					}
					catch
					{
						_networkSession = null;
						success = false;
					}
					if (successCallback != null)
					{
						successCallback(success);
					}
					processMessages = true;
				}, (object)callback);
			}
			catch
			{
				if (callback != null)
				{
					callback(false);
				}
			}
		}

		public void JoinGame(AvailableNetworkSession session, SuccessCallback callback)
		{
			processMessages = false;
			NetworkSession.BeginJoin(session, (AsyncCallback)delegate(IAsyncResult result)
			{
				bool success = true;
				SuccessCallback successCallback = (SuccessCallback)result.AsyncState;
				try
				{
					_networkSession = NetworkSession.EndJoin(result);
					RegisterNetworkCallbacks(_networkSession);
				}
				catch (Exception)
				{
					_networkSession = null;
					success = false;
				}
				if (successCallback != null)
				{
					successCallback(success);
				}
				processMessages = true;
			}, (object)callback);
		}

		private void _networkSession_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
		{
			OnSessionEnded(e.EndReason);
		}

		public virtual void OnSessionEnded(NetworkSessionEndReason reason)
		{
		}

		private void _networkSession_HostChanged(object sender, HostChangedEventArgs e)
		{
			OnHostChanged(e.OldHost, e.NewHost);
		}

		public virtual void OnHostChanged(NetworkGamer oldHost, NetworkGamer newHost)
		{
		}

		private void _networkSession_GameStarted(object sender, GameStartedEventArgs e)
		{
			OnGameStarted();
		}

		public virtual void OnGameStarted()
		{
		}

		private void _networkSession_GameEnded(object sender, GameEndedEventArgs e)
		{
			OnGameEnded();
		}

		public virtual void OnGameEnded()
		{
		}

		private void _networkSession_GamerJoined(object sender, GamerJoinedEventArgs e)
		{
			OnGamerJoined(e.Gamer);
		}

		protected virtual void OnGamerJoined(NetworkGamer gamer)
		{
		}

		private void _networkSession_GamerLeft(object sender, GamerLeftEventArgs e)
		{
			OnGamerLeft(e.Gamer);
		}

		protected virtual void OnGamerLeft(NetworkGamer gamer)
		{
		}

		protected override void Initialize()
		{
			DummyTexture = new Texture2D(base.GraphicsDevice, 1, 1);
			DummyTexture.SetData<Color>(new Color[1] { Color.White });
			SpriteBatch = new SpriteBatch(base.GraphicsDevice);
			base.Initialize();
		}

		protected override void LoadContent()
		{
			DebugFont = base.Content.Load<SpriteFont>("Debug");
			_loadStatus = LoadStatus.InProcess;
			TaskScheduler.QueueUserWorkItem(LoadThreadRoutine);
			base.LoadContent();
		}

		public void CrashGame(Exception e)
		{
			if (LastException == null)
			{
				LastException = e;
			}
		}

		private void LoadThreadRoutine()
		{
			SecondaryLoad();
			_loadStatus = LoadStatus.Complete;
			_doAfterLoad = true;
		}

		protected virtual void SecondaryLoad()
		{
		}

		protected virtual void SendNetworkUpdates(NetworkSession session, GameTime gameTime)
		{
		}

		protected virtual void LoadingUpdate(GameTime gameTime)
		{
		}

		public void SuspendSystemUpdates()
		{
			_doSystemUpdates = false;
		}

		public void ResumeSystemUpdates()
		{
			_doSystemUpdates = true;
		}

		protected override void Update(GameTime gameTime)
		{
			if (LastException != null)
			{
				throw LastException;
			}
			if (_doAfterLoad)
			{
				AfterLoad();
				_doAfterLoad = false;
			}
			Profiler.MarkFrame();
			CurrentGameTime = gameTime;
			SoundManager.Instance.Update();
			if (Stop)
			{
				return;
			}
			bool flag = false;
			try
			{
				if (Guide.IsVisible)
				{
					flag = true;
				}
			}
			catch
			{
			}
			if (!flag || !PauseDuringGuide)
			{
				if (_firstFrame)
				{
					OnFirstFrame();
					_firstFrame = false;
				}
				if (CurrentNetworkSession != null)
				{
					CurrentNetworkSession.Update();
					if (CurrentNetworkSession != null && processMessages)
					{
						ProcessNetworkMessages(CurrentGameTime);
						if (_networkSession != null)
						{
							SendNetworkUpdates(_networkSession, CurrentGameTime);
						}
					}
				}
				InputManager.Update();
				ScreenManager.ProcessInput(InputManager, CurrentGameTime);
				ScreenManager.Update(this, CurrentGameTime);
				EvalCodes();
				if (LastException != null)
				{
					throw LastException;
				}
				if (ScreenManager.Exiting && !Loading)
				{
					Exit();
				}
			}
			DialogManager.Update(CurrentGameTime);
			if (_doSystemUpdates)
			{
				base.Update(CurrentGameTime);
			}
			if (LastException == null)
			{
				return;
			}
			throw LastException;
		}

		protected void ShowTrialWarning(PlayerIndex player)
		{
			DialogManager.ShowMessageBox(player, "Full Mode Only", "This feature is only availible in the full version of the game.\n\nWould you like to purchase the game now?", new string[2] { "Yes", "No" }, 0, MessageBoxIcon.Warning, delegate(int? index)
			{
				int valueOrDefault = index.GetValueOrDefault();
				if (index.HasValue && valueOrDefault == 0)
				{
					ShowMarketPlace(player);
				}
			}, player);
		}

		private void EvalCodes()
		{
			if (Guide.IsTrialMode)
			{
				return;
			}
			CodeVal codeVal = CodeVal.None;
			GameController gameController = InputManager.Controllers[1];
			if (gameController.PressedButtons.A)
			{
				codeVal |= CodeVal.A;
			}
			if (gameController.PressedButtons.B)
			{
				codeVal |= CodeVal.B;
			}
			if (gameController.PressedButtons.X)
			{
				codeVal |= CodeVal.X;
			}
			if (gameController.PressedButtons.Y)
			{
				codeVal |= CodeVal.Y;
			}
			if (gameController.PressedDPad.Up)
			{
				codeVal |= CodeVal.Up;
			}
			if (gameController.PressedDPad.Down)
			{
				codeVal |= CodeVal.Down;
			}
			if (gameController.PressedDPad.Left)
			{
				codeVal |= CodeVal.Left;
			}
			if (gameController.PressedDPad.Right)
			{
				codeVal |= CodeVal.Right;
			}
			if (codeVal != 0)
			{
				recentCodes.Enqueue(codeVal);
				while (recentCodes.Count > CodeLimit)
				{
					recentCodes.Dequeue();
				}
			}
			if (recentCodes.Count < konamiCode.Length)
			{
				return;
			}
			CodeVal[] array = recentCodes.ToArray();
			for (int i = 0; i < konamiCode.Length; i++)
			{
				if (konamiCode[i] != array[i])
				{
					return;
				}
			}
			recentCodes.Clear();
			CheatsEnabled = !CheatsEnabled;
		}

		protected virtual void OnFirstFrame()
		{
		}

		protected virtual void OnMessage(Message message)
		{
		}

		private void ProcessNetworkMessages(GameTime gameTime)
		{
			if (_networkSession == null)
			{
				return;
			}
			int num = 0;
			while (_networkSession != null && num < ((ReadOnlyCollection<LocalNetworkGamer>)(object)_networkSession.LocalGamers).Count)
			{
				LocalNetworkGamer localNetworkGamer = ((ReadOnlyCollection<LocalNetworkGamer>)(object)_networkSession.LocalGamers)[num];
				while (_networkSession != null && localNetworkGamer.IsDataAvailable)
				{
					try
					{
						Message message = Message.GetMessage(localNetworkGamer);
						if (message is VoiceChatMessage)
						{
							if (_voiceChat != null)
							{
								_voiceChat.ProcessMessage((VoiceChatMessage)message);
							}
						}
						else if (message.Echo || !message.Sender.IsLocal)
						{
							OnMessage(message);
						}
					}
					catch (InvalidMessageException ex)
					{
						if (_networkSession.IsHost)
						{
							try
							{
								ex.Sender.Machine.RemoveFromSession();
							}
							catch
							{
							}
						}
						if (ex.Sender.IsHost)
						{
							LeaveGame();
						}
					}
				}
				num++;
			}
		}

		private void DrawBrightness()
		{
			Viewport viewport = base.GraphicsDevice.Viewport;
			SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
			SpriteBatch.Draw(DummyTexture, viewport.Bounds, new Color(Brightness, Brightness, Brightness));
			SpriteBatch.End();
		}

		private void DrawTitleSafeArea()
		{
			Viewport viewport = base.GraphicsDevice.Viewport;
			Rectangle titleSafeArea = viewport.TitleSafeArea;
			int num = viewport.X + viewport.Width;
			int num2 = viewport.Y + viewport.Height;
			Rectangle destinationRectangle = new Rectangle(viewport.X, viewport.Y, titleSafeArea.X - viewport.X, viewport.Height);
			Rectangle destinationRectangle2 = new Rectangle(titleSafeArea.Right, viewport.Y, num - titleSafeArea.Right, viewport.Height);
			Rectangle destinationRectangle3 = new Rectangle(titleSafeArea.Left, viewport.Y, titleSafeArea.Width, titleSafeArea.Top - viewport.Y);
			Rectangle destinationRectangle4 = new Rectangle(titleSafeArea.Left, titleSafeArea.Bottom, titleSafeArea.Width, num2 - titleSafeArea.Bottom);
			Color color = new Color(1f, 0f, 0f, 0.5f);
			SpriteBatch.Begin();
			SpriteBatch.Draw(DummyTexture, destinationRectangle, color);
			SpriteBatch.Draw(DummyTexture, destinationRectangle2, color);
			SpriteBatch.Draw(DummyTexture, destinationRectangle3, color);
			SpriteBatch.Draw(DummyTexture, destinationRectangle4, color);
			SpriteBatch.End();
		}

		public static void Run<T>(string errorURL, string name) where T : DNAGame, new()
		{
			Version version = new Version(0, 0);
			if (Debugger.IsAttached)
			{
				T val = new T();
				try
				{
					version = val.Version;
					val.Run();
					return;
				}
				finally
				{
					if (val != null)
					{
						((IDisposable)val).Dispose();
					}
				}
			}
			try
			{
				T val2 = new T();
				try
				{
					version = val2.Version;
					val2.Run();
				}
				finally
				{
					if (val2 != null)
					{
						((IDisposable)val2).Dispose();
					}
				}
			}
			catch (Exception e)
			{
				using (ExceptionGame exceptionGame = new ExceptionGame(e, errorURL, name, version, _gameStartTime))
				{
					exceptionGame.Run();
				}
			}
		}

		protected virtual void AfterLoad()
		{
		}

		protected override void Draw(GameTime gameTime)
		{
			if (Stop)
			{
				base.GraphicsDevice.Clear(Color.Black);
				return;
			}
			if (Loading)
			{
				base.GraphicsDevice.Clear(Color.Black);
			}
			ScreenManager.Draw(base.GraphicsDevice, SpriteBatch, gameTime);
			DrawBrightness();
			base.Draw(gameTime);
		}
	}
}
