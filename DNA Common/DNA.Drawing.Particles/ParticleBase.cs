using System;
using Microsoft.Xna.Framework;

namespace DNA.Drawing.Particles
{
	public class ParticleBase<T>
	{
		private string _texturePath;

		[NonSerialized]
		private T _texture = default(T);

		private ParticleBlendMode _blendMode = ParticleBlendMode.NonPreMult;

		private bool _randomizeRotations = true;

		private float _particlesPerSecond = 100f;

		private bool _localSpace;

		private bool _fadeOut = true;

		private TimeSpan _emmissionTime = TimeSpan.FromSeconds(0.0);

		private TimeSpan _particleLifeTime = TimeSpan.FromSeconds(1.0);

		private float _lifetimeVariation;

		private float _emitterVelocitySensitivity = 1f;

		private float _minHorizontalVelocity;

		private float _maxHorizontalVelocity;

		private float _minVerticalVelocity;

		private float _maxVerticalVelocity;

		private Vector3 _gravity = Vector3.Zero;

		private float _endVelocity = 1f;

		private Color _minColor = Color.White;

		private Color _maxColor = Color.White;

		private float _minRotateSpeed;

		private float _maxRotateSpeed;

		private float _minStartSize = 100f;

		private float _maxStartSize = 100f;

		private float _minEndSize = 100f;

		private float _maxEndSize = 100f;

		private bool _dieAfterEmmision = true;

		public string TexturePath
		{
			get
			{
				return _texturePath;
			}
			set
			{
				_texturePath = value;
			}
		}

		public T Texture
		{
			get
			{
				return _texture;
			}
			set
			{
				_texture = value;
			}
		}

		public ParticleBlendMode BlendMode
		{
			get
			{
				return _blendMode;
			}
			set
			{
				_blendMode = value;
			}
		}

		public bool RandomizeRotations
		{
			get
			{
				return _randomizeRotations;
			}
			set
			{
				_randomizeRotations = value;
			}
		}

		public float ParticlesPerSecond
		{
			get
			{
				return _particlesPerSecond;
			}
			set
			{
				_particlesPerSecond = value;
			}
		}

		public bool LocalSpace
		{
			get
			{
				return _localSpace;
			}
			set
			{
				_localSpace = value;
			}
		}

		public bool FadeOut
		{
			get
			{
				return _fadeOut;
			}
			set
			{
				_fadeOut = value;
			}
		}

		public TimeSpan EmmissionTime
		{
			get
			{
				return _emmissionTime;
			}
			set
			{
				_emmissionTime = value;
			}
		}

		public TimeSpan ParticleLifeTime
		{
			get
			{
				return _particleLifeTime;
			}
			set
			{
				_particleLifeTime = value;
			}
		}

		public float LifetimeVariation
		{
			get
			{
				return _lifetimeVariation;
			}
			set
			{
				_lifetimeVariation = value;
			}
		}

		public float EmitterVelocitySensitivity
		{
			get
			{
				return _emitterVelocitySensitivity;
			}
			set
			{
				_emitterVelocitySensitivity = value;
			}
		}

		public float HorizontalVelocityMin
		{
			get
			{
				return _minHorizontalVelocity;
			}
			set
			{
				_minHorizontalVelocity = value;
			}
		}

		public float HorizontalVelocityMax
		{
			get
			{
				return _maxHorizontalVelocity;
			}
			set
			{
				_maxHorizontalVelocity = value;
			}
		}

		public float VerticalVelocityMin
		{
			get
			{
				return _minVerticalVelocity;
			}
			set
			{
				_minVerticalVelocity = value;
			}
		}

		public float VerticalVelocityMax
		{
			get
			{
				return _maxVerticalVelocity;
			}
			set
			{
				_maxVerticalVelocity = value;
			}
		}

		public Vector3 Gravity
		{
			get
			{
				return _gravity;
			}
			set
			{
				_gravity = value;
			}
		}

		public float VelocityEnd
		{
			get
			{
				return _endVelocity;
			}
			set
			{
				_endVelocity = value;
			}
		}

		public Color ColorMin
		{
			get
			{
				return _minColor;
			}
			set
			{
				_minColor = value;
			}
		}

		public Color ColorMax
		{
			get
			{
				return _maxColor;
			}
			set
			{
				_maxColor = value;
			}
		}

		public float RotateSpeedMin
		{
			get
			{
				return _minRotateSpeed;
			}
			set
			{
				_minRotateSpeed = value;
			}
		}

		public float RotateSpeedMax
		{
			get
			{
				return _maxRotateSpeed;
			}
			set
			{
				_maxRotateSpeed = value;
			}
		}

		public float StartSizeMin
		{
			get
			{
				return _minStartSize;
			}
			set
			{
				_minStartSize = value;
			}
		}

		public float StartSizeMax
		{
			get
			{
				return _maxStartSize;
			}
			set
			{
				_maxStartSize = value;
			}
		}

		public float EndSizeMin
		{
			get
			{
				return _minEndSize;
			}
			set
			{
				_minEndSize = value;
			}
		}

		public float EndSizeMax
		{
			get
			{
				return _maxEndSize;
			}
			set
			{
				_maxEndSize = value;
			}
		}

		public bool DieAfterEmmision
		{
			get
			{
				return _dieAfterEmmision;
			}
			set
			{
				_dieAfterEmmision = value;
			}
		}
	}
}
