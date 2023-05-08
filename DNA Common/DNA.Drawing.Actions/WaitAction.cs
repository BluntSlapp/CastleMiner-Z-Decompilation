using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Actions
{
	public class WaitAction : State
	{
		private TimeSpan _endTime;

		private TimeSpan _elapsedTime;

		public override bool Complete
		{
			get
			{
				return _elapsedTime > _endTime;
			}
		}

		public WaitAction(TimeSpan time)
		{
			_endTime = time;
		}

		protected override void OnTick(DNAGame game, Entity actor, GameTime deltaT)
		{
			_elapsedTime += deltaT.get_ElapsedGameTime();
			base.OnTick(game, actor, deltaT);
		}

		protected override void OnStart(Entity actor)
		{
			_elapsedTime = TimeSpan.Zero;
		}
	}
}
