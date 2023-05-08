using System.IO;
using System.Text;
using DNA.CastleMinerZ.UI;
using DNA.Text;

namespace DNA.CastleMinerZ.Inventory
{
	public class GunInventoryItem : InventoryItem
	{
		private int _roundsInClip;

		private int _ammoCount;

		private bool gunReleased;

		public int RoundsInClip
		{
			get
			{
				return _roundsInClip;
			}
			set
			{
				_roundsInClip = value;
			}
		}

		public int AmmoCount
		{
			get
			{
				return _ammoCount;
			}
		}

		public GunInventoryItemClass GunClass
		{
			get
			{
				return (GunInventoryItemClass)base.ItemClass;
			}
		}

		public override void GetDisplayText(StringBuilder builder)
		{
			base.GetDisplayText(builder);
			builder.Append(" ");
			builder.Concat(RoundsInClip);
			builder.Append("/");
			builder.Concat(AmmoCount);
		}

		public GunInventoryItem(GunInventoryItemClass classtype, int stackCount)
			: base(classtype, stackCount)
		{
			RoundsInClip = classtype.ClipCapacity;
		}

		protected override void Read(BinaryReader reader)
		{
			base.Read(reader);
			RoundsInClip = reader.ReadInt32();
		}

		public override void Write(BinaryWriter writer)
		{
			base.Write(writer);
			writer.Write(RoundsInClip);
		}

		public bool Reload(InGameHUD hud)
		{
			_ammoCount = GunClass.AmmoCount(hud.PlayerInventory);
			int num = GunClass.ClipCapacity - RoundsInClip;
			if (num > _ammoCount)
			{
				num = _ammoCount;
			}
			if (num > GunClass.RoundsPerReload)
			{
				num = GunClass.RoundsPerReload;
			}
			if (num <= 0)
			{
				return false;
			}
			if (hud.PlayerInventory.Consume(GunClass.AmmoType, num))
			{
				RoundsInClip += num;
				_ammoCount -= num;
			}
			if (RoundsInClip < GunClass.ClipCapacity)
			{
				return _ammoCount > 0;
			}
			return false;
		}

		public override void ProcessInput(InGameHUD hud, CastleMinerZControllerMapping controller)
		{
			hud.LocalPlayer.Shouldering = controller.Shoulder.Held;
			_ammoCount = GunClass.AmmoCount(hud.PlayerInventory);
			bool flag = RoundsInClip < GunClass.ClipCapacity && _ammoCount > 0;
			if (flag && controller.Reload.Pressed)
			{
				gunReleased = !controller.Use.Held;
				hud.LocalPlayer.Reloading = true;
			}
			if (gunReleased && controller.Use.Pressed)
			{
				hud.LocalPlayer.Reloading = false;
			}
			if (!hud.LocalPlayer.Reloading && base.CoolDownTimer.Expired && ((controller.Use.Held && GunClass.Automatic) || controller.Use.Pressed))
			{
				if (RoundsInClip > 0)
				{
					hud.LocalPlayer.Reloading = false;
					hud.Shoot((GunInventoryItemClass)base.ItemClass);
					base.CoolDownTimer.Reset();
					RoundsInClip--;
					hud.LocalPlayer.UsingTool = true;
					CastleMinerZPlayerStats.ItemStats itemStats = CastleMinerZGame.Instance.PlayerStats.GetItemStats(base.ItemClass.ID);
					itemStats.Used++;
					hud.LocalPlayer.ApplyRecoil(GunClass.Recoil);
					if (InflictDamage())
					{
						hud.PlayerInventory.Remove(this);
					}
					if (RoundsInClip <= 0 && _ammoCount > 0)
					{
						hud.LocalPlayer.Reloading = true;
					}
					return;
				}
				if (flag)
				{
					hud.LocalPlayer.Reloading = true;
				}
			}
			hud.LocalPlayer.UsingTool = false;
		}
	}
}
