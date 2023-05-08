using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DNA.Audio;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using DNA.Drawing.Particles;
using DNA.Drawing.UI;
using DNA.Input;
using DNA.Text;
using DNA.Timers;
using DNA.Triggers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.UI
{
	public class InGameHUD : Screen
	{
		public class DamageIndicator
		{
			public Vector3 DamageSource;

			public OneShotTimer drawTimer;

			public float fadeAmount
			{
				get
				{
					if ((double)drawTimer.PercentComplete < 0.67)
					{
						return 1f;
					}
					return (1f - drawTimer.PercentComplete) * 3f;
				}
			}

			public DamageIndicator(Vector3 source)
			{
				DamageSource = source;
				drawTimer = new OneShotTimer(TimeSpan.FromSeconds(3.0));
			}
		}

		public const float MaxHealth = 1f;

		public const float MaxOxygen = 1f;

		private const float PROBE_LENGTH = 5f;

		public static InGameHUD Instance;

		public int maxDistanceTraveled;

		public float PlayerHealth = 1f;

		public float HealthRecoverRate = 0.75f;

		public OneShotTimer HealthRecoverTimer = new OneShotTimer(TimeSpan.FromSeconds(3.0));

		public float PlayerOxygen = 1f;

		public float OxygenDecayRate = 0.1f;

		public float OxygenHealthPenaltyRate = 0.2f;

		private float maxGunCameraShift = 0.04f;

		private Vector2 GunEyePointCameraLocation = Vector2.Zero;

		private int lightningFlashCount;

		private OneShotTimer timeToLightning;

		private OneShotTimer timeToThunder;

		private OneShotTimer timeToRespawn = new OneShotTimer(TimeSpan.FromSeconds(20.0));

		private OneShotTimer timeToShowRespawnText = new OneShotTimer(TimeSpan.FromSeconds(3.0));

		private OneShotTimer fadeInGameStart = new OneShotTimer(TimeSpan.FromSeconds(1.0));

		private Random rand = new Random();

		private List<DNA.Triggers.Trigger> _triggers = new List<DNA.Triggers.Trigger>();

		private bool gameBegun;

		private Angle compassRotation;

		private Vector3 trialMaxPosition;

		private DialogScreen _travelMaxDialog;

		private CrateScreen _crateScreen;

		public OneShotTimer drawDayTimer = new OneShotTimer(TimeSpan.FromSeconds(9.0));

		public int currentDay;

		private List<DamageIndicator> _damageIndicator = new List<DamageIndicator>();

		private Sprite _damageArrow;

		public ConstructionProbeClass ConstructionProbe = new ConstructionProbeClass();

		private ConsoleElement console;

		private CastleMinerZGame _game;

		private Sprite _gridSprite;

		private Sprite _selectorSprite;

		private Sprite _crosshair;

		private Sprite _emptyHealthBar;

		private Sprite _fullHealthBar;

		private Sprite _bubbleBar;

		private Sprite _compass;

		private Queue<AchievementManager<CastleMinerZPlayerStats>.Achievement> AcheivementsToDraw = new Queue<AchievementManager<CastleMinerZPlayerStats>.Achievement>();

		private AchievementManager<CastleMinerZPlayerStats>.Achievement displayedAcheivement;

		private OneShotTimer acheivementDisplayTimer = new OneShotTimer(TimeSpan.FromSeconds(10.0));

		private Vector2 acheimentDisplayLocation = new Vector2(453f, 439f);

		private string _achievementText1 = "";

		private string _achievementText2 = "";

		private int currentDistanceTraveled;

		private Vector2 _achievementLocation;

		private InventoryItem lastItem;

		private StringBuilder sbuilder = new StringBuilder();

		private StringBuilder distanceBuilder = new StringBuilder();

		private OneShotTimer lavaDamageTimer = new OneShotTimer(TimeSpan.FromSeconds(0.5));

		private bool lavaSoundPlayed;

		private Vector3 lastPosition = Vector3.Zero;

		private OneShotTimer _periodicSaveTimer = new OneShotTimer(TimeSpan.FromSeconds(10.0));

		private float lastTOD = -1f;

		private StringBuilder _builder = new StringBuilder();

		private bool _hideUI;

		private bool showPlayers;

		public PlayerInventory PlayerInventory
		{
			get
			{
				return LocalPlayer.PlayerInventory;
			}
		}

		private bool WaitToRespawn
		{
			get
			{
				if (_game.GameMode == GameModeTypes.Endurance && !timeToRespawn.Expired && LocalPlayer.Dead)
				{
					return _game.IsOnlineGame;
				}
				return false;
			}
		}

		public InventoryItem ActiveInventoryItem
		{
			get
			{
				return PlayerInventory.ActiveInventoryItem;
			}
		}

		public Player LocalPlayer
		{
			get
			{
				return _game.LocalPlayer;
			}
		}

		private BlockTerrain Terrain
		{
			get
			{
				return _game._terrain;
			}
		}

		public void ApplyDamage(float damageAmount, Vector3 damageSource)
		{
			if (!LocalPlayer.Dead)
			{
				_damageIndicator.Add(new DamageIndicator(damageSource));
				SoundManager.Instance.PlayInstance("Hit");
				HealthRecoverTimer.Reset();
				PlayerHealth -= damageAmount;
				if (PlayerHealth <= 0f)
				{
					PlayerHealth = 0f;
					KillPlayer();
				}
			}
		}

		public void KillPlayer()
		{
			CrateFocusMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, IntVector3.Zero, Point.Zero);
			SoundManager.Instance.PlayInstance("Fall");
			LocalPlayer.Dead = true;
			LocalPlayer.FPSMode = false;
			LocalPlayer.Avatar.HideHead = true;
			if (_game.Difficulty == GameDifficultyTypes.HARDCORE)
			{
				PlayerInventory.DropAll(true);
				PlayerInventory.SetDefaultInventory();
			}
			else
			{
				PlayerInventory.DropAll(false);
			}
			timeToRespawn = new OneShotTimer(TimeSpan.FromSeconds(20.0));
			timeToShowRespawnText = new OneShotTimer(TimeSpan.FromSeconds(3.0));
			if (_game.IsOnlineGame)
			{
				BroadcastTextMessage.Send(_game.MyNetworkGamer, LocalPlayer.Gamer.Gamertag + " Has Fallen");
			}
		}

		public void RespawnPlayer()
		{
			Player player = null;
			float num = float.MaxValue;
			foreach (NetworkGamer remoteGamer in _game.CurrentNetworkSession.RemoteGamers)
			{
				if (remoteGamer.Tag == null)
				{
					continue;
				}
				Player player2 = (Player)remoteGamer.Tag;
				if (player2 != null && !player2.Dead)
				{
					float num2 = player2.LocalPosition.LengthSquared();
					if (num2 < num)
					{
						player = player2;
						num = num2;
					}
				}
			}
			if (_game.GameMode != 0)
			{
				RefreshPlayer();
				_game.GameScreen.TeleportToLocation(WorldInfo.DefaultStartLocation, true);
				if (_game.IsOnlineGame)
				{
					BroadcastTextMessage.Send(_game.MyNetworkGamer, LocalPlayer.Gamer.Gamertag + " Has Respawned");
				}
			}
			else if (player == null)
			{
				if (_game.LocalPlayer.Gamer.IsHost && !_game.IsOnlineGame)
				{
					RestartLevelMessage.Send((LocalNetworkGamer)_game.LocalPlayer.Gamer);
				}
			}
			else
			{
				RefreshPlayer();
				_game.GameScreen.TeleportToLocation(player.LocalPosition, false);
				if (_game.IsOnlineGame)
				{
					BroadcastTextMessage.Send(_game.MyNetworkGamer, LocalPlayer.Gamer.Gamertag + " Has Respawned");
				}
			}
		}

		public bool AllPlayersDead()
		{
			Player player = null;
			foreach (NetworkGamer remoteGamer in _game.CurrentNetworkSession.RemoteGamers)
			{
				if (remoteGamer.Tag != null)
				{
					Player player2 = (Player)remoteGamer.Tag;
					if (player2 != null && !player2.Dead)
					{
						player = player2;
						break;
					}
				}
			}
			if (player == null)
			{
				return true;
			}
			return false;
		}

		public void RefreshPlayer()
		{
			LocalPlayer.Dead = false;
			LocalPlayer.FPSMode = true;
			PlayerHealth = 1f;
		}

		public static BlockTypeEnum GetBlock(IntVector3 worldPosition)
		{
			return BlockTerrain.Instance.GetBlockWithChanges(worldPosition);
		}

		public InGameHUD(CastleMinerZGame game)
			: base(true, false)
		{
			_triggers.Add(new TransitionMusicTrigger("Song6", 3400f));
			_triggers.Add(new TransitionMusicTrigger("Song5", 3000f));
			_triggers.Add(new TransitionMusicTrigger("Song4", 2300f));
			_triggers.Add(new TransitionMusicTrigger("Song3", 1600f));
			_triggers.Add(new TransitionMusicTrigger("Song2", 900f));
			_triggers.Add(new TransitionMusicTrigger("Song1", 200f));
			Instance = this;
			_game = game;
			_damageArrow = _game._uiSprites["DamageArrow"];
			_gridSprite = _game._uiSprites["HudGrid"];
			_selectorSprite = _game._uiSprites["Selector"];
			_crosshair = _game._uiSprites["CrossHair"];
			_emptyHealthBar = _game._uiSprites["HealthBarEmpty"];
			_fullHealthBar = _game._uiSprites["HealthBarFull"];
			_bubbleBar = _game._uiSprites["BubbleBar"];
			_compass = _game._uiSprites["Compass"];
			Rectangle titleSafeArea = _game.GraphicsDevice.Viewport.TitleSafeArea;
			console = new ConsoleElement(_game._consoleFont);
			console.GrabConsole();
			console.Location = new Vector2(titleSafeArea.Left, titleSafeArea.Top);
			console.Size = new Vector2((float)_game.GraphicsDevice.PresentationParameters.BackBufferWidth * 0.3f, (float)_game.GraphicsDevice.PresentationParameters.BackBufferHeight * 0.25f);
			timeToLightning = new OneShotTimer(TimeSpan.FromSeconds(rand.Next(5, 10)));
			timeToThunder = new OneShotTimer(TimeSpan.FromSeconds((float)rand.NextDouble() * 2f));
			lightningFlashCount = rand.Next(0, 4);
			_travelMaxDialog = new DialogScreen("Purchase Game", "You must purchase the game to travel further", null, false, _game.DialogScreenImage, _game._medFont, true);
			_travelMaxDialog.TitlePadding = new Vector2(55f, 15f);
			_travelMaxDialog.DescriptionPadding = new Vector2(25f, 35f);
			_travelMaxDialog.ButtonsPadding = new Vector2(25f, 20f);
			_travelMaxDialog.ClickSound = "Click";
			_travelMaxDialog.OpenSound = "Popup";
			_crateScreen = new CrateScreen(game, this);
		}

		public void DisplayAcheivement(AchievementManager<CastleMinerZPlayerStats>.Achievement acheivement)
		{
			AcheivementsToDraw.Enqueue(acheivement);
		}

		private void DrawAcheivement(GraphicsDevice device, SpriteBatch spriteBatch)
		{
			Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
			Sprite sprite = _game._uiSprites["AwardEnd"];
			Sprite sprite2 = _game._uiSprites["AwardCenter"];
			Sprite sprite3 = _game._uiSprites["AwardCircle"];
			float num = sprite.Width;
			Vector2 vector = new Vector2(79f, 10f);
			Vector2 vector2 = new Vector2(79f, 37f);
			float num2 = vector.X - num;
			Vector2 vector3 = _game._systemFont.MeasureString(_achievementText1);
			float num3 = Math.Max(val2: _game._systemFont.MeasureString(_achievementText2).X, val1: vector3.X) + num2;
			float num4 = num3 + num * 2f;
			float num5 = (float)titleSafeArea.Center.X - num4 / 2f;
			int num6 = (int)acheimentDisplayLocation.Y;
			_achievementLocation = new Vector2(num5, num6);
			sprite.Draw(spriteBatch, new Vector2(num5, num6), Color.White);
			sprite.Draw(spriteBatch, new Vector2(num5 + num3 + num, num6), 1f, Color.White, SpriteEffects.FlipHorizontally);
			sprite2.Draw(spriteBatch, new Rectangle((int)(num5 + num) - 1, num6, (int)(num3 + 2f), sprite2.Height), Color.White);
			sprite3.Draw(spriteBatch, new Vector2(num5, num6), Color.White);
			spriteBatch.DrawString(_game._systemFont, _achievementText1, vector + _achievementLocation, new Color(219, 219, 219));
			spriteBatch.DrawString(_game._systemFont, _achievementText2, vector2 + _achievementLocation, new Color(219, 219, 219));
		}

		private void EquipActiveItem()
		{
			if (lastItem != ActiveInventoryItem)
			{
				lastItem = ActiveInventoryItem;
				LocalPlayer.Equip(ActiveInventoryItem);
				if (ActiveInventoryItem is GunInventoryItem)
				{
					GunInventoryItem gunInventoryItem = (GunInventoryItem)ActiveInventoryItem;
					LocalPlayer.ReloadSound = gunInventoryItem.GunClass.ReloadSound;
				}
			}
		}

		private void UpdateAcheivements(GameTime gameTime)
		{
			_game.AcheivmentManager.Update();
			if (displayedAcheivement == null)
			{
				if (AcheivementsToDraw.Count > 0)
				{
					SoundManager.Instance.PlayInstance("Award");
					acheivementDisplayTimer.Reset();
					displayedAcheivement = AcheivementsToDraw.Dequeue();
					BroadcastTextMessage.Send(_game.MyNetworkGamer, LocalPlayer.Gamer.Gamertag + " has earned '" + displayedAcheivement.Name + "'");
					_achievementText2 = displayedAcheivement.HowToUnlock;
					_achievementText1 = displayedAcheivement.Name;
				}
			}
			else
			{
				acheivementDisplayTimer.Update(gameTime.get_ElapsedGameTime());
				if (acheivementDisplayTimer.Expired)
				{
					displayedAcheivement = null;
				}
			}
		}

		private void DrawAcheivements(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			float num = (float)device.Viewport.Height / 1080f;
			if (displayedAcheivement != null)
			{
				DrawAcheivement(device, spriteBatch);
			}
		}

		protected override void OnDraw(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			if (_hideUI)
			{
				return;
			}
			console.Draw(device, spriteBatch, gameTime, false);
			if (showPlayers)
			{
				DrawPlayerList(device, spriteBatch, gameTime);
			}
			else
			{
				Rectangle titleSafeArea = device.Viewport.TitleSafeArea;
				spriteBatch.Begin();
				int num = (int)((1f - PlayerHealth) * 120f);
				int r = (int)((1f - PlayerHealth) * 102f);
				if (num > 0)
				{
					spriteBatch.Draw(_game.DummyTexture, new Rectangle(0, 0, 1280, 720), new Color(r, 0, 0, num));
				}
				if (LocalPlayer.PercentSubmergedLava > 0f)
				{
					spriteBatch.Draw(_game.DummyTexture, new Rectangle(0, 0, 1280, 720), Color.Lerp(Color.Transparent, Color.Red, LocalPlayer.PercentSubmergedLava));
				}
				if (LocalPlayer.UnderLava)
				{
					spriteBatch.Draw(_game.DummyTexture, new Rectangle(0, 0, 1280, 720), Color.Red);
				}
				Rectangle sourceRectangle = new Rectangle(0, 0, _damageArrow.Width, _damageArrow.Height);
				Vector2 origin = new Vector2(_damageArrow.Width / 2, _damageArrow.Height + 150);
				Vector2 position = new Vector2(titleSafeArea.Center.X, titleSafeArea.Center.Y);
				int num2 = 0;
				while (num2 < _damageIndicator.Count)
				{
					_damageIndicator[num2].drawTimer.Update(gameTime.get_ElapsedGameTime());
					if (_damageIndicator[num2].drawTimer.Expired)
					{
						_damageIndicator.RemoveAt(num2);
						continue;
					}
					Vector3 normal = _damageIndicator[num2].DamageSource - LocalPlayer.LocalPosition;
					normal = Vector3.TransformNormal(normal, LocalPlayer.WorldToLocal);
					Angle rotation = Angle.ATan2(normal.X, 0f - normal.Z);
					_damageArrow.Draw(spriteBatch, position, sourceRectangle, new Color((int)(139f * _damageIndicator[num2].fadeAmount), 0, 0, (int)(255f * _damageIndicator[num2].fadeAmount)), rotation, origin, 0.75f, SpriteEffects.None, 0f);
					num2++;
				}
				if (LocalPlayer.Dead && timeToShowRespawnText.Expired && _game.GameScreen._uiGroup.CurrentScreen == this)
				{
					string text = "Press ";
					string text2 = " To Respawn";
					if (WaitToRespawn)
					{
						text = "Respawn In: ";
						text2 = "";
					}
					if (_game.IsOnlineGame && AllPlayersDead() && _game.GameMode == GameModeTypes.Endurance)
					{
						if (_game.CurrentNetworkSession.IsHost)
						{
							text = "Press ";
							text2 = " To Restart";
						}
						else
						{
							text = "Waiting For Host To Restart";
							text2 = "";
						}
					}
					Vector2 vector = _game._largeFont.MeasureString(text + text2);
					Vector2 vector2 = new Vector2((float)ControllerImages.A.Width * vector.Y / (float)ControllerImages.A.Height, vector.Y);
					Vector2 vector3 = new Vector2((float)titleSafeArea.Center.X - vector.X / 2f - vector2.X / 2f, (float)titleSafeArea.Center.Y - vector.Y / 2f);
					if (_game.GameMode == GameModeTypes.Endurance)
					{
						sbuilder.Length = 0;
						sbuilder.Append("Distance Traveled: ");
						sbuilder.Concat(maxDistanceTraveled);
						Vector2 vector4 = _game._largeFont.MeasureString(sbuilder);
						vector3 = new Vector2((float)titleSafeArea.Center.X - vector4.X / 2f, (float)titleSafeArea.Center.Y - vector4.Y - vector4.Y / 2f);
						spriteBatch.DrawOutlinedText(_game._largeFont, sbuilder, new Vector2(vector3.X, vector3.Y), Color.White, Color.Black, 2);
						sbuilder.Length = 0;
						sbuilder.Append(" In ");
						sbuilder.Concat(currentDay);
						sbuilder.Append(" Day");
						if (currentDay != 1)
						{
							sbuilder.Append("s");
						}
						vector4 = _game._largeFont.MeasureString(sbuilder);
						vector3 = new Vector2((float)titleSafeArea.Center.X - vector4.X / 2f, (float)titleSafeArea.Center.Y - vector4.Y / 2f);
						spriteBatch.DrawOutlinedText(_game._largeFont, sbuilder, new Vector2(vector3.X, vector3.Y), Color.White, Color.Black, 2);
						vector3 = new Vector2((float)titleSafeArea.Center.X - vector.X / 2f - vector2.X / 2f, (float)titleSafeArea.Center.Y - vector.Y / 2f + vector4.Y);
					}
					if ((WaitToRespawn || (AllPlayersDead() && !_game.CurrentNetworkSession.IsHost && _game.GameMode == GameModeTypes.Endurance)) && (!AllPlayersDead() || !_game.CurrentNetworkSession.IsHost))
					{
						vector3.X += vector2.X / 2f;
					}
					spriteBatch.DrawOutlinedText(_game._largeFont, text, new Vector2(vector3.X, vector3.Y), Color.White, Color.Black, 2);
					vector3.X += _game._largeFont.MeasureString(text).X;
					if (!WaitToRespawn || (AllPlayersDead() && _game.CurrentNetworkSession.IsHost && _game.GameMode == GameModeTypes.Endurance))
					{
						if (_game.GameMode != 0 || !AllPlayersDead() || _game.CurrentNetworkSession.IsHost)
						{
							spriteBatch.Draw(ControllerImages.A, new Rectangle((int)vector3.X, (int)vector3.Y, (int)vector2.X, (int)vector2.Y), Color.White);
							vector3.X += vector2.X;
							spriteBatch.DrawOutlinedText(_game._largeFont, text2, new Vector2(vector3.X, vector3.Y), Color.White, Color.Black, 2);
						}
					}
					else if (_game.IsOnlineGame && !AllPlayersDead() && _game.GameMode == GameModeTypes.Endurance)
					{
						sbuilder.Length = 0;
						sbuilder.Concat((int)(21.0 - timeToRespawn.ElaspedTime.TotalSeconds));
						spriteBatch.DrawOutlinedText(_game._largeFont, sbuilder, new Vector2(vector3.X, vector3.Y), Color.White, Color.Black, 2);
					}
				}
				float y = titleSafeArea.Y;
				distanceBuilder.Length = 0;
				distanceBuilder.Concat(currentDistanceTraveled);
				distanceBuilder.Append(" - ");
				distanceBuilder.Concat(maxDistanceTraveled);
				Vector2 vector5 = _game._medFont.MeasureString("Distance - Max");
				spriteBatch.DrawOutlinedText(_game._medFont, "Distance - Max", new Vector2((float)titleSafeArea.Right - vector5.X - 22f, titleSafeArea.Top), Color.White, Color.Black, 1);
				spriteBatch.DrawOutlinedText(_game._medFont, distanceBuilder, new Vector2((float)titleSafeArea.Right - vector5.X / 2f - _game._medFont.MeasureString(distanceBuilder).X / 2f, (float)titleSafeArea.Top + vector5.Y), Color.White, Color.Black, 1);
				if (ConstructionProbe.AbleToBuild && PlayerInventory.ActiveInventoryItem != null)
				{
					BlockTypeEnum block = GetBlock(ConstructionProbe._worldIndex);
					BlockType type = BlockType.GetType(block);
					spriteBatch.DrawOutlinedText(_game._medFont, type.Name, new Vector2((float)(titleSafeArea.X + titleSafeArea.Width / 2) - _game._medFont.MeasureString(type.ToString()).X / 2f, y), Color.White, Color.Black, 1);
				}
				Rectangle destinationRectangle = new Rectangle(titleSafeArea.Center.X - _gridSprite.Width / 2, titleSafeArea.Bottom - _gridSprite.Height, _gridSprite.Width, _gridSprite.Height);
				_gridSprite.Draw(spriteBatch, destinationRectangle, Color.White);
				Vector2 position2 = new Vector2(destinationRectangle.Left, destinationRectangle.Top - 30);
				float num3 = PlayerHealth / 1f;
				_emptyHealthBar.Draw(spriteBatch, position2, Color.White);
				_fullHealthBar.Draw(spriteBatch, position2, new Rectangle(0, 0, (int)((float)_fullHealthBar.Width * num3), _fullHealthBar.Height), Color.White);
				if (LocalPlayer.Underwater)
				{
					float num4 = PlayerOxygen / 1f;
					Vector2 vector6 = new Vector2(destinationRectangle.Center.X, destinationRectangle.Top - 30);
					_bubbleBar.Draw(spriteBatch, new Rectangle((int)(vector6.X + (float)_bubbleBar.Width * (1f - num4)), (int)vector6.Y, (int)((float)_bubbleBar.Width * num4), _bubbleBar.Height), new Rectangle((int)((float)_bubbleBar.Width * (1f - num4)), 0, (int)((float)_bubbleBar.Width * num4), _bubbleBar.Height), Color.White);
				}
				for (int i = 0; i < PlayerInventory.InventoryTray.Length; i++)
				{
					InventoryItem inventoryItem = PlayerInventory.InventoryTray[i];
					if (inventoryItem != null)
					{
						int x = 59 * i + destinationRectangle.Left + 2;
						inventoryItem.Draw2D(spriteBatch, new Rectangle(x, destinationRectangle.Top + 2, 64, 64));
					}
				}
				Rectangle destinationRectangle2 = new Rectangle(destinationRectangle.Left + 59 * PlayerInventory.SelectedInventoryIndex, destinationRectangle.Top, _selectorSprite.Width, _selectorSprite.Height);
				_selectorSprite.Draw(spriteBatch, destinationRectangle2, Color.White);
				sbuilder.Length = 0;
				if (ActiveInventoryItem != null)
				{
					Vector2 vector7 = _game._medFont.MeasureString(ActiveInventoryItem.Name);
					ActiveInventoryItem.GetDisplayText(sbuilder);
					spriteBatch.DrawOutlinedText(_game._medFont, sbuilder, new Vector2(destinationRectangle.Left, position2.Y - vector7.Y), Color.White, Color.Black, 2);
				}
				if (!LocalPlayer.Dead && !LocalPlayer.Shouldering)
				{
					_crosshair.Draw(spriteBatch, new Vector2(titleSafeArea.Center.X - _crosshair.Width / 2, titleSafeArea.Center.Y - _crosshair.Height / 2), Color.White);
				}
				if (!fadeInGameStart.Expired)
				{
					float num5 = (float)fadeInGameStart.ElaspedTime.TotalSeconds;
					num5 = num5 * 1f - num5;
					spriteBatch.Draw(_game.DummyTexture, device.Viewport.Bounds, new Color(num5, num5, num5, 1f - (float)fadeInGameStart.ElaspedTime.TotalSeconds));
				}
				if (!drawDayTimer.Expired && !LocalPlayer.Dead)
				{
					float num6 = 1f;
					float num7 = 1f / 3f;
					if (drawDayTimer.ElaspedTime < TimeSpan.FromSeconds(3.0))
					{
						num6 = drawDayTimer.PercentComplete / num7;
					}
					else if (drawDayTimer.ElaspedTime > TimeSpan.FromSeconds(6.0))
					{
						num6 = 1f - (drawDayTimer.PercentComplete - num7 * 2f) / num7;
					}
					sbuilder.Length = 0;
					sbuilder.Append("Day ");
					sbuilder.Concat(currentDay);
					spriteBatch.DrawOutlinedText(_game._largeFont, sbuilder, new Vector2(titleSafeArea.Left, (float)titleSafeArea.Bottom - _game._largeFont.MeasureString(sbuilder).Y), new Color(num6, num6, num6, num6), new Color(0f, 0f, 0f, num6), 2);
				}
				DrawAcheivements(device, spriteBatch, gameTime);
				spriteBatch.End();
			}
			base.OnDraw(device, spriteBatch, gameTime);
		}

		public bool DrawAbleToBuild()
		{
			IntVector3 neighborIndex = BlockTerrain.Instance.GetNeighborIndex(ConstructionProbe._worldIndex, ConstructionProbe._inFace);
			CastleMinerZGame.Instance.LocalPlayer.MovementProbe.SkipEmbedded = false;
			Vector3 worldPosition = CastleMinerZGame.Instance.LocalPlayer.WorldPosition;
			worldPosition.Y += 0.05f;
			CastleMinerZGame.Instance.LocalPlayer.MovementProbe.Init(worldPosition, worldPosition, CastleMinerZGame.Instance.LocalPlayer.PlayerAABB);
			if (BlockTerrain.Instance.ProbeTouchesBlock(CastleMinerZGame.Instance.LocalPlayer.MovementProbe, neighborIndex))
			{
				return false;
			}
			return true;
		}

		protected void DoConstructionModeUpdate()
		{
			if (PlayerInventory.ActiveInventoryItem == null)
			{
				ConstructionProbe.Init(new Vector3(0f), new Vector3(1f), false);
			}
			else
			{
				Matrix localToWorld = LocalPlayer.FPSCamera.LocalToWorld;
				Vector3 translation = localToWorld.Translation;
				ConstructionProbe.Init(translation, Vector3.Add(translation, Vector3.Multiply(localToWorld.Forward, 5f)), PlayerInventory.ActiveInventoryItem.ItemClass.IsMeleeWeapon);
				ConstructionProbe.SkipEmbedded = true;
				ConstructionProbe.Trace();
			}
			if (ConstructionProbe.AbleToBuild)
			{
				if (PlayerInventory.ActiveInventoryItem.ItemClass is BlockInventoryItemClass && !DrawAbleToBuild())
				{
					_game.GameScreen.SelectorEntity.Visible = false;
					return;
				}
				IntVector3 worldIndex = ConstructionProbe._worldIndex;
				Vector3 vector = worldIndex + new Vector3(0.5f, 0.5f, 0.5f);
				Vector3 vector2 = -ConstructionProbe._inNormal;
				Matrix localToParent = Matrix.Identity;
				float num = 0.51f;
				switch (ConstructionProbe._inFace)
				{
				case BlockFace.POSX:
					localToParent = Matrix.CreateWorld(vector + new Vector3(1f, 0f, 0f) * num, -Vector3.UnitY, Vector3.UnitX);
					break;
				case BlockFace.POSY:
					localToParent = Matrix.CreateWorld(vector + new Vector3(0f, 1f, 0f) * num, Vector3.UnitX, Vector3.UnitY);
					break;
				case BlockFace.POSZ:
					localToParent = Matrix.CreateWorld(vector + new Vector3(0f, 0f, 1f) * num, Vector3.UnitX, Vector3.UnitZ);
					break;
				case BlockFace.NEGX:
					localToParent = Matrix.CreateWorld(vector + new Vector3(-1f, 0f, 0f) * num, Vector3.UnitY, -Vector3.UnitX);
					break;
				case BlockFace.NEGY:
					localToParent = Matrix.CreateWorld(vector + new Vector3(0f, -1f, 0f) * num, -Vector3.UnitX, -Vector3.UnitY);
					break;
				case BlockFace.NEGZ:
					localToParent = Matrix.CreateWorld(vector + new Vector3(0f, 0f, -1f) * num, -Vector3.UnitX, -Vector3.UnitZ);
					break;
				}
				_game.GameScreen.CrackBox.LocalPosition = worldIndex + new Vector3(0.5f, -0.002f, 0.5f);
				_game.GameScreen.SelectorEntity.LocalToParent = localToParent;
				_game.GameScreen.SelectorEntity.Visible = true;
			}
			else
			{
				_game.GameScreen.SelectorEntity.Visible = false;
			}
		}

		public void Reset()
		{
			lastTOD = -1f;
		}

		protected override void OnUpdate(DNAGame game, GameTime gameTime)
		{
			if (!CastleMinerZGame.Instance.GameScreen.DoUpdate)
			{
				return;
			}
			LocalPlayer.UpdateGunEyePointCamera(GunEyePointCameraLocation);
			if (ActiveInventoryItem is GPSItem)
			{
				GPSItem gPSItem = (GPSItem)ActiveInventoryItem;
				_game.GameScreen.GPSMarker.Visible = true;
				_game.GameScreen.GPSMarker.LocalPosition = gPSItem.PointToLocation + new Vector3(0.5f, 1f, 0.5f);
				_game.GameScreen.GPSMarker.color = gPSItem.color;
			}
			else
			{
				_game.GameScreen.GPSMarker.Visible = false;
			}
			CastleMinerZPlayerStats.ItemStats itemStats = CastleMinerZGame.Instance.PlayerStats.GetItemStats(ActiveInventoryItem.ItemClass.ID);
			itemStats.TimeHeld += gameTime.get_ElapsedGameTime();
			if (!fadeInGameStart.Expired && LocalPlayer.Avatar.AvatarState != 0)
			{
				fadeInGameStart.Update(gameTime.get_ElapsedGameTime());
			}
			drawDayTimer.Update(gameTime.get_ElapsedGameTime());
			if (lastTOD < 0.4f && _game.GameScreen.TimeOfDay > 0.4f)
			{
				currentDay = (int)_game.GameScreen.Day + 1;
				if (_game.GameMode == GameModeTypes.Endurance && currentDay > 1)
				{
					CastleMinerZGame.Instance.PlayerStats.MaxDaysSurvived++;
				}
				SoundManager.Instance.PlayInstance("HorrorStinger");
				drawDayTimer.Reset();
			}
			lastTOD = _game.GameScreen.TimeOfDay;
			for (int i = 0; i < _triggers.Count; i++)
			{
				_triggers[i].Update();
			}
			if (LocalPlayer.Dead && !timeToShowRespawnText.Expired)
			{
				timeToShowRespawnText.Update(TimeSpan.FromSeconds(gameTime.get_ElapsedGameTime().TotalSeconds));
			}
			else if (WaitToRespawn)
			{
				timeToRespawn.Update(TimeSpan.FromSeconds(gameTime.get_ElapsedGameTime().TotalSeconds));
			}
			EquipActiveItem();
			Vector2 value = new Vector2(0f, 0f);
			Vector2 value2 = new Vector2(_game.LocalPlayer.LocalPosition.X, _game.LocalPlayer.LocalPosition.Z);
			currentDistanceTraveled = (int)Vector2.Distance(value, value2);
			if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Endurance)
			{
				CastleMinerZGame.Instance.PlayerStats.MaxDistanceTraveled = Math.Max(CastleMinerZGame.Instance.PlayerStats.MaxDistanceTraveled, currentDistanceTraveled);
				CastleMinerZGame.Instance.PlayerStats.MaxDepth = Math.Min(CastleMinerZGame.Instance.PlayerStats.MaxDepth, _game.LocalPlayer.LocalPosition.Y);
			}
			if (Guide.IsTrialMode)
			{
				if (currentDistanceTraveled <= 300)
				{
					trialMaxPosition = LocalPlayer.LocalPosition;
				}
				else if (currentDistanceTraveled > 301)
				{
					LocalPlayer.LocalPosition = trialMaxPosition;
					_game.GameScreen._uiGroup.ShowDialogScreen(_travelMaxDialog, delegate
					{
						_game.ShowMarketPlace();
					});
				}
			}
			if (!gameBegun && _game.GameMode == GameModeTypes.Endurance && _game.IsOnlineGame)
			{
				if (_game.CurrentNetworkSession.IsHost)
				{
					if (maxDistanceTraveled >= 100 || currentDay > 1)
					{
						_game.CurrentNetworkSession.SessionProperties.set_Item(4, (int?)1);
						gameBegun = true;
						Console.WriteLine("The Game Has Begun - No Other Players Can Join");
					}
				}
				else if (_game.CurrentNetworkSession.SessionProperties.get_Item(4) == 1)
				{
					gameBegun = true;
					Console.WriteLine("The Game Has Begun - No Other Players Can Join");
				}
			}
			if (currentDistanceTraveled > maxDistanceTraveled)
			{
				maxDistanceTraveled = currentDistanceTraveled;
			}
			Vector3 v = new Vector3(0f, 0f, 1f);
			Vector3 v2 = new Vector3(value2.X, 0f, value2.Y);
			compassRotation = v.AngleBetween(v2);
			if (LocalPlayer.InLava)
			{
				if (!lavaSoundPlayed)
				{
					lavaSoundPlayed = true;
					SoundManager.Instance.PlayInstance("Douse");
				}
				lavaDamageTimer.Update(gameTime.get_ElapsedGameTime());
				if (lavaDamageTimer.Expired)
				{
					ApplyDamage(0.25f, LocalPlayer.WorldPosition - new Vector3(0f, 10f, 0f));
					lavaDamageTimer.Reset();
					lavaSoundPlayed = false;
				}
			}
			else
			{
				lavaDamageTimer.Reset();
			}
			if (LocalPlayer.Underwater)
			{
				PlayerOxygen -= OxygenDecayRate * (float)gameTime.get_ElapsedGameTime().TotalSeconds;
				if (PlayerOxygen < 0f)
				{
					PlayerOxygen = 0f;
					PlayerHealth -= OxygenHealthPenaltyRate * (float)gameTime.get_ElapsedGameTime().TotalSeconds;
					HealthRecoverTimer.Reset();
				}
			}
			else
			{
				PlayerOxygen = 1f;
			}
			if (!LocalPlayer.Dead)
			{
				HealthRecoverTimer.Update(gameTime.get_ElapsedGameTime());
				if (PlayerHealth < 1f && HealthRecoverTimer.Expired)
				{
					PlayerHealth += HealthRecoverRate * (float)gameTime.get_ElapsedGameTime().TotalSeconds;
					if (PlayerHealth > 1f)
					{
						PlayerHealth = 1f;
					}
				}
			}
			_periodicSaveTimer.Update(gameTime.get_ElapsedGameTime());
			if (_periodicSaveTimer.Expired)
			{
				_periodicSaveTimer.Reset();
				_game.SaveData();
			}
			int num = _game._terrain.DepthUnderGround(_game.LocalPlayer.LocalPosition);
			if (lightningFlashCount <= 0 || num > 2 || _game.LocalPlayer.LocalPosition.Y <= -32f)
			{
				CastleMinerZGame.Instance.GameScreen._sky.drawLightning = false;
			}
			if (timeToLightning.Expired)
			{
				if (lightningFlashCount > 0 && !CastleMinerZGame.Instance.GameScreen._sky.drawLightning)
				{
					CastleMinerZGame.Instance.GameScreen._sky.drawLightning = true;
					lightningFlashCount--;
					timeToLightning = new OneShotTimer(TimeSpan.FromSeconds(rand.NextDouble() / 4.0 + 0.10000000149011612));
				}
				else if (lightningFlashCount > 0 && CastleMinerZGame.Instance.GameScreen._sky.drawLightning)
				{
					CastleMinerZGame.Instance.GameScreen._sky.drawLightning = false;
				}
				else if (timeToThunder.Expired)
				{
					if (lightningFlashCount < 3)
					{
						SoundManager.Instance.PlayInstance("thunderLow");
					}
					else
					{
						SoundManager.Instance.PlayInstance("thunderHigh");
					}
					timeToThunder = new OneShotTimer(TimeSpan.FromSeconds((float)rand.NextDouble() * 2f));
					timeToLightning = new OneShotTimer(TimeSpan.FromSeconds(rand.Next(10, 40)));
					lightningFlashCount = rand.Next(0, 4);
				}
				else
				{
					timeToThunder.Update(TimeSpan.FromSeconds(gameTime.get_ElapsedGameTime().TotalSeconds));
				}
			}
			else if (num <= 2 && _game.LocalPlayer.LocalPosition.Y > -32f)
			{
				timeToLightning.Update(TimeSpan.FromSeconds(gameTime.get_ElapsedGameTime().TotalSeconds));
				CastleMinerZGame.Instance.GameScreen._sky.drawLightning = false;
			}
			DoConstructionModeUpdate();
			UpdateAcheivements(gameTime);
			lastPosition = LocalPlayer.LocalPosition;
			base.OnUpdate(game, gameTime);
		}

		public void Shoot(GunInventoryItemClass gun)
		{
			Matrix localToWorld = LocalPlayer.FPSCamera.LocalToWorld;
			if (gun is PumpShotgunInventoryItemClass)
			{
				ShotgunShotMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, localToWorld, gun.Innaccuracy, gun.ID);
			}
			else
			{
				GunshotMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, localToWorld, gun.Innaccuracy, gun.ID);
			}
		}

		public void Melee(InventoryItem tool)
		{
			byte shooterID = ((_game.LocalPlayer == null || !_game.LocalPlayer.ValidGamer) ? byte.MaxValue : _game.LocalPlayer.Gamer.Id);
			ConstructionProbe.EnemyHit.TakeDamage(ConstructionProbe.GetIntersection(), Vector3.Normalize(ConstructionProbe._end - ConstructionProbe._start), tool.ItemClass, shooterID);
			ParticleEmitter particleEmitter = TracerManager._smokeEffect.CreateEmitter(CastleMinerZGame.Instance);
			particleEmitter.Reset();
			particleEmitter.Emitting = true;
			TracerManager.Instance.Scene.Children.Add(particleEmitter);
			particleEmitter.LocalPosition = ConstructionProbe.GetIntersection();
			particleEmitter.DrawPriority = 800;
			if (tool.InflictDamage())
			{
				PlayerInventory.Remove(tool);
			}
		}

		public void Dig(InventoryItem tool, bool effective)
		{
			if (!ConstructionProbe._collides || !BlockTerrain.Instance.OkayToBuildHere(ConstructionProbe._worldIndex))
			{
				return;
			}
			BlockTypeEnum block = GetBlock(ConstructionProbe._worldIndex);
			if (!BlockType.GetType(block).CanBeDug)
			{
				return;
			}
			DigMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, false, ConstructionProbe.GetIntersection(), ConstructionProbe._inNormal, block);
			if (effective)
			{
				if (block == BlockTypeEnum.Crate)
				{
					DestroyCrateMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex);
					Crate value;
					if (CastleMinerZGame.Instance.CurrentWorld.Crates.TryGetValue(ConstructionProbe._worldIndex, out value))
					{
						value.EjectContents();
					}
				}
				CastleMinerZGame.Instance.PlayerStats.DugBlock(block);
				InventoryItem inventoryItem = tool.CreatesWhenDug(block);
				if (inventoryItem != null)
				{
					PickupManager.Instance.CreatePickup(inventoryItem, IntVector3.ToVector3(ConstructionProbe._worldIndex) + new Vector3(0.5f, 0.5f, 0.5f), false);
				}
				AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, BlockTypeEnum.Empty);
				for (BlockFace blockFace = BlockFace.POSX; blockFace < BlockFace.NUM_FACES; blockFace++)
				{
					IntVector3 neighborIndex = BlockTerrain.Instance.GetNeighborIndex(ConstructionProbe._worldIndex, blockFace);
					BlockTypeEnum blockWithChanges = BlockTerrain.Instance.GetBlockWithChanges(neighborIndex);
					if (BlockType.GetType(blockWithChanges).Facing == blockFace)
					{
						InventoryItem.InventoryItemClass inventoryItemClass = BlockInventoryItemClass.BlockClasses[BlockType.GetType(blockWithChanges).ParentBlockType];
						PickupManager.Instance.CreatePickup(inventoryItemClass.CreateItem(1), IntVector3.ToVector3(neighborIndex) + new Vector3(0.5f, 0.5f, 0.5f), false);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, neighborIndex, BlockTypeEnum.Empty);
						if (blockWithChanges == BlockTypeEnum.LowerDoorOpenX || blockWithChanges == BlockTypeEnum.LowerDoorOpenZ || blockWithChanges == BlockTypeEnum.LowerDoorClosedX || blockWithChanges == BlockTypeEnum.LowerDoorClosedZ)
						{
							AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, neighborIndex + new IntVector3(0, 1, 0), BlockTypeEnum.Empty);
						}
					}
				}
				if (block == BlockTypeEnum.LowerDoorOpenX || block == BlockTypeEnum.LowerDoorOpenZ || block == BlockTypeEnum.LowerDoorClosedX || block == BlockTypeEnum.LowerDoorClosedZ)
				{
					AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, 1, 0), BlockTypeEnum.Empty);
				}
				if (block == BlockTypeEnum.UpperDoorOpen || block == BlockTypeEnum.UpperDoorClosed)
				{
					AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, -1, 0), BlockTypeEnum.Empty);
				}
			}
			if (tool.InflictDamage())
			{
				PlayerInventory.Remove(tool);
			}
		}

		public bool Build(BlockInventoryItem blockItem)
		{
			if (!ConstructionProbe._collides)
			{
				return false;
			}
			IntVector3 neighborIndex = BlockTerrain.Instance.GetNeighborIndex(ConstructionProbe._worldIndex, ConstructionProbe._inFace);
			BlockTypeEnum block = GetBlock(ConstructionProbe._worldIndex);
			if (!BlockTerrain.Instance.OkayToBuildHere(neighborIndex))
			{
				return false;
			}
			BlockType type = BlockType.GetType(block);
			if (!type.CanBuildOn)
			{
				return false;
			}
			if (!blockItem.CanPlaceHere(neighborIndex, ConstructionProbe._inFace))
			{
				return false;
			}
			bool flag = true;
			if (CastleMinerZGame.Instance.CurrentNetworkSession != null)
			{
				for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)CastleMinerZGame.Instance.CurrentNetworkSession.AllGamers).Count; i++)
				{
					NetworkGamer networkGamer = ((ReadOnlyCollection<NetworkGamer>)(object)CastleMinerZGame.Instance.CurrentNetworkSession.AllGamers)[i];
					if (networkGamer == null)
					{
						continue;
					}
					Player player = (Player)networkGamer.Tag;
					if (player != null)
					{
						player.MovementProbe.SkipEmbedded = false;
						Vector3 worldPosition = player.WorldPosition;
						worldPosition.Y += 0.05f;
						player.MovementProbe.Init(worldPosition, worldPosition, player.PlayerAABB);
						if (BlockTerrain.Instance.ProbeTouchesBlock(player.MovementProbe, neighborIndex))
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					BoundingBox box = default(BoundingBox);
					box.Min = IntVector3.ToVector3(neighborIndex) + new Vector3(0.01f, 0.01f, 0.01f);
					box.Max = box.Min + new Vector3(0.98f, 0.98f, 0.98f);
					if (!EnemyManager.Instance.TouchesZombies(box))
					{
						BlockTypeEnum block2 = GetBlock(ConstructionProbe._worldIndex);
						DigMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, true, ConstructionProbe.GetIntersection(), ConstructionProbe._inNormal, block2);
						blockItem.AlterBlock(LocalPlayer, neighborIndex, ConstructionProbe._inFace);
						return true;
					}
				}
			}
			return false;
		}

		public override void OnLostFocus()
		{
			GameTime gameTime = new GameTime(TimeSpan.FromSeconds(0.001), TimeSpan.FromSeconds(0.001));
			OnPlayerInput(new GameController(Screen.SelectedPlayerIndex.Value), gameTime);
			base.OnLostFocus();
		}

		protected void DrawPlayerList(GraphicsDevice device, SpriteBatch spriteBatch, GameTime gameTime)
		{
			SpriteFont medFont = _game._medFont;
			Viewport viewport = device.Viewport;
			Rectangle titleSafeArea = viewport.TitleSafeArea;
			spriteBatch.Begin();
			int count = ((ReadOnlyCollection<NetworkGamer>)(object)_game.CurrentNetworkSession.AllGamers).Count;
			int maxGamers = _game.CurrentNetworkSession.MaxGamers;
			_builder.Length = 0;
			_builder.Append("Players ").Concat(count).Append("/")
				.Concat(maxGamers);
			Vector2 vector = medFont.MeasureString(_builder);
			spriteBatch.DrawOutlinedText(medFont, _builder, new Vector2((float)titleSafeArea.Right - vector.X, (float)titleSafeArea.Bottom - vector.Y), Color.White, Color.Black, 2);
			float[] array = new float[1];
			float num = 0f;
			num += (array[0] = medFont.MeasureString("XXXXXXXXXXXXXXXXXXX ").X);
			float num2 = ((float)viewport.Width - num) / 2f;
			float num3 = titleSafeArea.Top;
			spriteBatch.DrawOutlinedText(medFont, "Player", new Vector2(num2, num3), Color.Orange, Color.Black, 2);
			num3 += (float)medFont.LineSpacing;
			for (int i = 0; i < ((ReadOnlyCollection<NetworkGamer>)(object)_game.CurrentNetworkSession.AllGamers).Count; i++)
			{
				NetworkGamer networkGamer = ((ReadOnlyCollection<NetworkGamer>)(object)_game.CurrentNetworkSession.AllGamers)[i];
				if (networkGamer.Tag == null)
				{
					continue;
				}
				Player player = (Player)networkGamer.Tag;
				spriteBatch.DrawOutlinedText(medFont, player.Gamer.Gamertag, new Vector2(num2, num3), player.Gamer.IsLocal ? Color.Red : Color.White, Color.Black, 2);
				if (player.Profile != null)
				{
					float num4 = (float)medFont.LineSpacing * 0.9f;
					float num5 = (float)medFont.LineSpacing - num4;
					if (player.GamerPicture != null)
					{
						spriteBatch.Draw(player.GamerPicture, new Rectangle((int)(num2 - (float)medFont.LineSpacing), (int)(num3 + num5), (int)num4, (int)num4), Color.White);
					}
				}
				num3 += (float)medFont.LineSpacing;
			}
			spriteBatch.End();
		}

		protected override void OnPlayerInput(GameController controller, GameTime gameTime)
		{
			CastleMinerZGame.Instance.PlayerStats.GetItemStats(ActiveInventoryItem.ItemClass.ID);
			CastleMinerZControllerMapping controllerMapping = _game._controllerMapping;
			controllerMapping.Sensitivity = _game.PlayerStats.controllerSensitivity;
			controllerMapping.InvertY = _game.PlayerStats.InvertYAxis;
			controllerMapping.ProcessInput(controller);
			float num = 5f;
			Vector2 vector = new Vector2(maxGunCameraShift, maxGunCameraShift);
			if (LocalPlayer.Shouldering)
			{
				vector /= 2f;
			}
			Vector2 vector2 = controllerMapping.Aiming * vector;
			GunEyePointCameraLocation += (vector2 - GunEyePointCameraLocation) * num * (float)gameTime.get_ElapsedGameTime().TotalSeconds;
			LocalPlayer.ProcessInput(_game._controllerMapping, gameTime);
			showPlayers = controllerMapping.PlayersScreen.Held && _game.IsOnlineGame;
			LocalPlayer.Avatar.Visible = !_hideUI;
			_game.ShowTitleSafeArea = !_hideUI;
			if (controller.PressedButtons.Start)
			{
				SoundManager.Instance.PlayInstance("Click");
				_game.GameScreen.ShowInGameMenu();
			}
			PlayerInventory.Update(gameTime);
			_game.GameScreen.CrackBox.CrackAmount = 0f;
			if (LocalPlayer.Dead)
			{
				LocalPlayer.UsingTool = false;
				if (controllerMapping.Jump.Pressed && timeToShowRespawnText.Expired)
				{
					if (_game.IsOnlineGame && AllPlayersDead() && _game.GameMode == GameModeTypes.Endurance)
					{
						if (_game.CurrentNetworkSession.IsHost)
						{
							RestartLevelMessage.Send((LocalNetworkGamer)_game.LocalPlayer.Gamer);
							BroadcastTextMessage.Send(_game.MyNetworkGamer, LocalPlayer.Gamer.Gamertag + " Has Restarted The Game");
						}
					}
					else if (!WaitToRespawn)
					{
						RespawnPlayer();
					}
				}
			}
			else
			{
				if (controllerMapping.NextItem.Pressed)
				{
					SoundManager.Instance.PlayInstance("Click");
					PlayerInventory.SelectedInventoryIndex++;
					PlayerInventory.SelectedInventoryIndex %= 8;
					LocalPlayer.Shouldering = false;
					PlayerInventory.ActiveInventoryItem.DigTime = TimeSpan.Zero;
				}
				else if (controllerMapping.PrevoiusItem.Pressed)
				{
					SoundManager.Instance.PlayInstance("Click");
					PlayerInventory.SelectedInventoryIndex--;
					if (PlayerInventory.SelectedInventoryIndex < 0)
					{
						PlayerInventory.SelectedInventoryIndex = 8 + PlayerInventory.SelectedInventoryIndex;
					}
					LocalPlayer.Shouldering = false;
					PlayerInventory.ActiveInventoryItem.DigTime = TimeSpan.Zero;
				}
				if (controllerMapping.BlockUI.Pressed)
				{
					_game.GameScreen.ShowBlockPicker();
					SoundManager.Instance.PlayInstance("Click");
				}
				if (controllerMapping.Activate.Pressed)
				{
					switch (GetBlock(ConstructionProbe._worldIndex))
					{
					case BlockTypeEnum.Crate:
					{
						Crate create = CastleMinerZGame.Instance.CurrentWorld.GetCreate(ConstructionProbe._worldIndex, true);
						_crateScreen.CurrentCrate = create;
						_game.GameScreen._uiGroup.PushScreen(_crateScreen);
						SoundManager.Instance.PlayInstance("Click");
						break;
					}
					case BlockTypeEnum.UpperDoorOpen:
					{
						DoorOpenCloseMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, false);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, BlockTypeEnum.UpperDoorClosed);
						BlockTypeEnum block2 = GetBlock(ConstructionProbe._worldIndex + new IntVector3(0, -1, 0));
						if (block2 == BlockTypeEnum.LowerDoorOpenX)
						{
							AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, -1, 0), BlockTypeEnum.LowerDoorClosedX);
						}
						else
						{
							AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, -1, 0), BlockTypeEnum.LowerDoorClosedZ);
						}
						SoundManager.Instance.PlayInstance("Click");
						break;
					}
					case BlockTypeEnum.UpperDoorClosed:
					{
						DoorOpenCloseMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, true);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, BlockTypeEnum.UpperDoorOpen);
						BlockTypeEnum block = GetBlock(ConstructionProbe._worldIndex + new IntVector3(0, -1, 0));
						if (block == BlockTypeEnum.LowerDoorClosedX)
						{
							AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, -1, 0), BlockTypeEnum.LowerDoorOpenX);
						}
						else
						{
							AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, -1, 0), BlockTypeEnum.LowerDoorOpenZ);
						}
						SoundManager.Instance.PlayInstance("Click");
						break;
					}
					case BlockTypeEnum.LowerDoorClosedX:
						DoorOpenCloseMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, true);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, BlockTypeEnum.LowerDoorOpenX);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, 1, 0), BlockTypeEnum.UpperDoorOpen);
						SoundManager.Instance.PlayInstance("Click");
						break;
					case BlockTypeEnum.LowerDoorClosedZ:
						DoorOpenCloseMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, true);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, BlockTypeEnum.LowerDoorOpenZ);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, 1, 0), BlockTypeEnum.UpperDoorOpen);
						SoundManager.Instance.PlayInstance("Click");
						break;
					case BlockTypeEnum.LowerDoorOpenX:
						DoorOpenCloseMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, false);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, BlockTypeEnum.LowerDoorClosedX);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, 1, 0), BlockTypeEnum.UpperDoorClosed);
						SoundManager.Instance.PlayInstance("Click");
						break;
					case BlockTypeEnum.LowerDoorOpenZ:
						DoorOpenCloseMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, false);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex, BlockTypeEnum.LowerDoorClosedZ);
						AlterBlockMessage.Send((LocalNetworkGamer)LocalPlayer.Gamer, ConstructionProbe._worldIndex + new IntVector3(0, 1, 0), BlockTypeEnum.UpperDoorClosed);
						SoundManager.Instance.PlayInstance("Click");
						break;
					default:
						SoundManager.Instance.PlayInstance("Error");
						break;
					}
				}
				if (ActiveInventoryItem == null)
				{
					LocalPlayer.UsingTool = false;
				}
				else
				{
					ActiveInventoryItem.ProcessInput(this, controllerMapping);
					PlayerInventory.RemoveEmptyItems();
				}
			}
			base.OnPlayerInput(controller, gameTime);
		}
	}
}
