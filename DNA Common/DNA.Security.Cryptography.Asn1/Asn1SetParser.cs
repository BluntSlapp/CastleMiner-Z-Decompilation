namespace DNA.Security.Cryptography.Asn1
{
	public interface Asn1SetParser : IAsn1Convertible
	{
		IAsn1Convertible ReadObject();
	}
}
