using System;
using System.IO;
using System.Text;
using DNA.Audio;
using DNA.CastleMinerZ.Net;
using DNA.CastleMinerZ.UI;
using DNA.Drawing.UI;
using DNA.IO;
using DNA.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA.CastleMinerZ.Inventory
{
	public class GPSItem : InventoryItem
	{
		private Vector3 _pointToLocation = Vector3.Zero;

		private string _GPSname = "Alpha";

		public Vector3 PointToLocation
		{
			get
			{
				return _pointToLocation;
			}
		}

		public Color color
		{
			get
			{
				switch (GPSClass.ID)
				{
				case InventoryItemIDs.GPS:
					return CMZColors.GetMaterialcColor(ToolMaterialTypes.Gold);
				case InventoryItemIDs.TeleportGPS:
					return CMZColors.GetMaterialcColor(ToolMaterialTypes.BloodStone);
				default:
					return CMZColors.GetMaterialcColor(ToolMaterialTypes.Diamond);
				}
			}
		}

		public GPSItemClass GPSClass
		{
			get
			{
				return (GPSItemClass)base.ItemClass;
			}
		}

		public override void GetDisplayText(StringBuilder builder)
		{
			base.GetDisplayText(builder);
			Vector3 localPosition = CastleMinerZGame.Instance.LocalPlayer.LocalPosition;
			builder.Append(": ");
			builder.Append(_GPSname);
			builder.Append(" - ");
			builder.Append("Distance: ");
			builder.Concat((int)Vector3.Distance(localPosition, PointToLocation));
		}

		public GPSItem(GPSItemClass classType, int stackCount)
			: base(classType, stackCount)
		{
		}

		protected override void Read(BinaryReader reader)
		{
			base.Read(reader);
			_pointToLocation = reader.ReadVector3();
			_GPSname = reader.ReadString();
		}

		public override void Write(BinaryWriter writer)
		{
			base.Write(writer);
			writer.Write(_pointToLocation);
			writer.Write(_GPSname);
		}

		private void ShowKeyboard()
		{
			Guide.BeginShowKeyboardInput(Screen.SelectedPlayerIndex.Value, "Name", "Enter A Name For This Locator", _GPSname, (AsyncCallback)delegate(IAsyncResult result)
			{
				string text = Guide.EndShowKeyboardInput(result);
				if (text != null)
				{
					if (text.Length > 10)
					{
						_GPSname = text.Substring(0, 10);
					}
					else
					{
						_GPSname = text;
					}
				}
			}, (object)null);
		}

		public void PlaceLocator(InGameHUD hud)
		{
			SoundManager.Instance.PlayInstance("locator");
			_pointToLocation = hud.ConstructionProbe._worldIndex + Vector3.Zero;
		}

		public override void ProcessInput(InGameHUD hud, CastleMinerZControllerMapping controller)
		{
			if (controller.Reload.Pressed)
			{
				ShowKeyboard();
				return;
			}
			switch (GPSClass.ID)
			{
			case InventoryItemIDs.GPS:
				if (controller.Use.Pressed)
				{
					PlaceLocator(hud);
					if (InflictDamage())
					{
						hud.PlayerInventory.Remove(this);
					}
					else
					{
						ShowKeyboard();
					}
				}
				break;
			case InventoryItemIDs.TeleportGPS:
				if (controller.Use.Pressed)
				{
					PlaceLocator(hud);
					ShowKeyboard();
				}
				else
				{
					if (!controller.Shoulder.Pressed)
					{
						break;
					}
					if (_pointToLocation == Vector3.Zero)
					{
						SoundManager.Instance.PlayInstance("Error");
						break;
					}
					SoundManager.Instance.PlayInstance("Teleport");
					string message = CastleMinerZGame.Instance.MyNetworkGamer.Gamertag + " Teleported To " + _GPSname;
					BroadcastTextMessage.Send(CastleMinerZGame.Instance.MyNetworkGamer, message);
					CastleMinerZGame.Instance.GameScreen.TeleportToLocation(_pointToLocation, false);
					if (InflictDamage())
					{
						hud.PlayerInventory.Remove(this);
					}
				}
				break;
			}
		}
	}
}
