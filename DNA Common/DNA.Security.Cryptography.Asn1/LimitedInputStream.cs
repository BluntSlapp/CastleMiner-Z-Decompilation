using System.IO;
using DNA.Security.Cryptography.Utilities.IO;

namespace DNA.Security.Cryptography.Asn1
{
	internal abstract class LimitedInputStream : BaseInputStream
	{
		protected readonly Stream _in;

		internal LimitedInputStream(Stream inStream)
		{
			_in = inStream;
		}

		protected virtual void SetParentEofDetect(bool on)
		{
			if (_in is IndefiniteLengthInputStream)
			{
				((IndefiniteLengthInputStream)_in).SetEofOn00(on);
			}
		}
	}
}
