using System;
using DNA.Security.Cryptography.Crypto.Parameters;
using DNA.Security.Cryptography.Security;
using DNA.Security.Cryptography.Utilities;

namespace DNA.Security.Cryptography.Crypto.Encodings
{
	public class Pkcs1Encoding : IAsymmetricBlockCipher
	{
		public const string StrictLengthEnabledProperty = "DNA.Security.Cryptography.Pkcs1.Strict";

		private const int HeaderLength = 10;

		private static readonly bool[] strictLengthEnabled;

		private SecureRandom random;

		private IAsymmetricBlockCipher engine;

		private bool forEncryption;

		private bool forPrivateKey;

		private bool useStrictLength;

		public static bool StrictLengthEnabled
		{
			get
			{
				return strictLengthEnabled[0];
			}
			set
			{
				strictLengthEnabled[0] = value;
			}
		}

		public string AlgorithmName
		{
			get
			{
				return engine.AlgorithmName + "/PKCS1Padding";
			}
		}

		static Pkcs1Encoding()
		{
			string environmentVariable = Platform.GetEnvironmentVariable("DNA.Security.Cryptography.Pkcs1.Strict");
			strictLengthEnabled = new bool[1] { environmentVariable == null || environmentVariable.Equals("true") };
		}

		public Pkcs1Encoding(IAsymmetricBlockCipher cipher)
		{
			engine = cipher;
			useStrictLength = StrictLengthEnabled;
		}

		public IAsymmetricBlockCipher GetUnderlyingCipher()
		{
			return engine;
		}

		public void Init(bool forEncryption, ICipherParameters parameters)
		{
			AsymmetricKeyParameter asymmetricKeyParameter;
			if (parameters is ParametersWithRandom)
			{
				ParametersWithRandom parametersWithRandom = (ParametersWithRandom)parameters;
				random = parametersWithRandom.Random;
				asymmetricKeyParameter = (AsymmetricKeyParameter)parametersWithRandom.Parameters;
			}
			else
			{
				random = new SecureRandom();
				asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
			}
			engine.Init(forEncryption, parameters);
			forPrivateKey = asymmetricKeyParameter.IsPrivate;
			this.forEncryption = forEncryption;
		}

		public int GetInputBlockSize()
		{
			int inputBlockSize = engine.GetInputBlockSize();
			if (!forEncryption)
			{
				return inputBlockSize;
			}
			return inputBlockSize - 10;
		}

		public int GetOutputBlockSize()
		{
			int outputBlockSize = engine.GetOutputBlockSize();
			if (!forEncryption)
			{
				return outputBlockSize - 10;
			}
			return outputBlockSize;
		}

		public byte[] ProcessBlock(byte[] input, int inOff, int length)
		{
			if (!forEncryption)
			{
				return DecodeBlock(input, inOff, length);
			}
			return EncodeBlock(input, inOff, length);
		}

		private byte[] EncodeBlock(byte[] input, int inOff, int inLen)
		{
			if (inLen > GetInputBlockSize())
			{
				throw new ArgumentException("input data too large", "inLen");
			}
			byte[] array = new byte[engine.GetInputBlockSize()];
			if (forPrivateKey)
			{
				array[0] = 1;
				for (int i = 1; i != array.Length - inLen - 1; i++)
				{
					array[i] = byte.MaxValue;
				}
			}
			else
			{
				random.NextBytes(array);
				array[0] = 2;
				for (int j = 1; j != array.Length - inLen - 1; j++)
				{
					while (array[j] == 0)
					{
						array[j] = (byte)random.NextInt();
					}
				}
			}
			array[array.Length - inLen - 1] = 0;
			Array.Copy(input, inOff, array, array.Length - inLen, inLen);
			return engine.ProcessBlock(array, 0, array.Length);
		}

		private byte[] DecodeBlock(byte[] input, int inOff, int inLen)
		{
			byte[] array = engine.ProcessBlock(input, inOff, inLen);
			if (array.Length < GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block truncated");
			}
			byte b = array[0];
			if (b != 1 && b != 2)
			{
				throw new InvalidCipherTextException("unknown block type");
			}
			if (useStrictLength && array.Length != engine.GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("block incorrect size");
			}
			int i;
			for (i = 1; i != array.Length; i++)
			{
				byte b2 = array[i];
				if (b2 == 0)
				{
					break;
				}
				if (b == 1 && b2 != byte.MaxValue)
				{
					throw new InvalidCipherTextException("block padding incorrect");
				}
			}
			i++;
			if (i >= array.Length || i < 10)
			{
				throw new InvalidCipherTextException("no data in block");
			}
			byte[] array2 = new byte[array.Length - i];
			Array.Copy(array, i, array2, 0, array2.Length);
			return array2;
		}
	}
}
