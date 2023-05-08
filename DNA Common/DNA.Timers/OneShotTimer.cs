using System;

namespace DNA.Timers
{
	public class OneShotTimer : Timer
	{
		public TimeSpan MaxTime;

		public bool AutoReset;

		public float PercentComplete
		{
			get
			{
				return Math.Min(1f, (float)(base.ElaspedTime.TotalSeconds / MaxTime.TotalSeconds));
			}
		}

		public bool Expired
		{
			get
			{
				return base.ElaspedTime >= MaxTime;
			}
		}

		public OneShotTimer(TimeSpan time)
		{
			MaxTime = time;
		}

		protected override void OnUpdate(TimeSpan time)
		{
			base.OnUpdate(time);
			if (AutoReset && Expired)
			{
				Reset();
			}
		}

		public OneShotTimer(TimeSpan time, bool autoReset)
		{
			AutoReset = autoReset;
			MaxTime = time;
		}
	}
}
