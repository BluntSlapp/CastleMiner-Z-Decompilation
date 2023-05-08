namespace DNA.Security.Cryptography.Crypto
{
	public interface IAsymmetricCipherKeyPairGenerator
	{
		void Init(KeyGenerationParameters parameters);

		AsymmetricCipherKeyPair GenerateKeyPair();
	}
}
