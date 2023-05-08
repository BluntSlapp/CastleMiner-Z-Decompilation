using System.Threading;

namespace DNA.Audio.SignalProcessing.Processors
{
	public abstract class BlockingSignalProcessor<T> : SignalProcessor<T>
	{
		private AutoResetEvent _processBlockEvent = new AutoResetEvent(false);

		private bool _exiting;

		protected abstract bool IsReady { get; }

		protected void SignalDataReady()
		{
			_processBlockEvent.Set();
		}

		public override bool ProcessBlock(T data)
		{
			while (!IsReady)
			{
				if (!_processBlockEvent.WaitOne())
				{
					return false;
				}
				if (_exiting)
				{
					return false;
				}
			}
			return true;
		}

		public override void OnStart()
		{
			_exiting = false;
			base.OnStart();
		}

		public override void OnStop()
		{
			_exiting = true;
			SignalDataReady();
			base.OnStop();
		}
	}
}
