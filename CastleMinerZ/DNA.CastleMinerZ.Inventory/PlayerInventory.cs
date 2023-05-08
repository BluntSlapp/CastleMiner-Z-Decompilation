using System;
using System.Collections.Generic;
using System.IO;
using DNA.Audio;
using DNA.CastleMinerZ.UI;
using DNA.IO.Storage;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.Inventory
{
	public class PlayerInventory
	{
		public const int MaxTrayItems = 8;

		public const int Version = 1;

		public const string Ident = "PINV";

		public InventoryItem[] Inventory = new InventoryItem[32];

		public InventoryItem[] InventoryTray = new InventoryItem[8];

		public List<Receipe> DiscoveredRecipies = new List<Receipe>();

		public int SelectedInventoryIndex;

		private Player _player;

		private InventoryItem _bareHands = InventoryItem.CreateItem(InventoryItemIDs.BareHands, 1);

		public GameScreen GameScreen
		{
			get
			{
				return CastleMinerZGame.Instance.GameScreen;
			}
		}

		public Player Player
		{
			set
			{
				_player = value;
			}
		}

		public InventoryItem ActiveInventoryItem
		{
			get
			{
				if (InventoryTray[SelectedInventoryIndex] == null)
				{
					return _bareHands;
				}
				return InventoryTray[SelectedInventoryIndex];
			}
		}

		public void SaveToStorage(SaveDevice saveDevice, string path)
		{
			try
			{
				string directoryName = Path.GetDirectoryName(path);
				if (!saveDevice.DirectoryExists(directoryName))
				{
					saveDevice.CreateDirectory(directoryName);
				}
				saveDevice.Save(path, true, true, delegate(Stream stream)
				{
					BinaryWriter binaryWriter = new BinaryWriter(stream);
					Save(binaryWriter);
					binaryWriter.Flush();
				});
			}
			catch
			{
			}
		}

		public void LoadFromStorage(SaveDevice saveDevice, string path)
		{
			saveDevice.Load(path, delegate(Stream stream)
			{
				BinaryReader reader = new BinaryReader(stream);
				Load(reader);
			});
		}

		public void Save(BinaryWriter writer)
		{
			writer.Write("PINV");
			writer.Write(1);
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (Inventory[i] == null)
				{
					writer.Write(false);
					continue;
				}
				writer.Write(true);
				Inventory[i].Write(writer);
			}
			for (int j = 0; j < InventoryTray.Length; j++)
			{
				if (InventoryTray[j] == null)
				{
					writer.Write(false);
					continue;
				}
				writer.Write(true);
				InventoryTray[j].Write(writer);
			}
		}

		private PlayerInventory()
		{
		}

		public void Load(BinaryReader reader)
		{
			if (reader.ReadString() != "PINV")
			{
				throw new Exception("Invalid Inv File");
			}
			if (reader.ReadInt32() != 1)
			{
				throw new Exception("Wrong Inv Version");
			}
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (reader.ReadBoolean())
				{
					Inventory[i] = InventoryItem.Create(reader);
					if (Inventory[i] != null && !Inventory[i].IsValid())
					{
						Inventory[i] = null;
					}
				}
				else
				{
					Inventory[i] = null;
				}
			}
			for (int j = 0; j < InventoryTray.Length; j++)
			{
				if (reader.ReadBoolean())
				{
					InventoryTray[j] = InventoryItem.Create(reader);
					if (InventoryTray[j] != null && !InventoryTray[j].IsValid())
					{
						InventoryTray[j] = null;
					}
				}
				else
				{
					InventoryTray[j] = null;
				}
			}
			DiscoverRecipies();
		}

		public void DiscoverRecipies()
		{
			DiscoveredRecipies.Clear();
			LinkedList<Receipe> linkedList = new LinkedList<Receipe>();
			Dictionary<Receipe, bool> dictionary = new Dictionary<Receipe, bool>();
			foreach (Receipe item in Receipe.CookBook)
			{
				if (Discovered(item) && CanCraft(item))
				{
					DiscoveredRecipies.Add(item);
					linkedList.AddLast(item);
					dictionary[item] = true;
				}
			}
			foreach (Receipe item2 in Receipe.CookBook)
			{
				if (Discovered(item2) && !CanCraft(item2))
				{
					DiscoveredRecipies.Add(item2);
					linkedList.AddLast(item2);
					dictionary[item2] = true;
				}
			}
			for (LinkedListNode<Receipe> linkedListNode = linkedList.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				foreach (InventoryItem ingredient in linkedListNode.Value.Ingredients)
				{
					foreach (Receipe item3 in Receipe.CookBook)
					{
						if (item3.Result.ItemClass == ingredient.ItemClass && !dictionary.ContainsKey(item3))
						{
							dictionary[item3] = true;
							linkedList.AddLast(item3);
							DiscoveredRecipies.Add(item3);
						}
					}
				}
			}
			if (DiscoveredRecipies.Count == 0)
			{
				DiscoveredRecipies.Add(Receipe.CookBook[0]);
			}
		}

		public bool Discovered(Receipe receipe)
		{
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (Inventory[i] == null)
				{
					continue;
				}
				if (receipe.Result.ItemClass == Inventory[i].ItemClass)
				{
					return true;
				}
				if (Inventory[i].ItemClass is GunInventoryItemClass)
				{
					GunInventoryItemClass gunInventoryItemClass = (GunInventoryItemClass)Inventory[i].ItemClass;
					if (receipe.Result.ItemClass == gunInventoryItemClass.AmmoType)
					{
						return true;
					}
				}
				for (int j = 0; j < receipe.Ingredients.Count; j++)
				{
					if (receipe.Ingredients[j].ItemClass == Inventory[i].ItemClass)
					{
						return true;
					}
				}
			}
			for (int k = 0; k < InventoryTray.Length; k++)
			{
				if (InventoryTray[k] == null)
				{
					continue;
				}
				if (receipe.Result.ItemClass == InventoryTray[k].ItemClass)
				{
					return true;
				}
				if (InventoryTray[k].ItemClass is GunInventoryItemClass)
				{
					GunInventoryItemClass gunInventoryItemClass2 = (GunInventoryItemClass)InventoryTray[k].ItemClass;
					if (receipe.Result.ItemClass == gunInventoryItemClass2.AmmoType)
					{
						return true;
					}
				}
				for (int l = 0; l < receipe.Ingredients.Count; l++)
				{
					if (receipe.Ingredients[l].ItemClass == InventoryTray[k].ItemClass)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int CountItems(InventoryItem.InventoryItemClass itemClass)
		{
			int num = 0;
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (Inventory[i] != null && itemClass == Inventory[i].ItemClass)
				{
					num += Inventory[i].StackCount;
				}
			}
			for (int j = 0; j < InventoryTray.Length; j++)
			{
				if (InventoryTray[j] != null && itemClass == InventoryTray[j].ItemClass)
				{
					num += InventoryTray[j].StackCount;
				}
			}
			return num;
		}

		public bool CanCraft(Receipe receipe)
		{
			for (int i = 0; i < receipe.Ingredients.Count; i++)
			{
				int num = CountItems(receipe.Ingredients[i].ItemClass);
				if (num < receipe.Ingredients[i].StackCount)
				{
					return false;
				}
			}
			return true;
		}

		public void Craft(Receipe receipe)
		{
			if (CastleMinerZGame.Instance.InfiniteResourceMode)
			{
				InventoryItem item = receipe.Result.ItemClass.CreateItem(receipe.Result.StackCount);
				AddInventoryItem(item);
				return;
			}
			for (int i = 0; i < receipe.Ingredients.Count; i++)
			{
				InventoryItem inventoryItem = receipe.Ingredients[i];
				int num = inventoryItem.StackCount;
				for (int j = 0; j < Inventory.Length; j++)
				{
					if (Inventory[j] != null && Inventory[j].ItemClass == inventoryItem.ItemClass)
					{
						if (num < Inventory[j].StackCount)
						{
							Inventory[j].StackCount -= num;
							num = 0;
						}
						else
						{
							num -= Inventory[j].StackCount;
							Inventory[j] = null;
						}
					}
				}
				for (int k = 0; k < InventoryTray.Length; k++)
				{
					if (InventoryTray[k] != null && InventoryTray[k].ItemClass == inventoryItem.ItemClass)
					{
						if (num < InventoryTray[k].StackCount)
						{
							InventoryTray[k].StackCount -= num;
							num = 0;
						}
						else
						{
							num -= InventoryTray[k].StackCount;
							InventoryTray[k] = null;
						}
					}
				}
			}
			InventoryItem item2 = receipe.Result.ItemClass.CreateItem(receipe.Result.StackCount);
			AddInventoryItem(item2);
		}

		public void Remove(InventoryItem item)
		{
			for (int i = 0; i < InventoryTray.Length; i++)
			{
				if (InventoryTray[i] == item)
				{
					InventoryTray[i] = null;
				}
			}
			for (int j = 0; j < Inventory.Length; j++)
			{
				if (Inventory[j] == item)
				{
					Inventory[j] = null;
				}
			}
		}

		public bool CanConsume(InventoryItem.InventoryItemClass itemType, int amount)
		{
			if (itemType == null)
			{
				return true;
			}
			for (int i = 0; i < InventoryTray.Length; i++)
			{
				if (InventoryTray[i] != null && InventoryTray[i].CanConsume(itemType, amount))
				{
					return true;
				}
			}
			for (int j = 0; j < Inventory.Length; j++)
			{
				if (Inventory[j] != null && Inventory[j].CanConsume(itemType, amount))
				{
					return true;
				}
			}
			return false;
		}

		public bool Consume(InventoryItem item, int amount)
		{
			if (CastleMinerZGame.Instance.InfiniteResourceMode)
			{
				return true;
			}
			if (item.StackCount >= amount)
			{
				item.StackCount -= amount;
				RemoveEmptyItems();
				return true;
			}
			return false;
		}

		public bool Consume(InventoryItem.InventoryItemClass itemType, int amount)
		{
			if (CastleMinerZGame.Instance.InfiniteResourceMode)
			{
				return true;
			}
			if (itemType == null)
			{
				return true;
			}
			int num = 0;
			int num2 = 0;
			while (num < int.MaxValue && amount > 0)
			{
				num = int.MaxValue;
				for (int i = 0; i < InventoryTray.Length; i++)
				{
					if (InventoryTray[i] != null && InventoryTray[i].ItemClass == itemType && InventoryTray[i].StackCount < num)
					{
						num = InventoryTray[i].StackCount;
						num2 = i;
					}
				}
				if (num < int.MaxValue)
				{
					if (InventoryTray[num2].StackCount > amount)
					{
						InventoryTray[num2].StackCount -= amount;
						amount = 0;
					}
					else
					{
						amount -= InventoryTray[num2].StackCount;
						InventoryTray[num2] = null;
					}
				}
			}
			num = 0;
			num2 = 0;
			while (num < int.MaxValue && amount > 0)
			{
				num = int.MaxValue;
				for (int j = 0; j < Inventory.Length; j++)
				{
					if (Inventory[j] != null && Inventory[j].ItemClass == itemType && Inventory[j].StackCount < num)
					{
						num = Inventory[j].StackCount;
						num2 = j;
					}
				}
				if (num < int.MaxValue)
				{
					if (Inventory[num2].StackCount > amount)
					{
						Inventory[num2].StackCount -= amount;
						amount = 0;
					}
					else
					{
						amount -= Inventory[num2].StackCount;
						Inventory[num2] = null;
					}
				}
			}
			return amount <= 0;
		}

		public void RemoveEmptyItems()
		{
			for (int i = 0; i < InventoryTray.Length; i++)
			{
				if (InventoryTray[i] != null && InventoryTray[i].StackCount <= 0)
				{
					InventoryTray[i] = null;
				}
			}
			for (int j = 0; j < Inventory.Length; j++)
			{
				if (Inventory[j] != null && Inventory[j].StackCount <= 0)
				{
					Inventory[j] = null;
				}
			}
		}

		public bool CanAdd(InventoryItem item)
		{
			for (int i = 0; i < InventoryTray.Length; i++)
			{
				if (InventoryTray[i] == null || InventoryTray[i].CanStack(item))
				{
					return true;
				}
			}
			for (int j = 0; j < Inventory.Length; j++)
			{
				if (Inventory[j] == null || Inventory[j].CanStack(item))
				{
					return true;
				}
			}
			return false;
		}

		public void AddInventoryItem(InventoryItem item)
		{
			for (int i = 0; i < InventoryTray.Length; i++)
			{
				if (InventoryTray[i] != null)
				{
					InventoryTray[i].Stack(item);
				}
			}
			for (int j = 0; j < Inventory.Length; j++)
			{
				if (Inventory[j] != null)
				{
					Inventory[j].Stack(item);
				}
			}
			if (item.StackCount <= 0)
			{
				DiscoverRecipies();
				return;
			}
			for (int k = 0; k < InventoryTray.Length; k++)
			{
				if (InventoryTray[k] == null)
				{
					InventoryTray[k] = item;
					DiscoverRecipies();
					return;
				}
			}
			for (int l = 0; l < Inventory.Length; l++)
			{
				if (Inventory[l] == null)
				{
					Inventory[l] = item;
					DiscoverRecipies();
					return;
				}
			}
			if (item.StackCount > 0)
			{
				Vector3 localPosition = _player.LocalPosition;
				localPosition.Y += 1f;
				PickupManager.Instance.CreatePickup(item, localPosition, true);
			}
		}

		public void DropAll(bool dropTray)
		{
			Vector3 localPosition = _player.LocalPosition;
			localPosition.Y += 1f;
			if (dropTray)
			{
				for (int i = 0; i < InventoryTray.Length; i++)
				{
					if (InventoryTray[i] != null)
					{
						PickupManager.Instance.CreatePickup(InventoryTray[i], localPosition, true);
						InventoryTray[i] = null;
					}
				}
			}
			for (int j = 0; j < Inventory.Length; j++)
			{
				if (Inventory[j] != null)
				{
					PickupManager.Instance.CreatePickup(Inventory[j], localPosition, true);
					Inventory[j] = null;
				}
			}
			DiscoverRecipies();
		}

		public void DropItem(InventoryItem item)
		{
			for (int i = 0; i < InventoryTray.Length; i++)
			{
				if (InventoryTray[i] == item)
				{
					InventoryTray[i] = null;
					Vector3 localPosition = _player.LocalPosition;
					localPosition.Y += 1f;
					PickupManager.Instance.CreatePickup(item, localPosition, true);
					SoundManager.Instance.PlayInstance("dropitem");
					DiscoverRecipies();
					return;
				}
			}
			for (int j = 0; j < Inventory.Length; j++)
			{
				if (Inventory[j] == item)
				{
					Inventory[j] = null;
					Vector3 localPosition2 = _player.LocalPosition;
					localPosition2.Y += 1f;
					PickupManager.Instance.CreatePickup(item, localPosition2, true);
					SoundManager.Instance.PlayInstance("dropitem");
					DiscoverRecipies();
					break;
				}
			}
		}

		public PlayerInventory(Player player, bool setDefault)
		{
			_player = player;
			if (setDefault)
			{
				SetDefaultInventory();
			}
		}

		public void SetDefaultInventory()
		{
			for (int i = 0; i < Inventory.Length; i++)
			{
				Inventory[i] = null;
			}
			for (int j = 0; j < InventoryTray.Length; j++)
			{
				InventoryTray[j] = null;
			}
			if (CastleMinerZGame.Instance.Difficulty != GameDifficultyTypes.HARDCORE)
			{
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.StonePickAxe, 1));
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.Compass, 1));
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.Pistol, 1));
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.Knife, 1));
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.Bullets, 200));
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.Torch, 16));
			}
			DiscoverRecipies();
		}

		public void GivePerks()
		{
			if (CastleMinerZGame.Instance.FrontEnd.PromoCodes[2].Redeemed)
			{
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.GoldPickAxe, 1));
			}
			if (CastleMinerZGame.Instance.FrontEnd.PromoCodes[3].Redeemed)
			{
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.Bullets, 1000));
			}
			if (CastleMinerZGame.Instance.FrontEnd.PromoCodes[1].Redeemed)
			{
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.AssultRifle, 1));
			}
			if (CastleMinerZGame.Instance.FrontEnd.PromoCodes[6].Redeemed)
			{
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.BloodStoneAssultRifle, 1));
			}
			if (CastleMinerZGame.Instance.FrontEnd.PromoCodes[7].Redeemed)
			{
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.BloodstonePickAxe, 1));
			}
			if (CastleMinerZGame.Instance.FrontEnd.PromoCodes[8].Redeemed)
			{
				AddInventoryItem(InventoryItem.CreateItem(InventoryItemIDs.TeleportGPS, 1));
			}
		}

		public void Update(GameTime gameTime)
		{
			_bareHands.Update(gameTime);
			for (int i = 0; i < Inventory.Length; i++)
			{
				if (Inventory[i] != null)
				{
					Inventory[i].Update(gameTime);
				}
			}
			for (int j = 0; j < InventoryTray.Length; j++)
			{
				if (InventoryTray[j] != null)
				{
					InventoryTray[j].Update(gameTime);
				}
			}
		}
	}
}
