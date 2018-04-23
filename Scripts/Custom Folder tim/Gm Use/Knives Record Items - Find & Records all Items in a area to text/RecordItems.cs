using System;
using System.IO;
using Server;
using Server.Targeting;
using Server.Commands;

namespace Server.Scripts.Commands 
{
    public class RecordItems
    {
        public static void Initialize()
        {
            CommandSystem.Register("RecordItems", AccessLevel.Counselor, new CommandEventHandler(OnRecord));
        }

        [Usage("RecordItems")]
        [Description("Records all non movable items in the current region, saving them to a txt file.")]
        public static void OnRecord(CommandEventArgs e)
        {
            if (e.Mobile.Region == null)
            {
                e.Mobile.SendMessage("You are not in a region.");
                return;
            }

            StreamWriter w = File.AppendText("region.txt");
            w.WriteLine("");
            w.WriteLine(e.Mobile.Region.GetType().Name + " Name: " + e.Mobile.Region.Name);
            w.WriteLine("");

            foreach (Item item in World.Items.Values)
                if(!item.Movable && item.Map == e.Mobile.Map && e.Mobile.Region.Contains(item.Location))
                    w.WriteLine("Type: " + item.GetType().Name + " ID: " + item.ItemID + " Location: " + item.Location);

            w.Close();
        }
    }
}
