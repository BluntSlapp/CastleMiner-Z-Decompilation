using System;

namespace DNA.Timers
{
	public class Timer
	{
		private TimeSpan _elapsedTime;

		public TimeSpan ElaspedTime
		{
			get
			{
				return _elapsedTime;
			}
		}

		protected virtual void OnUpdate(TimeSpan time)
		{
		}

		public void Update(TimeSpan time)
		{
			_elapsedTime += time;
			OnUpdate(time);
		}

		public void Reset()
		{
			_elapsedTime = TimeSpan.Zero;
		}
	}
}
