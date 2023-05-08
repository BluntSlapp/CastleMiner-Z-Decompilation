using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Actions
{
	public class ScaleToState : State
	{
		private Vector3 _scaleFrom;

		private Vector3 _scaleTo;

		private TimeSpan _time;

		private TimeSpan _elasped;

		public override bool Complete
		{
			get
			{
				return _elasped >= _time;
			}
		}

		public ScaleToState(Vector3 from, Vector3 to, TimeSpan time)
		{
			_scaleFrom = from;
			_scaleTo = to;
			_time = time;
		}

		protected override void OnStart(Entity entity)
		{
			_elasped = TimeSpan.Zero;
			entity.LocalScale = _scaleFrom;
			base.OnStart(entity);
		}

		protected override void OnTick(DNAGame game, Entity entity, GameTime deltaT)
		{
			_elasped += deltaT.get_ElapsedGameTime();
			if (_elasped > _time)
			{
				_elasped = _time;
			}
			float amount = (float)(_elasped.TotalSeconds / _time.TotalSeconds);
			entity.LocalScale = Vector3.Lerp(_scaleFrom, _scaleTo, amount);
			base.OnTick(game, entity, deltaT);
		}

		protected override void OnEnd(Entity entity)
		{
			entity.LocalScale = _scaleTo;
			base.OnEnd(entity);
		}
	}
}
