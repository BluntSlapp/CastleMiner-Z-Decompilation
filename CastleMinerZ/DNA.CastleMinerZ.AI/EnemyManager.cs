using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.UI;
using DNA.CastleMinerZ.Utils.Trace;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace DNA.CastleMinerZ.AI
{
	public class EnemyManager : Entity
	{
		private const int MIN_LOCAL_SURFACE_ENEMY_LIMIT = 5;

		private const int MAX_LOCAL_SURFACE_ENEMIES = 12;

		private const int MAX_LOCAL_CAVE_ENEMIES = 8;

		private const int MAX_TOTAL_ENEMIES = 40;

		private const float ZOMBIE_STICKY_DISTANCE = 2f;

		private const float ZOMBIE_STICKY_DISTANCE_SQ = 4f;

		private const float ZOMBIEFEST_DISTANCE = 5000f;

		private const float ONE_DAY_IN_METERS = 120f;

		private const float DISTANCE_BOUNDARY_WIDTH = 40f;

		private const float DISTANCE_BOUNDARY_VARIANCE = 10f;

		private const float MIN_TIME_BETWEEN_RANDOM_SPAWNS = 2f;

		private const float MAX_TIME_BETWEEN_RANDOM_SPAWNS = 5f;

		private const int MIN_ZOMBIES_PER_RANDOM_SPAWN = 2;

		private const int MAX_ZOMBIES_PER_RANDOM_SPAWN = 5;

		private const float MINIMUM_SUN_FOR_SKELETON = 0.4f;

		private const float MINIMUM_TORCH_FOR_SKELETON = 0.4f;

		private const float MINIMUM_LANTERN_FOR_ZOMBIE = 0.4f;

		private const float MINIMUM_LANTERN_DIST_FOR_ZOMBIE = 7.2f;

		private const float FIREBALL_BLOCK_DAMAGE_RADIUS = 3f;

		private const float FIREBALL_BLOCK_DAMAGE_RADIUS_SQ = 9f;

		public const float ZOMBIEFEST_PERCENT_MIDNIGHT = 0.9f;

		public const float MAX_NORMAL_PERCENT_MIDNIGHT = 0.8f;

		private const float IN_HELL_MIDNIGHT_VALUE = 0.85f;

		private const float HELL_DEPTH = -40f;

		private const float FIREBALL_RANGE = 5f;

		private const float ENDURANCE_DRAGON_HEALTH_INCREMENT = 100f;

		private const float ENDURANCE_DRAGON_KILLED_HEALTH_INCREMENT = -15f;

		private const float ENDURANCE_DRAGON_MIN_PAUSE_SECONDS = 10f;

		private const float ENDURACE_DRAGON_MAX_PAUSE_SECONDS = 30f;

		private const float DRAGON_MINIMUM_INTERVAL = 8f;

		private const float DRAGON_INTERVAL = 16f;

		private const float DRAGON_BOX_RADIUS = 500f;

		private const float EASY_DAMAGE_MULTIPLIER = 1f;

		private const float ZOMBIE_TEST_DENSITY = 0.5f;

		public static EnemyManager Instance;

		private IntVector3[] _fireballDamageBuffer;

		private BlockTypeEnum[] _fireballDamageItemTypes;

		private IntVector3[] _dependentItemsToRemoveBuffer;

		private BlockTypeEnum[] _dependItemTypes;

		private float _timeSinceLastSurfaceEnemy;

		private float _timeSinceLastCaveEnemy;

		private int _nextLocalEnemyID;

		private int _enemyCounter;

		private ContentManager _contentManager;

		private GraphicsDevice _graphicsDevice;

		private List<BaseZombie> _zombies;

		private DragonEntity _dragon;

		private DragonClientEntity _dragonClient;

		private List<FireballEntity> _fireballs;

		private int _localSurfaceEnemyCount;

		private int _localCaveEnemyCount;

		public float ClearedDistance = 50f;

		public int _distanceEnemiesLeftToSpawn;

		public float _nextDistanceEnemyTimer;

		public bool ZombieFestIsOn;

		public float _timeLeftTilFrenzy;

		public float _timeToFirstContact = 20f;

		public bool DragonPending;

		public BoundingBox DragonBox;

		public TimeSpan NextSpawnDragonTime;

		public TimeSpan NextDragonAllowedTime;

		public bool InitializeDragonBox;

		public int dragonDistanceIndex;

		private TimeSpan _nextEnduranceDragonTime;

		private DragonTypeEnum _nextEnduranceDragonType;

		private float _nextEnduranceDragonHealth = 25f;

		private float[] dragonDistances = new float[5] { 100f, 500f, 1000f, 2600f, 4000f };

		public DragonTypeEnum NextTimedDragonType;

		private Random _rnd;

		private DamageLOSProbe _damageLOSProbe = new DamageLOSProbe();

		private TraceProbe zombieProbe = new TraceProbe();

		public bool DragonControlledLocally
		{
			get
			{
				return _dragon != null;
			}
		}

		public EnemyManager()
		{
			Instance = this;
			Visible = false;
			Collidee = false;
			Collider = false;
			_graphicsDevice = CastleMinerZGame.Instance.GraphicsDevice;
			_contentManager = CastleMinerZGame.Instance.Content;
			_timeSinceLastSurfaceEnemy = 0f;
			_timeSinceLastCaveEnemy = 0f;
			_localSurfaceEnemyCount = 0;
			_localCaveEnemyCount = 0;
			_timeLeftTilFrenzy = -1f;
			_zombies = new List<BaseZombie>();
			ZombieFestIsOn = false;
			_fireballs = new List<FireballEntity>();
			_dragon = null;
			_dragonClient = null;
			int num = 8;
			_fireballDamageBuffer = new IntVector3[(num + 1) * num * num];
			_fireballDamageItemTypes = new BlockTypeEnum[_fireballDamageBuffer.Length];
			_dependentItemsToRemoveBuffer = new IntVector3[_fireballDamageBuffer.Length];
			_dependItemTypes = new BlockTypeEnum[_fireballDamageBuffer.Length];
			_rnd = new Random();
			InitializeDragonBox = true;
			dragonDistanceIndex = 0;
			NextDragonAllowedTime = TimeSpan.FromHours(1000000.0);
			NextTimedDragonType = DragonTypeEnum.FIRE;
		}

		public void HandleMessage(CastleMinerZMessage message)
		{
			if (message is UpdateDragonMessage)
			{
				HandleUpdateDragonMessage((UpdateDragonMessage)message);
			}
			else if (message is DragonAttackMessage)
			{
				HandleDragonAttackMessage((DragonAttackMessage)message);
			}
			else if (message is DetonateFireballMessage)
			{
				HandleDetonateFireballMessage((DetonateFireballMessage)message);
			}
			else if (message is SpawnDragonMessage)
			{
				HandleSpawnDragonMessage((SpawnDragonMessage)message);
			}
			else if (message is SpawnEnemyMessage)
			{
				HandleSpawnEnemyMessage((SpawnEnemyMessage)message);
			}
			else if (message is KillEnemyMessage)
			{
				HandleKillEnemyMessage((KillEnemyMessage)message);
			}
			else if (message is SpeedUpEnemyMessage)
			{
				HandleSpeedUpEnemyMessage((SpeedUpEnemyMessage)message);
			}
			else if (message is RemoveDragonMessage)
			{
				HandleRemoveDragonMessage((RemoveDragonMessage)message);
			}
			else if (message is KillDragonMessage)
			{
				HandleKillDragonMessage((KillDragonMessage)message);
			}
			else if (message is RequestDragonMessage)
			{
				HandleRequestDragonMessage((RequestDragonMessage)message);
			}
			else if (message is MigrateDragonMessage)
			{
				HandleMigrateDragonMessage((MigrateDragonMessage)message);
			}
			else if (message is ExistingDragonMessage)
			{
				HandleExistingDragonMessage((ExistingDragonMessage)message);
			}
		}

		public void ResetFarthestDistance()
		{
			ClearedDistance = 50f;
			Vector3 worldPosition = CastleMinerZGame.Instance.LocalPlayer.WorldPosition;
			worldPosition.Y = 0f;
			float num = worldPosition.Length();
			dragonDistanceIndex = Math.Min(dragonDistanceIndex, dragonDistances.Length);
			while (dragonDistanceIndex > 0 && num <= dragonDistances[dragonDistanceIndex - 1])
			{
				dragonDistanceIndex--;
			}
		}

		public float CalculateMidnight(float distance, float playerDepth)
		{
			if (playerDepth < -40f)
			{
				return 0.85f;
			}
			if (ZombieFestIsOn)
			{
				return 1f;
			}
			float percentMidnight = BlockTerrain.Instance.PercentMidnight;
			float num = 1f - Math.Min(distance / 5000f, 1f);
			num *= 0.5f;
			percentMidnight = Math.Max(0f, percentMidnight - num);
			percentMidnight /= 1f - num;
			return percentMidnight.Clamp(0f, 0.79f);
		}

		public float CalculatePlayerDistance()
		{
			Player localPlayer = CastleMinerZGame.Instance.LocalPlayer;
			float num = 0f;
			if (localPlayer != null)
			{
				Vector3 worldPosition = localPlayer.WorldPosition;
				worldPosition.Y = 0f;
				num = worldPosition.Length();
				if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Endurance)
				{
					num += (CastleMinerZGame.Instance.GameScreen.Day - 0.41f) * 120f;
				}
			}
			return num;
		}

		public void RegisterGunShot(Vector3 location)
		{
			if (_dragon != null)
			{
				_dragon.RegisterGunshot(location);
			}
		}

		public float GetMinEnemySpawnTime(float d)
		{
			if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Endurance || CastleMinerZGame.Instance.GameMode == GameModeTypes.DragonEndurance || CastleMinerZGame.Instance.Difficulty == GameDifficultyTypes.HARD || CastleMinerZGame.Instance.Difficulty == GameDifficultyTypes.HARDCORE)
			{
				return MathHelper.Lerp(5f, 1f, d / 3450f);
			}
			return MathHelper.Lerp(45f, 20f, d / 3450f);
		}

		private void HandleSpawnEnemyMessage(SpawnEnemyMessage msg)
		{
			Player player = (Player)msg.Sender.Tag;
			if (player == null)
			{
				return;
			}
			bool flag = false;
			if (player.IsLocal)
			{
				flag = true;
			}
			else if (_zombies.Count < 35)
			{
				flag = true;
			}
			else if (_zombies.Count < 40)
			{
				_enemyCounter++;
				if ((_enemyCounter & 3) == 1)
				{
					flag = true;
				}
			}
			if (flag)
			{
				BaseZombie z = new BaseZombie(this, msg.EnemyTypeID, player, msg.SpawnPosition, msg.EnemyID, msg.RandomSeed, msg.InitPkg);
				AddZombie(z);
			}
		}

		private void SpawnDragon(DragonTypeEnum type, bool forBiome)
		{
			if (!DragonPending && _dragonClient == null)
			{
				DragonPending = true;
				if (CastleMinerZGame.Instance.IsGameHost)
				{
					SpawnDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, CastleMinerZGame.Instance.LocalPlayer.Gamer.Id, type, forBiome, -1f);
				}
				else
				{
					RequestDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, type, forBiome);
				}
			}
		}

		private void AskForDragon(bool forBiome, DragonTypeEnum dragonType)
		{
			if (!(CastleMinerZGame.Instance.CurrentGameTime.get_TotalGameTime() < NextDragonAllowedTime) && _dragon == null && _dragonClient == null)
			{
				Vector3 worldPosition = CastleMinerZGame.Instance.LocalPlayer.WorldPosition;
				worldPosition.Y = 0f;
				if (worldPosition.LengthSquared() > 1.296E+07f)
				{
					dragonType = DragonTypeEnum.SKELETON;
				}
				if (dragonType != DragonTypeEnum.COUNT)
				{
					SpawnDragon(dragonType, forBiome);
				}
			}
		}

		private void CheckDragonBox(GameTime time)
		{
			if (CastleMinerZGame.Instance.LocalPlayer == null || !CastleMinerZGame.Instance.LocalPlayer.ValidGamer)
			{
				return;
			}
			if (InitializeDragonBox || DragonBox.Contains(CastleMinerZGame.Instance.LocalPlayer.WorldPosition) == ContainmentType.Disjoint)
			{
				NextTimedDragonType = DragonTypeEnum.FIRE;
				RecalculateDragonBox(time);
				if (InitializeDragonBox)
				{
					NextDragonAllowedTime = CastleMinerZGame.Instance.CurrentGameTime.get_TotalGameTime() + TimeSpan.FromSeconds(5.0);
				}
				InitializeDragonBox = false;
			}
			else if (time.get_TotalGameTime() > NextSpawnDragonTime)
			{
				NextSpawnDragonTime += TimeSpan.FromMinutes(1.0);
				Player localPlayer = CastleMinerZGame.Instance.LocalPlayer;
				Vector3 worldPosition = localPlayer.WorldPosition;
				worldPosition.Y = 0f;
				if (NextTimedDragonType < DragonTypeEnum.COUNT || worldPosition.LengthSquared() > 1.296E+07f)
				{
					AskForDragon(false, NextTimedDragonType);
					NextTimedDragonType = (DragonTypeEnum)Math.Min((int)(NextTimedDragonType + 1), 5);
				}
			}
		}

		private void RecalculateDragonBox(GameTime t)
		{
			Vector3 worldPosition = CastleMinerZGame.Instance.LocalPlayer.WorldPosition;
			DragonBox = new BoundingBox(new Vector3(worldPosition.X - 500f, -100f, worldPosition.Z - 500f), new Vector3(worldPosition.X + 500f, 100f, worldPosition.Z + 500f));
			float num = 16f;
			switch (NextTimedDragonType)
			{
			case DragonTypeEnum.FIRE:
				num = 32f;
				break;
			case DragonTypeEnum.FOREST:
				num = 48f;
				break;
			case DragonTypeEnum.LIZARD:
				num = 64f;
				break;
			case DragonTypeEnum.ICE:
				num = 80f;
				break;
			case DragonTypeEnum.SKELETON:
				num = 80f;
				break;
			case DragonTypeEnum.COUNT:
				num = 80f;
				break;
			}
			NextSpawnDragonTime = t.get_TotalGameTime() + TimeSpan.FromMinutes(num + 16f * MathTools.RandomFloat(-0.25f, 0.25f));
		}

		public void BroadcastExistingDragonMessage(byte newClientID)
		{
			if (_dragon != null)
			{
				float currentHealth = -1f;
				if (_dragonClient != null)
				{
					currentHealth = _dragonClient.Health;
				}
				ExistingDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, newClientID, _dragon.EType.EType, _dragon.ForBiome, currentHealth);
			}
		}

		private void SetCurrentTimedDragonType(DragonTypeEnum dragonType)
		{
			if (dragonType != DragonTypeEnum.COUNT)
			{
				NextTimedDragonType = dragonType + 1;
			}
		}

		private void HandleExistingDragonMessage(ExistingDragonMessage msg)
		{
			if (CastleMinerZGame.Instance.IsLocalPlayerId(msg.NewClientID))
			{
				DragonPending = false;
				if (_dragonClient != null)
				{
					_dragonClient.RemoveFromParent();
				}
				_dragonClient = new DragonClientEntity(msg.EnemyTypeID, msg.CurrentHealth);
				if (!msg.ForBiome)
				{
					SetCurrentTimedDragonType(msg.EnemyTypeID);
				}
				base.Scene.Children.Add(_dragonClient);
				RecalculateDragonBox(CastleMinerZGame.Instance.CurrentGameTime);
			}
		}

		private void HandleSpawnDragonMessage(SpawnDragonMessage msg)
		{
			DragonPending = false;
			if (CastleMinerZGame.Instance.IsLocalPlayerId(msg.SpawnerID))
			{
				_dragon = new DragonEntity(msg.EnemyTypeID, msg.ForBiome, null);
				base.Scene.Children.Add(_dragon);
			}
			if (_dragonClient != null)
			{
				_dragonClient.RemoveFromParent();
			}
			_dragonClient = new DragonClientEntity(msg.EnemyTypeID, msg.Health);
			if (!msg.ForBiome)
			{
				SetCurrentTimedDragonType(msg.EnemyTypeID);
			}
			base.Scene.Children.Add(_dragonClient);
			RecalculateDragonBox(CastleMinerZGame.Instance.CurrentGameTime);
		}

		private void HandleRequestDragonMessage(RequestDragonMessage msg)
		{
			if (CastleMinerZGame.Instance.IsGameHost && !DragonPending)
			{
				DragonPending = true;
				SpawnDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, msg.Sender.Id, msg.EnemyTypeID, msg.ForBiome, -1f);
			}
		}

		private void HandleMigrateDragonMessage(MigrateDragonMessage msg)
		{
			if (_dragonClient != null && CastleMinerZGame.Instance.IsLocalPlayerId(msg.TargetID))
			{
				_dragon = new DragonEntity(msg.MigrationInfo.EType, msg.MigrationInfo.ForBiome, msg.MigrationInfo);
				base.Scene.Children.Add(_dragon);
			}
		}

		public void MigrateDragon(Player target, DragonHostMigrationInfo miginfo)
		{
			if (CastleMinerZGame.Instance.LocalPlayer != null && CastleMinerZGame.Instance.LocalPlayer.ValidGamer)
			{
				MigrateDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, target.Gamer.Id, miginfo);
			}
			_dragon.RemoveFromParent();
			_dragon = null;
		}

		public void DragonHasBeenHit()
		{
			if (_dragon != null)
			{
				_dragon.ChancesToNotAttack = 0;
			}
		}

		public void RemoveDragonEntity()
		{
			if (_dragon != null)
			{
				_dragon.RemoveFromParent();
				_dragon = null;
			}
			if (_dragonClient == null)
			{
				return;
			}
			if (CastleMinerZGame.Instance.GameMode == GameModeTypes.DragonEndurance)
			{
				if (CastleMinerZGame.Instance.IsGameHost)
				{
					_nextEnduranceDragonTime = CastleMinerZGame.Instance.CurrentGameTime.get_TotalGameTime() + TimeSpan.FromSeconds(MathTools.RandomFloat(10f, 30f));
				}
			}
			else
			{
				NextDragonAllowedTime = CastleMinerZGame.Instance.CurrentGameTime.get_TotalGameTime() + TimeSpan.FromSeconds(8.0);
				RecalculateDragonBox(CastleMinerZGame.Instance.CurrentGameTime);
			}
			_dragonClient.RemoveFromParent();
			_dragonClient = null;
		}

		public void HandleRemoveDragonMessage(RemoveDragonMessage msg)
		{
			RemoveDragonEntity();
		}

		public void SpawnDragonPickups(Vector3 location)
		{
			int num = MathTools.RandomInt(1, 4) + MathTools.RandomInt(1, 5);
			float num2 = location.Length();
			float min = (num2 / 5000f).Clamp(0f, 1f);
			for (int i = 0; i < num; i++)
			{
				InventoryItem inventoryItem = null;
				float num3 = MathTools.RandomFloat(min, 1f);
				float y = base.LocalPosition.Y;
				inventoryItem = (((double)num3 < 0.5) ? InventoryItem.CreateItem(InventoryItemIDs.Copper, 1) : (((double)num3 < 0.6) ? InventoryItem.CreateItem(InventoryItemIDs.Iron, 1) : (((double)num3 < 0.8) ? InventoryItem.CreateItem(InventoryItemIDs.Gold, 1) : ((!((double)num3 < 0.9)) ? InventoryItem.CreateItem(InventoryItemIDs.BloodStoneBlock, 1) : InventoryItem.CreateItem(InventoryItemIDs.Diamond, 1)))));
				if (inventoryItem != null)
				{
					PickupManager.Instance.CreateUpwardPickup(inventoryItem, location + new Vector3(0f, 1f, 0f), 3f);
				}
			}
		}

		public void HandleKillDragonMessage(KillDragonMessage msg)
		{
			if (_dragon != null)
			{
				_dragon.RemoveFromParent();
				_dragon = null;
			}
			if (_dragonClient == null || _dragonClient.Dead)
			{
				return;
			}
			if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Endurance)
			{
				switch (_dragonClient.EType.EType)
				{
				case DragonTypeEnum.SKELETON:
					CastleMinerZGame.Instance.PlayerStats.UndeadDragonKills++;
					break;
				case DragonTypeEnum.LIZARD:
					CastleMinerZGame.Instance.PlayerStats.SandDragonKills++;
					break;
				case DragonTypeEnum.ICE:
					CastleMinerZGame.Instance.PlayerStats.IceDragonKills++;
					break;
				case DragonTypeEnum.FOREST:
					CastleMinerZGame.Instance.PlayerStats.ForestDragonKills++;
					break;
				case DragonTypeEnum.FIRE:
					CastleMinerZGame.Instance.PlayerStats.FireDragonKills++;
					break;
				}
			}
			else if (CastleMinerZGame.Instance.GameMode == GameModeTypes.DragonEndurance && CastleMinerZGame.Instance.IsGameHost)
			{
				_nextEnduranceDragonHealth += -15f;
			}
			if (CastleMinerZGame.Instance.IsLocalPlayerId(msg.KillerID))
			{
				if (CastleMinerZGame.Instance.IsOnlineGame)
				{
					string message = CastleMinerZGame.Instance.LocalPlayer.Gamer.Gamertag + " Has Killed The " + _dragonClient.EType.GetDragonName();
					BroadcastTextMessage.Send(CastleMinerZGame.Instance.MyNetworkGamer, message);
				}
				CastleMinerZGame.Instance.PlayerStats.GetItemStats(msg.WeaponID);
				if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Endurance)
				{
					CastleMinerZGame.Instance.PlayerStats.TotalKills++;
				}
				_dragonClient.Kill(true);
			}
			else
			{
				_dragonClient.Kill(false);
			}
		}

		public void HandleUpdateDragonMessage(UpdateDragonMessage msg)
		{
			if (_dragonClient != null)
			{
				_dragonClient.HandleUpdateDragonMessage(msg);
			}
		}

		public void HandleDragonAttackMessage(DragonAttackMessage msg)
		{
			if (_dragonClient != null)
			{
				_dragonClient.HandleDragonAttackMessage(msg);
			}
		}

		public void RemoveDragon()
		{
			RemoveDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer);
		}

		public void AddFireball(FireballEntity fb)
		{
			_fireballs.Add(fb);
		}

		public void RemoveFireball(FireballEntity fb)
		{
			_fireballs.Remove(fb);
			fb.RemoveFromParent();
		}

		public void HandleDetonateFireballMessage(DetonateFireballMessage msg)
		{
			for (int i = 0; i < _fireballs.Count; i++)
			{
				if (_fireballs[i].FireballIndex == msg.Index)
				{
					_fireballs[i].Detonate(msg.Location);
					break;
				}
			}
			DragonType dragonType = DragonType.GetDragonType(msg.EType);
			if (CastleMinerZGame.Instance.LocalPlayer != null && CastleMinerZGame.Instance.LocalPlayer.ValidLivingGamer)
			{
				Vector3 worldPosition = CastleMinerZGame.Instance.LocalPlayer.WorldPosition;
				worldPosition.Y += 1f;
				float num = Vector3.DistanceSquared(worldPosition, msg.Location);
				if (num < 25f)
				{
					float num2 = Math.Max((float)Math.Sqrt(num) - 1f, 0f);
					_damageLOSProbe.Init(msg.Location, worldPosition);
					_damageLOSProbe.DragonTypeIndex = (int)dragonType.EType;
					BlockTerrain.Instance.Trace(_damageLOSProbe);
					float num3 = Math.Min(_damageLOSProbe.TotalDamageMultiplier * (1f - num2 / 5f), 1f);
					if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Survival && CastleMinerZGame.Instance.Difficulty == GameDifficultyTypes.EASY)
					{
						num3 *= 1f;
					}
					num3 *= dragonType.FireballDamage;
					InGameHUD.Instance.ApplyDamage(num3, msg.Location);
				}
			}
			DragonDamageType damageType = dragonType.DamageType;
			BlockTypeEnum type = ((damageType == DragonDamageType.ICE) ? BlockTypeEnum.Ice : BlockTypeEnum.Empty);
			for (byte b = 0; b < msg.NumBlocks; b = (byte)(b + 1))
			{
				BlockTerrain.Instance.SetBlock(msg.BlocksToRemove[b], type);
			}
		}

		private int RememberDependentObjects(IntVector3 worldIndex, int numDependents)
		{
			for (BlockFace blockFace = BlockFace.POSX; blockFace < BlockFace.NUM_FACES; blockFace++)
			{
				IntVector3 neighborIndex = BlockTerrain.Instance.GetNeighborIndex(worldIndex, blockFace);
				BlockTypeEnum blockWithChanges = BlockTerrain.Instance.GetBlockWithChanges(neighborIndex);
				if (BlockType.GetType(blockWithChanges).Facing == blockFace)
				{
					_dependentItemsToRemoveBuffer[numDependents] = neighborIndex;
					_dependItemTypes[numDependents++] = blockWithChanges;
				}
			}
			return numDependents;
		}

		private bool VectorWillBeDamaged(IntVector3 tester, int numBlocks)
		{
			for (int i = 0; i < numBlocks; i++)
			{
				if (_fireballDamageBuffer[i] == tester)
				{
					return true;
				}
			}
			return false;
		}

		public void DetonateFireball(Vector3 position, int index, DragonType dragonType)
		{
			Vector3 vector = new Vector3((float)Math.Floor(position.X) + 0.5f, (float)Math.Floor(position.Y) + 0.5f, (float)Math.Floor(position.Z) + 0.5f);
			Vector3 zero = Vector3.Zero;
			Vector3 zero2 = Vector3.Zero;
			int num = 0;
			int num2 = 0;
			IntVector3 zero3 = IntVector3.Zero;
			DragonTypeEnum eType = dragonType.EType;
			bool flag = dragonType.DamageType == DragonDamageType.DESTRUCTION;
			if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Endurance || CastleMinerZGame.Instance.GameMode == GameModeTypes.DragonEndurance || CastleMinerZGame.Instance.Difficulty == GameDifficultyTypes.HARD || CastleMinerZGame.Instance.Difficulty == GameDifficultyTypes.HARDCORE)
			{
				zero.X = -3f;
				while (zero.X <= 3f)
				{
					zero2.X = zero.X + vector.X;
					zero.Y = -3f;
					while (zero.Y <= 3f)
					{
						zero2.Y = zero.Y + vector.Y;
						zero.Z = -3f;
						while (zero.Z <= 3f)
						{
							zero2.Z = zero.Z + vector.Z;
							if (Vector3.DistanceSquared(zero2, position) <= 9f)
							{
								IntVector3 intVector = (IntVector3)zero2;
								IntVector3 localIndex = BlockTerrain.Instance.GetLocalIndex(intVector);
								if (BlockTerrain.Instance.IsIndexValid(localIndex))
								{
									BlockTypeEnum typeIndex = Block.GetTypeIndex(BlockTerrain.Instance.GetBlockAt(localIndex));
									if (!DragonType.BreakLookup[(int)eType, (int)typeIndex] && typeIndex != BlockTypeEnum.UpperDoorClosed && typeIndex != BlockTypeEnum.UpperDoorOpen)
									{
										_fireballDamageItemTypes[num] = typeIndex;
										_fireballDamageBuffer[num++] = intVector;
										if (flag)
										{
											num2 = RememberDependentObjects(intVector, num2);
										}
										if (typeIndex == BlockTypeEnum.LowerDoorOpenX || typeIndex == BlockTypeEnum.LowerDoorOpenZ || typeIndex == BlockTypeEnum.LowerDoorClosedX || typeIndex == BlockTypeEnum.LowerDoorClosedZ)
										{
											intVector.Y++;
											_fireballDamageItemTypes[num] = BlockTypeEnum.UpperDoorOpen;
											_fireballDamageBuffer[num++] = intVector;
											if (flag)
											{
												num2 = RememberDependentObjects(intVector, num2);
											}
										}
									}
								}
							}
							zero.Z += 1f;
						}
						zero.Y += 1f;
					}
					zero.X += 1f;
				}
			}
			int num3 = num;
			for (int i = 0; i < num2; i++)
			{
				if (!VectorWillBeDamaged(_dependentItemsToRemoveBuffer[i], num3))
				{
					InventoryItem.InventoryItemClass inventoryItemClass = BlockInventoryItemClass.BlockClasses[BlockType.GetType(_dependItemTypes[i]).ParentBlockType];
					PickupManager.Instance.CreatePickup(inventoryItemClass.CreateItem(1), IntVector3.ToVector3(_dependentItemsToRemoveBuffer[i]) + new Vector3(0.5f, 0.5f, 0.5f), false);
					_fireballDamageItemTypes[num] = _dependItemTypes[i];
					_fireballDamageBuffer[num++] = _dependentItemsToRemoveBuffer[i];
					if (_dependItemTypes[i] == BlockTypeEnum.LowerDoorOpenX || _dependItemTypes[i] == BlockTypeEnum.LowerDoorOpenZ || _dependItemTypes[i] == BlockTypeEnum.LowerDoorClosedX || _dependItemTypes[i] == BlockTypeEnum.LowerDoorClosedZ)
					{
						_fireballDamageItemTypes[num] = BlockTypeEnum.UpperDoorOpen;
						_fireballDamageBuffer[num++] = _dependentItemsToRemoveBuffer[i] + new IntVector3(0, 1, 0);
					}
				}
			}
			for (int j = 0; j < num3; j++)
			{
				if (_fireballDamageItemTypes[j] == BlockTypeEnum.Crate)
				{
					DestroyCrateMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, _fireballDamageBuffer[j]);
					Crate value;
					if (CastleMinerZGame.Instance.CurrentWorld.Crates.TryGetValue(_fireballDamageBuffer[j], out value))
					{
						value.EjectContents();
					}
				}
			}
			DetonateFireballMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, position, index, num, _fireballDamageBuffer, dragonType.EType);
		}

		public void HandleKillEnemyMessage(KillEnemyMessage msg)
		{
			for (int i = 0; i < _zombies.Count; i++)
			{
				if (_zombies[i].IsDead || _zombies[i].Target.Gamer.Id != msg.TargetID || _zombies[i].EnemyID != msg.EnemyID)
				{
					continue;
				}
				if (CastleMinerZGame.Instance.IsLocalPlayerId(msg.KillerID))
				{
					CastleMinerZPlayerStats.ItemStats itemStats = CastleMinerZGame.Instance.PlayerStats.GetItemStats(msg.WeaponID);
					_zombies[i].CreatePickup();
					if (_zombies[i].EType.FoundIn == EnemyType.FoundInEnum.ABOVEGROUND)
					{
						itemStats.KillsZombies++;
					}
					else
					{
						itemStats.KillsSkeleton++;
					}
					if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Endurance)
					{
						CastleMinerZGame.Instance.PlayerStats.TotalKills++;
					}
				}
				_zombies[i].Kill();
				break;
			}
		}

		public void HandleSpeedUpEnemyMessage(SpeedUpEnemyMessage msg)
		{
			for (int i = 0; i < _zombies.Count; i++)
			{
				if (_zombies[i].Target.Gamer.Id == msg.TargetID && _zombies[i].EnemyID == msg.EnemyID)
				{
					if (!_zombies[i].IsMovingFast && !_zombies[i].IsDead)
					{
						_zombies[i].SpeedUp();
					}
					break;
				}
			}
		}

		public IShootableEnemy Trace(TraceProbe tp, bool meleeWeapon)
		{
			IShootableEnemy shootableEnemy = null;
			BlockTerrain.Instance.Trace(tp);
			if (_zombies.Count != 0)
			{
				BaseZombie baseZombie = null;
				if (tp._collides)
				{
					zombieProbe.Init(tp._start, tp.GetIntersection());
				}
				else
				{
					zombieProbe.Init(tp._start, tp._end);
				}
				Vector3 vector = zombieProbe._end - zombieProbe._start;
				if (vector.LengthSquared() <= 0.0001f)
				{
					return baseZombie;
				}
				vector.Normalize();
				float num = Vector3.Dot(zombieProbe._start, vector);
				float num2 = Vector3.Dot(zombieProbe._end, vector);
				float inT = zombieProbe._inT;
				foreach (BaseZombie zombie in _zombies)
				{
					if (!zombie.IsHittable)
					{
						continue;
					}
					Vector3 worldPosition = zombie.WorldPosition;
					float num3 = Vector3.Dot(worldPosition, vector);
					if (num - num3 > 3f || num3 - num2 > 3f)
					{
						continue;
					}
					Vector3 vector2 = worldPosition - zombieProbe._start;
					Vector3 vector3 = Vector3.Cross(vector, vector2);
					if (vector3.LengthSquared() > 0.0001f)
					{
						vector3.Normalize();
						vector3 = Vector3.Cross(vector3, vector);
						if (Math.Abs(Vector3.Dot(worldPosition, vector3) - Vector3.Dot(zombieProbe._start, vector3)) > 3f)
						{
							continue;
						}
					}
					BoundingBox playerAABB = zombie.PlayerAABB;
					playerAABB.Min += worldPosition;
					playerAABB.Max += worldPosition;
					zombieProbe.TestBoundBox(playerAABB);
					if (zombieProbe._collides && inT != zombieProbe._inT)
					{
						float chanceOfBulletStrike = zombie.EType.ChanceOfBulletStrike;
						Vector3 intersection = zombieProbe.GetIntersection();
						if (chanceOfBulletStrike == 1f || meleeWeapon || zombie.IsHeadshot(intersection) || (float)_rnd.NextDouble() <= chanceOfBulletStrike)
						{
							baseZombie = zombie;
							num2 = Vector3.Dot(intersection, vector);
							inT = zombieProbe._inT;
						}
						else
						{
							chanceOfBulletStrike = 2f;
						}
					}
				}
				if (baseZombie != null)
				{
					tp._collides = true;
					tp._end = zombieProbe._end;
					tp._inT = zombieProbe._inT;
					tp._inNormal = zombieProbe._inNormal;
					tp._inFace = zombieProbe._inFace;
					shootableEnemy = baseZombie;
				}
			}
			if (shootableEnemy == null && !tp._collides && _dragonClient != null)
			{
				tp.Reset();
				if (_dragonClient.Trace(tp))
				{
					shootableEnemy = _dragonClient;
				}
			}
			return shootableEnemy;
		}

		public void AddZombie(BaseZombie z)
		{
			_zombies.Add(z);
			base.Scene.Children.Add(z);
		}

		public void RemoveZombie(BaseZombie z)
		{
			_zombies.Remove(z);
			if (z.Target == CastleMinerZGame.Instance.LocalPlayer)
			{
				if (z.EType.FoundIn == EnemyType.FoundInEnum.ABOVEGROUND)
				{
					_localSurfaceEnemyCount--;
				}
				else
				{
					_localCaveEnemyCount--;
				}
			}
			z.RemoveFromParent();
		}

		public bool TouchesZombies(BoundingBox box)
		{
			for (int i = 0; i < _zombies.Count; i++)
			{
				if (_zombies[i].Touches(box))
				{
					return true;
				}
			}
			return false;
		}

		private int MakeNextEnemyID()
		{
			return _nextLocalEnemyID++;
		}

		private void SpawnRandomZombies(Vector3 plrpos)
		{
			Vector3 vector = plrpos;
			vector.Y += 1f;
			float distance = CalculatePlayerDistance();
			float midnight = CalculateMidnight(distance, plrpos.Y);
			Vector3 worldVelocity = CastleMinerZGame.Instance.LocalPlayer.PlayerPhysics.WorldVelocity;
			worldVelocity *= 5f;
			EnemyTypeEnum zombie = EnemyType.GetZombie(distance);
			int spawnRadius = EnemyType.Types[(int)zombie].SpawnRadius;
			vector.X += worldVelocity.X + (float)_rnd.Next(-spawnRadius, spawnRadius + 1);
			vector.Z += worldVelocity.Z + (float)_rnd.Next(-spawnRadius, spawnRadius + 1);
			if (BlockTerrain.Instance.RegionIsLoaded(vector))
			{
				vector = BlockTerrain.Instance.FindTopmostGroundLocation(vector);
				_distanceEnemiesLeftToSpawn--;
				SpawnEnemyMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, vector, zombie, midnight, MakeNextEnemyID(), _rnd.Next());
				_localSurfaceEnemyCount++;
			}
		}

		private void SpawnAbovegroundEnemy(Vector3 plrpos)
		{
			Vector3 vector = plrpos;
			vector.Y += 1f;
			float num = CalculatePlayerDistance();
			float num2 = CalculateMidnight(num, plrpos.Y);
			if (num2 <= 0.0001f)
			{
				_timeSinceLastSurfaceEnemy = 0f;
				return;
			}
			float num3 = MathHelper.Lerp(60f, GetMinEnemySpawnTime(num), (float)Math.Pow(num2, 0.25));
			if (!(_timeSinceLastSurfaceEnemy > num3 * (1f + (float)_rnd.NextDouble() * 0.5f)))
			{
				return;
			}
			Vector3 worldVelocity = CastleMinerZGame.Instance.LocalPlayer.PlayerPhysics.WorldVelocity;
			worldVelocity *= 5f;
			EnemyTypeEnum abovegroundEnemy = EnemyType.GetAbovegroundEnemy(num2, num);
			int spawnRadius = EnemyType.Types[(int)abovegroundEnemy].SpawnRadius;
			vector.X += worldVelocity.X + (float)_rnd.Next(-spawnRadius, spawnRadius + 1);
			vector.Z += worldVelocity.Z + (float)_rnd.Next(-spawnRadius, spawnRadius + 1);
			if (BlockTerrain.Instance.RegionIsLoaded(vector))
			{
				vector = ((!(plrpos.Y > -40f)) ? BlockTerrain.Instance.FindSafeStartLocation(vector) : BlockTerrain.Instance.FindTopmostGroundLocation(vector));
				Vector3 vector2 = new Vector3(vector.X, vector.Y + 0.5f, vector.Z);
				float simpleTorchlightAtPoint = BlockTerrain.Instance.GetSimpleTorchlightAtPoint(vector2);
				if (simpleTorchlightAtPoint < 0.4f || !ItemBlockEntityManager.Instance.NearLantern(vector2, 7.2f))
				{
					_timeSinceLastSurfaceEnemy = 0f;
					SpawnEnemyMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, vector, abovegroundEnemy, num2, MakeNextEnemyID(), _rnd.Next());
					_localSurfaceEnemyCount++;
				}
			}
		}

		private void SpawnBelowgroundEnemy(Vector3 plrpos, float gametime)
		{
			Vector3 vector = plrpos;
			vector.Y += 1f;
			int num = (int)(0f - (plrpos.Y - 20f)).Clamp(0f, 50f);
			float num2 = (float)num / 50f;
			float num3 = (float)Math.Sin(gametime / 60f % 2f * (float)Math.PI);
			num3 = ((!(num3 > 0f)) ? 0f : ((float)Math.Sqrt(num3)));
			num2 *= num3;
			float num4 = ((plrpos.Y < -40f) ? 3500f : CalculatePlayerDistance());
			float num5 = MathHelper.Lerp(60f, GetMinEnemySpawnTime(num4), num2);
			if (!(_timeSinceLastCaveEnemy > num5 * (1f + (float)_rnd.NextDouble() * 0.5f)))
			{
				return;
			}
			EnemyTypeEnum belowgroundEnemy = EnemyType.GetBelowgroundEnemy(num, num4);
			int spawnRadius = EnemyType.Types[(int)belowgroundEnemy].SpawnRadius;
			int num6 = _rnd.Next(-spawnRadius, spawnRadius);
			num6 = ((num6 > 0) ? (num6 + 5) : (num6 - 5));
			vector.X += num6;
			num6 = _rnd.Next(-spawnRadius, spawnRadius);
			num6 = ((num6 > 0) ? (num6 + 5) : (num6 - 5));
			vector.Z += num6;
			if (!BlockTerrain.Instance.RegionIsLoaded(vector))
			{
				return;
			}
			vector = BlockTerrain.Instance.FindClosestCeiling(vector);
			if (vector.LengthSquared() != 0f)
			{
				Vector3 position = vector;
				position.Y -= 1f;
				Vector2 simpleLightAtPoint = BlockTerrain.Instance.GetSimpleLightAtPoint(position);
				if (simpleLightAtPoint.X <= 0.4f && simpleLightAtPoint.Y <= 0.4f)
				{
					_timeSinceLastCaveEnemy = 0f;
					SpawnEnemyMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, vector, belowgroundEnemy, 0f, MakeNextEnemyID(), _rnd.Next());
					_localCaveEnemyCount++;
				}
			}
		}

		private void SpawnTestEnemy(Vector3 plrpos)
		{
			BaseZombie z = new BaseZombie(this, EnemyTypeEnum.FELGUARD, CastleMinerZGame.Instance.LocalPlayer, plrpos, 50, 1, EnemyType.Types[50].CreateInitPackage(0.5f));
			AddZombie(z);
		}

		public float AttentuateVelocity(Player plr, Vector3 fwd, Vector3 worldPos)
		{
			float num = 1f;
			for (int i = 0; i < _zombies.Count; i++)
			{
				if (_zombies[i].Target != plr || !_zombies[i].IsBlocking)
				{
					continue;
				}
				Vector3 value = _zombies[i].WorldPosition - worldPos;
				float num2 = value.LengthSquared();
				float num3 = 1f;
				if (num2 < 4f && Math.Abs(value.Y) < 1.5f)
				{
					num3 = 0.5f;
					if (num2 > 0.0001f)
					{
						Vector3 vector = Vector3.Normalize(value);
						float num4 = Vector3.Dot(vector, fwd);
						if (num4 > 0f)
						{
							num3 *= Math.Min(1f, 2f * (1f - num4));
						}
					}
				}
				num *= num3;
			}
			return num;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			if (CastleMinerZGame.Instance.Difficulty == GameDifficultyTypes.NOENEMIES)
			{
				return;
			}
			Player localPlayer = CastleMinerZGame.Instance.LocalPlayer;
			if (localPlayer != null && _zombies.Count < 40)
			{
				_timeSinceLastSurfaceEnemy += (float)gameTime.get_ElapsedGameTime().TotalSeconds;
				_timeSinceLastCaveEnemy += (float)gameTime.get_ElapsedGameTime().TotalSeconds;
				Vector3 worldPosition = localPlayer.WorldPosition;
				Vector3 position = worldPosition;
				position.Y += 1f;
				float simpleSunlightAtPoint = BlockTerrain.Instance.GetSimpleSunlightAtPoint(position);
				float num = new Vector2(worldPosition.X, worldPosition.Z).Length();
				if (CastleMinerZGame.Instance.GameMode != GameModeTypes.DragonEndurance)
				{
					if (CastleMinerZGame.Instance.IsGameHost && dragonDistanceIndex < dragonDistances.Length && num > dragonDistances[dragonDistanceIndex])
					{
						AskForDragon(true, (DragonTypeEnum)dragonDistanceIndex);
						dragonDistanceIndex++;
					}
					CheckDragonBox(gameTime);
					if (BlockTerrain.Instance.PercentMidnight > 0.9f)
					{
						if (!ZombieFestIsOn)
						{
							if (_timeLeftTilFrenzy == -1f)
							{
								_timeLeftTilFrenzy = 3f;
							}
							else if (_timeLeftTilFrenzy > 0f)
							{
								_timeLeftTilFrenzy -= (float)gameTime.get_ElapsedGameTime().TotalSeconds;
								if (_timeLeftTilFrenzy < 0f)
								{
									_timeLeftTilFrenzy = -1f;
									ZombieFestIsOn = true;
								}
							}
						}
					}
					else
					{
						ZombieFestIsOn = false;
					}
					if (CastleMinerZGame.Instance.GameMode == GameModeTypes.Endurance || CastleMinerZGame.Instance.Difficulty == GameDifficultyTypes.HARD || CastleMinerZGame.Instance.Difficulty == GameDifficultyTypes.HARDCORE)
					{
						if (num > ClearedDistance)
						{
							_timeToFirstContact = -1f;
							if (simpleSunlightAtPoint > 0.01f)
							{
								if (_distanceEnemiesLeftToSpawn == 0)
								{
									_nextDistanceEnemyTimer = MathTools.RandomFloat(2f);
								}
								_distanceEnemiesLeftToSpawn += MathTools.RandomInt(2, 5);
							}
							float num2 = (float)Math.Floor(1.5 + (double)(num / 40f)) * 40f;
							ClearedDistance = MathTools.RandomFloat(num2 - 10f, num2 + 10f);
						}
						if (_timeToFirstContact > 0f)
						{
							_timeToFirstContact -= (float)gameTime.get_ElapsedGameTime().TotalSeconds;
							if (_timeToFirstContact < 0f)
							{
								_nextDistanceEnemyTimer = MathTools.RandomFloat(2f);
								_distanceEnemiesLeftToSpawn += MathTools.RandomInt(2, 5);
							}
						}
						if ((float)_distanceEnemiesLeftToSpawn > 0f)
						{
							if (CastleMinerZGame.Instance.LocalPlayer.Dead)
							{
								_distanceEnemiesLeftToSpawn = 0;
							}
							else
							{
								_nextDistanceEnemyTimer -= (float)gameTime.get_ElapsedGameTime().TotalSeconds;
								if (_nextDistanceEnemyTimer <= 0f)
								{
									SpawnRandomZombies(worldPosition);
									if (_distanceEnemiesLeftToSpawn > 0)
									{
										_nextDistanceEnemyTimer += MathTools.RandomFloat(2f, 5f);
									}
								}
							}
						}
					}
				}
				else if (CastleMinerZGame.Instance.IsGameHost && !DragonPending && _dragonClient == null && gameTime.get_TotalGameTime() > _nextEnduranceDragonTime)
				{
					DragonPending = true;
					SpawnDragonMessage.Send((LocalNetworkGamer)CastleMinerZGame.Instance.LocalPlayer.Gamer, CastleMinerZGame.Instance.LocalPlayer.Gamer.Id, _nextEnduranceDragonType, false, _nextEnduranceDragonHealth);
					_nextEnduranceDragonHealth += 100f;
					_nextEnduranceDragonType++;
					if (_nextEnduranceDragonType == DragonTypeEnum.COUNT)
					{
						_nextEnduranceDragonType = DragonTypeEnum.FIRE;
					}
				}
				int count = ((ReadOnlyCollection<NetworkGamer>)(object)CastleMinerZGame.Instance.CurrentNetworkSession.AllGamers).Count;
				if (count > 0)
				{
					if (CastleMinerZGame.Instance.GameMode != GameModeTypes.DragonEndurance && _localSurfaceEnemyCount < 5 + 12 / count)
					{
						_timeSinceLastSurfaceEnemy += (float)gameTime.get_ElapsedGameTime().TotalSeconds;
						SpawnAbovegroundEnemy(worldPosition);
					}
					if (simpleSunlightAtPoint != -1f && simpleSunlightAtPoint <= 0.4f && _localCaveEnemyCount < 8 / count)
					{
						_timeSinceLastCaveEnemy += (float)gameTime.get_ElapsedGameTime().TotalSeconds;
						SpawnBelowgroundEnemy(worldPosition, (float)gameTime.get_TotalGameTime().TotalSeconds);
					}
				}
			}
			base.OnUpdate(gameTime);
		}
	}
}
