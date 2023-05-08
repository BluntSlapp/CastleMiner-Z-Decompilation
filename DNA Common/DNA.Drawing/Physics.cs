using System;

namespace DNA.Drawing
{
	public abstract class Physics
	{
		private Entity _owner;

		public Entity Owner
		{
			get
			{
				return _owner;
			}
		}

		public Physics(Entity owner)
		{
			_owner = owner;
		}

		public abstract void Accelerate(TimeSpan dt);

		public abstract void Move(TimeSpan dt);

		public abstract void Simulate(TimeSpan dt);
	}
}
