using System;
using System.Collections.Generic;

namespace DNA.Drawing.Animation
{
	public abstract class BaseAnimationPlayer
	{
		private string _name;

		private float _speed = 1f;

		private bool _playing = true;

		public virtual string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public float Speed
		{
			get
			{
				return _speed;
			}
			set
			{
				_speed = value;
			}
		}

		public bool Playing
		{
			get
			{
				return _playing;
			}
		}

		public void Play()
		{
			_playing = true;
		}

		public void Pause()
		{
			_playing = false;
		}

		public void Stop()
		{
			_playing = false;
			Reset();
		}

		protected virtual void Reset()
		{
		}

		public abstract void Update(TimeSpan timeSpan, IList<Bone> boneTransforms);
	}
}
