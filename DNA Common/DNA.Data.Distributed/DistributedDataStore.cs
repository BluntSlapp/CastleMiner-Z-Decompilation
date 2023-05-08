using System;
using System.Collections.Generic;
using System.IO;

namespace DNA.Data.Distributed
{
	public class DistributedDataStore
	{
		private Dictionary<Guid, DistributedRecord> _records = new Dictionary<Guid, DistributedRecord>();

		public void Set(DistributedRecord record)
		{
			_records[record.ID] = record;
		}

		public void Remove(DistributedRecord record)
		{
			_records.Remove(record.ID);
		}

		public void Remove(Guid id)
		{
			_records.Remove(id);
		}

		public void Commit(Stream stream, string user)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(_records.Count);
			MemoryStream memoryStream = new MemoryStream();
			foreach (KeyValuePair<Guid, DistributedRecord> record in _records)
			{
				memoryStream.Position = 0L;
				DistributedRecord value = record.Value;
				binaryWriter.Write(value.RecordTypeID);
				value.Serialize(memoryStream, user);
				byte[] buffer = memoryStream.GetBuffer();
				binaryWriter.Write(buffer, 0, (int)memoryStream.Position);
			}
		}
	}
}
