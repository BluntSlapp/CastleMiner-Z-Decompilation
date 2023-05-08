using System;
using DNA.Drawing.Animation;

namespace DNA.Avatars
{
	public class AvatarAnimationCollection : LayeredAnimationPlayer
	{
		private Avatar _avatar;

		public AvatarAnimationCollection(Avatar avatar)
			: base(16)
		{
			_avatar = avatar;
		}

		public AnimationPlayer Play(string id, int channel, TimeSpan blendTime)
		{
			AnimationPlayer animation = AvatarAnimationManager.Instance.GetAnimation(id, _avatar.IsMale);
			PlayAnimation(channel, animation, blendTime);
			return animation;
		}
	}
}
