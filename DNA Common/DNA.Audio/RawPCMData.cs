using System;
using System.Collections.Generic;
using System.IO;

namespace DNA.Audio
{
	public class RawPCMData
	{
		private enum WavCompression
		{
			Unknown = 0,
			PCM = 1,
			MicrosoftADPCM = 2,
			ITUG711alaw = 6,
			ITUG711Alaw = 7,
			IMAADPCM = 17,
			ITUG723ADPCM = 20,
			GSM610 = 49,
			ITUG721ADPCM = 64,
			MPEG = 80
		}

		private byte[] _channelData = new byte[0];

		public byte[] ChannelData
		{
			get
			{
				return _channelData;
			}
		}

		public int Channels { get; private set; }

		public int SampleRate { get; private set; }

		public int BitsPerSample { get; private set; }

		public int Samples
		{
			get
			{
				return _channelData.Length / (Channels * (BitsPerSample >> 3));
			}
		}

		public TimeSpan Time
		{
			get
			{
				return TimeSpan.FromSeconds((double)Samples / (double)SampleRate);
			}
		}

		public RawPCMData()
		{
			Channels = 1;
		}

		public RawPCMData(int channels, int samples, int sampleRate, int bitsPerSample)
		{
			Channels = channels;
			SampleRate = sampleRate;
			BitsPerSample = bitsPerSample;
			_channelData = new byte[channels * samples * (BitsPerSample >> 3)];
		}

		public RawPCMData(int channels, int sampleRate, int bitsPerSample, byte[] channelData)
		{
			Channels = channels;
			SampleRate = sampleRate;
			BitsPerSample = bitsPerSample;
			_channelData = channelData;
		}

		public RawPCMData(RealPCMData data, int bitsPerSample)
		{
			Convert(data, bitsPerSample);
		}

		public void Alloc(int channels, int samples)
		{
			_channelData = new byte[channels * samples * (BitsPerSample >> 3)];
		}

		public void Convert(RealPCMData data, int bitsPerSample)
		{
			if (data.Channels != Channels || data.Samples != data.Samples || BitsPerSample != bitsPerSample)
			{
				BitsPerSample = bitsPerSample;
				Alloc(data.Channels, data.Samples);
			}
			int bitsPerSample2 = BitsPerSample;
			if (bitsPerSample2 == 16)
			{
				for (int i = 0; i < Channels; i++)
				{
					int num = 2;
					int num2 = Channels * num;
					float[] data2 = data.GetData(i);
					int num3 = num * i;
					foreach (float num4 in data2)
					{
						short num5 = (short)(num4 * 32768f);
						_channelData[num3 + 1] = (byte)((uint)num5 & 0xFFu);
						_channelData[num3] = (byte)(num5 >> 8);
						num3 += num2;
					}
				}
				return;
			}
			throw new NotImplementedException();
		}

		private void LoadWavInternal(Stream stream)
		{
			LoadWav(new BinaryReader(stream));
		}

		public static RawPCMData LoadWav(string path)
		{
			using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return LoadWav(stream);
			}
		}

		public static RawPCMData LoadWav(Stream stream)
		{
			RawPCMData rawPCMData = new RawPCMData();
			rawPCMData.LoadWavInternal(stream);
			return rawPCMData;
		}

		private int LoadWav(BinaryReader reader)
		{
			_channelData = null;
			if (reader.ReadUInt32() != 1179011410)
			{
				throw new Exception("Not a valid Wav File");
			}
			int num = reader.ReadInt32();
			long position = reader.BaseStream.Position;
			if (reader.ReadUInt32() != 1163280727)
			{
				throw new Exception("Not a valid Wav File");
			}
			long num2 = position + num;
			new List<List<float>>();
			while (reader.BaseStream.Position < num2)
			{
				uint num3 = reader.ReadUInt32();
				int num4 = reader.ReadInt32();
				long position2 = reader.BaseStream.Position;
				long num5 = position2 + num4;
				switch (num3)
				{
				case 544501094u:
				{
					WavCompression wavCompression = (WavCompression)reader.ReadUInt16();
					reader.ReadUInt16();
					SampleRate = reader.ReadInt32();
					reader.ReadInt32();
					reader.ReadUInt16();
					BitsPerSample = reader.ReadUInt16();
					if (wavCompression != WavCompression.PCM)
					{
						throw new Exception("Unsupported Wav Compression " + wavCompression);
					}
					break;
				}
				case 1953393779u:
					throw new Exception("Silence Chunks not Supported");
				case 1635017060u:
					if (_channelData != null)
					{
						throw new Exception("Mutiple Wav Chunks Not Supported");
					}
					_channelData = reader.ReadBytes(num4);
					if (BitsPerSample == 16)
					{
						for (int i = 0; i < _channelData.Length; i += 2)
						{
							byte b = _channelData[i];
							_channelData[i] = _channelData[i + 1];
							_channelData[i + 1] = b;
						}
					}
					break;
				}
				if (reader.BaseStream.Position < num5)
				{
					reader.BaseStream.Position = num5;
				}
			}
			return BitsPerSample;
		}

		public void SaveWav(string path)
		{
			using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				SaveWav(stream);
			}
		}

		public void SaveWav(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			SaveWav(writer);
		}

		public void SaveWav(BinaryWriter writer)
		{
			writer.Write(1179011410u);
			int num = BitsPerSample * Channels >> 3;
			int num2 = num * Samples;
			writer.Write(36 + num2);
			writer.Write(1163280727u);
			writer.Write(544501094u);
			writer.Write(16);
			writer.Write((short)1);
			writer.Write((short)Channels);
			writer.Write(SampleRate);
			writer.Write(SampleRate * num);
			writer.Write((short)num);
			writer.Write((short)BitsPerSample);
			writer.Write(1635017060u);
			writer.Write(num2);
			int sample = Samples;
			int channel = Channels;
			byte[] array;
			if (BitsPerSample == 16)
			{
				array = new byte[_channelData.Length];
				for (int i = 0; i < array.Length; i += 2)
				{
					byte b = array[i];
					array[i] = array[i + 1];
					array[i + 1] = b;
				}
			}
			else
			{
				array = _channelData;
			}
			writer.Write(array);
		}
	}
}
