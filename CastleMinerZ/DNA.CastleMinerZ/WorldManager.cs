using System;
using System.Collections.Generic;
using DNA.IO.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA.CastleMinerZ
{
	public class WorldManager
	{
		private Dictionary<Guid, WorldInfo> SavedWorlds = new Dictionary<Guid, WorldInfo>(0);

		private SignedInGamer _gamer;

		private SaveDevice _device;

		public WorldInfo[] GetWorlds()
		{
			WorldInfo[] array = new WorldInfo[SavedWorlds.Count];
			SavedWorlds.Values.CopyTo(array, 0);
			return array;
		}

		public WorldManager(SignedInGamer gamer, SaveDevice device)
		{
			_device = device;
			_gamer = gamer;
			WorldInfo[] array = WorldInfo.LoadWorldInfo(_device);
			WorldInfo[] array2 = array;
			foreach (WorldInfo worldInfo in array2)
			{
				SavedWorlds[worldInfo.WorldID] = worldInfo;
			}
		}

		public void Delete(WorldInfo info)
		{
			SavedWorlds.Remove(info.WorldID);
			try
			{
				_device.DeleteDirectory(info.SavePath);
			}
			catch
			{
			}
		}

		public void TakeOwnership(WorldInfo info)
		{
			if (SavedWorlds.ContainsKey(info.WorldID))
			{
				SavedWorlds.Remove(info.WorldID);
			}
			info.TakeOwnership(_gamer, _device);
			SavedWorlds[info.WorldID] = info;
		}

		public void RegisterNetworkWorld(WorldInfo newWorld)
		{
			WorldInfo value;
			if (SavedWorlds.TryGetValue(newWorld.WorldID, out value))
			{
				newWorld.LastPosition = value.LastPosition;
				newWorld.SavePath = value.SavePath;
			}
			else
			{
				newWorld.LastPosition = Vector3.Zero;
			}
			SavedWorlds[newWorld.WorldID] = newWorld;
			newWorld.SaveToStorage(_gamer, CastleMinerZGame.Instance.SaveDevice);
		}
	}
}
