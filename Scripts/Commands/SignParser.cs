using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Commands
{
    public class SignParser
    {
        private static readonly Queue<Item> m_ToDelete = new Queue<Item>();
        public static void Initialize()
        {
            CommandSystem.Register("SignGen", AccessLevel.Administrator, SignGen_OnCommand);
            CommandSystem.Register("SignSave", AccessLevel.Administrator, SignSave_OnCommand);
            CommandSystem.Register("SignRemove", AccessLevel.Administrator, SignRemove_OnCommand);
        }

        [Usage("SignRemove")]
        [Description("Removes the targeted sign from the world and from the signs configuration file.")]
        public static void SignRemove_OnCommand(CommandEventArgs c)
        {
            c.Mobile.Target = new SignRemoveTarget();
        }

        [Usage("SignSave")]
        [Description("Saves the targeted sign to the signs configuration file.")]
        public static void SignSave_OnCommand(CommandEventArgs c)
        {
            c.Mobile.Target = new SignSaveTarget();
        }

        [Usage("SignGen")]
        [Description("Generates world/shop signs on all facets.")]
        public static void SignGen_OnCommand(CommandEventArgs c)
        {
            Parse(c.Mobile);
        }

        public static void Parse(Mobile from)
        {
            from.SendMessage("Generating signs, please wait.");
            List<SignEntry> list = SignEntry.LoadConfig("Data/signs.cfg");

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

                switch (e.m_Map)
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
                    Add_Static(e.m_ItemID, e.m_Location, maps[j], e.m_Text);
            }

            from.SendMessage("Sign generating complete.");
        }

        public static void Add_Static(int itemID, Point3D location, Map map, string name)
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

            Item sign;

            if (name.StartsWith("#"))
            {
                sign = new LocalizedSign(itemID, Utility.ToInt32(name.Substring(1)));
            }
            else
            {
                sign = new Sign(itemID)
                {
                    Name = name
                };
            }
            WeakEntityCollection.Add("sign", sign);

            if (map == Map.Malas)
            {
                if (location.X >= 965 && location.Y >= 502 && location.X <= 1012 && location.Y <= 537)
                    sign.Hue = 0x47E;
                else if (location.X >= 1960 && location.Y >= 1278 && location.X < 2106 && location.Y < 1413)
                    sign.Hue = 0x44E;
            }

            sign.MoveToWorld(location, map);
        }

        private class SignEntry
        {
            public static readonly int BritMapId = 0;
            public readonly string m_Text;
            public readonly Point3D m_Location;
            public readonly int m_ItemID;
            public readonly int m_Map;

            public SignEntry(string text, Point3D pt, int itemID, int mapLoc)
            {
                m_Text = text;
                m_Location = pt;
                m_ItemID = itemID;
                m_Map = mapLoc;
            }

            public static int GetIdForMap(Map map)
            {
                if (map == Map.Felucca) return 1;
                if (map == Map.Trammel) return 2;
                if (map == Map.Ilshenar) return 3;
                if (map == Map.Malas) return 4;
                if (map == Map.Tokuno) return 5;
                if (map == Map.TerMur) return 6;
                throw new ArgumentException(string.Format("Unhandled map {0}", map.Name));
            }

            public static List<SignEntry> LoadConfig(string path)
            {
                List<SignEntry> list = new List<SignEntry>();
                string cfg = Path.Combine(Core.BaseDirectory, path);

                if (File.Exists(cfg))
                {
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
                }

                return list;
            }

            public static void WriteConfig(List<SignEntry> signs, string path)
            {
                string cfg = Path.Combine(Core.BaseDirectory, path);
                using (StreamWriter op = new StreamWriter(cfg))
                {
                    foreach (SignEntry sign in signs)
                    {
                        string line = string.Format("{0} {1} {2} {3} {4} {5}", sign.m_Map,
                            sign.m_ItemID, sign.m_Location.X, sign.m_Location.Y,
                            sign.m_Location.Z, sign.m_Text);
                        op.WriteLine(line);
                    }
                }
            }
        }

        private class SignRemoveTarget : Target
        {
            public SignRemoveTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (!(o is Sign)) return;
                Sign target = (Sign)o;
                int targetMap = SignEntry.GetIdForMap(target.Map);
                SignEntry toRemove = null;

                List<SignEntry> signs = SignEntry.LoadConfig("Data/signs.cfg");
                foreach (SignEntry sign in signs)
                {
                    if (sign.m_Map == targetMap ||
                        (sign.m_Map == 0 && (
                            targetMap == 1 ||
                            targetMap == 2
                        )))
                    {
                        if (sign.m_Location.CompareTo(target.Location) == 0)
                        {
                            RemoveSign(target);
                            toRemove = sign;
                        }
                    }
                }

                if (toRemove != null)
                {
                    signs.Remove(toRemove);
                    SignEntry.WriteConfig(signs, "Data/signs.cfg");
                }
                else
                {
                    from.SendMessage("An entry for that sign was not found.");
                }
            }

            private void RemoveSign(Sign sign)
            {
                IPooledEnumerable eable;

                m_ToDelete.Enqueue(sign);
                if (sign.Map == Map.Trammel)
                {
                    eable = Map.Felucca.GetItemsInRange(sign.Location, 0);
                    foreach (Item item in eable)
                    {
                        if (item is Sign && item.Z == sign.Location.Z && item.ItemID == sign.ItemID)
                            m_ToDelete.Enqueue(item);
                    }
                    eable.Free();
                }
                if (sign.Map == Map.Felucca)
                {
                    eable = Map.Trammel.GetItemsInRange(sign.Location, 0);
                    foreach (Item item in eable)
                    {
                        if (item is Sign && item.Z == sign.Location.Z && item.ItemID == sign.ItemID)
                            m_ToDelete.Enqueue(item);
                    }
                    eable.Free();
                }

                while (m_ToDelete.Count > 0)
                    m_ToDelete.Dequeue().Delete();
            }
        }

        private class SignSaveTarget : Target
        {
            public int TargetMap { get; set; }
            private Sign m_Sign;
            private Mobile m_From;

            public SignSaveTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (!(o is Sign)) return;
                m_Sign = (Sign)o;
                m_From = from;

                TargetMap = SignEntry.GetIdForMap(m_Sign.Map);

                if (m_Sign.Map == Map.Felucca ||
                    m_Sign.Map == Map.Trammel)
                {
                    from.SendGump(new BritGump(this));
                }
                else
                {
                    DoSave();
                }
            }

            public void DoSave()
            {
                List<SignEntry> signs = SignEntry.LoadConfig("Data/signs.cfg");
                foreach (SignEntry sign in signs)
                {
                    if (sign.m_Map == TargetMap &&
                        sign.m_Location.CompareTo(m_Sign.Location) == 0)
                    {
                        m_From.SendMessage("A sign is already configured for this location.");
                        return;
                    }
                }
                signs.Add(new SignEntry(m_Sign.Name, m_Sign.Location, m_Sign.ItemID, TargetMap));
                SignEntry.WriteConfig(signs, "Data/signs.cfg");
            }
        }

        private class BritGump : Gump
        {
            private readonly SignSaveTarget m_Target;

            public BritGump(SignSaveTarget target)
                : base(30, 20)
            {
                m_Target = target;
                Dragable = false;
                Resizable = false;
                Closable = false;

                AddPage(0);
                AddBackground(0, 0, 550, 440, 5054);
                AddBackground(10, 10, 530, 420, 3000);

                AddLabel(20, 20, 0, "Add this sign to both facets?");
                AddButton(20, 40, 2453, 2454, 0, GumpButtonType.Reply, 0);
                AddButton(450, 40, 2450, 2451, 1, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 1)
                {
                    m_Target.TargetMap = SignEntry.BritMapId;
                }
                m_Target.DoSave();
            }
        }
    }
}
