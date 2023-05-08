using System;
using DNA.CastleMinerZ.AI;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class StickInventoryItemClass : ModelInventoryItemClass
	{
		public StickInventoryItemClass(InventoryItemIDs id, Color color, Model model, string name, string description1, string description2, float meleeDamage)
			: base(id, model, name, description1, description2, 64, TimeSpan.FromSeconds(0.30000001192092896), color)
		{
			_playerMode = PlayerMode.Generic;
			EnemyDamage = meleeDamage;
			EnemyDamageType = DamageType.BLUNT;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			CastleMinerToolModel castleMinerToolModel = new CastleMinerToolModel(_model);
			castleMinerToolModel.EnablePerPixelLighting();
			castleMinerToolModel.ToolColor = Color.Transparent;
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f);
				Matrix matrix2 = (castleMinerToolModel.LocalToParent = Matrix.Transform(Matrix.CreateScale(32f / castleMinerToolModel.GetLocalBoundingSphere().Radius), rotation));
				castleMinerToolModel.EnableDefaultLighting();
				break;
			}
			case ItemUse.Hand:
				castleMinerToolModel.LocalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI / 2f) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI / 4f);
				castleMinerToolModel.LocalPosition = new Vector3(0f, 0.11126215f, 0f);
				break;
			}
			return castleMinerToolModel;
		}
	}
}
