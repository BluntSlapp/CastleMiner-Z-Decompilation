using System;
using DNA.CastleMinerZ.AI;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class GunInventoryItemClass : ModelInventoryItemClass
	{
		private string _reloadSound;

		public ToolMaterialTypes Material;

		public TimeSpan ReloadTime = TimeSpan.FromSeconds(3.0);

		public Angle Spread = Angle.FromDegrees(10f);

		public Angle Recoil = Angle.FromDegrees(3f);

		public bool Automatic;

		public int RoundsPerReload = 32;

		public int ClipCapacity = 32;

		public float ShoulderMagnification = 1f;

		public float FlightTime = 2f;

		public float Innaccuracy = 1f;

		public float Velocity = 100f;

		public Vector4 TracerColor = Color.White.ToVector4();

		public InventoryItem.InventoryItemClass AmmoType;

		public string ReloadSound
		{
			get
			{
				return _reloadSound;
			}
		}

		public TimeSpan FireRate
		{
			get
			{
				return _coolDownTime;
			}
		}

		public override bool IsMeleeWeapon
		{
			get
			{
				return false;
			}
		}

		public GunInventoryItemClass(InventoryItemIDs id, Model model, string name, string description1, string description2, TimeSpan fireRate, ToolMaterialTypes material, float bulletDamage, float itemSelfDamage, InventoryItem.InventoryItemClass ammoClass, string shootSound, string reloadSound)
			: base(id, model, name, description1, description2, 1, fireRate, CMZColors.GetMaterialcColor(material), shootSound)
		{
			_reloadSound = reloadSound;
			AmmoType = ammoClass;
			Material = material;
			ItemSelfDamagePerUse = itemSelfDamage;
			TracerColor = CMZColors.GetMaterialcColor(Material).ToVector4();
			ToolColor = CMZColors.GetMaterialcColor(Material);
			_playerMode = PlayerMode.Assault;
			EnemyDamage = bulletDamage;
			EnemyDamageType = DamageType.BULLET;
		}

		public int AmmoCount(PlayerInventory inventory)
		{
			if (AmmoType == null)
			{
				return 1000;
			}
			return inventory.CountItems(AmmoType);
		}

		public override InventoryItem CreateItem(int stackCount)
		{
			return new GunInventoryItem(this, stackCount);
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			GunEntity gunEntity = new GunEntity(_model);
			gunEntity.EnablePerPixelLighting();
			gunEntity.ToolColor = ToolColor;
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f);
				Matrix matrix2 = (gunEntity.LocalToParent = Matrix.Transform(Matrix.CreateScale(32f / gunEntity.GetLocalBoundingSphere().Radius), rotation));
				gunEntity.EnableDefaultLighting();
				break;
			}
			case ItemUse.Hand:
				gunEntity.EnablePerPixelLighting();
				break;
			case ItemUse.Pickup:
				gunEntity.EnablePerPixelLighting();
				break;
			}
			return gunEntity;
		}
	}
}
