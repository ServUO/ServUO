using System;
using System.IO;

using Server.Commands;

namespace Server.Misc
{
    public class Bounds
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Initialize()
        {
            CommandSystem.Register("GenBounds", AccessLevel.Administrator, new CommandEventHandler(GenBounds_OnCommand));
        }

        [Usage("GenBounds")]
        [Description("GenBounds")]
        public static void GenBounds_OnCommand(CommandEventArgs e)
        {
            if (Ultima.Files.MulPath["artlegacymul.uop"] != null || (Ultima.Files.MulPath["art.mul"] != null && Ultima.Files.MulPath["artidx.mul"] != null))
            {
                log.Info("Generating Bounds.bin...");

                FileStream fs = new FileStream("Data/Binary/Bounds.bin", FileMode.Create, FileAccess.Write);

                BinaryWriter bin = new BinaryWriter(fs);

                int xMin, yMin, xMax, yMax;

                for (int i = 0; i < Ultima.Art.GetMaxItemID(); ++i)
                {
                    Ultima.Art.Measure(Item.GetBitmap(i), out xMin, out yMin, out xMax, out yMax);

                    bin.Write((ushort)xMin);
                    bin.Write((ushort)yMin);
                    bin.Write((ushort)xMax);
                    bin.Write((ushort)yMax);
                }

                log.Info("Bounds.bin generated!");
                bin.Close();
            }
            else
            {
                log.Error("Could not generate Bounds.bin: Art files missing.");
            }
        }
    }
}
