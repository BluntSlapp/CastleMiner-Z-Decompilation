using Microsoft.Xna.Framework;

namespace DNA.Drawing
{
	public class State
	{
		private bool _started;

		public bool Started
		{
			get
			{
				return _started;
			}
		}

		public virtual bool Complete
		{
			get
			{
				return _started;
			}
		}

		public void Start(Entity entity)
		{
			_started = true;
			OnStart(entity);
		}

		public void End(Entity entity)
		{
			OnEnd(entity);
		}

		public void Tick(DNAGame game, Entity entity, GameTime time)
		{
			OnTick(game, entity, time);
		}

		protected virtual void OnStart(Entity entity)
		{
		}

		protected virtual void OnEnd(Entity entity)
		{
		}

		protected virtual void OnTick(DNAGame game, Entity entity, GameTime deltaT)
		{
		}
	}
}
