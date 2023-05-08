using System.IO;
using DNA.IO;
using Microsoft.Xna.Framework;

namespace DNA.CastleMinerZ.AI
{
	public class DragonHostMigrationInfo
	{
		public float NextDragonTime;

		public float Roll;

		public float TargetRoll;

		public float Pitch;

		public float TargetPitch;

		public float Yaw;

		public float TargetYaw;

		public float Velocity;

		public float TargetVelocity;

		public float DefaultHeading;

		public float FlapDebt;

		public float NextUpdateTime;

		public int NextFireballIndex;

		public bool ForBiome;

		public DragonTypeEnum EType;

		public DragonAnimEnum Animation;

		public Vector3 Position;

		public Vector3 Target;

		public void Write(BinaryWriter writer)
		{
			writer.Write(NextDragonTime);
			writer.Write(Roll);
			writer.Write(TargetRoll);
			writer.Write(Pitch);
			writer.Write(TargetPitch);
			writer.Write(Yaw);
			writer.Write(TargetYaw);
			writer.Write(Velocity);
			writer.Write(TargetVelocity);
			writer.Write(DefaultHeading);
			writer.Write(FlapDebt);
			writer.Write(NextFireballIndex);
			writer.Write(ForBiome);
			writer.Write(NextUpdateTime);
			writer.Write((byte)Animation);
			writer.Write((byte)EType);
			writer.Write(Position);
			writer.Write(Target);
		}

		public static DragonHostMigrationInfo Read(BinaryReader reader)
		{
			DragonHostMigrationInfo dragonHostMigrationInfo = new DragonHostMigrationInfo();
			dragonHostMigrationInfo.NextDragonTime = reader.ReadSingle();
			dragonHostMigrationInfo.Roll = reader.ReadSingle();
			dragonHostMigrationInfo.TargetRoll = reader.ReadSingle();
			dragonHostMigrationInfo.Pitch = reader.ReadSingle();
			dragonHostMigrationInfo.TargetPitch = reader.ReadSingle();
			dragonHostMigrationInfo.Yaw = reader.ReadSingle();
			dragonHostMigrationInfo.TargetYaw = reader.ReadSingle();
			dragonHostMigrationInfo.Velocity = reader.ReadSingle();
			dragonHostMigrationInfo.TargetVelocity = reader.ReadSingle();
			dragonHostMigrationInfo.DefaultHeading = reader.ReadSingle();
			dragonHostMigrationInfo.FlapDebt = reader.ReadSingle();
			dragonHostMigrationInfo.NextFireballIndex = reader.ReadInt32();
			dragonHostMigrationInfo.ForBiome = reader.ReadBoolean();
			dragonHostMigrationInfo.NextUpdateTime = reader.ReadSingle();
			dragonHostMigrationInfo.Animation = (DragonAnimEnum)reader.ReadByte();
			dragonHostMigrationInfo.EType = (DragonTypeEnum)reader.ReadByte();
			dragonHostMigrationInfo.Position = reader.ReadVector3();
			dragonHostMigrationInfo.Target = reader.ReadVector3();
			return dragonHostMigrationInfo;
		}
	}
}
