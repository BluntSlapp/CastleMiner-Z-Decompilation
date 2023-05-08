using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class RealToSpectralGroup : SignalProcessorGroup<RealPCMData, SpectralData>
	{
		private const int MAX_FRAME_LENGTH = 4096;

		private float[] gInFIFO = new float[4096];

		private float[] gOutFIFO = new float[4096];

		private float[] gFFTworksp = new float[8192];

		private float[] gLastPhase = new float[2049];

		private float[] gSumPhase = new float[2049];

		private float[] gOutputAccum = new float[8192];

		private int gRover;

		public int OverSampling = 2;

		public int FFTSize = 256;

		private float[] _windowVals;

		private ShortTimeFFT _fft = new ShortTimeFFT(1024);

		private SpectralData _specbuffer = new SpectralData(1, 1024);

		private void Alloc()
		{
			_specbuffer = new SpectralData(1, FFTSize / 2 + 1);
			_fft = new ShortTimeFFT(FFTSize);
			_windowVals = new float[FFTSize];
			for (int i = 0; i < FFTSize; i++)
			{
				_windowVals[i] = -0.5f * (float)Math.Cos(Math.PI * 2.0 * (double)(float)i / (double)(float)FFTSize) + 0.5f;
			}
		}

		public override bool ProcessBlock(RealPCMData data)
		{
			if (_windowVals == null)
			{
				Alloc();
			}
			int samples = data.Samples;
			int sampleRate = data.SampleRate;
			float[] data2 = data.GetData(0);
			int fFTSize = FFTSize;
			int overSampling = OverSampling;
			float num = (float)Math.PI;
			float num2 = (float)Math.PI * 2f;
			float[] dst = data2;
			int num3 = fFTSize / 2;
			int num4 = fFTSize / overSampling;
			float num5 = (float)sampleRate / (float)fFTSize;
			float num6 = (float)(Math.PI * 2.0 * (double)(float)num4 / (double)(float)fFTSize);
			int num7 = fFTSize - num4;
			if (gRover == 0)
			{
				gRover = num7;
			}
			int num8 = 0;
			int num9 = 0;
			FrequencyPair[] data3 = _specbuffer.GetData(0);
			while (num8 < samples)
			{
				int num10 = samples - num8;
				int num11 = fFTSize - gRover;
				if (num11 > num10)
				{
					num11 = num10;
				}
				Buffer.BlockCopy(data2, num8 * 4, gInFIFO, gRover * 4, num11 * 4);
				Buffer.BlockCopy(gOutFIFO, (gRover - num7) * 4, dst, num9 * 4, num11 * 4);
				num8 += num11;
				num9 += num11;
				gRover += num11;
				if (gRover < fFTSize)
				{
					continue;
				}
				gRover = num7;
				for (int i = 0; i < fFTSize; i++)
				{
					gFFTworksp[2 * i] = gInFIFO[i] * _windowVals[i];
					gFFTworksp[2 * i + 1] = 0f;
				}
				_fft.Transform(gFFTworksp, -1);
				for (int i = 0; i <= num3; i++)
				{
					float num12 = gFFTworksp[2 * i];
					float num13 = gFFTworksp[2 * i + 1];
					float num14 = 2f * (float)Math.Sqrt(num12 * num12 + num13 * num13);
					float num15 = (float)Math.Atan2(num13, num12);
					float num16 = num15 - gLastPhase[i];
					gLastPhase[i] = num15;
					num16 -= (float)i * num6;
					int num17 = (int)(num16 / num);
					num17 = ((num17 < 0) ? (num17 - (num17 & 1)) : (num17 + (num17 & 1)));
					num16 -= num * (float)num17;
					num16 = (float)overSampling * num16 / num2;
					num16 = (float)i * num5 + num16 * num5;
					data3[i].Magnitude = num14;
					data3[i].Value.Hertz = num16;
				}
				for (int j = 0; j < base.Count; j++)
				{
					SignalProcessor<SpectralData> signalProcessor = base[j];
					if (signalProcessor.Active)
					{
						signalProcessor.ProcessBlock(_specbuffer);
					}
				}
				for (int i = 0; i <= num3; i++)
				{
					float num14 = data3[i].Magnitude;
					float num16 = data3[i].Value.Hertz;
					num16 -= (float)i * num5;
					num16 /= num5;
					num16 = num2 * num16 / (float)overSampling;
					num16 += (float)i * num6;
					gSumPhase[i] += num16;
					float num15 = gSumPhase[i];
					gFFTworksp[2 * i] = num14 * (float)Math.Cos(num15);
					gFFTworksp[2 * i + 1] = num14 * (float)Math.Sin(num15);
				}
				for (int i = fFTSize + 2; i < 2 * fFTSize; i++)
				{
					gFFTworksp[i] = 0f;
				}
				_fft.Transform(gFFTworksp, 1);
				float num18 = 2f / (float)(num3 * overSampling);
				for (int i = 0; i < fFTSize; i++)
				{
					gOutputAccum[i] += num18 * _windowVals[i] * gFFTworksp[2 * i];
				}
				Buffer.BlockCopy(gOutputAccum, 0, gOutFIFO, 0, num4 * 4);
				Buffer.BlockCopy(gOutputAccum, num4 * 4, gOutputAccum, 0, fFTSize * 4);
				Buffer.BlockCopy(gInFIFO, num4 * 4, gInFIFO, 0, num7 * 4);
			}
			return true;
		}
	}
}
