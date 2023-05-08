using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DNA.CastleMinerZ.AI
{
	public abstract class EnemyType
	{
		public enum ModelNameEnum
		{
			ZOMBIE,
			SKELETONZOMBIE,
			SKELETONARCHER,
			SKELETONAXES,
			SKELETONSWORD,
			FELGUARD
		}

		public enum TextureNameEnum
		{
			ZOMBIE_0,
			ZOMBIE_1,
			ZOMBIE_2,
			ZOMBIE_3,
			ZOMBIE_4,
			ZOMBIE_5,
			ZOMBIE_6,
			SKELETON_0,
			SKELETON_1,
			SKELETON_2,
			SKELETON_3,
			SKELETON_4,
			FELGUARD,
			COUNT
		}

		public enum FoundInEnum
		{
			ABOVEGROUND,
			CAVES,
			HELL
		}

		public struct InitPackage
		{
			public float SlowSpeed;

			public float FastSpeed;

			public float EmergeSpeed;

			public float RunActivationTime;

			public float NormalActivationTime;

			public static InitPackage Read(BinaryReader reader)
			{
				InitPackage result = default(InitPackage);
				result.SlowSpeed = (float)reader.ReadInt32() / 1000f;
				result.FastSpeed = (float)reader.ReadInt32() / 1000f;
				result.EmergeSpeed = (float)reader.ReadInt32() / 1000f;
				result.RunActivationTime = (float)reader.ReadInt32() / 1000f;
				result.NormalActivationTime = (float)reader.ReadInt32() / 1000f;
				return result;
			}

			public void Write(BinaryWriter writer)
			{
				writer.Write((int)(1000f * SlowSpeed));
				writer.Write((int)(1000f * FastSpeed));
				writer.Write((int)(1000f * EmergeSpeed));
				writer.Write((int)(1000f * RunActivationTime));
				writer.Write((int)(1000f * NormalActivationTime));
			}
		}

		private const float DISTANCE_TO_HELL = 3400f;

		public static readonly string[] _modelNames = new string[6] { "Enemies\\Zombies\\zombieHigh", "Enemies\\Skeletons\\SkeletonZombie", "Enemies\\Skeletons\\SkeletonArcher", "Enemies\\Skeletons\\SkeletonAxes", "Enemies\\Skeletons\\SkeletonSword", "Enemies\\Demon\\Demon" };

		public static readonly float[] _modelScales = new float[6] { 1f, 1f, 1f, 1f, 1f, 1f };

		public static readonly float[] _modelFacing = new float[6] { 3.141592f, 3.141592f, 3.141592f, 3.141592f, 3.141592f, 3.141592f };

		public static readonly string[] _textureNames = new string[13]
		{
			"Enemies\\Zombies\\zombie-01_0", "Enemies\\Zombies\\zombie-06", "Enemies\\Zombies\\zombie-10", "Enemies\\Zombies\\zombie-11", "Enemies\\Zombies\\zombie-13", "Enemies\\Zombies\\zombie-14", "Enemies\\Zombies\\zombie-17", "Enemies\\Skeletons\\skele-01_0", "Enemies\\Skeletons\\skele-04", "Enemies\\Skeletons\\skele-05",
			"Enemies\\Skeletons\\skele-06", "Enemies\\Skeletons\\skele-07", "Enemies\\Demon\\demon_0"
		};

		public static EnemyType[] Types = null;

		private static int belowGroundSpawned = 0;

		public EnemyTypeEnum EType;

		public FoundInEnum FoundIn;

		public Texture2D EnemyTexture;

		public TextureNameEnum TextureIndex;

		public float Scale;

		public float Facing;

		public float ChanceOfBulletStrike;

		public string ModelName;

		public float StartingHealth;

		public int SpawnRadius;

		public float AttackAnimationSpeed = 1f;

		public float DieAnimationSpeed = 1f;

		public float HitAnimationSpeed = 1f;

		public float SpawnAnimationSpeed = 1f;

		public bool HasRunFast;

		public float FastJumpSpeed = 10f;

		public float BaseSlowSpeed = 2f;

		public float RandomSlowSpeed = 3.5f;

		public float BaseFastSpeed = 6f;

		public float BaseRunActivationTime = 1000f;

		public float RandomRunActivationTime;

		public float BaseNormalActivationTime = 1000f;

		public static EnemyType GetEnemyType(EnemyTypeEnum et)
		{
			return Types[(int)et];
		}

		public static void Init()
		{
			if (Types != null)
			{
				return;
			}
			Types = new EnemyType[51]
			{
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_0_0, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_0, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_0_1, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_1, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_0_2, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_2, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_0_3, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_3, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_0_4, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_4, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_0_5, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_5, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_0_6, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_6, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_1_0, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_0, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_1_1, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_1, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_1_2, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_2, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_1_3, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_3, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_1_4, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_4, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_1_5, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_5, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_0_6, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_6, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_2_0, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_0, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_2_1, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_1, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_2_3, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_2, FoundInEnum.ABOVEGROUND),
				new ZombieEnemyType(EnemyTypeEnum.ZOMBIE_2_4, ModelNameEnum.ZOMBIE, TextureNameEnum.ZOMBIE_3, FoundInEnum.ABOVEGROUND),
				new ArcherSkeletonEnemyType(EnemyTypeEnum.ARCHER_0_0, ModelNameEnum.SKELETONARCHER, TextureNameEnum.SKELETON_0, FoundInEnum.ABOVEGROUND, SkeletonClassEnum.ARCHER),
				new ArcherSkeletonEnemyType(EnemyTypeEnum.ARCHER_0_1, ModelNameEnum.SKELETONARCHER, TextureNameEnum.SKELETON_1, FoundInEnum.ABOVEGROUND, SkeletonClassEnum.ARCHER),
				new ArcherSkeletonEnemyType(EnemyTypeEnum.ARCHER_0_2, ModelNameEnum.SKELETONARCHER, TextureNameEnum.SKELETON_2, FoundInEnum.ABOVEGROUND, SkeletonClassEnum.ARCHER),
				new ArcherSkeletonEnemyType(EnemyTypeEnum.ARCHER_0_3, ModelNameEnum.SKELETONARCHER, TextureNameEnum.SKELETON_3, FoundInEnum.ABOVEGROUND, SkeletonClassEnum.ARCHER),
				new ArcherSkeletonEnemyType(EnemyTypeEnum.ARCHER_0_4, ModelNameEnum.SKELETONARCHER, TextureNameEnum.SKELETON_4, FoundInEnum.ABOVEGROUND, SkeletonClassEnum.ARCHER),
				new ArcherSkeletonEnemyType(EnemyTypeEnum.ARCHER_1_0, ModelNameEnum.SKELETONARCHER, TextureNameEnum.SKELETON_0, FoundInEnum.ABOVEGROUND, SkeletonClassEnum.ARCHER),
				new ArcherSkeletonEnemyType(EnemyTypeEnum.ARCHER_1_1, ModelNameEnum.SKELETONARCHER, TextureNameEnum.SKELETON_1, FoundInEnum.ABOVEGROUND, SkeletonClassEnum.ARCHER),
				new ArcherSkeletonEnemyType(EnemyTypeEnum.ARCHER_1_2, ModelNameEnum.SKELETONARCHER, TextureNameEnum.SKELETON_2, FoundInEnum.ABOVEGROUND, SkeletonClassEnum.ARCHER),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_0_0, ModelNameEnum.SKELETONZOMBIE, TextureNameEnum.SKELETON_0, FoundInEnum.CAVES, SkeletonClassEnum.ZOMBIE),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_0_1, ModelNameEnum.SKELETONZOMBIE, TextureNameEnum.SKELETON_1, FoundInEnum.CAVES, SkeletonClassEnum.ZOMBIE),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_0_2, ModelNameEnum.SKELETONZOMBIE, TextureNameEnum.SKELETON_2, FoundInEnum.CAVES, SkeletonClassEnum.ZOMBIE),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_0_3, ModelNameEnum.SKELETONZOMBIE, TextureNameEnum.SKELETON_3, FoundInEnum.CAVES, SkeletonClassEnum.ZOMBIE),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_0_4, ModelNameEnum.SKELETONZOMBIE, TextureNameEnum.SKELETON_4, FoundInEnum.CAVES, SkeletonClassEnum.ZOMBIE),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_SWORD_0_0, ModelNameEnum.SKELETONSWORD, TextureNameEnum.SKELETON_0, FoundInEnum.CAVES, SkeletonClassEnum.SWORD),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_1_0, ModelNameEnum.SKELETONZOMBIE, TextureNameEnum.SKELETON_1, FoundInEnum.CAVES, SkeletonClassEnum.ZOMBIE),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_SWORD_0_1, ModelNameEnum.SKELETONSWORD, TextureNameEnum.SKELETON_2, FoundInEnum.CAVES, SkeletonClassEnum.SWORD),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_1_1, ModelNameEnum.SKELETONZOMBIE, TextureNameEnum.SKELETON_3, FoundInEnum.CAVES, SkeletonClassEnum.ZOMBIE),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_SWORD_0_2, ModelNameEnum.SKELETONSWORD, TextureNameEnum.SKELETON_4, FoundInEnum.CAVES, SkeletonClassEnum.SWORD),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_1_2, ModelNameEnum.SKELETONZOMBIE, TextureNameEnum.SKELETON_0, FoundInEnum.CAVES, SkeletonClassEnum.ZOMBIE),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_SWORD_0_3, ModelNameEnum.SKELETONSWORD, TextureNameEnum.SKELETON_1, FoundInEnum.CAVES, SkeletonClassEnum.SWORD),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_AXES_0_0, ModelNameEnum.SKELETONAXES, TextureNameEnum.SKELETON_2, FoundInEnum.CAVES, SkeletonClassEnum.AXES),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_SWORD_0_4, ModelNameEnum.SKELETONSWORD, TextureNameEnum.SKELETON_3, FoundInEnum.CAVES, SkeletonClassEnum.SWORD),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_AXES_0_1, ModelNameEnum.SKELETONAXES, TextureNameEnum.SKELETON_4, FoundInEnum.CAVES, SkeletonClassEnum.AXES),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_SWORD_1_0, ModelNameEnum.SKELETONSWORD, TextureNameEnum.SKELETON_0, FoundInEnum.CAVES, SkeletonClassEnum.SWORD),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_AXES_0_2, ModelNameEnum.SKELETONAXES, TextureNameEnum.SKELETON_1, FoundInEnum.CAVES, SkeletonClassEnum.AXES),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_SWORD_1_1, ModelNameEnum.SKELETONSWORD, TextureNameEnum.SKELETON_2, FoundInEnum.CAVES, SkeletonClassEnum.SWORD),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_AXES_0_3, ModelNameEnum.SKELETONAXES, TextureNameEnum.SKELETON_3, FoundInEnum.CAVES, SkeletonClassEnum.AXES),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_SWORD_1_2, ModelNameEnum.SKELETONSWORD, TextureNameEnum.SKELETON_4, FoundInEnum.CAVES, SkeletonClassEnum.SWORD),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_AXES_0_4, ModelNameEnum.SKELETONAXES, TextureNameEnum.SKELETON_0, FoundInEnum.CAVES, SkeletonClassEnum.AXES),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_AXES_1_0, ModelNameEnum.SKELETONAXES, TextureNameEnum.SKELETON_1, FoundInEnum.CAVES, SkeletonClassEnum.AXES),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_AXES_1_1, ModelNameEnum.SKELETONAXES, TextureNameEnum.SKELETON_2, FoundInEnum.CAVES, SkeletonClassEnum.AXES),
				new SkeletonEnemyType(EnemyTypeEnum.SKEL_AXES_1_2, ModelNameEnum.SKELETONAXES, TextureNameEnum.SKELETON_3, FoundInEnum.CAVES, SkeletonClassEnum.AXES),
				new FelguardEnemyType()
			};
			for (int i = 0; i < _modelNames.Length; i++)
			{
				CastleMinerZGame.Instance.Content.Load<Model>(_modelNames[i]);
			}
			float num = 1f;
			for (EnemyTypeEnum enemyTypeEnum = EnemyTypeEnum.ZOMBIE_0_0; enemyTypeEnum <= EnemyTypeEnum.ZOMBIE_2_4; enemyTypeEnum++)
			{
				float num2 = (float)enemyTypeEnum / 16f;
				if (enemyTypeEnum == EnemyTypeEnum.ZOMBIE_2_4)
				{
					Types[(int)enemyTypeEnum].StartingHealth = num * 1.5f;
				}
				else
				{
					Types[(int)enemyTypeEnum].StartingHealth = num;
				}
				Types[(int)enemyTypeEnum].BaseSlowSpeed = 0.5f + num2 * 1.5f;
				Types[(int)enemyTypeEnum].BaseFastSpeed = 6.5f + num2 * 1.5f;
				Types[(int)enemyTypeEnum].BaseRunActivationTime = 4f - num2;
				Types[(int)enemyTypeEnum].RandomRunActivationTime = 1f;
				Types[(int)enemyTypeEnum].FastJumpSpeed = 13f + 3f * num2;
				num += 1f;
			}
			num = 1f;
			for (EnemyTypeEnum enemyTypeEnum2 = EnemyTypeEnum.ARCHER_0_0; enemyTypeEnum2 <= EnemyTypeEnum.ARCHER_1_2; enemyTypeEnum2++)
			{
				Types[(int)enemyTypeEnum2].StartingHealth = num;
				num += 2.5f;
			}
			num = 1f;
			for (EnemyTypeEnum enemyTypeEnum3 = EnemyTypeEnum.SKEL_0_0; enemyTypeEnum3 <= EnemyTypeEnum.SKEL_AXES_1_2; enemyTypeEnum3++)
			{
				Types[(int)enemyTypeEnum3].StartingHealth = num;
				num += 0.9f;
			}
			Types[50].StartingHealth = num * 7f;
		}

		private static EnemyTypeEnum FindEnemy(float dstep, float distance, EnemyTypeEnum firstEnemy, EnemyTypeEnum lastEnemy)
		{
			int num = lastEnemy - firstEnemy;
			float num2 = distance / dstep;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			int num6 = (int)Math.Floor(num2);
			int num7 = num6 - 1;
			int num8 = num7 - 1;
			float num9 = num2 - (float)num6;
			if (num6 > num)
			{
				num6 = num;
				num7 = num - 1;
				num8 = num - 2;
				num9 = 1f;
				num3 = 1f;
				num4 = 1f;
				num5 = 0.5f;
			}
			else
			{
				num3 = (float)Math.Sin(num9 * ((float)Math.PI / 2f) / 3f);
				if (num7 >= 0)
				{
					num4 = (float)Math.Sin((1f + num9) * ((float)Math.PI / 2f) / 3f);
				}
				if (num8 >= 0)
				{
					num5 = (float)Math.Sin((2f + num9) * ((float)Math.PI / 2f) / 3f);
				}
			}
			float num10 = MathTools.RandomFloat(num3 + num4 + num5);
			int num11 = ((num10 <= num3) ? num6 : ((!(num10 <= num3 + num4)) ? num8 : num7));
			return num11 + firstEnemy;
		}

		public static EnemyTypeEnum GetZombie(float distance)
		{
			return FindEnemy(188.888885f, distance, EnemyTypeEnum.ZOMBIE_0_0, EnemyTypeEnum.ZOMBIE_2_4);
		}

		public static EnemyTypeEnum GetAbovegroundEnemy(float percentMidnight, float distance)
		{
			float num = (float)Math.Pow(1f - percentMidnight, 4.0);
			if (MathTools.RandomFloat() < num)
			{
				return FindEnemy(425f, distance, EnemyTypeEnum.ARCHER_0_0, EnemyTypeEnum.ARCHER_1_2);
			}
			return FindEnemy(188.888885f, distance, EnemyTypeEnum.ZOMBIE_0_0, EnemyTypeEnum.ZOMBIE_2_4);
		}

		public static EnemyTypeEnum GetBelowgroundEnemy(float depth, float distance)
		{
			float num = 141.666672f;
			distance += depth * 2f * num / 50f;
			if (belowGroundSpawned > 20)
			{
				EnemyTypeEnum enemyTypeEnum = FindEnemy(num, distance, EnemyTypeEnum.SKEL_0_0, EnemyTypeEnum.FELGUARD);
				if (enemyTypeEnum == EnemyTypeEnum.FELGUARD)
				{
					belowGroundSpawned = 0;
				}
				return enemyTypeEnum;
			}
			belowGroundSpawned++;
			return FindEnemy(num, distance, EnemyTypeEnum.SKEL_0_0, EnemyTypeEnum.SKEL_AXES_1_2);
		}

		public abstract float GetDamageTypeMultiplier(DamageType damageType, bool headShot);

		public abstract IFSMState<BaseZombie> GetEmergeState(BaseZombie entity);

		public abstract IFSMState<BaseZombie> GetAttackState(BaseZombie entity);

		public abstract IFSMState<BaseZombie> GetGiveUpState(BaseZombie entity);

		public abstract IFSMState<BaseZombie> GetHitState(BaseZombie entity);

		public abstract IFSMState<BaseZombie> GetDieState(BaseZombie entity);

		public virtual IFSMState<BaseZombie> GetRestartState(BaseZombie entity)
		{
			return GetChaseState(entity);
		}

		public virtual float GetMaxSpeed()
		{
			return 2f;
		}

		public virtual float GetSlowSpeed()
		{
			return BaseSlowSpeed;
		}

		public virtual float GetFastSpeed()
		{
			return BaseFastSpeed;
		}

		public virtual IFSMState<BaseZombie> GetChaseState(BaseZombie entity)
		{
			return EnemyStates.Chase;
		}

		public EnemyType(EnemyTypeEnum t, ModelNameEnum model, TextureNameEnum tname, FoundInEnum foundin)
		{
			EType = t;
			ModelName = _modelNames[(int)model];
			Scale = _modelScales[(int)model];
			Facing = _modelFacing[(int)model];
			FoundIn = foundin;
			EnemyTexture = CastleMinerZGame.Instance.Content.Load<Texture2D>(_textureNames[(int)tname]);
			SpawnRadius = 15;
			TextureIndex = tname;
		}

		public InitPackage CreateInitPackage(float midnight)
		{
			InitPackage result = default(InitPackage);
			result.SlowSpeed = MathTools.RandomFloat(BaseSlowSpeed, BaseSlowSpeed + RandomSlowSpeed);
			if (HasRunFast)
			{
				result.FastSpeed = MathTools.RandomFloat(BaseFastSpeed - 0.25f, BaseFastSpeed + 0.25f);
				result.RunActivationTime = MathHelper.Lerp(BaseRunActivationTime, BaseRunActivationTime * 0.5f, MathTools.RandomFloat(midnight));
				result.NormalActivationTime = 45f;
				if (midnight > 0.8f)
				{
					result.EmergeSpeed = SpawnAnimationSpeed;
				}
				else if (SpawnAnimationSpeed == 1f)
				{
					result.EmergeSpeed = 1f;
				}
				else
				{
					result.EmergeSpeed = MathHelper.Lerp(SpawnAnimationSpeed / 2f, SpawnAnimationSpeed, Math.Min(1f, midnight * 2f));
				}
			}
			else
			{
				result.FastSpeed = 0f;
				result.RunActivationTime = 0f;
				result.NormalActivationTime = 0f;
				result.EmergeSpeed = SpawnAnimationSpeed;
			}
			return result;
		}
	}
}
