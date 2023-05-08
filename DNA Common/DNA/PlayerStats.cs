using System;
using System.IO;

namespace DNA
{
	public abstract class PlayerStats
	{
		private const int FileIdent = 1095783254;

		public string GamerTag;

		public DateTime DateRecorded;

		public abstract int Version { get; }

		public void Save(BinaryWriter writer)
		{
			writer.Write(1095783254);
			writer.Write(Version);
			writer.Write(GamerTag);
			writer.Write(DateRecorded.Ticks);
			SaveData(writer);
		}

		protected abstract void SaveData(BinaryWriter writer);

		public void Load(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num != 1095783254)
			{
				throw new Exception();
			}
			int version = reader.ReadInt32();
			GamerTag = reader.ReadString();
			DateRecorded = new DateTime(reader.ReadInt64());
			LoadData(reader, version);
		}

		protected abstract void LoadData(BinaryReader reader, int version);
	}
}
