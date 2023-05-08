using System;
using System.Collections.Generic;
using System.IO;
using DNA.Reflection;
using DNA.Security.Cryptography;

namespace DNA.Data.Distributed
{
	public abstract class DistributedRecord
	{
		private IHashProvider hasher = new MD5HashProvider();

		private Guid _id = Guid.NewGuid();

		private Hash _hash;

		private string Name;

		private static Type[] _recordTypes;

		private static Dictionary<Type, int> _recordIDs;

		public int RecordTypeID
		{
			get
			{
				if (_recordIDs == null)
				{
					PopulateMessageTypes();
				}
				return _recordIDs[GetType()];
			}
		}

		public Guid ID
		{
			get
			{
				return _id;
			}
		}

		protected abstract void SerializeData(Stream stream);

		protected abstract void DeserializeData(Stream stream);

		public void Serialize(Stream stream, string user)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			MemoryStream memoryStream = new MemoryStream();
			SerializeData(memoryStream);
			Hash hash = hasher.Compute(memoryStream.GetBuffer(), 0L, memoryStream.Position);
			bool flag = hash != _hash;
			_hash = hash;
			binaryWriter.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
			binaryWriter.Write(_hash.Data.Length);
			binaryWriter.Write(_hash.Data);
			binaryWriter.Write(Name);
		}

		public DistributedRecord(string name, string creator)
		{
			Name = name;
		}

		private static bool TypeFilter(Type type)
		{
			if (type.IsSubclassOf(typeof(DistributedRecord)))
			{
				return !type.IsAbstract;
			}
			return false;
		}

		private static void PopulateMessageTypes()
		{
			_recordTypes = ReflectionTools.GetTypes(TypeFilter);
			_recordIDs = new Dictionary<Type, int>();
			for (int i = 0; i < _recordTypes.Length; i++)
			{
				_recordIDs[_recordTypes[i]] = i;
			}
		}
	}
}
