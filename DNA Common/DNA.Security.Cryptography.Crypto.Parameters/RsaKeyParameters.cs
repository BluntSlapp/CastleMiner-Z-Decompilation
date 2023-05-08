using DNA.Security.Cryptography.Math;

namespace DNA.Security.Cryptography.Crypto.Parameters
{
	public class RsaKeyParameters : AsymmetricKeyParameter
	{
		private readonly BigInteger modulus;

		private readonly BigInteger exponent;

		public BigInteger Modulus
		{
			get
			{
				return modulus;
			}
		}

		public BigInteger Exponent
		{
			get
			{
				return exponent;
			}
		}

		public RsaKeyParameters(bool isPrivate, BigInteger modulus, BigInteger exponent)
			: base(isPrivate)
		{
			this.modulus = modulus;
			this.exponent = exponent;
		}

		public override bool Equals(object obj)
		{
			RsaKeyParameters rsaKeyParameters = obj as RsaKeyParameters;
			if (rsaKeyParameters == null)
			{
				return false;
			}
			if (rsaKeyParameters.IsPrivate == base.IsPrivate && rsaKeyParameters.Modulus.Equals(modulus))
			{
				return rsaKeyParameters.Exponent.Equals(exponent);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return modulus.GetHashCode() ^ exponent.GetHashCode() ^ base.IsPrivate.GetHashCode();
		}
	}
}
