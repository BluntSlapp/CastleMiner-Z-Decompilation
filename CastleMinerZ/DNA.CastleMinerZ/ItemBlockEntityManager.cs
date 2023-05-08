using System;
using System.Collections.Generic;
using System.Diagnostics;
using DNA.CastleMinerZ.Inventory;
using DNA.CastleMinerZ.Terrain;
using DNA.Drawing;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ
{
	public class ItemBlockEntityManager : Entity
	{
		private enum ListWalkState
		{
			PROCESS_QUEUES,
			SORT,
			DISTANT,
			PRUNEANDSPAWN
		}

		private const int MAX_TORCHES = 500;

		private const int MAX_FLAMES = 100;

		private const int MAX_LANTERNS = 500;

		public static ItemBlockEntityManager Instance;

		public Random Rnd = new Random();

		public List<ItemBlockEntityHolder> BlockEntities = new List<ItemBlockEntityHolder>();

		public List<ItemBlockEntityHolder> Accumulator = new List<ItemBlockEntityHolder>();

		private int _currentWalkLocation;

		private ListWalkState _listWalkState;

		private Stopwatch _queueTimer = Stopwatch.StartNew();

		public List<Vector3> LanternLocations = new List<Vector3>();

		private long _maxMillis = 10L;

		public ItemBlockEntityManager()
		{
			Visible = false;
			Collidee = false;
			Collider = false;
			Instance = this;
		}

		private int SearchListForItem(Vector3 pos, List<ItemBlockEntityHolder> list)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				float num = Vector3.DistanceSquared(list[i].Position, pos);
				if (num < 0.01f)
				{
					return i;
				}
			}
			return -1;
		}

		protected void HandleTorchCommand(BlockTerrain.ItemBlockCommand ibc)
		{
			Vector3 vector = IntVector3.ToVector3(ibc.WorldPosition);
			vector += new Vector3(0.5f, 0.5f, 0.5f);
			if (ibc.AddItem)
			{
				if (BlockTerrain.Instance.IsInsideWorld(vector) && SearchListForItem(vector, BlockEntities) == -1)
				{
					ItemBlockEntityHolder itemBlockEntityHolder = ItemBlockEntityHolder.Alloc();
					itemBlockEntityHolder.Position = vector;
					itemBlockEntityHolder.BlockType = ibc.BlockType;
					BlockEntities.Add(itemBlockEntityHolder);
				}
				return;
			}
			int num = SearchListForItem(vector, BlockEntities);
			if (num != -1)
			{
				ItemBlockEntityHolder itemBlockEntityHolder2 = BlockEntities[num];
				if (itemBlockEntityHolder2.InWorldEntity != null)
				{
					itemBlockEntityHolder2.InWorldEntity.RemoveFromParent();
				}
				itemBlockEntityHolder2.Release();
				int num2 = BlockEntities.Count - 1;
				if (num < num2)
				{
					BlockEntities[num] = BlockEntities[num2];
				}
				BlockEntities.RemoveAt(num2);
			}
		}

		protected void HandleLanternCommand(BlockTerrain.ItemBlockCommand ibc)
		{
			Vector3 vector = IntVector3.ToVector3(ibc.WorldPosition);
			vector += new Vector3(0.5f, 0.5f, 0.5f);
			int count = LanternLocations.Count;
			bool flag = false;
			float num = -1f;
			int num2 = -1;
			if (count >= 500 && ibc.AddItem)
			{
				for (int i = 0; i < count; i++)
				{
					float num3 = Vector3.DistanceSquared(LanternLocations[i], vector);
					if (num3 < 0.01f)
					{
						flag = true;
						break;
					}
					if (num3 > num)
					{
						num = num3;
						num2 = i;
					}
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					if (!(Vector3.DistanceSquared(LanternLocations[j], vector) < 0.01f))
					{
						continue;
					}
					flag = true;
					if (!ibc.AddItem)
					{
						count--;
						if (j < count)
						{
							LanternLocations[j] = LanternLocations[count];
						}
						LanternLocations.RemoveAt(count);
					}
					break;
				}
			}
			if (ibc.AddItem && !flag && BlockTerrain.Instance.IsInsideWorld(vector))
			{
				if (num2 >= 0)
				{
					LanternLocations[num2] = vector;
				}
				else
				{
					LanternLocations.Add(vector);
				}
			}
		}

		private void RemoveOutOfBounders(List<ItemBlockEntityHolder> list)
		{
			int num = list.Count;
			int num2 = 0;
			while (num2 < num)
			{
				if (!BlockTerrain.Instance.IsInsideWorld(list[num2].Position))
				{
					if (list[num2].InWorldEntity != null)
					{
						list[num2].InWorldEntity.RemoveFromParent();
					}
					list[num2].Release();
					num--;
					if (num2 < num)
					{
						list[num2] = list[num];
					}
					list.RemoveAt(num);
				}
				else
				{
					num2++;
				}
			}
		}

		private void CreateEntity(ItemBlockEntityHolder h)
		{
			ItemBlockInventoryItemClass itemBlockInventoryItemClass = (ItemBlockInventoryItemClass)BlockInventoryItemClass.BlockClasses[BlockType.GetType(h.BlockType).ParentBlockType];
			Entity entity = itemBlockInventoryItemClass.CreateWorldEntity(false, h.BlockType);
			entity.LocalPosition = h.Position;
			entity.DrawPriority = 100;
			base.Scene.Children.Add(entity);
			h.InWorldEntity = entity;
		}

		private void UpdateNearby(float dt)
		{
			Vector3 worldPosition = CastleMinerZGame.Instance.LocalPlayer.WorldPosition;
			_queueTimer.Reset();
			_queueTimer.Start();
			if (_listWalkState == ListWalkState.PROCESS_QUEUES)
			{
				while (!BlockTerrain.Instance.ItemBlockCommandQueue.Empty)
				{
					BlockTerrain.ItemBlockCommand itemBlockCommand = BlockTerrain.Instance.ItemBlockCommandQueue.Dequeue();
					BlockTypeEnum parentBlockType = BlockType.GetType(itemBlockCommand.BlockType).ParentBlockType;
					if (parentBlockType == BlockTypeEnum.Lantern)
					{
						HandleLanternCommand(itemBlockCommand);
					}
					else
					{
						HandleTorchCommand(itemBlockCommand);
					}
					if (_queueTimer.ElapsedMilliseconds > _maxMillis)
					{
						return;
					}
				}
				_currentWalkLocation = 0;
				_listWalkState = ListWalkState.SORT;
				RemoveOutOfBounders(BlockEntities);
				if (_queueTimer.ElapsedMilliseconds > _maxMillis)
				{
					return;
				}
			}
			if (_listWalkState == ListWalkState.SORT)
			{
				int count = BlockEntities.Count;
				while (_currentWalkLocation < count)
				{
					ItemBlockEntityHolder itemBlockEntityHolder = BlockEntities[_currentWalkLocation++];
					itemBlockEntityHolder.Distance = Vector3.DistanceSquared(itemBlockEntityHolder.Position, worldPosition);
					int count2 = Accumulator.Count;
					if (count2 == 0)
					{
						Accumulator.Add(itemBlockEntityHolder);
					}
					else
					{
						count2--;
						while (count2 >= 0 && itemBlockEntityHolder.Distance < Accumulator[count2].Distance)
						{
							count2--;
						}
						Accumulator.Insert(count2 + 1, itemBlockEntityHolder);
					}
					if (_queueTimer.ElapsedMilliseconds > _maxMillis)
					{
						return;
					}
				}
				BlockEntities.Clear();
				_currentWalkLocation = 0;
				_listWalkState = ListWalkState.PRUNEANDSPAWN;
			}
			if (_listWalkState != ListWalkState.PRUNEANDSPAWN)
			{
				return;
			}
			int count3 = Accumulator.Count;
			int num = Math.Min(count3, 100);
			while (_currentWalkLocation < num)
			{
				ItemBlockEntityHolder itemBlockEntityHolder2 = Accumulator[_currentWalkLocation++];
				if (itemBlockEntityHolder2.InWorldEntity == null)
				{
					CreateEntity(itemBlockEntityHolder2);
				}
				itemBlockEntityHolder2.TorchFlame = true;
				itemBlockEntityHolder2.TimeUntilFlameRemovalAllowed = 1f;
				itemBlockEntityHolder2.TimeUntilTorchRemovalAllowed = 1f;
				if (_queueTimer.ElapsedMilliseconds > _maxMillis)
				{
					return;
				}
			}
			if (count3 > 100)
			{
				num = Math.Min(count3, 500);
				while (_currentWalkLocation < num)
				{
					ItemBlockEntityHolder itemBlockEntityHolder3 = Accumulator[_currentWalkLocation++];
					if (itemBlockEntityHolder3.InWorldEntity == null)
					{
						CreateEntity(itemBlockEntityHolder3);
					}
					itemBlockEntityHolder3.TimeUntilTorchRemovalAllowed = 1f;
					if (itemBlockEntityHolder3.TorchFlame)
					{
						itemBlockEntityHolder3.TimeUntilFlameRemovalAllowed -= dt;
						if (itemBlockEntityHolder3.TimeUntilFlameRemovalAllowed <= 0f)
						{
							itemBlockEntityHolder3.TorchFlame = false;
						}
					}
					if (_queueTimer.ElapsedMilliseconds > _maxMillis)
					{
						return;
					}
				}
				if (count3 > 500)
				{
					while (_currentWalkLocation < count3)
					{
						ItemBlockEntityHolder itemBlockEntityHolder4 = Accumulator[_currentWalkLocation++];
						if (itemBlockEntityHolder4.InWorldEntity != null)
						{
							itemBlockEntityHolder4.TimeUntilTorchRemovalAllowed -= dt;
							if (itemBlockEntityHolder4.TimeUntilTorchRemovalAllowed <= 0f)
							{
								itemBlockEntityHolder4.InWorldEntity = null;
							}
						}
						if (_queueTimer.ElapsedMilliseconds > _maxMillis)
						{
							return;
						}
					}
				}
			}
			List<ItemBlockEntityHolder> accumulator = Accumulator;
			Accumulator = BlockEntities;
			BlockEntities = accumulator;
			_currentWalkLocation = 0;
			_listWalkState = ListWalkState.PROCESS_QUEUES;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			UpdateNearby((float)gameTime.get_ElapsedGameTime().TotalSeconds);
			int num = LanternLocations.Count;
			int num2 = 0;
			while (num2 < num)
			{
				if (!BlockTerrain.Instance.IsInsideWorld(LanternLocations[num2]))
				{
					num--;
					if (num2 < num)
					{
						LanternLocations[num2] = LanternLocations[num];
					}
					LanternLocations.RemoveAt(num);
				}
				else
				{
					num2++;
				}
			}
			base.OnUpdate(gameTime);
		}

		public bool NearLantern(Vector3 pos, float minDist)
		{
			int count = LanternLocations.Count;
			float num = minDist * minDist;
			for (int i = 0; i < count; i++)
			{
				if (Vector3.DistanceSquared(pos, LanternLocations[i]) <= num)
				{
					return true;
				}
			}
			return false;
		}
	}
}
