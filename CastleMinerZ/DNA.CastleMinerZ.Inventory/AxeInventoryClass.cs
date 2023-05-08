using System;
using DNA.CastleMinerZ.AI;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class AxeInventoryClass : ModelInventoryItemClass
	{
		public ToolMaterialTypes Material;

		public AxeInventoryClass(InventoryItemIDs id, ToolMaterialTypes material, Model model, string name, string description1, string description2, float meleeDamage)
			: base(id, model, name, description1, description2, 1, TimeSpan.FromSeconds(0.30000001192092896), Color.White)
		{
			_playerMode = PlayerMode.Pick;
			Material = material;
			ToolColor = CMZColors.GetMaterialcColor(Material);
			EnemyDamage = meleeDamage;
			EnemyDamageType = DamageType.BLADE;
			switch (Material)
			{
			case ToolMaterialTypes.Wood:
				ItemSelfDamagePerUse = 0.005f;
				break;
			case ToolMaterialTypes.Stone:
				ItemSelfDamagePerUse = 0.0025f;
				break;
			case ToolMaterialTypes.Copper:
				ItemSelfDamagePerUse = 0.00125f;
				break;
			case ToolMaterialTypes.Iron:
				ItemSelfDamagePerUse = 0.0005f;
				break;
			case ToolMaterialTypes.Gold:
				ItemSelfDamagePerUse = 0.00033333333f;
				break;
			case ToolMaterialTypes.Diamond:
				ItemSelfDamagePerUse = 0.00025f;
				break;
			case ToolMaterialTypes.BloodStone:
				ItemSelfDamagePerUse = 0.0002f;
				break;
			}
		}

		public override InventoryItem CreateItem(int stackCount)
		{
			return new AxeInventoryItem(this, stackCount);
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			CastleMinerToolModel castleMinerToolModel = new CastleMinerToolModel(_model);
			castleMinerToolModel.EnablePerPixelLighting();
			castleMinerToolModel.ToolColor = ToolColor;
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f);
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(32f / castleMinerToolModel.GetLocalBoundingSphere().Radius), rotation);
				Vector3 vector2 = (localToParent.Translation = new Vector3(-10f, -10f, 0f));
				castleMinerToolModel.LocalToParent = localToParent;
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
