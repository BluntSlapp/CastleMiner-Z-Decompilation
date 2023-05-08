using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DNA.CastleMinerZ.AI;
using DNA.CastleMinerZ.Terrain;
using DNA.CastleMinerZ.UI;
using DNA.Drawing;
using DNA.Text;
using DNA.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class InventoryItem
	{
		public abstract class InventoryItemClass
		{
			public InventoryItemIDs ID;

			protected string _name;

			protected string _description1;

			protected string _description2;

			public int MaxStackCount;

			public float EnemyDamage;

			public DamageType EnemyDamageType;

			protected TimeSpan _coolDownTime;

			public float ItemSelfDamagePerUse;

			private string _useSoundCue;

			protected PlayerMode _playerMode;

			public string UseSound
			{
				get
				{
					return _useSoundCue;
				}
			}

			public TimeSpan CoolDownTime
			{
				get
				{
					return _coolDownTime;
				}
			}

			public string Name
			{
				get
				{
					return _name;
				}
			}

			public string Description1
			{
				get
				{
					return _description1;
				}
			}

			public string Description2
			{
				get
				{
					return _description2;
				}
			}

			public PlayerMode PlayerAnimationMode
			{
				get
				{
					return _playerMode;
				}
			}

			public virtual bool IsMeleeWeapon
			{
				get
				{
					return true;
				}
			}

			public virtual float PickupTimeoutLength
			{
				get
				{
					return 30f;
				}
			}

			public InventoryItemClass(InventoryItemIDs id, string name, string description1, string description2, int maxStack, TimeSpan coolDownTime)
			{
				_playerMode = PlayerMode.Generic;
				_useSoundCue = null;
				_coolDownTime = coolDownTime;
				_name = name;
				_description1 = description1;
				_description2 = description2;
				MaxStackCount = maxStack;
				EnemyDamage = 0.1f;
				EnemyDamageType = DamageType.BLUNT;
				ID = id;
			}

			public InventoryItemClass(InventoryItemIDs id, string name, string description1, string description2, int maxStack, TimeSpan coolDownTime, string useSound)
			{
				_useSoundCue = useSound;
				_coolDownTime = coolDownTime;
				_name = name;
				_description1 = description1;
				_description2 = description2;
				MaxStackCount = maxStack;
				EnemyDamage = 0.1f;
				EnemyDamageType = DamageType.BLUNT;
				ID = id;
			}

			public abstract Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer);

			public virtual InventoryItem CreateItem(int stackCount)
			{
				return new InventoryItem(this, stackCount);
			}

			public void Draw2D(SpriteBatch batch, Rectangle destRect, Color color)
			{
				if (_2DImages == null)
				{
					FinishInitialization(batch.GraphicsDevice);
				}
				int iD = (int)ID;
				Rectangle value = new Rectangle((iD & 7) * 64, iD / 8 * 64, 64, 64);
				batch.Draw((Texture2D)_2DImages, destRect, (Rectangle?)value, color);
			}

			public void Draw2D(SpriteBatch batch, Rectangle destRect)
			{
				Draw2D(batch, destRect, Color.White);
			}
		}

		public const int UIItemsPerRow = 8;

		public const int UIItemSize = 64;

		public const int UIMapWidth = 512;

		public const int UIMapHeight = 640;

		protected static Dictionary<InventoryItemIDs, InventoryItemClass> AllItems = new Dictionary<InventoryItemIDs, InventoryItemClass>();

		public static RenderTarget2D _2DImages = null;

		private InventoryItemClass _class;

		private OneShotTimer _coolDownTimer;

		private int _stackCount;

		public float ItemHealthLevel = 1f;

		public TimeSpan DigTime = TimeSpan.Zero;

		public IntVector3 DigLocation;

		private StringBuilder sbuilder = new StringBuilder();

		protected OneShotTimer CoolDownTimer
		{
			get
			{
				return _coolDownTimer;
			}
		}

		public InventoryItemClass ItemClass
		{
			get
			{
				return _class;
			}
		}

		public int StackCount
		{
			get
			{
				return _stackCount;
			}
			set
			{
				_stackCount = value;
			}
		}

		public int MaxStackCount
		{
			get
			{
				return _class.MaxStackCount;
			}
		}

		public string Name
		{
			get
			{
				return _class.Name;
			}
		}

		public string Description1
		{
			get
			{
				return _class.Description1;
			}
		}

		public string Description2
		{
			get
			{
				return _class.Description2;
			}
		}

		public bool IsMeleeWeapon
		{
			get
			{
				return _class.IsMeleeWeapon;
			}
		}

		public float EnemyDamage
		{
			get
			{
				return _class.EnemyDamage;
			}
		}

		public DamageType EnemyDamageType
		{
			get
			{
				return _class.EnemyDamageType;
			}
		}

		public PlayerMode PlayerMode
		{
			get
			{
				return _class.PlayerAnimationMode;
			}
		}

		public static InventoryItem CreateItem(InventoryItemIDs id, int stackCount)
		{
			return AllItems[id].CreateItem(stackCount);
		}

		public static InventoryItemClass GetClass(InventoryItemIDs id)
		{
			return AllItems[id];
		}

		public static Entity CreateEntity(InventoryItemIDs id, ItemUse use, bool attachedToLocalPlayer)
		{
			InventoryItemClass @class = GetClass(id);
			return @class.CreateEntity(use, attachedToLocalPlayer);
		}

		public static void Initalize(ContentManager content)
		{
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.WoodBlock, BlockTypeEnum.Wood, "Made from logs", "This is a raw material that must be found", 0.075f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.RockBlock, BlockTypeEnum.Rock, "Commonly found underground", "This is a raw material that must be found", 0.1f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.SandBlock, BlockTypeEnum.Sand, "Found on the surface", "This is a raw material that must be found", 0.01f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.DirtBlock, BlockTypeEnum.Dirt, "Found on the surface", "This is a raw material that must be found", 0.01f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.LogBlock, BlockTypeEnum.Log, "Comes from trees", "This is a raw material that must be found", 0.075f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.LanternBlock, BlockTypeEnum.Lantern, "Lights the world", "More durable than a torch", 0.075f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.BloodStoneBlock, BlockTypeEnum.BloodStone, "Found in hell", "Bloodstone is very hard", 0.15f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.SpaceRock, BlockTypeEnum.SpaceRock, "Comes from space", "Used to make SCI-FI tools", 0.15f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.IronWall, BlockTypeEnum.IronWall, "Strong walls for building", "Prevents some monsters from digging", 0.1f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.CopperWall, BlockTypeEnum.CopperWall, "Strong walls for building", "Prevents some monsters from digging", 0.1f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.GoldenWall, BlockTypeEnum.GoldenWall, "Strong walls for building", "Prevents some monsters from digging", 0.1f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.DiamondWall, BlockTypeEnum.DiamondWall, "Strong walls for building", "Prevents some monsters from digging", 0.1f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.Crate, BlockTypeEnum.Crate, "Used for storing items", "", 0.1f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.Snow, BlockTypeEnum.Snow, "Found on the surface", "This is a raw material that must be found", 0.01f));
			RegisterItemClass(new BlockInventoryItemClass(InventoryItemIDs.Ice, BlockTypeEnum.Ice, "Found on the surface", "This is a raw material that must be found", 0.01f));
			Model model = content.Load<Model>("PickAxe");
			RegisterItemClass(new PickInventoryItemClass(InventoryItemIDs.StonePickAxe, ToolMaterialTypes.Stone, model, "Stone PickAxe", "Used for breaking certain stones and ores", "", 0.1f));
			RegisterItemClass(new PickInventoryItemClass(InventoryItemIDs.CopperPickAxe, ToolMaterialTypes.Copper, model, "Copper PickAxe", "Used for breaking certain stones and ores", "", 0.2f));
			RegisterItemClass(new PickInventoryItemClass(InventoryItemIDs.IronPickAxe, ToolMaterialTypes.Iron, model, "Iron PickAxe", "Used for breaking certain stones and ores", "", 0.4f));
			RegisterItemClass(new PickInventoryItemClass(InventoryItemIDs.GoldPickAxe, ToolMaterialTypes.Gold, model, "Gold PickAxe", "Used for breaking certain stones and ores", "", 0.8f));
			RegisterItemClass(new PickInventoryItemClass(InventoryItemIDs.DiamondPickAxe, ToolMaterialTypes.Diamond, model, "Diamond PickAxe", "Used for breaking certain stones and ores", "", 1.6f));
			RegisterItemClass(new PickInventoryItemClass(InventoryItemIDs.BloodstonePickAxe, ToolMaterialTypes.BloodStone, model, "BloodStone PickAxe", "Used for breaking certain stones and ores", "", 3f));
			Model model2 = content.Load<Model>("Spade");
			RegisterItemClass(new SpadeInventoryClass(InventoryItemIDs.StoneSpade, ToolMaterialTypes.Stone, model2, "Stone Spade", "Used for digging dirt and sand", "", 0.1f));
			RegisterItemClass(new SpadeInventoryClass(InventoryItemIDs.CopperSpade, ToolMaterialTypes.Copper, model2, "Copper Spade", "Used for digging dirt and sand", "", 0.2f));
			RegisterItemClass(new SpadeInventoryClass(InventoryItemIDs.IronSpade, ToolMaterialTypes.Iron, model2, "Iron Spade", "Used for digging dirt and sand", "", 0.4f));
			RegisterItemClass(new SpadeInventoryClass(InventoryItemIDs.GoldSpade, ToolMaterialTypes.Gold, model2, "Gold Spade", "Used for digging dirt and sand", "", 0.8f));
			RegisterItemClass(new SpadeInventoryClass(InventoryItemIDs.DiamondSpade, ToolMaterialTypes.Diamond, model2, "Diamond Spade", "Used for digging dirt and sand", "", 1.6f));
			Model model3 = content.Load<Model>("Axe");
			RegisterItemClass(new AxeInventoryClass(InventoryItemIDs.StoneAxe, ToolMaterialTypes.Stone, model3, "Stone Axe", "Used for chopping wood", "Can also be used for basic melee defense", 0.15f));
			RegisterItemClass(new AxeInventoryClass(InventoryItemIDs.CopperAxe, ToolMaterialTypes.Copper, model3, "Copper Axe", "Used for chopping wood", "Can also be used for basic melee defense", 0.3f));
			RegisterItemClass(new AxeInventoryClass(InventoryItemIDs.IronAxe, ToolMaterialTypes.Iron, model3, "Iron Axe", "Used for chopping wood", "Can also be used for basic melee defense", 0.5f));
			RegisterItemClass(new AxeInventoryClass(InventoryItemIDs.GoldAxe, ToolMaterialTypes.Gold, model3, "Gold Axe", "Used for chopping wood", "Can also be used for basic melee defense", 1f));
			RegisterItemClass(new AxeInventoryClass(InventoryItemIDs.DiamondAxe, ToolMaterialTypes.Diamond, model3, "Diamond Axe", "Used for chopping wood", "Can also be used for basic melee defense", 2f));
			Model model4 = content.Load<Model>("Ammo");
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.BrassCasing, model4, "Brass Casing", "Used for making ammunition", "", 5000, TimeSpan.FromSeconds(0.3), Color.Transparent, CMZColors.Brass));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.IronCasing, model4, "Iron Casing", "Used for making ammunition", "", 5000, TimeSpan.FromSeconds(0.3), Color.Transparent, CMZColors.Iron));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.GoldCasing, model4, "Gold Casing", "Used for making ammunition", "", 5000, TimeSpan.FromSeconds(0.3), Color.Transparent, CMZColors.Gold));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.Bullets, model4, "Bullets", "Ammo for conventional weapons", "", 5000, TimeSpan.FromSeconds(0.3), Color.DarkGray, CMZColors.Brass));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.IronBullets, model4, "Iron Bullets", "Ammo for gold weapons", "", 5000, TimeSpan.FromSeconds(0.3), Color.LightGray, CMZColors.Brass));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.GoldBullets, model4, "Gold Bullets", "Ammo for diamond weapons", "", 5000, TimeSpan.FromSeconds(0.3), new Color(255, 215, 0), CMZColors.Iron));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.DiamondBullets, model4, "Diamond Bullets", "Ammo for bloodstone", "", 5000, TimeSpan.FromSeconds(0.3), Color.Cyan, CMZColors.Gold));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.BloodStoneBullets, model4, "BloodStone Bullets", "", "", 5000, TimeSpan.FromSeconds(0.3), Color.DarkRed, CMZColors.Diamond));
			RegisterItemClass(new StickInventoryItemClass(InventoryItemIDs.Stick, Color.White, model, "Wood Stick", "Use this to make various items", "Such as a pickaxe or a torch", 0.05f));
			RegisterItemClass(new TorchInventoryItemClass());
			RegisterItemClass(new DoorInventoryItemClass());
			Model model5 = content.Load<Model>("Ore");
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.Coal, model5, "Coal", "Used to craft items", "This is a raw material that must be found", 64, TimeSpan.FromSeconds(0.30000001192092896), CMZColors.Coal, CMZColors.Coal));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.IronOre, model5, "Iron Ore", "Can be made into iron", "This is a raw material that must be found", 64, TimeSpan.FromSeconds(0.30000001192092896), CMZColors.IronOre, Color.White));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.CopperOre, model5, "Copper Ore", "Can be made into copper", "This is a raw material that must be found", 64, TimeSpan.FromSeconds(0.30000001192092896), CMZColors.CopperOre, Color.White));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.GoldOre, model5, "Gold Ore", "Can be made into gold", "This is a raw material that must be found", 64, TimeSpan.FromSeconds(0.30000001192092896), CMZColors.Gold, Color.White));
			Model model6 = content.Load<Model>("Gems");
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.Diamond, model6, "Diamond", "Very hard substance", "Used to make diamond tools", 64, TimeSpan.FromSeconds(0.30000001192092896), CMZColors.Diamond, Color.White));
			Model model7 = content.Load<Model>("Bars");
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.Iron, model7, "Iron", "Used to craft items", "Made from Iron ore", 64, TimeSpan.FromSeconds(0.30000001192092896), CMZColors.Iron, Color.White));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.Copper, model7, "Copper", "Used to craft items", "Made from Copper ore", 64, TimeSpan.FromSeconds(0.30000001192092896), CMZColors.Copper, Color.White));
			RegisterItemClass(new ModelInventoryItemClass(InventoryItemIDs.Gold, model7, "Gold", "Used to craft items", "Made from Gold ore", 64, TimeSpan.FromSeconds(0.30000001192092896), CMZColors.Gold, Color.White));
			Model model8 = content.Load<Model>("Compass");
			RegisterItemClass(new CompassInventoryItemClass(InventoryItemIDs.Compass, model8));
			Model model9 = content.Load<Model>("Locator");
			Model model10 = content.Load<Model>("Teleporter");
			RegisterItemClass(new GPSItemClass(InventoryItemIDs.GPS, model9, "Locator", "Show the direction to a chosen location and GPS coordinates", ""));
			RegisterItemClass(new GPSItemClass(InventoryItemIDs.TeleportGPS, model10, "Teleporter", "Show the direction to a chosen location and GPS coordinates", "Use the item by pressing the left trigger to teleport to the chosen location"));
			Model model11 = content.Load<Model>("Clock");
			RegisterItemClass(new ClockInventoryItemClass(InventoryItemIDs.Clock, model11));
			RegisterItemClass(new BareHandInventoryItemClass());
			RegisterItemClass(new KnifeInventoryItemClass(InventoryItemIDs.Knife, ToolMaterialTypes.Iron, "Knife", "Basic Melee Defense", "", 0.5f, 0.02f, TimeSpan.FromSeconds(0.5)));
			RegisterItemClass(new KnifeInventoryItemClass(InventoryItemIDs.GoldKnife, ToolMaterialTypes.Gold, "Gold Knife", "Basic Melee Defense", "", 1f, 0.01f, TimeSpan.FromSeconds(0.4)));
			RegisterItemClass(new KnifeInventoryItemClass(InventoryItemIDs.DiamondKnife, ToolMaterialTypes.Diamond, "Diamond Knife", "Basic Melee Defense", "", 2f, 0.005f, TimeSpan.FromSeconds(0.3)));
			RegisterItemClass(new KnifeInventoryItemClass(InventoryItemIDs.BloodStoneKnife, ToolMaterialTypes.BloodStone, "BloodStone Knife", "Basic Melee Defense", "", 4f, 0.00333333341f, TimeSpan.FromSeconds(0.25)));
			RegisterItemClass(new AssultRifleInventoryItemClass(InventoryItemIDs.AssultRifle, ToolMaterialTypes.Iron, "Assault Rifle", "High power full auto", "Uses Bullets", 0.5f, 0.001f, GetClass(InventoryItemIDs.Bullets)));
			RegisterItemClass(new PumpShotgunInventoryItemClass(InventoryItemIDs.PumpShotgun, ToolMaterialTypes.Iron, "Shotgun", "Short range burst fire", "Uses Bullets", 0.3f, 0.001f, GetClass(InventoryItemIDs.Bullets)));
			RegisterItemClass(new SMGInventoryItemClass(InventoryItemIDs.SMGGun, ToolMaterialTypes.Iron, "Sub Machine Gun", "Hige rate of fire", "Uses Bullets", 0.3f, 0.001f, GetClass(InventoryItemIDs.Bullets)));
			RegisterItemClass(new BoltRifleInventoryItemClass(InventoryItemIDs.BoltActionRifle, ToolMaterialTypes.Iron, "Rifle", "High power very accurate", "Uses Bullets", 0.5f, 0.001f, GetClass(InventoryItemIDs.Bullets)));
			RegisterItemClass(new PistolInventoryItemClass(InventoryItemIDs.Pistol, ToolMaterialTypes.Iron, "Pistol", "Basic semi automatic gun", "Uses Bullets", 0.3f, 0.001f, GetClass(InventoryItemIDs.Bullets)));
			RegisterItemClass(new AssultRifleInventoryItemClass(InventoryItemIDs.GoldAssultRifle, ToolMaterialTypes.Gold, "Gold Assault Rifle", "High power full auto", "Uses Iron Bullets", 2.5f, 0.000454545458f, GetClass(InventoryItemIDs.IronBullets)));
			RegisterItemClass(new PumpShotgunInventoryItemClass(InventoryItemIDs.GoldPumpShotgun, ToolMaterialTypes.Gold, "Gold Shotgun", "Short range burst fire", "Uses Iron Bullets", 1f, 0.000454545458f, GetClass(InventoryItemIDs.IronBullets)));
			RegisterItemClass(new SMGInventoryItemClass(InventoryItemIDs.GoldSMGGun, ToolMaterialTypes.Gold, "Gold Sub Machine Gun", "Hige rate of fire", "Uses Iron Bullets", 1f, 0.000454545458f, GetClass(InventoryItemIDs.IronBullets)));
			RegisterItemClass(new BoltRifleInventoryItemClass(InventoryItemIDs.GoldBoltActionRifle, ToolMaterialTypes.Gold, "Gold Rifle", "High power very accurate", "Uses Iron Bullets", 2.5f, 0.000454545458f, GetClass(InventoryItemIDs.IronBullets)));
			RegisterItemClass(new PistolInventoryItemClass(InventoryItemIDs.GoldPistol, ToolMaterialTypes.Gold, "Gold Pistol", "Basic semi automatic gun", "Uses Iron Bullets", 1f, 0.000454545458f, GetClass(InventoryItemIDs.IronBullets)));
			RegisterItemClass(new AssultRifleInventoryItemClass(InventoryItemIDs.DiamondAssultRifle, ToolMaterialTypes.Diamond, "Diamond Assault Rifle", "High power full auto", "Uses Gold Bullets", 6f, 0.000239234447f, GetClass(InventoryItemIDs.GoldBullets)));
			RegisterItemClass(new PumpShotgunInventoryItemClass(InventoryItemIDs.DiamondPumpShotgun, ToolMaterialTypes.Diamond, "Diamond Shotgun", "Short range burst fire", "Uses Gold Bullets", 4f, 0.000239234447f, GetClass(InventoryItemIDs.GoldBullets)));
			RegisterItemClass(new SMGInventoryItemClass(InventoryItemIDs.DiamondSMGGun, ToolMaterialTypes.Diamond, "Diamond Sub Machine Gun", "Hige rate of fire", "Uses Gold Bullets", 4f, 0.000239234447f, GetClass(InventoryItemIDs.GoldBullets)));
			RegisterItemClass(new BoltRifleInventoryItemClass(InventoryItemIDs.DiamondBoltActionRifle, ToolMaterialTypes.Diamond, "Diamond Rifle", "High power very accurate", "Uses Gold Bullets", 6f, 0.000239234447f, GetClass(InventoryItemIDs.GoldBullets)));
			RegisterItemClass(new PistolInventoryItemClass(InventoryItemIDs.DiamondPistol, ToolMaterialTypes.Diamond, "Diamond Pistol", "Basic semi automatic gun", "Uses Gold Bullets", 4f, 0.000239234447f, GetClass(InventoryItemIDs.GoldBullets)));
			RegisterItemClass(new AssultRifleInventoryItemClass(InventoryItemIDs.BloodStoneAssultRifle, ToolMaterialTypes.BloodStone, "BloodStone Assault Rifle", "High power full auto", "Uses Diamond Bullets", 12f, 3.84615378E-05f, GetClass(InventoryItemIDs.DiamondBullets)));
			RegisterItemClass(new PumpShotgunInventoryItemClass(InventoryItemIDs.BloodStonePumpShotgun, ToolMaterialTypes.BloodStone, "BloodStone Shotgun", "Short range burst fire", "Uses Diamond Bullets", 8f, 3.84615378E-05f, GetClass(InventoryItemIDs.DiamondBullets)));
			RegisterItemClass(new SMGInventoryItemClass(InventoryItemIDs.BloodStoneSMGGun, ToolMaterialTypes.BloodStone, "BloodStone Sub Machine Gun", "Hige rate of firee", "Uses Diamond Bullets", 8f, 3.84615378E-05f, GetClass(InventoryItemIDs.DiamondBullets)));
			RegisterItemClass(new BoltRifleInventoryItemClass(InventoryItemIDs.BloodStoneBoltActionRifle, ToolMaterialTypes.BloodStone, "BloodStone Rifle", "High power very accurate", "Uses Diamond Bullets", 12f, 3.84615378E-05f, GetClass(InventoryItemIDs.DiamondBullets)));
			RegisterItemClass(new PistolInventoryItemClass(InventoryItemIDs.BloodStonePistol, ToolMaterialTypes.BloodStone, "BloodStone Pistol", "Basic semi automatic gun", "Uses Diamond Bullets", 8f, 3.84615378E-05f, GetClass(InventoryItemIDs.DiamondBullets)));
		}

		private static void RegisterItemClass(InventoryItemClass itemClass)
		{
			AllItems[itemClass.ID] = itemClass;
		}

		public static void FinishInitialization(GraphicsDevice device)
		{
			if (_2DImages != null)
			{
				return;
			}
			_2DImages = new RenderTarget2D(CastleMinerZGame.Instance.GraphicsDevice, 512, 640, false, SurfaceFormat.Color, DepthFormat.Depth16);
			Viewport viewport = device.Viewport;
			RasterizerState rasterizerState = device.RasterizerState;
			DepthStencilState depthStencilState = device.DepthStencilState;
			device.SetRenderTarget(_2DImages);
			Color color = new Color(0f, 0f, 0f, 0f);
			device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, color, 1f, 0);
			device.Viewport = new Viewport(0, 0, 512, 640);
			device.RasterizerState = RasterizerState.CullCounterClockwise;
			device.DepthStencilState = DepthStencilState.Default;
			Matrix projection = Matrix.CreateOrthographic(512f, 640f, 0.1f, 500f);
			GameTime gameTime = new GameTime();
			BlockEntity.InitUIRendering(projection);
			foreach (InventoryItemClass value in AllItems.Values)
			{
				int iD = (int)value.ID;
				Entity entity = value.CreateEntity(ItemUse.UI, false);
				Vector3 vector = new Vector3(-256 + (iD & 7) * 64 + 32, -320 + iD / 8 * 64 + 32, -200f);
				vector.Y = 0f - vector.Y;
				entity.LocalPosition += vector;
				entity.Update(CastleMinerZGame.Instance, gameTime);
				entity.Draw(device, gameTime, Matrix.Identity, projection);
			}
			device.SetRenderTarget(null);
			device.Viewport = viewport;
			device.RasterizerState = rasterizerState;
			device.DepthStencilState = depthStencilState;
		}

		public static InventoryItem Create(BinaryReader reader)
		{
			InventoryItem inventoryItem = null;
			InventoryItemIDs id = (InventoryItemIDs)reader.ReadInt16();
			inventoryItem = CreateItem(id, 0);
			inventoryItem.Read(reader);
			return inventoryItem;
		}

		public virtual bool IsValid()
		{
			if (_stackCount <= MaxStackCount && _stackCount > 0 && ItemClass.ID != InventoryItemIDs.SpaceRock)
			{
				return ItemClass.ID != InventoryItemIDs.BloodStoneBullets;
			}
			return false;
		}

		public virtual void GetDisplayText(StringBuilder builder)
		{
			builder.Append(_class.Name);
		}

		public bool CanStack(InventoryItem item)
		{
			if (item != this && _class == item._class)
			{
				return StackCount < MaxStackCount;
			}
			return false;
		}

		public void Stack(InventoryItem item)
		{
			if (_class == item._class && item != this && StackCount < MaxStackCount)
			{
				StackCount += item.StackCount;
				item.StackCount = 0;
				if (StackCount > MaxStackCount)
				{
					item.StackCount += StackCount - MaxStackCount;
					StackCount = MaxStackCount;
				}
			}
		}

		public InventoryItem Split()
		{
			InventoryItem inventoryItem = CreateItem(ItemClass.ID, StackCount / 2);
			StackCount -= inventoryItem.StackCount;
			return inventoryItem;
		}

		public InventoryItem PopOneItem()
		{
			InventoryItem result = CreateItem(ItemClass.ID, 1);
			StackCount--;
			return result;
		}

		protected InventoryItem(InventoryItemClass cls, int stackCount)
		{
			_class = cls;
			_coolDownTimer = new OneShotTimer(_class.CoolDownTime);
			StackCount = stackCount;
		}

		public bool CanConsume(InventoryItemClass itemType, int amount)
		{
			if (_class != itemType || StackCount < amount)
			{
				return false;
			}
			return true;
		}

		public virtual InventoryItem CreatesWhenDug(BlockTypeEnum block)
		{
			switch (block)
			{
			case BlockTypeEnum.Grass:
				return CreateItem(InventoryItemIDs.DirtBlock, 1);
			case BlockTypeEnum.SpaceRock:
				return CreateItem(InventoryItemIDs.DirtBlock, 1);
			case BlockTypeEnum.SurfaceLava:
				return CreateItem(InventoryItemIDs.RockBlock, 1);
			default:
				return BlockInventoryItemClass.CreateBlockItem(block, 1);
			}
		}

		public virtual bool InflictDamage()
		{
			ItemHealthLevel -= ItemClass.ItemSelfDamagePerUse;
			if (CastleMinerZGame.Instance.InfiniteResourceMode)
			{
				ItemHealthLevel -= ItemClass.ItemSelfDamagePerUse;
			}
			if (ItemHealthLevel <= 0f)
			{
				return true;
			}
			return false;
		}

		public virtual TimeSpan TimeToDig(BlockTypeEnum blockType)
		{
			switch (blockType)
			{
			case BlockTypeEnum.SpaceRock:
				return TimeSpan.FromSeconds(1.0);
			case BlockTypeEnum.SurfaceLava:
				return TimeSpan.FromSeconds(0.0);
			case BlockTypeEnum.Rock:
				return TimeSpan.FromSeconds(10.0);
			case BlockTypeEnum.Ice:
				return TimeSpan.FromSeconds(5.0);
			case BlockTypeEnum.Log:
				return TimeSpan.FromSeconds(4.0);
			case BlockTypeEnum.Wood:
				return TimeSpan.FromSeconds(3.0);
			case BlockTypeEnum.Leaves:
				return TimeSpan.FromSeconds(1.0);
			case BlockTypeEnum.Sand:
				return TimeSpan.FromSeconds(1.0);
			case BlockTypeEnum.Snow:
				return TimeSpan.FromSeconds(1.0);
			case BlockTypeEnum.Dirt:
				return TimeSpan.FromSeconds(1.5);
			case BlockTypeEnum.Grass:
				return TimeSpan.FromSeconds(1.5);
			case BlockTypeEnum.Torch:
				return TimeSpan.FromSeconds(0.0);
			case BlockTypeEnum.LowerDoor:
				return TimeSpan.FromSeconds(2.0);
			case BlockTypeEnum.Lantern:
				return TimeSpan.FromSeconds(2.0);
			case BlockTypeEnum.Crate:
				return TimeSpan.FromSeconds(2.0);
			default:
				return TimeSpan.MaxValue;
			}
		}

		public virtual void ProcessInput(InGameHUD hud, CastleMinerZControllerMapping controller)
		{
			if (hud.ConstructionProbe._worldIndex != DigLocation)
			{
				DigLocation = hud.ConstructionProbe._worldIndex;
				DigTime = TimeSpan.Zero;
			}
			if (controller.Use.Held || controller.Shoulder.Held)
			{
				BlockTypeEnum blockWithChanges = BlockTerrain.Instance.GetBlockWithChanges(hud.ConstructionProbe._worldIndex);
				BlockType type = BlockType.GetType(blockWithChanges);
				TimeSpan timeSpan = TimeToDig(type.ParentBlockType);
				float crackAmount = (float)(DigTime.TotalSeconds / timeSpan.TotalSeconds);
				CastleMinerZGame.Instance.GameScreen.CrackBox.CrackAmount = crackAmount;
				if (type.IsItemEntity)
				{
					CastleMinerZGame.Instance.GameScreen.CrackBox.CrackAmount = 0f;
				}
				if (CoolDownTimer.Expired)
				{
					CoolDownTimer.Reset();
					hud.LocalPlayer.UsingTool = true;
					CastleMinerZPlayerStats.ItemStats itemStats = CastleMinerZGame.Instance.PlayerStats.GetItemStats(ItemClass.ID);
					itemStats.Used++;
					if (hud.ConstructionProbe.AbleToBuild)
					{
						if (DigTime >= timeSpan)
						{
							hud.Dig(this, true);
							DigTime = TimeSpan.Zero;
						}
						else
						{
							hud.Dig(this, false);
						}
					}
					else if (hud.ConstructionProbe.HitZombie)
					{
						hud.Melee(this);
					}
					return;
				}
			}
			else
			{
				DigTime = TimeSpan.Zero;
			}
			hud.LocalPlayer.UsingTool = false;
		}

		public void Update(GameTime gameTime)
		{
			if (InGameHUD.Instance != null && InGameHUD.Instance.ConstructionProbe.AbleToBuild)
			{
				DigTime += gameTime.get_ElapsedGameTime();
			}
			else
			{
				DigTime = TimeSpan.Zero;
			}
			_coolDownTimer.Update(gameTime.get_ElapsedGameTime());
		}

		public void Draw2D(SpriteBatch spriteBatch, Rectangle dest, Color color)
		{
			_class.Draw2D(spriteBatch, dest, color);
			if (StackCount > 1)
			{
				sbuilder.Length = 0;
				sbuilder.Concat(StackCount);
				SpriteFont smallFont = CastleMinerZGame.Instance._smallFont;
				spriteBatch.DrawOutlinedText(smallFont, sbuilder, new Vector2(dest.X + 8, dest.Y + dest.Height - smallFont.LineSpacing), Color.White, Color.Black, 1);
			}
		}

		public void Draw2D(SpriteBatch spriteBatch, Rectangle dest)
		{
			if (ItemClass.ItemSelfDamagePerUse > 0f)
			{
				spriteBatch.Draw(CastleMinerZGame.Instance.DummyTexture, new Rectangle(dest.X + 9, dest.Bottom - 16, dest.Width - 18, 7), Color.Black);
				spriteBatch.Draw(CastleMinerZGame.Instance.DummyTexture, new Rectangle(dest.X + 10, dest.Bottom - 15, (int)((float)(dest.Width - 20) * ItemHealthLevel), 5), new Color(67, 188, 0));
			}
			Draw2D(spriteBatch, dest, Color.White);
		}

		public Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			return _class.CreateEntity(use, attachedToLocalPlayer);
		}

		public virtual void Write(BinaryWriter writer)
		{
			writer.Write((short)_class.ID);
			writer.Write((short)StackCount);
			writer.Write(ItemHealthLevel);
		}

		protected virtual void Read(BinaryReader reader)
		{
			StackCount = reader.ReadInt16();
			ItemHealthLevel = reader.ReadSingle();
		}
	}
}
