using Server.Commands;
using System;
using System.IO;

namespace Server.Bounds
{
    public class Bounds
    {
        public static void Initialize()
        {
            CommandSystem.Register("GenBounds", AccessLevel.Administrator, GenBounds_OnCommand);
        }

        [Usage("GenBounds")]
        [Description("GenBounds")]
        public static void GenBounds_OnCommand(CommandEventArgs e)
        {
            try
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.Write("Generating Bounds.bin...");
                Utility.PopColor();

                FileStream fs = new FileStream("Data/Binary/Bounds.bin", FileMode.Create, FileAccess.Write);

                BinaryWriter bin = new BinaryWriter(fs);

                for (int i = 0; i <= ArtData.MaxItemID; ++i)
                {
                    ArtData.Measure(Item.GetBitmap(i), out int xMin, out int yMin, out int xMax, out int yMax);

                    bin.Write((ushort)xMin);
                    bin.Write((ushort)yMin);
                    bin.Write((ushort)xMax);
                    bin.Write((ushort)yMax);
                }
                
                bin.Flush();
                bin.Close();

                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine("done");
                Utility.PopColor();
            }
            catch (Exception x)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("GenBounds Failed: ");
                Console.WriteLine(x);
                Utility.PopColor();
            }
        }
    }
}
