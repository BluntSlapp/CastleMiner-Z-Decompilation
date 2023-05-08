using System;
using System.IO;
using DNA.Collections;

namespace DNA.Audio
{
	public class RealPCMData
	{
		private float[][] _channelData = new float[1][] { new float[0] };

		public int SampleRate { get; private set; }

		public int Channels
		{
			get
			{
				return _channelData.Length;
			}
		}

		public int Samples
		{
			get
			{
				return _channelData[0].Length;
			}
		}

		public TimeSpan Time
		{
			get
			{
				return TimeSpan.FromSeconds((double)Samples / (double)SampleRate);
			}
		}

		public float[] GetData(int channel)
		{
			return _channelData[channel];
		}

		public void Convert(RawPCMData data)
		{
			if (data.Samples != Samples || Channels != data.Channels)
			{
				Alloc(Channels, data.Samples);
			}
			SampleRate = data.SampleRate;
			int bitsPerSample = data.BitsPerSample;
			if (bitsPerSample == 16)
			{
				int num = 2 * Channels;
				byte[] channelData = data.ChannelData;
				for (int i = 0; i < data.Channels; i++)
				{
					float[] array = _channelData[i];
					int num2 = 0;
					for (int j = i * 2; j < data.ChannelData.Length; j += num)
					{
						short num3 = channelData[j + 1];
						short num4 = (short)(channelData[j] << 8);
						short num5 = (short)(num3 | num4);
						float num6 = (float)num5 * 3.05175781E-05f;
						array[num2++] = num6;
					}
				}
				return;
			}
			throw new NotImplementedException();
		}

		private RealPCMData()
		{
		}

		public void Alloc(int channels, int samples)
		{
			_channelData = ArrayTools.AllocSquareJaggedArray<float>(channels, samples);
		}

		public RealPCMData(int channels, int samples, int sampleRate)
		{
			SampleRate = sampleRate;
			Alloc(channels, samples);
		}

		public void CombineChannels()
		{
			if (Channels == 1)
			{
				return;
			}
			float[][] array = ArrayTools.AllocSquareJaggedArray<float>(1, Samples);
			int samples = Samples;
			int channels = Channels;
			for (int i = 0; i < samples; i++)
			{
				array[0][i] = 0f;
				for (int j = 0; j < Channels; j++)
				{
					array[0][i] += _channelData[j][i];
				}
				array[0][i] /= channels;
			}
			_channelData = array;
		}

		public void AdjustVolume(float modifier)
		{
			int samples = Samples;
			for (int i = 0; i < Channels; i++)
			{
				for (int j = 0; j < samples; j++)
				{
					_channelData[i][j] *= modifier;
				}
			}
		}

		public void TrimEndSilence(float threshold)
		{
			int num = 0;
			int length = _channelData.GetLength(1);
			for (int i = 0; i < Channels; i++)
			{
				for (int num2 = length - 1; num2 > num; num2--)
				{
					if (Math.Abs(_channelData[i][num2]) > threshold)
					{
						num = num2;
						break;
					}
				}
			}
			if (num == 0)
			{
				return;
			}
			float[][] array = ArrayTools.AllocSquareJaggedArray<float>(Channels, num);
			for (int j = 0; j < Channels; j++)
			{
				for (int k = 0; k < num; k++)
				{
					array[j][k] = _channelData[j][k];
				}
			}
			_channelData = array;
		}

		public void TrimBeginSilence(float threshold)
		{
			int num = Samples;
			_channelData.GetLength(1);
			for (int i = 0; i < Channels; i++)
			{
				for (int j = 0; j < num; j++)
				{
					if (Math.Abs(_channelData[i][j]) > threshold)
					{
						num = j;
						break;
					}
				}
			}
			int num2 = Samples - num;
			float[][] array = ArrayTools.AllocSquareJaggedArray<float>(Channels, num2);
			for (int k = 0; k < Channels; k++)
			{
				for (int l = 0; l < num2; l++)
				{
					array[k][l] = _channelData[k][l + num];
				}
			}
			_channelData = array;
		}

		public void AdjustSpeed(float modifier)
		{
			SampleRate = (int)Math.Ceiling((float)SampleRate * modifier);
		}

		public void Resample(int newSampleRate)
		{
			float num3 = 1f / (float)newSampleRate;
			float num4 = 1f / (float)SampleRate;
			int samples = Samples;
			int num = (int)((long)Samples * (long)newSampleRate / SampleRate);
			float[][] array = ArrayTools.AllocSquareJaggedArray<float>(Channels, num);
			for (int i = 0; i < Channels; i++)
			{
				for (int j = 0; j < num; j++)
				{
					int num2 = (int)Math.Floor((float)j * (float)(samples - 1) / (float)num);
					array[i][j] = _channelData[i][num2];
				}
			}
			_channelData = array;
			SampleRate = newSampleRate;
		}

		public float GetAverageAmplitude(int sampleStart, int sampleEnd)
		{
			float num = 0f;
			for (int i = 0; i < Channels; i++)
			{
				for (int j = sampleStart; j <= sampleEnd; j++)
				{
					num += Math.Abs(_channelData[i][j]);
				}
			}
			return num / (float)(sampleEnd - sampleStart + 1);
		}

		public float GetMaxAmplitude(int sampleStart, int sampleEnd)
		{
			float num = float.MinValue;
			for (int i = 0; i < Channels; i++)
			{
				for (int j = sampleStart; j <= sampleEnd; j++)
				{
					float val = Math.Abs(_channelData[i][j]);
					num = Math.Max(num, val);
				}
			}
			return num;
		}

		public float GetMaxAmplitude()
		{
			return GetMaxAmplitude(0, Samples - 1);
		}

		public void Normalize()
		{
			float num = GetMaxAmplitude();
			if (num == 0f)
			{
				num = 1f;
			}
			AdjustVolume(1f / num);
		}

		private void LoadWavInternal(Stream stream)
		{
			RawPCMData data = RawPCMData.LoadWav(stream);
			Convert(data);
		}

		public static RealPCMData LoadWav(string path)
		{
			using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return LoadWav(stream);
			}
		}

		public static RealPCMData LoadWav(Stream stream)
		{
			RealPCMData realPCMData = new RealPCMData();
			realPCMData.LoadWavInternal(stream);
			return realPCMData;
		}

		public void SaveWav(string path, int BitsPerSample)
		{
			using (FileStream stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				SaveWav(stream, BitsPerSample);
			}
		}

		public void SaveWav(Stream stream, int BitsPerSample)
		{
			new BinaryWriter(stream);
			RawPCMData rawPCMData = new RawPCMData();
			rawPCMData.Convert(this, BitsPerSample);
			rawPCMData.SaveWav(stream);
		}
	}
}
