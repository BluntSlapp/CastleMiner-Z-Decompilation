using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Animation
{
	public class AnimBlender
	{
		private AnimationPlayer _currentAnimation;

		private AnimationPlayer _previousAnimation;

		private TimeSpan _blendTotalTime = TimeSpan.FromSeconds(0.25);

		private TimeSpan _blendCurrentTime;

		public AnimationPlayer ActiveAnimation
		{
			get
			{
				return _currentAnimation;
			}
		}

		public void Play(AnimationPlayer nextAnimation, TimeSpan blendTime)
		{
			_previousAnimation = _currentAnimation;
			_currentAnimation = nextAnimation;
			_blendTotalTime = blendTime;
			_blendCurrentTime = TimeSpan.Zero;
		}

		public void Update(TimeSpan elapsedAnimationTime, IList<Bone> bones)
		{
			if (_currentAnimation == null && _previousAnimation == null)
			{
				return;
			}
			if (_currentAnimation != null)
			{
				_currentAnimation.Update(elapsedAnimationTime);
			}
			if (_previousAnimation != null)
			{
				_previousAnimation.Update(elapsedAnimationTime);
			}
			_blendCurrentTime += elapsedAnimationTime;
			float num = 1f;
			if (_blendTotalTime.TotalSeconds > 0.0)
			{
				num = (float)(_blendCurrentTime.TotalSeconds / _blendTotalTime.TotalSeconds);
			}
			if (num >= 1f)
			{
				num = 1f;
				_previousAnimation = null;
				if (_currentAnimation == null)
				{
					return;
				}
				for (int i = 0; i < bones.Count; i++)
				{
					if (_currentAnimation.GetBoneInfluence(i))
					{
						bones[i].Translation = _currentAnimation.Translations[i];
						bones[i].Rotation = _currentAnimation.Rotations[i];
						bones[i].Scale = _currentAnimation.Scales[i];
					}
				}
				return;
			}
			for (int j = 0; j < bones.Count; j++)
			{
				if (_previousAnimation != null && _previousAnimation.GetBoneInfluence(j))
				{
					if (_currentAnimation != null && _currentAnimation.GetBoneInfluence(j))
					{
						bones[j].Translation = Vector3.Lerp(_previousAnimation.Translations[j], _currentAnimation.Translations[j], num);
						bones[j].Rotation = Quaternion.Slerp(_previousAnimation.Rotations[j], _currentAnimation.Rotations[j], num);
						bones[j].Scale = Vector3.Lerp(_previousAnimation.Scales[j], _currentAnimation.Scales[j], num);
					}
					else
					{
						bones[j].Translation = Vector3.Lerp(_previousAnimation.Translations[j], bones[j].Translation, num);
						bones[j].Rotation = Quaternion.Slerp(_previousAnimation.Rotations[j], bones[j].Rotation, num);
						bones[j].Scale = Vector3.Lerp(_previousAnimation.Scales[j], bones[j].Scale, num);
					}
				}
				else if (_currentAnimation != null && _currentAnimation.GetBoneInfluence(j))
				{
					bones[j].Translation = Vector3.Lerp(bones[j].Translation, _currentAnimation.Translations[j], num);
					bones[j].Rotation = Quaternion.Slerp(bones[j].Rotation, _currentAnimation.Rotations[j], num);
					bones[j].Scale = Vector3.Lerp(bones[j].Scale, _currentAnimation.Scales[j], num);
				}
			}
		}
	}
}
