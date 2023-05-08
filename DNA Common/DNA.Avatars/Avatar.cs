using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DNA.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.Avatars
{
	public class Avatar : Entity
	{
		public const float WalkSpeed = 0.947f;

		public const float RunSpeed = 4f;

		private static AvatarDescription _defaultDescription;

		public static readonly byte[] DefaultDescriptionData;

		public static readonly ReadOnlyCollection<int> DefaultParentBones;

		public static readonly ReadOnlyCollection<string> BoneNames;

		public static readonly ReadOnlyCollection<Matrix> DefaultBindPose;

		private AvatarRenderer _avatarRenderer;

		private AvatarAnimationCollection _animations;

		private AvatarExpression _expression = default(AvatarExpression);

		private AvatarDescription _avatarDescription;

		private Matrix[] _bonesToAvatar = new Matrix[71];

		private Matrix[] _boneTransformBuffer = new Matrix[71];

		private Dictionary<AvatarBone, Entity> _partMap = new Dictionary<AvatarBone, Entity>();

		private Matrix[] _wireFrameWorldTransforms;

		private VertexPositionColor[] _wireFrameVerts;

		public PerspectiveCamera EyePointCamera = new PerspectiveCamera();

		public bool HideHead;

		private bool _skeletonBuilt;

		private Skeleton _skeleton = Bone.BuildSkeleton(DefaultBindPose, DefaultParentBones, BoneNames);

		private BasicEffect _wireFrameEffect;

		public SignedInGamer _signedInGamer;

		private Matrix eyeToHead = Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)Math.PI) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI)) * Matrix.CreateTranslation(new Vector3(0f, 0.05f, 0f));

		private static RenderTarget2D AvatarTestTarget;

		public static AvatarDescription DefaultDescription
		{
			get
			{
				if (_defaultDescription == null)
				{
					_defaultDescription = AvatarDescription.CreateRandom();
				}
				return _defaultDescription;
			}
		}

		public ReadOnlyCollection<int> ParentBones
		{
			get
			{
				if (_avatarRenderer != null && _avatarRenderer.State == AvatarRendererState.Ready)
				{
					return _avatarRenderer.get_ParentBones();
				}
				return DefaultParentBones;
			}
		}

		public ReadOnlyCollection<Matrix> BindPose
		{
			get
			{
				if (_avatarRenderer != null && _avatarRenderer.State == AvatarRendererState.Ready)
				{
					return _avatarRenderer.get_BindPose();
				}
				return DefaultBindPose;
			}
		}

		public Skeleton Skeleton
		{
			get
			{
				return _skeleton;
			}
		}

		public AvatarDescription Description
		{
			get
			{
				return _avatarDescription;
			}
		}

		public AvatarRendererState AvatarState
		{
			get
			{
				return _avatarRenderer.State;
			}
		}

		public AvatarRenderer AvatarRenderer
		{
			get
			{
				return _avatarRenderer;
			}
		}

		public float AvatarHeight
		{
			get
			{
				return _avatarDescription.Height;
			}
		}

		public AvatarExpression Expression
		{
			get
			{
				return _expression;
			}
			set
			{
				_expression = value;
			}
		}

		public AvatarAnimationCollection Animations
		{
			get
			{
				return _animations;
			}
		}

		public SignedInGamer SignedInGamer
		{
			get
			{
				return _signedInGamer;
			}
		}

		public bool IsMale
		{
			get
			{
				return _avatarDescription.BodyType == AvatarBodyType.Male;
			}
		}

		static Avatar()
		{
			DefaultDescriptionData = new byte[1021]
			{
				1, 0, 0, 0, 0, 191, 0, 0, 0, 191,
				0, 0, 0, 0, 16, 0, 0, 3, 31, 0,
				3, 193, 200, 241, 9, 161, 156, 178, 224, 0,
				8, 0, 0, 3, 43, 0, 3, 193, 200, 241,
				9, 161, 156, 178, 224, 0, 32, 0, 0, 3,
				59, 0, 3, 193, 200, 241, 9, 161, 156, 178,
				224, 0, 0, 128, 0, 2, 234, 0, 3, 193,
				200, 241, 9, 161, 156, 178, 224, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 32, 0, 2, 158, 0,
				3, 193, 200, 241, 9, 161, 156, 178, 224, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 64, 0, 2,
				100, 0, 3, 193, 200, 241, 9, 161, 156, 178,
				224, 63, 128, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 255, 215, 170, 113, 255, 110, 83,
				38, 255, 181, 97, 87, 255, 99, 129, 167, 255,
				73, 52, 33, 255, 83, 149, 202, 255, 73, 52,
				33, 255, 207, 89, 105, 255, 207, 89, 105, 0,
				0, 0, 2, 0, 0, 0, 1, 193, 200, 241,
				9, 161, 156, 178, 224, 0, 2, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 1, 0, 2, 0, 3, 193,
				200, 241, 9, 161, 156, 178, 224, 0, 1, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 4, 1, 178, 0,
				3, 193, 200, 241, 9, 161, 156, 178, 224, 0,
				4, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 8, 0,
				88, 0, 1, 193, 200, 241, 9, 161, 156, 178,
				224, 0, 8, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				16, 0, 144, 0, 1, 193, 200, 241, 9, 161,
				156, 178, 224, 0, 16, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 32, 0, 49, 0, 1, 193, 200, 241,
				9, 161, 156, 178, 224, 0, 32, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 32, 0, 49, 0, 1, 193, 200, 241,
				9, 161, 156, 178, 224, 0, 32, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 16, 0, 144, 0, 1, 193,
				200, 241, 9, 161, 156, 178, 224, 0, 16, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 8, 0, 88, 0,
				1, 193, 200, 241, 9, 161, 156, 178, 224, 0,
				8, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 4, 1,
				178, 0, 3, 193, 200, 241, 9, 161, 156, 178,
				224, 0, 4, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 224, 0, 2,
				77, 216, 48, 81, 160, 3, 51, 5, 26, 3,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 19, 202, 106, 209, 13, 230, 203, 203, 185,
				0, 179, 142, 247, 181, 126, 186, 157, 86, 192,
				29
			};
			DefaultParentBones = new ReadOnlyCollection<int>(new int[71]
			{
				-1, 0, 0, 0, 0, 1, 2, 2, 3, 3,
				1, 6, 5, 6, 5, 8, 5, 8, 5, 14,
				12, 11, 16, 15, 14, 20, 20, 20, 22, 22,
				22, 25, 25, 25, 28, 28, 28, 33, 33, 33,
				33, 33, 33, 33, 36, 36, 36, 36, 36, 36,
				36, 37, 38, 39, 40, 43, 44, 45, 46, 47,
				50, 51, 52, 53, 54, 55, 56, 57, 58, 59,
				60
			});
			DefaultBindPose = new ReadOnlyCollection<Matrix>(new Matrix[71]
			{
				new Matrix(-1.06f, 4.517219E-16f, 1.263618E-07f, 0f, 4.560241E-16f, 1.05f, -2.059464E-16f, 0f, -1.263618E-07f, -2.040034E-16f, -1.06f, 0f, 1.00638E-06f, 0.7929635f, -0.00918531f, 1f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0f, 0f, -2.910383E-11f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.09005711f, -0.1072717f, 0.008671193f, 1f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, -0.09005711f, -0.1072717f, 0.008671193f, 0.9999999f),
				new Matrix(0.9f, 0f, -3.929107E-25f, 0f, 0f, 1f, -1.964554E-25f, 0f, 3.722312E-25f, 2.067951E-25f, 0.95f, 0f, 0f, 0.03059953f, -2.910383E-11f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0f, 0.09179866f, -0.007715893f, 0.9999999f),
				new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, -7.070368E-06f, -0.2687335f, -0.01300271f, 0.9999999f),
				new Matrix(0.85f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 0.85f, 0f, 0f, -0.1343725f, -0.006499312f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 4.135903E-25f, 0f, -4.135903E-25f, 1f, 0f, 0f, -4.135903E-25f, 0f, 1f, 0f, -7.070368E-06f, -0.2687335f, -0.01300271f, 0.9999999f),
				new Matrix(0.85f, 4.135903E-25f, 3.515517E-25f, 0f, -3.515517E-25f, 1f, 0f, 0f, -3.515517E-25f, 0f, 0.85f, 0f, 0f, -0.1343725f, -0.006499312f, 0.9999999f),
				new Matrix(0.8f, 0f, -3.308722E-25f, 0f, 0f, 1f, -1.654361E-25f, 0f, 3.308722E-25f, 2.067951E-25f, 0.8f, 0f, 0f, 0.06119912f, -0.005143928f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.005812414f, -0.2596922f, -0.02537671f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.007403408f, 0.1224214f, 0.01426828f, 0.9999999f),
				new Matrix(0.85f, 0f, -3.515517E-25f, 0f, 0f, 1f, -1.757759E-25f, 0f, 3.515517E-25f, 2.067951E-25f, 0.85f, 0f, 0.002913281f, -0.1298461f, -0.01268019f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0f, 0.1737709f, -0.01882024f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.00579828f, -0.2596922f, -0.02537671f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, -0.007403407f, 0.1224214f, 0.01426828f, 0.9999999f),
				new Matrix(0.85f, 4.135903E-25f, 0f, 0f, -3.515517E-25f, 1f, -1.757759E-25f, 0f, 0f, 2.067951E-25f, 0.85f, 0f, -0.00289914f, -0.1298461f, -0.01268019f, 0.9999999f),
				new Matrix(0.95f, 0f, -3.722312E-25f, 0f, 0f, 1f, -1.861156E-25f, 0f, 3.929107E-25f, 2.067951E-25f, 0.9f, 0f, 0f, 0.04343986f, -0.004703019f, 0.9999999f),
				new Matrix(0.95f, 0f, -3.929107E-25f, 0f, 0f, 0.95f, -1.964554E-25f, 0f, 3.929107E-25f, 1.964554E-25f, 0.95f, 0f, -7.071068E-06f, 0.1183337f, 0.02284149f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.1176131f, 5.775504E-05f, -0.0279201f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.007855958f, -0.1010247f, 0.1342524f, 1f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.117613f, 5.775318E-05f, -0.0279201f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.007855952f, -0.1010247f, 0.1342524f, 0.9999999f),
				new Matrix(0.85f, 0f, -3.515517E-25f, 0f, 0f, 1f, -1.757759E-25f, 0f, 3.515517E-25f, 2.067951E-25f, 0.85f, 0f, -7.071068E-06f, 0.03944456f, 0.007605664f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.1696066f, -0.003995299f, 0.0005878786f, 0.9999999f),
				new Matrix(1f, 0f, -3.515517E-25f, 0f, 0f, 0.85f, -1.757759E-25f, 0f, 4.135903E-25f, 1.757759E-25f, 0.85f, 0f, 0.0848033f, -0.00199765f, 0.0002898554f, 0.9999999f),
				new Matrix(1f, 0f, -3.515517E-25f, 0f, 0f, 0.85f, -1.757759E-25f, 0f, 4.135903E-25f, 1.757759E-25f, 0.85f, 0f, 0f, 0f, -5.820766E-11f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.1696066f, -0.003995301f, 0.0005878787f, 0.9999999f),
				new Matrix(1f, 3.515517E-25f, 0f, 0f, -4.135903E-25f, 0.85f, -1.757759E-25f, 0f, 0f, 1.757759E-25f, 0.85f, 0f, -0.08480332f, -0.001997652f, 0.0002898555f, 0.9999999f),
				new Matrix(1f, 3.515517E-25f, 0f, 0f, -4.135903E-25f, 0.85f, -1.757759E-25f, 0f, 0f, 1.757759E-25f, 0.85f, 0f, 0f, -1.862645E-09f, 0f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.1027992f, -0.002424836f, 0.001567673f, 0.9999999f),
				new Matrix(1f, 0f, -3.515517E-25f, 0f, 0f, 0.85f, -1.757759E-25f, 0f, 4.135903E-25f, 1.757759E-25f, 0.85f, 0f, 0.07710293f, -0.001824439f, 0.001175754f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.1541988f, -0.003637314f, 0.002347426f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.1027992f, -0.002424838f, 0.001567673f, 0.9999999f),
				new Matrix(1f, 3.515517E-25f, 0f, 0f, -4.135903E-25f, 0.85f, -1.757759E-25f, 0f, 0f, 1.757759E-25f, 0.85f, 0f, -0.07710292f, -0.00182444f, 0.001175754f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.1541988f, -0.003637316f, 0.002347426f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.1083924f, -0.02864808f, 0.04242924f, 1f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.1101885f, -0.02752805f, 0.01115742f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.1034568f, -0.03065729f, -0.01861204f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.09029046f, -0.03503358f, -0.04665053f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.07399872f, -0.1849946f, 4.082802E-06f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.1110087f, -0.1110014f, 4.082802E-06f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0f, 0f, 0.009895937f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.1083924f, -0.02864808f, 0.04242924f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.1101884f, -0.02752805f, 0.01115742f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.1034568f, -0.03065729f, -0.01861204f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.09029045f, -0.03503358f, -0.04665053f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.07399871f, -0.1849946f, 4.08286E-06f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.1110087f, -0.1110014f, 4.08286E-06f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, 0f, -1.862645E-09f, 0.009895937f, 0.9999999f),
				new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.04690241f, -1.862645E-09f, 0.0003796703f, 1f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.04973078f, 0f, -1.224695E-05f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.04768014f, 0f, -0.000355173f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.04028386f, 0f, -0.0003551769f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.06541445f, -0.03722751f, 0.04949193f, 1f),
				new Matrix(1f, 4.135903E-25f, 4.135903E-25f, 0f, -4.135903E-25f, 1f, 0f, 0f, -4.135903E-25f, 0f, 1f, 0f, -0.04690242f, 0f, 0.0003796704f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.04973083f, -1.862645E-09f, -1.224689E-05f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.04768026f, -1.862645E-09f, -0.0003551729f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.04028386f, -1.862645E-09f, -0.0003551766f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.06542858f, -0.03722751f, 0.04949194f, 0.9999999f),
				new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.03380674f, -1.862645E-09f, 1.224864E-05f, 1f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.03515738f, 0f, -5.820766E-11f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.03452808f, 0f, 1.22448E-05f, 0.9999999f),
				new Matrix(1f, 0f, -4.135903E-25f, 0f, 0f, 1f, -2.067951E-25f, 0f, 4.135903E-25f, 2.067951E-25f, 1f, 0f, 0.02958536f, 0f, -2.328306E-10f, 0.9999999f),
				new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0.03209555f, -0.01738984f, 0.01951019f, 1f),
				new Matrix(1f, 4.135903E-25f, 4.135903E-25f, 0f, -4.135903E-25f, 1f, 0f, 0f, -4.135903E-25f, 0f, 1f, 0f, -0.03380674f, 0f, 1.22487E-05f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.03515732f, -1.862645E-09f, 0f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.03452795f, -1.862645E-09f, 1.224491E-05f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 0f, 0f, -4.135903E-25f, 1f, -2.067951E-25f, 0f, 0f, 2.067951E-25f, 1f, 0f, -0.02958536f, -1.862645E-09f, 0f, 0.9999999f),
				new Matrix(1f, 4.135903E-25f, 4.135903E-25f, 0f, -4.135903E-25f, 1f, 0f, 0f, -4.135903E-25f, 0f, 1f, 0f, -0.03209561f, -0.01738983f, 0.01951019f, 0.9999999f)
			});
			AvatarTestTarget = null;
			List<string> list = new List<string>();
			for (int i = 0; i < 71; i++)
			{
				AvatarBone avatarBone = (AvatarBone)i;
				list.Add(((object)avatarBone).ToString());
			}
			BoneNames = new ReadOnlyCollection<string>(list);
		}

		public Entity GetAvatarPart(AvatarBone bone)
		{
			return _partMap[bone];
		}

		private void InitalizeParts()
		{
			for (int i = 0; i < 71; i++)
			{
				_bonesToAvatar[i] = Matrix.Identity;
			}
			_partMap[AvatarBone.Neck] = new Entity();
			_partMap[AvatarBone.Root] = new Entity();
			_partMap[AvatarBone.BackLower] = new Entity();
			_partMap[AvatarBone.Head] = new Entity();
			_partMap[AvatarBone.SpecialRight] = new Entity();
			_partMap[AvatarBone.SpecialLeft] = new Entity();
			_partMap[AvatarBone.PropLeft] = new Entity();
			_partMap[AvatarBone.ShoulderLeft] = new Entity();
			_partMap[AvatarBone.ShoulderRight] = new Entity();
			_partMap[AvatarBone.PropRight] = new Entity();
			base.Children.Add(EyePointCamera);
			foreach (KeyValuePair<AvatarBone, Entity> item in _partMap)
			{
				base.Children.Add(item.Value);
			}
		}

		public void SetAsPlayerAvatar(PlayerIndex index)
		{
			_signedInGamer = Gamer.SignedInGamers[index];
			IAsyncResult asyncResult = AvatarDescription.BeginGetFromGamer((Gamer)_signedInGamer, (AsyncCallback)delegate
			{
			}, (object)null);
			asyncResult.AsyncWaitHandle.WaitOne();
			_avatarDescription = AvatarDescription.EndGetFromGamer(asyncResult);
			_avatarRenderer = new AvatarRenderer(_avatarDescription, false);
		}

		public Avatar(PlayerIndex index)
		{
			_animations = new AvatarAnimationCollection(this);
			_expression.Mouth = AvatarMouth.Neutral;
			_signedInGamer = Gamer.SignedInGamers[index];
			IAsyncResult asyncResult = AvatarDescription.BeginGetFromGamer((Gamer)_signedInGamer, (AsyncCallback)delegate
			{
			}, (object)null);
			asyncResult.AsyncWaitHandle.WaitOne();
			_avatarDescription = AvatarDescription.EndGetFromGamer(asyncResult);
			_avatarRenderer = new AvatarRenderer(_avatarDescription, false);
			if (!_avatarDescription.IsValid)
			{
				_avatarDescription = AvatarDescription.CreateRandom();
			}
			_expression = default(AvatarExpression);
			InitalizeParts();
		}

		public Avatar(AvatarDescription description)
		{
			_animations = new AvatarAnimationCollection(this);
			_expression.Mouth = AvatarMouth.Neutral;
			_avatarDescription = description;
			_avatarRenderer = new AvatarRenderer(_avatarDescription, false);
			_expression = default(AvatarExpression);
			InitalizeParts();
		}

		public Avatar(bool useRandom)
		{
			_animations = new AvatarAnimationCollection(this);
			_expression.Mouth = AvatarMouth.Neutral;
			if (useRandom)
			{
				MakeRandom();
			}
			else
			{
				MakeDefault();
			}
			InitalizeParts();
		}

		public Matrix GetBoneToAvatar(AvatarBone bone)
		{
			return _bonesToAvatar[(int)bone];
		}

		public void UpdateParts()
		{
			UpdateBones();
			foreach (KeyValuePair<AvatarBone, Entity> item in _partMap)
			{
				Vector3 scale;
				Quaternion rotation;
				Vector3 translation;
				_bonesToAvatar[(int)item.Key].Decompose(out scale, out rotation, out translation);
				item.Value.LocalPosition = translation;
				item.Value.LocalScale = scale;
				item.Value.LocalRotation = rotation;
			}
		}

		private void UpdateBonesFromParts()
		{
			ReadOnlyCollection<int> parentBones = ParentBones;
			ReadOnlyCollection<Matrix> bindPose = BindPose;
			foreach (KeyValuePair<AvatarBone, Entity> item in _partMap)
			{
				int key = (int)item.Key;
				Matrix matrix = ((parentBones[key] != -1) ? _bonesToAvatar[parentBones[key]] : Matrix.Identity);
				Matrix matrix2 = Matrix.Invert(matrix);
				Matrix matrix3 = bindPose[key];
				Matrix matrix4 = Matrix.Invert(matrix3);
				Matrix localToParent = item.Value.LocalToParent;
				Skeleton[key].SetTransform(localToParent * matrix2 * matrix4);
			}
		}

		private void UpdateBones()
		{
			ReadOnlyCollection<int> parentBones = ParentBones;
			ReadOnlyCollection<Matrix> bindPose = BindPose;
			for (int i = 0; i < 71; i++)
			{
				Matrix matrix = ((parentBones[i] != -1) ? _bonesToAvatar[parentBones[i]] : Matrix.Identity);
				Matrix matrix2 = Matrix.Multiply(Skeleton[i].Transform, bindPose[i]);
				_bonesToAvatar[i] = Matrix.Multiply(matrix2, matrix);
			}
		}

		public static List<int> FindInfluencedBones(AvatarBone avatarBone)
		{
			List<int> list = new List<int>();
			list.Add((int)avatarBone);
			for (int i = list[0] + 1; i < DefaultParentBones.Count; i++)
			{
				if (list.Contains(DefaultParentBones[i]))
				{
					list.Add(i);
				}
			}
			return list;
		}

		public static bool[] GetInfluncedBoneList(AvatarBone bone)
		{
			return GetInfluncedBoneList(new AvatarBone[1] { bone });
		}

		public static bool[] GetInfluncedBoneList(IList<AvatarBone> bones)
		{
			new Dictionary<int, int>();
			bool[] array = new bool[71];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = false;
			}
			foreach (AvatarBone bone in bones)
			{
				List<int> list = FindInfluencedBones(bone);
				foreach (int item in list)
				{
					array[item] = true;
				}
			}
			return array;
		}

		public static bool[] GetInfluncedBoneList(IList<AvatarBone> bones, IList<AvatarBone> maskedBones)
		{
			new Dictionary<int, int>();
			bool[] array;
			if (bones != null)
			{
				array = GetInfluncedBoneList(bones);
			}
			else
			{
				array = new bool[71];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = true;
				}
			}
			bool[] influncedBoneList = GetInfluncedBoneList(maskedBones);
			for (int j = 0; j < influncedBoneList.Length; j++)
			{
				if (influncedBoneList[j])
				{
					array[j] = false;
				}
			}
			return array;
		}

		protected override void OnUpdate(GameTime gameTime)
		{
			if (!_skeletonBuilt && _avatarRenderer.State == AvatarRendererState.Ready)
			{
				BuildSkeleton();
			}
			_animations.Update(gameTime.get_ElapsedGameTime(), Skeleton);
			UpdateParts();
			Matrix matrix = _bonesToAvatar[19];
			Vector3 translation = matrix.Translation;
			Vector3 forward = matrix.Forward;
			Vector3 up = matrix.Up;
			forward.Normalize();
			up.Normalize();
			matrix = Matrix.CreateWorld(translation, forward, up);
			Matrix localToParent = eyeToHead * matrix;
			EyePointCamera.LocalToParent = localToParent;
			if (HideHead)
			{
				Bone bone = Skeleton[19];
				bone.Scale = new Vector3(0.001f, 0.001f, 0.001f);
			}
			Skeleton.CopyTransformsTo(_boneTransformBuffer);
			base.OnUpdate(gameTime);
		}

		public bool IsVisibleAvatar(GraphicsDevice device)
		{
			int num = 32;
			if (AvatarTestTarget == null)
			{
				AvatarTestTarget = new RenderTarget2D(device, num, num, false, SurfaceFormat.Bgra4444, DepthFormat.Depth16, 1, RenderTargetUsage.PreserveContents);
			}
			device.SetRenderTarget(AvatarTestTarget);
			device.Clear(new Color(0, 0, 0, 0));
			_avatarRenderer.World = Matrix.Identity;
			_avatarRenderer.View = Matrix.CreateLookAt(new Vector3(0f, _avatarDescription.Height / 2f, 5.5f), new Vector3(0f, _avatarDescription.Height / 2f, 0f), Vector3.Up);
			_avatarRenderer.Projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, 1f, 0.1f, 1000f);
			_avatarRenderer.Draw((IList<Matrix>)BindPose, Expression);
			_avatarRenderer.Draw((IList<Matrix>)BindPose, Expression);
			device.SetRenderTarget(null);
			byte[] array = new byte[num * num * 4];
			AvatarTestTarget.GetData(array);
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != 0)
				{
					num2++;
				}
			}
			float num3 = (float)num2 / (float)array.Length;
			if (num3 < 0.01f)
			{
				return false;
			}
			return true;
		}

		public override void Draw(GraphicsDevice device, GameTime gameTime, Matrix view, Matrix projection)
		{
			_avatarRenderer.World = base.LocalToWorld;
			_avatarRenderer.View = view;
			_avatarRenderer.Projection = projection;
			try
			{
				_avatarRenderer.Draw((IList<Matrix>)_boneTransformBuffer, _expression);
			}
			catch
			{
			}
			base.Draw(device, gameTime, view, projection);
		}

		public void MakeDefault()
		{
			_avatarDescription = DefaultDescription;
			_avatarRenderer = new AvatarRenderer(_avatarDescription);
			_expression = default(AvatarExpression);
		}

		private void DefaultSkeleton()
		{
			_skeletonBuilt = false;
			_skeleton = Bone.BuildSkeleton(DefaultBindPose, DefaultParentBones, BoneNames);
		}

		private void BuildSkeleton()
		{
			_skeleton = Bone.BuildSkeleton(_avatarRenderer);
			_skeletonBuilt = true;
		}

		public void MakeRandom()
		{
			_avatarDescription = AvatarDescription.CreateRandom();
			_avatarRenderer = new AvatarRenderer(_avatarDescription);
			DefaultSkeleton();
			_expression = default(AvatarExpression);
		}

		public static bool Compare(AvatarDescription ad1, AvatarDescription ad2)
		{
			if (ad1.Description.Length != ad1.Description.Length)
			{
				return false;
			}
			for (int i = 0; i < ad1.Description.Length; i++)
			{
				if (ad1.Description[i] != ad2.Description[i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool Compare(byte[] ad1, byte[] ad2)
		{
			if (ad1.Length != ad1.Length)
			{
				return false;
			}
			for (int i = 0; i < ad1.Length; i++)
			{
				if (ad1[i] != ad2[i])
				{
					return false;
				}
			}
			return true;
		}

		protected virtual void OnDescriptionChanged()
		{
		}

		public void SetDescription(byte[] description)
		{
			if (!Compare(_avatarDescription.Description, description))
			{
				OnDescriptionChanged();
				_avatarDescription = new AvatarDescription(description);
				_avatarRenderer = new AvatarRenderer(_avatarDescription);
			}
		}

		public void SetDescription(AvatarDescription description)
		{
			if (!Compare(_avatarDescription, description))
			{
				OnDescriptionChanged();
				_avatarDescription = description;
				_avatarRenderer = new AvatarRenderer(_avatarDescription);
			}
		}

		private void DrawWireframeBones(GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
		{
			if (_wireFrameWorldTransforms == null)
			{
				_wireFrameWorldTransforms = new Matrix[Skeleton.Count];
			}
			for (int i = 0; i < _wireFrameWorldTransforms.Length; i++)
			{
				_wireFrameWorldTransforms[i] = Skeleton[i].Transform * DefaultBindPose[i];
			}
			_wireFrameWorldTransforms[0] *= base.LocalToWorld;
			for (int j = 1; j < _wireFrameWorldTransforms.Length; j++)
			{
				_wireFrameWorldTransforms[j] *= _wireFrameWorldTransforms[DefaultParentBones[j]];
			}
			if (_wireFrameVerts == null)
			{
				_wireFrameVerts = new VertexPositionColor[_wireFrameWorldTransforms.Length * 2];
			}
			_wireFrameVerts[0].Color = Color.Blue;
			_wireFrameVerts[0].Position = _wireFrameWorldTransforms[0].Translation;
			_wireFrameVerts[1] = _wireFrameVerts[0];
			for (int k = 2; k < _wireFrameWorldTransforms.Length * 2; k += 2)
			{
				_wireFrameVerts[k].Position = _wireFrameWorldTransforms[k / 2].Translation;
				_wireFrameVerts[k].Color = Color.Red;
				_wireFrameVerts[k + 1].Position = _wireFrameWorldTransforms[DefaultParentBones[k / 2]].Translation;
				_wireFrameVerts[k + 1].Color = Color.Green;
			}
			if (_wireFrameEffect == null)
			{
				_wireFrameEffect = new BasicEffect(graphicsDevice);
			}
			_wireFrameEffect.LightingEnabled = false;
			_wireFrameEffect.TextureEnabled = false;
			_wireFrameEffect.VertexColorEnabled = true;
			_wireFrameEffect.Projection = projection;
			_wireFrameEffect.View = view;
			_wireFrameEffect.World = Matrix.Identity;
			for (int l = 0; l < _wireFrameEffect.CurrentTechnique.Passes.Count; l++)
			{
				EffectPass effectPass = _wireFrameEffect.CurrentTechnique.Passes[l];
				effectPass.Apply();
				graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, _wireFrameVerts, 0, _wireFrameWorldTransforms.Length);
			}
		}
	}
}
