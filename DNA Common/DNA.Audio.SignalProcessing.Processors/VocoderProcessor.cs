using System;

namespace DNA.Audio.SignalProcessing.Processors
{
	public class VocoderProcessor : SignalProcessor<RealPCMData>
	{
		private class FFT
		{
			private float[] _sinTable;

			private float[] _cosTable;

			private int[] _revTable;

			private static int ilog2(int n)
			{
				int num = -1;
				while (n != 0)
				{
					num++;
					n >>= 1;
				}
				return num;
			}

			private static int BitReverse(int k, int nu)
			{
				int num = 0;
				for (int i = 0; i < nu; i++)
				{
					num <<= 1;
					num |= k & 1;
					k >>= 1;
				}
				return num;
			}

			private void CreateArrays(int n)
			{
				int nu = ilog2(n);
				_sinTable = new float[n];
				_cosTable = new float[n];
				_revTable = new int[n];
				for (int i = 0; i < n; i++)
				{
					double num = Math.PI * 2.0 * (double)i / (double)n;
					_cosTable[i] = (float)Math.Cos(num);
					_sinTable[i] = (float)Math.Sin(num);
					_revTable[i] = BitReverse(i, nu);
				}
			}

			public FFT(int windowSize)
			{
				int nu = ilog2(windowSize);
				_sinTable = new float[windowSize];
				_cosTable = new float[windowSize];
				_revTable = new int[windowSize];
				for (int i = 0; i < windowSize; i++)
				{
					double num = Math.PI * 2.0 * (double)i / (double)windowSize;
					_cosTable[i] = (float)Math.Cos(num);
					_sinTable[i] = (float)Math.Sin(num);
					_revTable[i] = BitReverse(i, nu);
				}
			}

			public void DoFFT(float[,] data, int windowSize, int sign)
			{
				int num = ilog2(windowSize);
				int num2 = windowSize;
				int num3 = num;
				for (int i = 0; i < num; i++)
				{
					num2 /= 2;
					num3--;
					for (int j = 0; j < windowSize; j += num2)
					{
						int num4 = 0;
						while (num4 < num2)
						{
							int num5 = _revTable[j >> num3];
							float num6 = data[j + num2, 0] * _cosTable[num5] + data[j + num2, 1] * _sinTable[num5] * (float)sign;
							float num7 = data[j + num2, 1] * _cosTable[num5] - data[j + num2, 0] * _sinTable[num5] * (float)sign;
							data[j + num2, 0] = data[j, 0] - num6;
							data[j + num2, 1] = data[j, 1] - num7;
							data[j, 0] += num6;
							data[j, 1] += num7;
							num4++;
							j++;
						}
					}
				}
				for (int j = 0; j < windowSize; j++)
				{
					int num4 = _revTable[j];
					if (num4 > j)
					{
						float num8 = data[j, 0];
						float num9 = data[j, 1];
						data[j, 0] = data[num4, 0];
						data[j, 1] = data[num4, 1];
						data[num4, 0] = num8;
						data[num4, 1] = num9;
					}
				}
			}
		}

		private const int MaxWindowSize = 4096;

		private int _windowLength = 512;

		private int _windowOverlap = 256;

		private int _bandCount = 16;

		private bool _Normalize = true;

		private float[] CarrierBuffer;

		private float[] _modulatorBuffer = new float[4096];

		private float[,] _loopedCarrierBuffer;

		private float[,] _outputBuffer = new float[4096, 2];

		private float[] _outputOld = new float[4096];

		private float[] _outputNew = new float[4096];

		private FFT _fft1;

		private FFT _fft2;

		public VocoderProcessor(RealPCMData carrier)
		{
			_fft1 = new FFT(_windowLength);
			_fft2 = new FFT(_windowLength / 2);
			ProcessCarrier(carrier);
		}

		private void ProcessCarrier(RealPCMData carrier)
		{
			CarrierBuffer = new float[_windowLength];
			CarrierBuffer = carrier.GetData(0);
			float[] array = new float[_windowLength];
			_loopedCarrierBuffer = new float[4096, 2];
			ReadLooped(CarrierBuffer, array, 0, _windowLength);
			ToComplexArray(array, _loopedCarrierBuffer, _windowLength);
			_fft1.DoFFT(_loopedCarrierBuffer, _windowLength, 1);
			NormalizeFFT(_loopedCarrierBuffer, _windowLength);
		}

		private static int ReadBuffer(float[] buffer, int sourcePos, float[] dest, int destPos, int length)
		{
			int num = buffer.Length - sourcePos;
			if (num > length)
			{
				num = length;
			}
			int num2 = length - num;
			for (int i = 0; i < num; i++)
			{
				dest[i + destPos] = buffer[i + sourcePos];
			}
			for (int j = 0; j < num2; j++)
			{
				dest[j + destPos] = 0f;
			}
			return num;
		}

		private static void ReadLooped(float[] buffer, float[] dest, int dpos, int length)
		{
			int num = 0;
			while (length > 0)
			{
				if (num >= buffer.Length)
				{
					num = 0;
				}
				dest[dpos] = buffer[num];
				dpos++;
				num++;
				length--;
			}
		}

		private static void CopyBuffer(float[] source, float[] dest, int length)
		{
			for (int i = 0; i < length; i++)
			{
				dest[i] = source[i];
			}
		}

		private static void ToComplexArray(float[] source, float[,] dest, int length)
		{
			for (int i = 0; i < length; i++)
			{
				dest[i, 0] = source[i];
				dest[i, 1] = 0f;
			}
		}

