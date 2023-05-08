using System;
using System.Collections.Generic;
using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Terrain;

namespace DNA.CastleMinerZ
{
	public class CastleMinerZPlayerStats : PlayerStats
	{
		public class ItemStats
		{
			private InventoryItemIDs _itemID;

			public TimeSpan TimeHeld;

			public int Crafted;

			public int Used;

			public int Hits;

			public int KillsZombies;

			public int KillsSkeleton;

			public int KillsHell;

			public InventoryItemIDs ItemID
			{
				get
				{
					return _itemID;
				}
			}

			public ItemStats(InventoryItemIDs itemID)
			{
				_itemID = itemID;
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write(TimeHeld.Ticks);
				writer.Write(Crafted);
				writer.Write(Used);
				writer.Write(Hits);
				writer.Write(KillsZombies);
				writer.Write(KillsSkeleton);
				writer.Write(KillsHell);
			}

			public void Read(BinaryReader reader)
			{
				TimeHeld = TimeSpan.FromTicks(reader.ReadInt64());
				Crafted = reader.ReadInt32();
				Used = reader.ReadInt32();
				Hits = reader.ReadInt32();
				KillsZombies = reader.ReadInt32();
				KillsSkeleton = reader.ReadInt32();
				KillsHell = reader.ReadInt32();
			}
		}

		public bool InvertYAxis;

		public DateTime TimeOfPurchase;

		public DateTime FirstPlayTime = DateTime.UtcNow;

		public TimeSpan TimeInTrial;

		public TimeSpan TimeInFull;

		public TimeSpan TimeInMenu;

		public TimeSpan TimeOnline;

		public int GamesPlayed;

		public int MaxDaysSurvived;

		public float MaxDistanceTraveled;

		public float MaxDepth;

		public int TotalItemsCrafted;

		public int TotalKills;

		public int UndeadDragonKills;

		public int ForestDragonKills;

		public int IceDragonKills;

		public int FireDragonKills;

		public int SandDragonKills;

		public bool v1Player;

		public float brightness;

		public float musicVolume = 1f;

		public float controllerSensitivity = 1f;

		private Dictionary<BlockTypeEnum, int> BlocksDug = new Dictionary<BlockTypeEnum, int>();

		private Dictionary<InventoryItemIDs, ItemStats> AllItemStats = new Dictionary<InventoryItemIDs, ItemStats>();

		public Dictionary<string, DateTime> BanList = new Dictionary<string, DateTime>();

		public override int Version
		{
			get
			{
				return 3;
			}
		}

		public int BlocksDugCount(BlockTypeEnum type)
		{
			int value;
			if (!BlocksDug.TryGetValue(type, out value))
			{
				return 0;
			}
			return value;
		}

		public void DugBlock(BlockTypeEnum type)
		{
			int value = 0;
			BlocksDug.TryGetValue(type, out value);
			value++;
			BlocksDug[type] = value;
		}

		public ItemStats GetItemStats(InventoryItemIDs ItemID)
		{
			ItemStats value;
			if (!AllItemStats.TryGetValue(ItemID, out value))
			{
				value = new ItemStats(ItemID);
				AllItemStats[ItemID] = value;
			}
			return value;
		}

		protected override void SaveData(BinaryWriter writer)
		{
			writer.Write(InvertYAxis);
			writer.Write(TimeOfPurchase.Ticks);
			writer.Write(FirstPlayTime.Ticks);
			writer.Write((float)TimeInTrial.TotalMinutes);
			writer.Write((float)TimeInFull.TotalMinutes);
			writer.Write((float)TimeInMenu.TotalMinutes);
			writer.Write((float)TimeOnline.TotalMinutes);
			writer.Write(GamesPlayed);
			writer.Write(MaxDaysSurvived);
			writer.Write(MaxDistanceTraveled);
			writer.Write(MaxDepth);
			writer.Write(TotalItemsCrafted);
			writer.Write(TotalKills);
			writer.Write(UndeadDragonKills);
			writer.Write(FireDragonKills);
			writer.Write(ForestDragonKills);
			writer.Write(IceDragonKills);
			writer.Write(SandDragonKills);
			writer.Write(BlocksDug.Count);
			foreach (KeyValuePair<BlockTypeEnum, int> item in BlocksDug)
			{
				writer.Write((int)item.Key);
				writer.Write(item.Value);
			}
			writer.Write(BanList.Count);
			foreach (KeyValuePair<string, DateTime> ban in BanList)
			{
				writer.Write(ban.Key);
				writer.Write(ban.Value.Ticks);
			}
			writer.Write(AllItemStats.Count);
			foreach (KeyValuePair<InventoryItemIDs, ItemStats> allItemStat in AllItemStats)
			{
				writer.Write((int)allItemStat.Key);
				allItemStat.Value.Write(writer);
			}
			writer.Write(v1Player);
			writer.Write(brightness);
			writer.Write(musicVolume);
			writer.Write(controllerSensitivity);
		}

		protected override void LoadData(BinaryReader reader, int version)
		{
			InvertYAxis = reader.ReadBoolean();
			TimeOfPurchase = new DateTime(reader.ReadInt64());
			FirstPlayTime = new DateTime(reader.ReadInt64());
			TimeInTrial = TimeSpan.FromMinutes(reader.ReadSingle());
			TimeInFull = TimeSpan.FromMinutes(reader.ReadSingle());
			TimeInMenu = TimeSpan.FromMinutes(reader.ReadSingle());
			TimeOnline = TimeSpan.FromMinutes(reader.ReadSingle());
			GamesPlayed = reader.ReadInt32();
			MaxDaysSurvived = reader.ReadInt32();
			MaxDistanceTraveled = reader.ReadSingle();
			MaxDepth = reader.ReadSingle();
			TotalItemsCrafted = reader.ReadInt32();
			TotalKills = reader.ReadInt32();
			if (version == 1)
			{
				v1Player = true;
				return;
			}
			UndeadDragonKills = reader.ReadInt32();
			FireDragonKills = reader.ReadInt32();
			ForestDragonKills = reader.ReadInt32();
			IceDragonKills = reader.ReadInt32();
			SandDragonKills = reader.ReadInt32();
			int num = reader.ReadInt32();
			BlocksDug.Clear();
			for (int i = 0; i < num; i++)
			{
				BlocksDug[(BlockTypeEnum)reader.ReadInt32()] = reader.ReadInt32();
			}
			num = reader.ReadInt32();
			BanList.Clear();
			for (int j = 0; j < num; j++)
			{
				BanList[reader.ReadString()] = new DateTime(reader.ReadInt64());
			}
			num = reader.ReadInt32();
			AllItemStats.Clear();
			for (int k = 0; k < num; k++)
			{
				InventoryItemIDs inventoryItemIDs = (InventoryItemIDs)reader.ReadInt32();
				ItemStats itemStats = new ItemStats(inventoryItemIDs);
				itemStats.Read(reader);
				AllItemStats[inventoryItemIDs] = itemStats;
			}
			v1Player = reader.ReadBoolean();
			if (version >= 3)
			{
				brightness = reader.ReadSingle();
				musicVolume = reader.ReadSingle();
				controllerSensitivity = reader.ReadSingle();
			}
		}
	}
}
