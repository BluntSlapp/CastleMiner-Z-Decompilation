using System.Collections.Generic;
using DNA.Drawing.Animation;
using Microsoft.Xna.Framework.GamerServices;

namespace DNA.Avatars
{
	public class AvatarAnimationManager
	{
		private class AvatarAnimationSet
		{
			public AnimationClip Male;

			public AnimationClip Female;

			public bool[] InfluencedBones;

			public bool Looping;

			public AnimationClip GetClip(bool male)
			{
				if (male)
				{
					if (Male == null)
					{
						return Female;
					}
					return Male;
				}
				if (Female == null)
				{
					return Male;
				}
				return Female;
			}

			public AvatarAnimationSet(AnimationClip male, AnimationClip female, bool[] influencedBones, bool looping)
			{
				Male = male;
				Female = female;
				InfluencedBones = influencedBones;
				Looping = looping;
			}
		}

		public static AvatarAnimationManager Instance = new AvatarAnimationManager();

		private Dictionary<string, AvatarAnimationSet> _clips = new Dictionary<string, AvatarAnimationSet>();

		public static readonly AnimationClip DefaultAnimationClip;

		public void RegisterAnimation(string name, AnimationClip clip, bool looping, IList<AvatarBone> bones, IList<AvatarBone> maskedBones)
		{
			_clips[name] = new AvatarAnimationSet(clip, null, Avatar.GetInfluncedBoneList(bones, maskedBones), looping);
		}

		public void RegisterAnimation(string name, AnimationClip maleClip, AnimationClip femaleClip, bool looping, IList<AvatarBone> bones, IList<AvatarBone> maskedBones)
		{
			_clips[name] = new AvatarAnimationSet(maleClip, femaleClip, Avatar.GetInfluncedBoneList(bones, maskedBones), looping);
		}

		public void RegisterAnimation(string name, AnimationClip clip, bool looping, IList<AvatarBone> bones)
		{
			_clips[name] = new AvatarAnimationSet(clip, null, Avatar.GetInfluncedBoneList(bones), looping);
		}

		public void RegisterAnimation(string name, AnimationClip maleClip, AnimationClip femaleClip, bool looping, IList<AvatarBone> bones)
		{
			_clips[name] = new AvatarAnimationSet(maleClip, femaleClip, Avatar.GetInfluncedBoneList(bones), looping);
		}

		public void RegisterAnimation(string name, AnimationClip clip, bool looping)
		{
			_clips[name] = new AvatarAnimationSet(clip, null, null, looping);
		}

		public void RegisterAnimation(string name, AnimationClip maleClip, AnimationClip femaleClip, bool looping)
		{
			_clips[name] = new AvatarAnimationSet(maleClip, femaleClip, null, looping);
		}

		public AnimationPlayer GetAnimation(string name, bool male)
		{
			AvatarAnimationSet avatarAnimationSet = _clips[name];
			AnimationPlayer animationPlayer = new AnimationPlayer(avatarAnimationSet.GetClip(male));
			animationPlayer.Name = name;
			animationPlayer.Looping = avatarAnimationSet.Looping;
			animationPlayer.SetInfluncedBones(avatarAnimationSet.InfluencedBones);
			return animationPlayer;
		}

		public void GetAnimation(AnimationPlayer player, string name, bool male)
		{
			AvatarAnimationSet avatarAnimationSet = _clips[name];
			player.SetClip(avatarAnimationSet.GetClip(male));
			player.Name = name;
			player.Looping = avatarAnimationSet.Looping;
			player.SetInfluncedBones(avatarAnimationSet.InfluencedBones);
		}

		private AvatarAnimationManager()
		{
		}
	}
}
