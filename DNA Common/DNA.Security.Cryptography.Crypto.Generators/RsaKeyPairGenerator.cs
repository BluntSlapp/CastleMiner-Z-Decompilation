using DNA.Security.Cryptography.Crypto.Parameters;
using DNA.Security.Cryptography.Math;

namespace DNA.Security.Cryptography.Crypto.Generators
{
	public class RsaKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
	{
		private const int DefaultTests = 12;

		private static readonly BigInteger DefaultPublicExponent = BigInteger.ValueOf(65537L);

		private RsaKeyGenerationParameters param;

		public void Init(KeyGenerationParameters parameters)
		{
			if (parameters is RsaKeyGenerationParameters)
			{
				param = (RsaKeyGenerationParameters)parameters;
			}
			else
			{
				param = new RsaKeyGenerationParameters(DefaultPublicExponent, parameters.Random, parameters.Strength, 12);
			}
		}

		public AsymmetricCipherKeyPair GenerateKeyPair()
		{
			int strength = param.Strength;
			int num = (strength + 1) / 2;
			int bitLength = strength - num;
			int num2 = strength / 3;
			BigInteger publicExponent = param.PublicExponent;
			BigInteger bigInteger;
			do
			{
				bigInteger = new BigInteger(num, 1, param.Random);
			}
			while (bigInteger.Mod(publicExponent).Equals(BigInteger.One) || !bigInteger.IsProbablePrime(param.Certainty) || !publicExponent.Gcd(bigInteger.Subtract(BigInteger.One)).Equals(BigInteger.One));
			BigInteger bigInteger2;
			BigInteger bigInteger3;
			while (true)
			{
				bigInteger2 = new BigInteger(bitLength, 1, param.Random);
				if (bigInteger2.Subtract(bigInteger).Abs().BitLength >= num2 && !bigInteger2.Mod(publicExponent).Equals(BigInteger.One) && bigInteger2.IsProbablePrime(param.Certainty) && publicExponent.Gcd(bigInteger2.Subtract(BigInteger.One)).Equals(BigInteger.One))
				{
					bigInteger3 = bigInteger.Multiply(bigInteger2);
					if (bigInteger3.BitLength == param.Strength)
					{
						break;
					}
					bigInteger = bigInteger.Max(bigInteger2);
				}
			}
			BigInteger bigInteger4;
			if (bigInteger.CompareTo(bigInteger2) < 0)
			{
				bigInteger4 = bigInteger;
				bigInteger = bigInteger2;
				bigInteger2 = bigInteger4;
			}
			BigInteger bigInteger5 = bigInteger.Subtract(BigInteger.One);
			BigInteger bigInteger6 = bigInteger2.Subtract(BigInteger.One);
			bigInteger4 = bigInteger5.Multiply(bigInteger6);
			BigInteger bigInteger7 = publicExponent.ModInverse(bigInteger4);
			BigInteger dP = bigInteger7.Remainder(bigInteger5);
			BigInteger dQ = bigInteger7.Remainder(bigInteger6);
			BigInteger qInv = bigInteger2.ModInverse(bigInteger);
			return new AsymmetricCipherKeyPair(new RsaKeyParameters(false, bigInteger3, publicExponent), new RsaPrivateCrtKeyParameters(bigInteger3, publicExponent, bigInteger7, bigInteger, bigInteger2, dP, dQ, qInv));
		}
	}
}
