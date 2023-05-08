using Microsoft.Xna.Framework.Audio;

namespace DNA.Audio
{
	public class SoundCue3D
	{
		private AudioEmitter _emitter;

		private Cue _cue;

		public AudioEmitter AudioEmitter
		{
			get
			{
				return _emitter;
			}
		}

		public Cue Cue
		{
			get
			{
				return _cue;
			}
		}

		public bool IsCreated
		{
			get
			{
				return Cue.IsCreated;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return Cue.IsDisposed;
			}
		}

		public bool IsPaused
		{
			get
			{
				return Cue.IsPaused;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return Cue.IsPlaying;
			}
		}

		public bool IsPrepared
		{
			get
			{
				return Cue.IsPrepared;
			}
		}

		public bool IsPreparing
		{
			get
			{
				return Cue.IsPreparing;
			}
		}

		public bool IsStopped
		{
			get
			{
				return Cue.IsStopped;
			}
		}

		public bool IsStopping
		{
			get
			{
				return Cue.IsStopping;
			}
		}

		public string Name
		{
			get
			{
				return Cue.Name;
			}
		}

		public void Set(Cue cue, AudioEmitter emitter)
		{
			_cue = cue;
			_emitter = emitter;
		}

		public SoundCue3D(Cue cue, AudioEmitter emitter)
		{
			_cue = cue;
			_emitter = emitter;
		}

		public void Dispose()
		{
			Cue.Dispose();
		}

		public float GetVariable(string name)
		{
			return Cue.GetVariable(name);
		}

		public void Pause()
		{
			Cue.Pause();
		}

		public void Play()
		{
			Cue.Play();
		}

		public void Resume()
		{
			Cue.Resume();
		}

		public void SetVariable(string name, float value)
		{
			Cue.SetVariable(name, value);
		}

		public void Stop(AudioStopOptions options)
		{
			Cue.Stop(options);
		}

		public void Apply3D(AudioListener listener)
		{
			_cue.Apply3D(listener, _emitter);
		}
	}
}
