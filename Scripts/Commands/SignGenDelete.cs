using System;
using System.Collections.Generic;
using System.IO;
using Server.Items;

namespace Server.Commands
{
    public class SignParserDelete
    {
        private static readonly Queue<Item> m_ToDelete = new Queue<Item>();
        public static void Initialize()
        {
            CommandSystem.Register("SignGenDelete", AccessLevel.Administrator, new CommandEventHandler(SignGenDelete_OnCommand));
        }

        [Usage("SignGenDelete")]
        [Description("Deletes world/shop signs on all facets.")]
        public static void SignGenDelete_OnCommand(CommandEventArgs c)
        {
            Parse(c.Mobile);
        }

        public static void Parse(Mobile from)
        {
            string cfg = Path.Combine(Core.BaseDirectory, "Data/signs.cfg");

            if (File.Exists(cfg))
            {
                List<SignEntry> list = new List<SignEntry>();
                from.SendMessage("Deleting signs, please wait.");

                using (StreamReader ip = new StreamReader(cfg))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        string[] split = line.Split(' ');

                        SignEntry e = new SignEntry(
                            line.Substring(split[0].Length + 1 + split[1].Length + 1 + split[2].Length + 1 + split[3].Length + 1 + split[4].Length + 1),
                            new Point3D(Utility.ToInt32(split[2]), Utility.ToInt32(split[3]), Utility.ToInt32(split[4])),
                            Utility.ToInt32(split[1]), Utility.ToInt32(split[0]));

                        list.Add(e);
                    }
                }

                Map[] brit = new Map[] { Map.Felucca, Map.Trammel };
                Map[] fel = new Map[] { Map.Felucca };
                Map[] tram = new Map[] { Map.Trammel };
                Map[] ilsh = new Map[] { Map.Ilshenar };
                Map[] malas = new Map[] { Map.Malas };
                Map[] tokuno = new Map[] { Map.Tokuno };

                for (int i = 0; i < list.Count; ++i)
                {
                    SignEntry e = list[i];
                    Map[] maps = null;

                    switch ( e.m_Map )
                    {
                        case 0:
                            maps = brit;
                            break; // Trammel and Felucca
                        case 1:
                            maps = fel;
                            break;  // Felucca
                        case 2:
                            maps = tram;
                            break; // Trammel
                        case 3:
                            maps = ilsh;
                            break; // Ilshenar
                        case 4:
                            maps = malas;
                            break; // Malas
                        case 5:
                            maps = tokuno;
                            break; // Tokuno Islands
                    }

                    for (int j = 0; maps != null && j < maps.Length; ++j)
                        Delete_Static(e.m_ItemID, e.m_Location, maps[j], e.m_Text);
                }

                from.SendMessage("Sign deleting complete.");
            }
            else
            {
                from.SendMessage("{0} not found!", cfg);
            }
        }

        public static void Delete_Static(int itemID, Point3D location, Map map, string name)
        {
            IPooledEnumerable eable = map.GetItemsInRange(location, 0);

            foreach (Item item in eable)
            {
                if (item is Sign && item.Z == location.Z && item.ItemID == itemID)
                    m_ToDelete.Enqueue(item);
            }

            eable.Free();

            while (m_ToDelete.Count > 0)
                m_ToDelete.Dequeue().Delete();
        }

        private class SignEntry
        {
            public readonly string m_Text;
            public readonly Point3D m_Location;
            public readonly int m_ItemID;
            public readonly int m_Map;
            public SignEntry(string text, Point3D pt, int itemID, int mapLoc)
            {
                this.m_Text = text;
                this.m_Location = pt;
                this.m_ItemID = itemID;
                this.m_Map = mapLoc;
            }
        }
    }
}