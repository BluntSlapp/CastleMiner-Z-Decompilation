using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Drawing.Particles
{
	public class ParticleEffect : ParticleBase<Texture2D>
	{
		[NonSerialized]
		private List<ParticleEmitter> _liveEffects = new List<ParticleEmitter>();

		protected virtual ParticleEmitter CreateEmitterInternal(DNAGame game)
		{
			return new ParticleEmitter(game, this);
		}

		public ParticleEmitter CreateEmitter(DNAGame game)
		{
			ParticleEmitter particleEmitter = null;
			for (int i = 0; i < _liveEffects.Count; i++)
			{
				ParticleEmitter particleEmitter2 = _liveEffects[i];
				if (particleEmitter2.Parent == null && particleEmitter2.Loaded)
				{
					particleEmitter = particleEmitter2;
					break;
				}
			}
			if (particleEmitter == null)
			{
				particleEmitter = CreateEmitterInternal(game);
				_liveEffects.Add(particleEmitter);
			}
			else
			{
				particleEmitter.Reset();
			}
			return particleEmitter;
		}
	}
}
