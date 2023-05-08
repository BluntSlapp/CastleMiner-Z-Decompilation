namespace DNA.Audio.SignalProcessing
{
	public abstract class SignalProcessor<T>
	{
		public bool Active = true;

		public virtual int? SampleRate
		{
			get
			{
				return null;
			}
		}

		public virtual int? Channels
		{
			get
			{
				return null;
			}
		}

		public abstract bool ProcessBlock(T data);

		public virtual void OnStart()
		{
		}

		public virtual void OnStop()
		{
		}
	}
}
