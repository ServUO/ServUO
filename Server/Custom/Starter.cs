using Launcher;

namespace Server
{
	public class Starter : IRun
	{
		public void Run( string[] args )
		{
			Core.Main(args);
		}
	}
}
