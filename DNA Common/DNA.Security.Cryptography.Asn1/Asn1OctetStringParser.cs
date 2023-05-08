using System.IO;

namespace DNA.Security.Cryptography.Asn1
{
	public interface Asn1OctetStringParser : IAsn1Convertible
	{
		Stream GetOctetStream();
	}
}
