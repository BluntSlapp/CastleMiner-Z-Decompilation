using System;
using DNA.CastleMinerZ.AI;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class PumpShotgunInventoryItemClass : GunInventoryItemClass
	{
		public PumpShotgunInventoryItemClass(InventoryItemIDs id, ToolMaterialTypes material, string name, string description1, string description2, float damage, float durabilitydamage, InventoryItem.InventoryItemClass ammotype)
			: base(id, CastleMinerZGame.Instance.Content.Load<Model>("PumpShotgun"), name, description1, description2, TimeSpan.FromMinutes(1.0 / 60.0), material, damage, durabilitydamage, ammotype, "Shotgun", "ShotGunReload")
		{
			_playerMode = PlayerMode.PumnpShotgun;
			Recoil = Angle.FromDegrees(10f);
			ReloadTime = TimeSpan.FromSeconds(0.567);
			Angle.FromDegrees(20f);
			Automatic = false;
			ClipCapacity = 6;
			RoundsPerReload = 1;
			FlightTime = 0.4f;
			Innaccuracy = 0.1f;
			Velocity = 50f;
			EnemyDamageType = DamageType.SHOTGUN;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			ModelEntity modelEntity = (ModelEntity)base.CreateEntity(use, attachedToLocalPlayer);
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f) * Quaternion.CreateFromYawPitchRoll(0f, -(float)Math.PI / 2f, 0f);
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(38.4f / modelEntity.GetLocalBoundingSphere().Radius), rotation);
				localToParent.Translation = new Vector3(4f, -12f, -16f);
				modelEntity.LocalToParent = localToParent;
				break;
			}
			case ItemUse.Pickup:
			{
				Matrix matrix2 = (modelEntity.LocalToParent = Matrix.CreateFromYawPitchRoll(0f, -(float)Math.PI / 2f, 0f));
				break;
			}
			}
			return modelEntity;
		}
	}
}
