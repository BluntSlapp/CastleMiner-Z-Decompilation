using DNA.Reflection;

namespace DNA.CastleMinerZ
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			CommonAssembly.Initalize();
			DNAGame.Run<CastleMinerZGame>("Bugs@CastleMiner.com", "CastleMiner Z");
		}
	}
}
