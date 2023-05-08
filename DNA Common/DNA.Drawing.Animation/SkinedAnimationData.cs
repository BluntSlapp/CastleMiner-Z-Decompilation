using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DNA.Drawing.Animation
{
	public class SkinedAnimationData : AnimationData
	{
		[ContentSerializer]
		public Matrix[] InverseBindPose { get; private set; }

		[ContentSerializer]
		public Skeleton Skeleton { get; private set; }

		public SkinedAnimationData(Dictionary<string, AnimationClip> animationClips, List<Matrix> inverseBindPose, Skeleton skeleton)
			: base(animationClips)
		{
			InverseBindPose = inverseBindPose.ToArray();
			Skeleton = skeleton;
		}

		private SkinedAnimationData()
		{
		}
	}
}
