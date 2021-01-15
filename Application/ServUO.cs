using Server;

namespace ServUO
{
	public static class ServUO
	{
		private static void Main(string[] args)
		{
			Core.Setup(args);
			Core.Run();
		}
	}
}
