using System;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.Inventory
{
	public class ModelInventoryItemClass : InventoryItem.InventoryItemClass
	{
		protected Model _model;

		public Color ToolColor = Color.White;

		public Color ToolColor2 = Color.White;

		public ModelInventoryItemClass(InventoryItemIDs id, Model model, string name, string description1, string description2, int maxStack, TimeSpan ts, Color recolor1)
			: base(id, name, description1, description2, maxStack, ts)
		{
			ToolColor = recolor1;
			ToolColor2 = Color.White;
			_model = model;
		}

		public ModelInventoryItemClass(InventoryItemIDs id, Model model, string name, string description1, string description2, int maxStack, TimeSpan ts, Color recolor1, Color recolor2)
			: base(id, name, description1, description2, maxStack, ts)
		{
			ToolColor = recolor1;
			ToolColor2 = recolor2;
			_model = model;
		}

		public ModelInventoryItemClass(InventoryItemIDs id, Model model, string name, string description1, string description2, int maxStack, TimeSpan ts, Color recolor1, string useSound)
			: base(id, name, description1, description2, maxStack, ts, useSound)
		{
			ToolColor = recolor1;
			ToolColor2 = Color.White;
			_model = model;
		}

		public ModelInventoryItemClass(InventoryItemIDs id, Model model, string name, string description1, string description2, int maxStack, TimeSpan ts, Color recolor1, Color recolor2, string useSound)
			: base(id, name, description1, description2, maxStack, ts, useSound)
		{
			ToolColor = recolor1;
			ToolColor2 = recolor2;
			_model = model;
		}

		public override Entity CreateEntity(ItemUse use, bool attachedToLocalPlayer)
		{
			CastleMinerToolModel castleMinerToolModel = new CastleMinerToolModel(_model);
			castleMinerToolModel.EnablePerPixelLighting();
			castleMinerToolModel.ToolColor = ToolColor;
			castleMinerToolModel.ToolColor2 = ToolColor2;
			switch (use)
			{
			case ItemUse.UI:
			{
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(0f, 0f, -(float)Math.PI / 4f);
				float num = 28.8f / castleMinerToolModel.GetLocalBoundingSphere().Radius;
				if (ID >= InventoryItemIDs.Iron && ID <= InventoryItemIDs.Gold)
				{
					num *= 1.5f;
				}
				Matrix localToParent = Matrix.Transform(Matrix.CreateScale(num), rotation);
				if (ID >= InventoryItemIDs.BrassCasing && ID <= InventoryItemIDs.BloodStoneBullets)
				{
					Vector3 translation = castleMinerToolModel.GetLocalBoundingSphere().Center * num;
					translation.X -= 7f;
					localToParent.Translation = translation;
				}
				else if (ID == InventoryItemIDs.Diamond)
				{
					Vector3 translation2 = localToParent.Translation;
					translation2.X -= 3f;
					translation2.Y -= 10f;
					localToParent.Translation = translation2;
				}
				else if (ID >= InventoryItemIDs.Iron && ID <= InventoryItemIDs.Gold)
				{
					Vector3 translation3 = localToParent.Translation;
					translation3.X -= 5f;
					translation3.Y -= 5f;
					localToParent.Translation = translation3;
				}
				else if (ID >= InventoryItemIDs.Coal && ID < InventoryItemIDs.Diamond)
				{
					Vector3 translation4 = localToParent.Translation;
					translation4.X += 10f;
					localToParent.Translation = translation4;
				}
				castleMinerToolModel.LocalToParent = localToParent;
				castleMinerToolModel.EnableDefaultLighting();
				break;
			}
			case ItemUse.Hand:
				castleMinerToolModel.LocalRotation = new Quaternion(0.4816553f, 0.05900274f, 0.8705468f, -0.08170173f);
				castleMinerToolModel.LocalPosition = new Vector3(0f, 0.1119255f, 0f);
				if (ID >= InventoryItemIDs.Coal && ID <= InventoryItemIDs.GoldOre)
				{
					castleMinerToolModel.LocalScale = new Vector3(0.35f, 0.35f, 0.35f);
					castleMinerToolModel.LocalPosition = new Vector3(0f, 0.0719255f, 0f);
				}
				else
				{
					castleMinerToolModel.LocalScale = new Vector3(0.5f, 0.5f, 0.5f);
				}
				castleMinerToolModel.EnablePerPixelLighting();
				break;
			case ItemUse.Pickup:
				if (ID >= InventoryItemIDs.BrassCasing && ID <= InventoryItemIDs.BloodStoneBullets)
				{
					Matrix matrix2 = (castleMinerToolModel.LocalToParent = Matrix.CreateScale(2.5f));
				}
				castleMinerToolModel.EnablePerPixelLighting();
				break;
			}
			return castleMinerToolModel;
		}
	}
}
