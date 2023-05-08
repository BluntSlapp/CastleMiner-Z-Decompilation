using System;
using System.IO;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class MemoryRecordProcessor : SignalProcessor<RawPCMData>
	{
		private int RecordMax = 10000000;

		private bool recording;

		private MemoryStream _recordStream = new MemoryStream();

		public float RecordBufferFull
		{
			get
			{
				return (float)_recordStream.Length / (float)RecordMax;
			}
		}

		public int BytesRecorded
		{
			get
			{
				return (int)_recordStream.Position;
			}
		}

		public bool Recording
		{
			get
			{
				return recording;
			}
		}

		public void StartRecord()
		{
			if (recording)
			{
				throw new Exception("Already Recording");
			}
			recording = true;
			_recordStream.Position = 0L;
			_recordStream.SetLength(0L);
		}

		public void EndRecord()
		{
			if (!recording)
			{
				throw new Exception("Not Recording");
			}
			recording = false;
		}

		public byte[] GetData()
		{
			return _recordStream.ToArray();
		}

		public MemoryRecordProcessor(int MaxSize)
		{
			RecordMax = MaxSize;
		}

		public override bool ProcessBlock(RawPCMData data)
		{
			if (recording && RecordBufferFull < 1f)
			{
				_recordStream.Write(data.ChannelData, 0, data.ChannelData.Length);
			}
			return true;
		}
	}
}
