using Server.Items;
using System.Collections;
using System.IO;

namespace Server.Commands
{
    public class ExportCommand
    {
        private const string ExportFile = @"C:\Uo\WorldForge\items.wsc";
        public static void Initialize()
        {
            CommandSystem.Register("ExportWSC", AccessLevel.Administrator, Export_OnCommand);
        }

        public static void Export_OnCommand(CommandEventArgs e)
        {
            StreamWriter w = new StreamWriter(ExportFile);
            ArrayList remove = new ArrayList();
            int count = 0;

            e.Mobile.SendMessage("Exporting all static items to \"{0}\"...", ExportFile);
            e.Mobile.SendMessage("This will delete all static items in the world.  Please make a backup.");

            foreach (Item item in World.Items.Values)
            {
                if ((item is Static || item is BaseFloor || item is BaseWall) &&
                    item.RootParent == null)
                {
                    w.WriteLine("SECTION WORLDITEM {0}", count);
                    w.WriteLine("{");
                    w.WriteLine("SERIAL {0}", item.Serial);
                    w.WriteLine("NAME #");
                    w.WriteLine("NAME2 #");
                    w.WriteLine("ID {0}", item.ItemID);
                    w.WriteLine("X {0}", item.X);
                    w.WriteLine("Y {0}", item.Y);
                    w.WriteLine("Z {0}", item.Z);
                    w.WriteLine("COLOR {0}", item.Hue);
                    w.WriteLine("CONT -1");
                    w.WriteLine("TYPE 0");
                    w.WriteLine("AMOUNT 1");
                    w.WriteLine("WEIGHT 255");
                    w.WriteLine("OWNER -1");
                    w.WriteLine("SPAWN -1");
                    w.WriteLine("VALUE 1");
                    w.WriteLine("}");
                    w.WriteLine("");

                    count++;
                    remove.Add(item);
                    w.Flush();
                }
            }

            w.Close();

            foreach (Item item in remove)
                item.Delete();

            e.Mobile.SendMessage("Export complete.  Exported {0} statics.", count);
        }
    }
}
/*SECTION WORLDITEM 1
{
SERIAL 1073741830
NAME #
NAME2 #
ID 1709
X 1439
Y 1613
Z 20
CONT -1
TYPE 12
AMOUNT 1
WEIGHT 25500
OWNER -1
SPAWN -1
VALUE 1
}*/
