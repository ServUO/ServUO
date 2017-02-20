using System;
using System.IO;
using Server.Commands;

namespace Server.Bounds
{
	public class Bounds
	{
		public static void Initialize()
		{	
			CommandSystem.Register("GenBounds", AccessLevel.Administrator, new CommandEventHandler(GenBounds_OnCommand));		
		}
		
		[Usage("GenBounds")]
		[Description("GenBounds")]
		public static void GenBounds_OnCommand(CommandEventArgs e)
		{
			if(Ultima.Files.MulPath["artlegacymul.uop"] != null || (Ultima.Files.MulPath["art.mul"] != null && Ultima.Files.MulPath["artidx.mul"] != null))
			{	
				Utility.PushColor(ConsoleColor.Yellow);
				Console.Write("Generating Bounds.bin...");
				Utility.PopColor();
				
				FileStream fs = new FileStream( "Data/Binary/Bounds.bin", FileMode.Create, FileAccess.Write );
			
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
				Console.WriteLine("done");
				Utility.PopColor();
				bin.Close();	
			}
			else
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Art files missing.");
				Utility.PopColor();
			}
		}
	}
}
