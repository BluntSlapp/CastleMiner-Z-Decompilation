using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class PistolInventoryItemClass : GunInventoryItemClass
	{
		public PistolInventoryItemClass(InventoryItemIDs id, ToolMaterialTypes material, string name, string description1, string description2, float damage, float durabilitydamage, InventoryItem.InventoryItemClass ammotype)
			: base(id, CastleMinerZGame.Instance.Content.Load<Model>("Colt"), name, description1, description2, TimeSpan.FromSeconds(0.10000000149011612), material, damage, durabilitydamage, ammotype, "GunShot4", "Reload")
		{
			_playerMode = PlayerMode.Pistol;
			ShoulderMagnification = 1.2f;
			ReloadTime = TimeSpan.FromSeconds(1.6299999952316284);
			Angle.FromDegrees(6f);
			Automatic = false;
			RoundsPerReload = (ClipCapacity = 8);
			Innaccuracy = 0.05f;
			FlightTime = 1f;
			Velocity = 75f;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			ModelEntity modelEntity = (ModelEntity)base.CreateEntity(use, attachedToLocalPlayer);
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f) * Quaternion.CreateFromYawPitchRoll(0f, -(float)Math.PI / 2f, 0f);
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(25.6f / modelEntity.GetLocalBoundingSphere().Radius), rotation);
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