		private static void ComplexToSample(float[,] complex_array, float[] sample_array, int length)
		{
			for (int i = 0; i < length; i++)
			{
				sample_array[i] = complex_array[i, 0];
			}
		}

		private static int WriteBuffer(float[] output, int outputPos, float[] source, int sourcePos, int length)
		{
			for (int i = 0; i < length; i++)
			{
				output[outputPos + i] = source[sourcePos + i];
			}
			return length;
		}

		private void DoVocode(float[] modulatorBuffer)
		{
			int num = modulatorBuffer.Length;
			int num7 = (num - _windowOverlap) / (_windowLength - _windowOverlap);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			do
			{
				int num5 = ReadBuffer(modulatorBuffer, num3, _modulatorBuffer, _windowOverlap, _windowLength - _windowOverlap);
				num3 += num5;
				VocodeWindow(_modulatorBuffer, _loopedCarrierBuffer, _outputBuffer);
				ComplexToSample(_outputBuffer, _outputNew, _windowLength);
				for (int i = 0; i < _windowOverlap; i++)
				{
					_outputNew[i] = _outputNew[i] * ((float)i / (float)_windowOverlap) + _outputOld[_windowLength - _windowOverlap + i] * ((float)(_windowOverlap - i) / (float)_windowOverlap);
				}
				int num6 = WriteBuffer(modulatorBuffer, num4, _outputNew, 0, _windowLength - _windowOverlap);
				num4 += num6;
				for (int i = 0; i < _windowOverlap; i++)
				{
					_modulatorBuffer[i] = _modulatorBuffer[_windowLength - _windowOverlap + i];
				}
				float[] outputOld = _outputOld;
				_outputOld = _outputNew;
				_outputNew = outputOld;
				num2++;
			}
			while (num3 < modulatorBuffer.Length);
		}

		private void VocodeWindow(float[] modulator, float[,] carrier, float[,] output)
		{
			int num = _windowLength / (_bandCount * 2);
			int num2 = _windowLength / 2 - num * (_bandCount - 1);
			RealFFTMag(modulator, _windowLength);
			for (int i = 0; i < _bandCount; i++)
			{
				int num3 = ((i == _bandCount - 1) ? num2 : num);
				float num4 = 0f;
				float num5 = 0f;
				int num6 = 0;
				int num7 = i * num;
				int num8 = _windowLength - num7 - 1;
				while (num6 < num3)
				{
					if (_Normalize)
					{
						float num9 = carrier[num7, 0] * carrier[num7, 0] + carrier[num7, 1] * carrier[num7, 1];
						float num10 = carrier[num8, 0] * carrier[num8, 0] + carrier[num8, 1] * carrier[num8, 1];
						num5 += (float)(Math.Sqrt(num9) + Math.Sqrt(num10));
					}
					num4 += modulator[num7];
					num6++;
					num7++;
					num8--;
				}
				if (!_Normalize)
				{
					num5 = 1f;
				}
				if (num5 == 0f)
				{
					num5 = 0.0001f;
				}
				float num11 = num4 / num5;
				num6 = 0;
				num7 = i * num;
				num8 = _windowLength - num7 - 1;
				while (num6 < num3)
				{
					output[num7, 0] = carrier[num7, 0] * num11;
					output[num7, 1] = carrier[num7, 1] * num11;
					output[num8, 0] = carrier[num8, 0] * num11;
					output[num8, 1] = carrier[num8, 1] * num11;
					num6++;
					num7++;
					num8--;
				}
			}
			_fft1.DoFFT(output, _windowLength, -1);
		}

		private void RealFFTMag(float[] data, int windowlength)
		{
			float[,] array = new float[windowlength / 2, 2];
			for (int i = 0; i < windowlength; i += 2)
			{
				array[i >> 1, 0] = data[i];
				array[i >> 1, 1] = data[i + 1];
			}
			_fft2.DoFFT(array, windowlength / 2, 1);
			data[0] = (array[0, 0] + array[0, 1]) / (float)windowlength;
			for (int i = 1; i < windowlength / 2; i++)
			{
				double num = Math.PI * 2.0 * (double)i / (double)windowlength;
				double num2 = Math.Cos(num);
				double num3 = Math.Sin(num);
				double num4 = (array[i, 1] + array[windowlength / 2 - i, 1]) / 2f;
				double num5 = (array[i, 0] - array[windowlength / 2 - i, 0]) / 2f;
				double num6 = (double)((array[i, 0] + array[windowlength / 2 - i, 0]) / 2f) + num2 * num4 - num3 * num5;
				double num7 = (double)((array[i, 1] - array[windowlength / 2 - i, 1]) / 2f) - num3 * num4 - num2 * num5;
				num6 /= (double)(windowlength / 2);
				num7 /= (double)(windowlength / 2);
				data[i] = (float)Math.Sqrt(num6 * num6 + num7 * num7);
			}
			data[windowlength / 2] = (array[0, 0] - array[0, 1]) / (float)windowlength;
		}

		private void NormalizeFFT(float[,] x, int n)
		{
			for (int i = 0; i < n; i++)
			{
				x[i, 0] /= n;
				x[i, 1] /= n;
			}
		}

		private void Initalize(int size)
		{
			_modulatorBuffer = new float[size];
			_outputBuffer = new float[size, 2];
			_outputOld = new float[size];
			_outputNew = new float[size];
		}

		public override bool ProcessBlock(RealPCMData data)
		{
			return true;
		}
	}
}
