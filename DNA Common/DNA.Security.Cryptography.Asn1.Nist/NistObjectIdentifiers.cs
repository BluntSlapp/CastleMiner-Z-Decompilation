namespace DNA.Security.Cryptography.Asn1.Nist
{
	public sealed class NistObjectIdentifiers
	{
		public static readonly DerObjectIdentifier NistAlgorithm = new DerObjectIdentifier("2.16.840.1.101.3.4");

		public static readonly DerObjectIdentifier IdSha256 = new DerObjectIdentifier(string.Concat(NistAlgorithm, ".2.1"));

		public static readonly DerObjectIdentifier IdSha384 = new DerObjectIdentifier(string.Concat(NistAlgorithm, ".2.2"));

		public static readonly DerObjectIdentifier IdSha512 = new DerObjectIdentifier(string.Concat(NistAlgorithm, ".2.3"));

		public static readonly DerObjectIdentifier IdSha224 = new DerObjectIdentifier(string.Concat(NistAlgorithm, ".2.4"));

		public static readonly DerObjectIdentifier Aes = new DerObjectIdentifier(string.Concat(NistAlgorithm, ".1"));

		public static readonly DerObjectIdentifier IdAes128Ecb = new DerObjectIdentifier(string.Concat(Aes, ".1"));

		public static readonly DerObjectIdentifier IdAes128Cbc = new DerObjectIdentifier(string.Concat(Aes, ".2"));

		public static readonly DerObjectIdentifier IdAes128Ofb = new DerObjectIdentifier(string.Concat(Aes, ".3"));

		public static readonly DerObjectIdentifier IdAes128Cfb = new DerObjectIdentifier(string.Concat(Aes, ".4"));

		public static readonly DerObjectIdentifier IdAes128Wrap = new DerObjectIdentifier(string.Concat(Aes, ".5"));

		public static readonly DerObjectIdentifier IdAes192Ecb = new DerObjectIdentifier(string.Concat(Aes, ".21"));

		public static readonly DerObjectIdentifier IdAes192Cbc = new DerObjectIdentifier(string.Concat(Aes, ".22"));

		public static readonly DerObjectIdentifier IdAes192Ofb = new DerObjectIdentifier(string.Concat(Aes, ".23"));

		public static readonly DerObjectIdentifier IdAes192Cfb = new DerObjectIdentifier(string.Concat(Aes, ".24"));

		public static readonly DerObjectIdentifier IdAes192Wrap = new DerObjectIdentifier(string.Concat(Aes, ".25"));

		public static readonly DerObjectIdentifier IdAes256Ecb = new DerObjectIdentifier(string.Concat(Aes, ".41"));

		public static readonly DerObjectIdentifier IdAes256Cbc = new DerObjectIdentifier(string.Concat(Aes, ".42"));

		public static readonly DerObjectIdentifier IdAes256Ofb = new DerObjectIdentifier(string.Concat(Aes, ".43"));

		public static readonly DerObjectIdentifier IdAes256Cfb = new DerObjectIdentifier(string.Concat(Aes, ".44"));

		public static readonly DerObjectIdentifier IdAes256Wrap = new DerObjectIdentifier(string.Concat(Aes, ".45"));

		public static readonly DerObjectIdentifier IdDsaWithSha2 = new DerObjectIdentifier(string.Concat(NistAlgorithm, ".3"));

		public static readonly DerObjectIdentifier DsaWithSha224 = new DerObjectIdentifier(string.Concat(IdDsaWithSha2, ".1"));

		public static readonly DerObjectIdentifier DsaWithSha256 = new DerObjectIdentifier(string.Concat(IdDsaWithSha2, ".2"));

		public static readonly DerObjectIdentifier DsaWithSha384 = new DerObjectIdentifier(string.Concat(IdDsaWithSha2, ".3"));

		public static readonly DerObjectIdentifier DsaWithSha512 = new DerObjectIdentifier(string.Concat(IdDsaWithSha2, ".4"));

		private NistObjectIdentifiers()
		{
		}
	}
}
