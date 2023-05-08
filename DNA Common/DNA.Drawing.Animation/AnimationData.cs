using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace DNA.Drawing.Animation
{
	public class AnimationData
	{
		[ContentSerializer]
		public Dictionary<string, AnimationClip> AnimationClips { get; private set; }

		public AnimationData(Dictionary<string, AnimationClip> animationClips)
		{
			AnimationClips = animationClips;
		}

		protected AnimationData()
		{
		}
	}
}
