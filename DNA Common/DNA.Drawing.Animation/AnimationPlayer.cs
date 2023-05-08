using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Animation
{
	public class AnimationPlayer : BaseAnimationPlayer
	{
		private AnimationClip _currentAnimationClip;

		private bool[] _influencedBones;

		private bool _onPong;

		private bool _reversed;

		private bool _looping = true;

		private bool _pingPong;

		private TimeSpan _currentTime = TimeSpan.Zero;

		private Vector3[] _translations;

		private Quaternion[] _rotations;

		private Vector3[] _scales;

		public override string Name
		{
			get
			{
				if (base.Name == null)
				{
					return _currentAnimationClip.Name;
				}
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		public bool Finished
		{
			get
			{
				if (Looping)
				{
					return false;
				}
				if (PingPong && _onPong)
				{
					return Progress <= 0f;
				}
				return Progress >= 1f;
			}
		}

		public bool Reversed
		{
			get
			{
				return _reversed;
			}
			set
			{
				_reversed = value;
			}
		}

		public bool Looping
		{
			get
			{
				return _looping;
			}
			set
			{
				_looping = value;
			}
		}

		public bool PingPong
		{
			get
			{
				return _pingPong;
			}
			set
			{
				_pingPong = value;
			}
		}

		public TimeSpan CurrentTime
		{
			get
			{
				return _currentTime;
			}
			set
			{
				_currentTime = value;
			}
		}

		public float Progress
		{
			get
			{
				return (float)(_currentTime.TotalSeconds / Duration.TotalSeconds);
			}
			set
			{
				_onPong = false;
				_currentTime = TimeSpan.FromSeconds((double)value * Duration.TotalSeconds);
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return _currentAnimationClip.Duration;
			}
		}

		public Vector3[] Translations
		{
			get
			{
				return _translations;
			}
		}

		public Quaternion[] Rotations
		{
			get
			{
				return _rotations;
			}
		}

		public Vector3[] Scales
		{
			get
			{
				return _scales;
			}
		}

		public bool GetBoneInfluence(int boneNumber)
		{
			if (_influencedBones == null)
			{
				return true;
			}
			return _influencedBones[boneNumber];
		}

		protected override void Reset()
		{
			_onPong = false;
			_currentTime = TimeSpan.Zero;
		}

		public void SetInfluncedBones(bool[] boneArray)
		{
			if (boneArray != null && boneArray.Length != _currentAnimationClip.BoneCount)
			{
				throw new Exception("bone array size mismatch");
			}
			_influencedBones = boneArray;
		}

		public void ResetInfluncedBones()
		{
			_influencedBones = null;
		}

		public void SetClip(AnimationClip clip)
		{
			_onPong = false;
			_reversed = false;
			_looping = true;
			_pingPong = false;
			_currentTime = TimeSpan.Zero;
			_currentAnimationClip = clip;
			if (_influencedBones == null || _influencedBones.Length != _currentAnimationClip.BoneCount)
			{
				_influencedBones = new bool[_currentAnimationClip.BoneCount];
			}
			for (int i = 0; i < _influencedBones.Length; i++)
			{
				_influencedBones[i] = true;
			}
			if (_translations.Length != _currentAnimationClip.BoneCount)
			{
				_translations = new Vector3[_currentAnimationClip.BoneCount];
			}
			if (_rotations.Length != _currentAnimationClip.BoneCount)
			{
				_rotations = new Quaternion[_currentAnimationClip.BoneCount];
			}
			if (_scales.Length != _currentAnimationClip.BoneCount)
			{
				_scales = new Vector3[_currentAnimationClip.BoneCount];
			}
		}

		public void SetClip(AnimationClip clip, IList<Bone> influenceBones)
		{
			_onPong = false;
			_reversed = false;
			_looping = true;
			_pingPong = false;
			_currentTime = TimeSpan.Zero;
			_currentAnimationClip = clip;
			if (_influencedBones == null || _influencedBones.Length != _currentAnimationClip.BoneCount)
			{
				_influencedBones = new bool[_currentAnimationClip.BoneCount];
			}
			for (int i = 0; i < influenceBones.Count; i++)
			{
				_influencedBones[influenceBones[i].Index] = true;
			}
			if (_translations.Length != _currentAnimationClip.BoneCount)
			{
				_translations = new Vector3[_currentAnimationClip.BoneCount];
			}
			if (_rotations.Length != _currentAnimationClip.BoneCount)
			{
				_rotations = new Quaternion[_currentAnimationClip.BoneCount];
			}
			if (_scales.Length != _currentAnimationClip.BoneCount)
			{
				_scales = new Vector3[_currentAnimationClip.BoneCount];
			}
		}

		public AnimationPlayer(AnimationClip clip, IList<Bone> influenceBones)
		{
			_currentAnimationClip = clip;
			_influencedBones = new bool[_currentAnimationClip.BoneCount];
			for (int i = 0; i < influenceBones.Count; i++)
			{
				_influencedBones[influenceBones[i].Index] = true;
			}
			_translations = new Vector3[_currentAnimationClip.BoneCount];
			_rotations = new Quaternion[_currentAnimationClip.BoneCount];
			_scales = new Vector3[_currentAnimationClip.BoneCount];
		}

		public AnimationPlayer(AnimationClip clip)
		{
			_currentAnimationClip = clip;
			_scales = new Vector3[_currentAnimationClip.BoneCount];
			_translations = new Vector3[_currentAnimationClip.BoneCount];
			_rotations = new Quaternion[_currentAnimationClip.BoneCount];
		}

		public void Update(TimeSpan timeSpan)
		{
			TimeSpan timeSpan2 = TimeSpan.FromSeconds(timeSpan.TotalSeconds * (double)base.Speed);
			if (base.Playing)
			{
				if (_onPong)
				{
					_currentTime -= timeSpan2;
				}
				else
				{
					_currentTime += timeSpan2;
				}
			}
			if (_currentTime > Duration)
			{
				if (PingPong)
				{
					_currentTime = Duration - (_currentTime - Duration);
					_onPong = true;
				}
				else if (Looping)
				{
					while (_currentTime > Duration)
					{
						_currentTime -= Duration;
					}
				}
				else
				{
					_currentTime = Duration;
				}
			}
			else if (_currentTime < TimeSpan.Zero)
			{
				if (PingPong)
				{
					if (Looping)
					{
						_onPong = false;
						_currentTime = -_currentTime;
					}
					else
					{
						_currentTime = TimeSpan.Zero;
					}
				}
				else if (Looping)
				{
					while (_currentTime < TimeSpan.Zero)
					{
						_currentTime += Duration;
					}
				}
				else
				{
					_currentTime = TimeSpan.Zero;
				}
			}
			TimeSpan position = _currentTime;
			if (_reversed)
			{
				position = Duration - _currentTime;
			}
			_currentAnimationClip.CopyTransforms(_translations, _rotations, _scales, position, _influencedBones);
		}

		public override void Update(TimeSpan timeSpan, IList<Bone> boneTransforms)
		{
			Update(timeSpan);
			for (int i = 0; i < boneTransforms.Count; i++)
			{
				if (_influencedBones == null || _influencedBones[i])
				{
					boneTransforms[i].SetTransform(_translations[i], _rotations[i], _scales[i]);
				}
			}
		}
	}
}
