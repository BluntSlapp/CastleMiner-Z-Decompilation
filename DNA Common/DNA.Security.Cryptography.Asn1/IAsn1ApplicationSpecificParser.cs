namespace DNA.Security.Cryptography.Asn1
{
	public interface IAsn1ApplicationSpecificParser : IAsn1Convertible
	{
		IAsn1Convertible ReadObject();
	}
}
