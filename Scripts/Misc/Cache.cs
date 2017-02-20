using System;
using System.IO;
using Server.Commands;

namespace Server.Cache
{
	public class Cache
	{
				public static void CreateFolder()
				{
					Directory.CreateDirectory("Cache");
				}
	}
	
	public class Bounds
	{
		public static void Initialize()
		{	
			if((Ultima.Files.MulPath["artlegacymul.uop"] != null || (Ultima.Files.MulPath["art.mul"] != null && Ultima.Files.MulPath["artidx.mul"] != null)) && !File.Exists("Cache/Bounds.bin"))
			{	
				Utility.PushColor(ConsoleColor.Yellow);
				Console.Write("Cache: Generating Bounds.bin...");
				
				Cache.CreateFolder();
				FileStream fs = new FileStream( "Cache/Bounds.bin", FileMode.Create, FileAccess.Write );
			
				BinaryWriter bin = new BinaryWriter( fs );
			
				int xMin, yMin, xMax, yMax;
						
					for ( int i = 0; i < Ultima.Art.GetMaxItemID(); ++i )
					{
						Ultima.Art.Measure(Item.GetBitmap(i), out xMin, out yMin, out xMax, out yMax);
						
						bin.Write((ushort)xMin);
						bin.Write((ushort)yMin);
						bin.Write((ushort)xMax);
						bin.Write((ushort)yMax);
					}
				Utility.PushColor(ConsoleColor.Green);
				Console.WriteLine("done. This requires a restart to take effect.");
				Utility.PopColor();
				bin.Close();	
			}
		}
	}
	
	public class Objects
	{
		public static void Initialize()
		{	
			if(!File.Exists("Cache/objects.xml"))
			{
				Utility.PushColor(ConsoleColor.Yellow);
				Console.Write("Cache: Generating objects.xml...");
				Utility.PopColor();
				Cache.CreateFolder();
				Categorization.RebuildCategorization();
				Utility.PushColor(ConsoleColor.Green);
				Console.WriteLine("done. This requires a restart to take effect.");
				Utility.PopColor();
			}
		}
	}
}
