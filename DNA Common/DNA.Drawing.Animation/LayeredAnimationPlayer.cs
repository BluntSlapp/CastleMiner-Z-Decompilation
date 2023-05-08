using System;
using System.Collections.Generic;

namespace DNA.Drawing.Animation
{
	public class LayeredAnimationPlayer : BaseAnimationPlayer
	{
		private AnimBlender[] _blenders;

		public AnimationPlayer this[int index]
		{
			get
			{
				return _blenders[index].ActiveAnimation;
			}
		}

		public AnimationPlayer GetAnimation(int channel)
		{
			return _blenders[channel].ActiveAnimation;
		}

		public void PlayAnimation(int channel, AnimationPlayer player, TimeSpan blendTime)
		{
			_blenders[channel].Play(player, blendTime);
		}

		public void ClearAnimation(int channel, TimeSpan blendTime)
		{
			PlayAnimation(channel, null, blendTime);
		}

		public LayeredAnimationPlayer(int channels)
		{
			_blenders = new AnimBlender[channels];
			for (int i = 0; i < _blenders.Length; i++)
			{
				_blenders[i] = new AnimBlender();
			}
		}

		public override void Update(TimeSpan timeSpan, IList<Bone> boneTransforms)
		{
			for (int i = 0; i < _blenders.Length; i++)
			{
				_blenders[i].Update(timeSpan, boneTransforms);
			}
		}
	}
}
