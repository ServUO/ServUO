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
            if (ArtData.CheckFile)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.Write("Generating Bounds.bin...");
                Utility.PopColor();

                FileStream fs = new FileStream("Data/Binary/Bounds.bin", FileMode.Create, FileAccess.Write);

                BinaryWriter bin = new BinaryWriter(fs);

                for (int i = 0; i < ArtData.GetMaxItemID(); ++i)
                {
                    ArtData.Measure(Item.GetBitmap(i), out int xMin, out int yMin, out int xMax, out int yMax);

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
