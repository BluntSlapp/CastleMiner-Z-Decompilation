using System;
using System.Collections.Generic;
using DNA.Profiling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing
{
	public class Scene : Entity
	{
		private class DistanceComparer : IComparer<Entity>
		{
			private Scene _owner;

			public DistanceComparer(Scene owner)
			{
				_owner = owner;
			}

			public int Compare(Entity e1, Entity e2)
			{
				Matrix matrix = e1.LocalToWorld * _owner._view;
				Matrix matrix2 = e2.LocalToWorld * _owner._view;
				Vector3 vector = Vector3.Transform(Vector3.Zero, matrix);
				Vector3 vector2 = Vector3.Transform(Vector3.Zero, matrix);
				if (vector2.Z > vector.Z)
				{
					return 1;
				}
				if (vector2.Z < vector.Z)
				{
					return -1;
				}
				return 0;
			}
		}

		private List<Entity> Colliders = new List<Entity>();

		private List<Entity> Collidees = new List<Entity>();

		private Matrix _view;

		private List<Entity> toDraw = new List<Entity>();

		private List<Entity> toSort = new List<Entity>();

		private IComparer<Entity> comparer;

		protected override void OnUpdate(GameTime gameTime)
		{
			double totalSecond = gameTime.get_ElapsedGameTime().TotalSeconds;
			using (Profiler.TimeSection("Physics", ProfilerThreadEnum.MAIN))
			{
				for (int i = 0; i < base.Children.Count; i++)
				{
					Entity entity = base.Children[i];
					if (entity.Physics != null)
					{
						entity.Physics.Accelerate(gameTime.get_ElapsedGameTime());
					}
				}
				Colliders.Clear();
				Collidees.Clear();
				for (int j = 0; j < base.Children.Count; j++)
				{
					Entity entity2 = base.Children[j];
					if (entity2.Collider)
					{
						Colliders.Add(entity2);
					}
					if (entity2.Collidee)
					{
						Collidees.Add(entity2);
					}
				}
				using (Profiler.TimeSection("Collision", ProfilerThreadEnum.MAIN))
				{
					for (int k = 0; k < Colliders.Count; k++)
					{
						Entity entity3 = Colliders[k];
						entity3.ResolveCollsions(Collidees, gameTime);
					}
				}
				for (int l = 0; l < base.Children.Count; l++)
				{
					Entity entity4 = base.Children[l];
					if (entity4.Physics != null)
					{
						entity4.Physics.Move(gameTime.get_ElapsedGameTime());
					}
				}
			}
			base.OnUpdate(gameTime);
		}

		private void DrawList(GraphicsDevice device, List<Entity> drawList, GameTime gameTime, Matrix projection)
		{
			int num = int.MaxValue;
			for (int i = 0; i < drawList.Count; i++)
			{
				Entity entity = drawList[i];
				num = Math.Min(num, entity.DrawPriority);
			}
			bool flag = false;
			while (!flag)
			{
				flag = true;
				int num2 = num;
				num = int.MaxValue;
				for (int j = 0; j < drawList.Count; j++)
				{
					Entity entity2 = drawList[j];
					if (num2 == entity2.DrawPriority)
					{
						if (device.SamplerStates[0] != entity2.SamplerState)
						{
							device.SamplerStates[0] = entity2.SamplerState;
						}
						if (device.BlendState != entity2.BlendState)
						{
							device.BlendState = entity2.BlendState;
						}
						if (device.RasterizerState != entity2.RasterizerState)
						{
							device.RasterizerState = entity2.RasterizerState;
						}
						if (device.DepthStencilState != entity2.DepthStencilState)
						{
							device.DepthStencilState = entity2.DepthStencilState;
						}
						entity2.Draw(device, gameTime, _view, projection);
					}
					else if (entity2.DrawPriority > num2 && entity2.DrawPriority < num)
					{
						num = entity2.DrawPriority;
						flag = false;
					}
				}
			}
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			if (comparer == null)
			{
				comparer = new DistanceComparer(this);
			}
			_view = view;
			toSort.Clear();
			toDraw.Clear();
			GetDrawList(toSort, toDraw);
			DrawList(device, toDraw, gameTime, projection);
			toSort.Sort(comparer);
			DrawList(device, toSort, gameTime, projection);
			base.Draw(device, gameTime, view, projection);
		}
	}
}
