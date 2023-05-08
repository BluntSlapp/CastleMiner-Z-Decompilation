using System.Reflection;

namespace DNA.Reflection
{
	public static class CommonAssembly
	{
		public static void Initalize()
		{
			ReflectionTools.RegisterAssembly(Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly());
		}
	}
}
