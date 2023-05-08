using System;
using System.Diagnostics;
using System.Threading;

namespace DNA.Audio.SignalProcessing
{
	public class DSP
	{
		public RawProcessorGroup Processors = new RawProcessorGroup();

		private Thread ProcessingThread;

		private RawPCMData _processBuffer;

		private int _windowSize = 1024;

		public float CPULoad;

		private bool _running;

		private Stopwatch stopWatch = new Stopwatch();

		public int SampleRate
		{
			get
			{
				return Processors.SampleRate.Value;
			}
		}

		private int WindowSize
		{
			get
			{
				return _windowSize;
			}
		}

		public bool Running
		{
			get
			{
				return _running;
			}
		}

		public DSP()
		{
			_processBuffer = null;
		}

		public void Start()
		{
			if (_running || ProcessingThread != null)
			{
				throw new Exception("DSP Already Running");
			}
			if (_processBuffer != null)
			{
				int channels = _processBuffer.Channels;
				int? channels2 = Processors.Channels;
				if (channels == channels2.GetValueOrDefault() && channels2.HasValue && _processBuffer.Samples == WindowSize && _processBuffer.SampleRate == SampleRate)
				{
					goto IL_00a6;
				}
			}
			_processBuffer = new RawPCMData(Processors.Channels.Value, WindowSize, SampleRate, 16);
			goto IL_00a6;
			IL_00a6:
			_running = true;
			ProcessingThread = new Thread(ProcessThread);
			Processors.OnStart();
			ProcessingThread.Name = "DSP Processing Thread";
			ProcessingThread.Start();
		}

		public void Stop()
		{
			if (!Running)
			{
				throw new Exception("Dsp not runnning");
			}
			Processors.OnStop();
			lock (this)
			{
				if (ProcessingThread != null)
				{
					if (ProcessingThread.IsAlive)
					{
						ProcessingThread.Join();
					}
					ProcessingThread = null;
				}
			}
		}

		public void ProcessThread()
		{
			try
			{
				ProcessingThread.SetProcessorAffinity(new int[1] { 4 });
				while (true)
				{
					stopWatch.Reset();
					stopWatch.Start();
					if (Processors.ProcessBlock(_processBuffer))
					{
						stopWatch.Stop();
						TimeSpan elapsed = stopWatch.get_Elapsed();
						TimeSpan timeSpan = TimeSpan.FromSeconds((float)WindowSize / (float)SampleRate);
						CPULoad = (float)(elapsed.TotalMilliseconds / timeSpan.TotalMilliseconds);
						continue;
					}
					break;
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				_running = false;
			}
		}
	}
}
