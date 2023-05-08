using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Actions
{
	public class MoveToState : State
	{
		private Vector3 _startPosition;

		private Vector3 _endPosition;

		private TimeSpan _totalTime;

		private TimeSpan _currentTime;

		public override bool Complete
		{
			get
			{
				return _currentTime >= _totalTime;
			}
		}

		public MoveToState(Vector3 finalLocaiton, TimeSpan time)
		{
			_totalTime = time;
			_endPosition = finalLocaiton;
		}

		protected override void OnStart(Entity entity)
		{
			_currentTime = TimeSpan.Zero;
			_startPosition = entity.LocalPosition;
			base.OnStart(entity);
		}

		protected override void OnTick(DNAGame game, Entity entity, GameTime deltaT)
		{
			_currentTime += deltaT.get_ElapsedGameTime();
			if (_currentTime > _totalTime)
			{
				_currentTime = _totalTime;
			}
			float amount = (float)((_totalTime.TotalSeconds - _currentTime.TotalSeconds) / _totalTime.TotalSeconds);
			entity.LocalPosition = Vector3.Lerp(_endPosition, _startPosition, amount);
			base.OnTick(game, entity, deltaT);
		}
	}
}
