using System;
using System.Collections.Generic;
using System.IO;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Terrain.WorldBuilders;
using DNA.IO;
using DNA.IO.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA.CastleMinerZ
{
	public class WorldInfo
	{
		private const int Version = 3;

		public static Vector3 DefaultStartLocation = new Vector3(8f, 128f, -8f);

		public Dictionary<IntVector3, Crate> Crates = new Dictionary<IntVector3, Crate>();

		private static readonly string BasePath = "Worlds";

		private static readonly string FileName = "world.info";

		private string _savePath;

		public WorldTypeIDs _terrainVersion = WorldTypeIDs.CastleMinerZ;

		private string _name = "World";

		private string _ownerGamerTag;

		private string _creatorGamerTag;

		private DateTime _createdDate;

		private DateTime _lastPlayedDate;

		private int _seed;

		private Guid _worldID;

		private Vector3 _lastPosition = DefaultStartLocation;

		public bool InfiniteResourceMode;

		public string SavePath
		{
			get
			{
				if (OwnerGamerTag == null)
				{
					return null;
				}
				return _savePath;
			}
			set
			{
				_savePath = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public string OwnerGamerTag
		{
			get
			{
				return _ownerGamerTag;
			}
		}

		public string CreatorGamerTag
		{
			get
			{
				return _creatorGamerTag;
			}
		}

		public DateTime CreatedDate
		{
			get
			{
				return _createdDate;
			}
		}

		public DateTime LastPlayedDate
		{
			get
			{
				return _lastPlayedDate;
			}
			set
			{
				_lastPlayedDate = value;
			}
		}

		public int Seed
		{
			get
			{
				return _seed;
			}
		}

		public Guid WorldID
		{
			get
			{
				return _worldID;
			}
		}

		public Vector3 LastPosition
		{
			get
			{
				return _lastPosition;
			}
			set
			{
				_lastPosition = value;
			}
		}

		private WorldInfo()
		{
			CreateSavePath();
		}

		public WorldInfo(BinaryReader reader)
		{
			CreateSavePath();
			Load(reader);
		}

		public WorldInfo(WorldInfo info)
		{
			_savePath = info._savePath;
			_terrainVersion = info._terrainVersion;
			_name = info._name;
			_ownerGamerTag = info._ownerGamerTag;
			_creatorGamerTag = info._creatorGamerTag;
			_createdDate = info._createdDate;
			_lastPlayedDate = info._lastPlayedDate;
			_seed = info._seed;
			_worldID = info._worldID;
			_lastPosition = info._lastPosition;
			InfiniteResourceMode = info.InfiniteResourceMode;
		}

		public Crate GetCreate(IntVector3 crateLocation, bool createIfMissing)
		{
			Crate value;
			if (!Crates.TryGetValue(crateLocation, out value))
			{
				if (!createIfMissing)
				{
					return null;
				}
				value = new Crate(crateLocation);
				Crates[crateLocation] = value;
			}
			return value;
		}

		public static WorldInfo CreateNewWorld(SignedInGamer gamer)
		{
			Random random = new Random();
			WorldInfo worldInfo = new WorldInfo();
			int seed = 839880689;
			if (!Guide.IsTrialMode)
			{
				seed = random.Next();
			}
			worldInfo.MakeNew(gamer, seed);
			return worldInfo;
		}

		public static WorldInfo CreateNewWorld(SignedInGamer gamer, int seed)
		{
			WorldInfo worldInfo = new WorldInfo();
			worldInfo.MakeNew(gamer, seed);
			return worldInfo;
		}

		public static WorldInfo CreateNewWorld(int seed)
		{
			return CreateNewWorld(null, seed);
		}

		public static WorldInfo[] LoadWorldInfo(SaveDevice device)
		{
			try
			{
				if (!device.DirectoryExists(BasePath))
				{
					return new WorldInfo[0];
				}
				List<WorldInfo> list = new List<WorldInfo>();
				string[] directories = device.GetDirectories(BasePath);
				string[] array = directories;
				foreach (string text in array)
				{
					WorldInfo worldInfo = null;
					try
					{
						worldInfo = LoadFromStroage(text, device);
					}
					catch
					{
						worldInfo = null;
					}
					if (worldInfo != null)
					{
						list.Add(worldInfo);
						continue;
					}
					try
					{
						device.DeleteDirectoryAsync(text);
					}
					catch
					{
					}
				}
				return list.ToArray();
			}
			catch
			{
				return new WorldInfo[0];
			}
		}

		public void CreateSavePath()
		{
			_savePath = Path.Combine(path2: Guid.NewGuid().ToString(), path1: BasePath);
		}

		private void MakeNew(SignedInGamer creator, int seed)
		{
			if (creator == null)
			{
				_name = "New World " + DateTime.Now.ToString("g");
			}
			else
			{
				_name = string.Concat(creator, "'s World ", DateTime.Now.ToString("g"));
			}
			CreateSavePath();
			if (creator == null)
			{
				_ownerGamerTag = (_creatorGamerTag = null);
			}
			else
			{
				_ownerGamerTag = (_creatorGamerTag = creator.Gamertag);
			}
			_createdDate = (_lastPlayedDate = DateTime.Now);
			_worldID = Guid.NewGuid();
			_seed = seed;
		}

		public void TakeOwnership(SignedInGamer gamer, SaveDevice device)
		{
			if (_creatorGamerTag == null)
			{
				_creatorGamerTag = gamer.Gamertag;
			}
			_ownerGamerTag = gamer.Gamertag;
			_worldID = Guid.NewGuid();
			SaveToStorage(gamer, device);
		}

		public void SaveToStorage(SignedInGamer gamer, SaveDevice saveDevice)
		{
			try
			{
				if (!saveDevice.DirectoryExists(SavePath))
				{
					saveDevice.CreateDirectory(SavePath);
				}
				string fileName = Path.Combine(SavePath, FileName);
				saveDevice.Save(fileName, true, true, delegate(Stream stream)
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

		private static WorldInfo LoadFromStroage(string folder, SaveDevice saveDevice)
		{
			WorldInfo info = new WorldInfo();
			saveDevice.Load(Path.Combine(folder, FileName), delegate(Stream stream)
			{
				BinaryReader reader = new BinaryReader(stream);
				info.Load(reader);
				info._savePath = folder;
			});
			return info;
		}

		public void Save(BinaryWriter writer)
		{
			writer.Write(3);
			writer.Write((int)_terrainVersion);
			writer.Write(_name);
			writer.Write(_ownerGamerTag);
			writer.Write(_creatorGamerTag);
			writer.Write(_createdDate.Ticks);
			writer.Write(_lastPlayedDate.Ticks);
			writer.Write(_seed);
			writer.Write(_worldID.ToByteArray());
			writer.Write(_lastPosition);
			writer.Write(Crates.Count);
			foreach (KeyValuePair<IntVector3, Crate> crate in Crates)
			{
				crate.Value.Write(writer);
			}
			writer.Write(InfiniteResourceMode);
		}

		private void Load(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 1 || num > 3)
			{
				throw new Exception("Bad Info Version");
			}
			_terrainVersion = (WorldTypeIDs)reader.ReadInt32();
			_name = reader.ReadString();
			_ownerGamerTag = reader.ReadString();
			_creatorGamerTag = reader.ReadString();
			_createdDate = new DateTime(reader.ReadInt64());
			_lastPlayedDate = new DateTime(reader.ReadInt64());
			_seed = reader.ReadInt32();
			_worldID = new Guid(reader.ReadBytes(16));
			_lastPosition = reader.ReadVector3();
			if (num > 1)
			{
				int num2 = reader.ReadInt32();
				Crates.Clear();
				for (int i = 0; i < num2; i++)
				{
					Crate crate = new Crate(reader);
					Crates[crate.Location] = crate;
				}
			}
			if (num > 2)
			{
				InfiniteResourceMode = reader.ReadBoolean();
			}
		}

		public WorldBuilder GetBuilder()
		{
			return new CastleMinerZBuilder(this);
		}
	}
}
