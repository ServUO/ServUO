#region References
using Server.Commands;
using Server.Network;
using System;
using System.Collections;
using System.Reflection;
#endregion

namespace Server.Gumps
{
    public class SetBodyGump : Gump
    {
        private static ArrayList m_Monster, m_Animal, m_Sea, m_Human;
        private const int LabelColor32 = 0xFFFFFF;
        private const int SelectedColor32 = 0x8080FF;
        private const int TextColor32 = 0xFFFFFF;
        private readonly PropertyInfo m_Property;
        private readonly Mobile m_Mobile;
        private readonly object m_Object;
        private readonly Stack m_Stack;
        private readonly int m_Page;
        private readonly ArrayList m_List;
        private readonly int m_OurPage;
        private readonly ArrayList m_OurList;
        private readonly ModelBodyType m_OurType;

        public SetBodyGump(PropertyInfo prop, Mobile mobile, object o, Stack stack, int page, ArrayList list)
            : this(prop, mobile, o, stack, page, list, 0, null, ModelBodyType.Invalid)
        { }

        public SetBodyGump(
            PropertyInfo prop,
            Mobile mobile,
            object o,
            Stack stack,
            int page,
            ArrayList list,
            int ourPage,
            ArrayList ourList,
            ModelBodyType ourType)
            : base(20, 30)
        {
            m_Property = prop;
            m_Mobile = mobile;
            m_Object = o;
            m_Stack = stack;
            m_Page = page;
            m_List = list;
            m_OurPage = ourPage;
            m_OurList = ourList;
            m_OurType = ourType;

            AddPage(0);

            AddBackground(0, 0, 525, 328, 5054);

            AddImageTiled(10, 10, 505, 20, 0xA40);
            AddAlphaRegion(10, 10, 505, 20);

            AddImageTiled(10, 35, 505, 283, 0xA40);
            AddAlphaRegion(10, 35, 505, 283);

            AddTypeButton(10, 10, 1, "Monster", ModelBodyType.Monsters);
            AddTypeButton(130, 10, 2, "Animal", ModelBodyType.Animals);
            AddTypeButton(250, 10, 3, "Marine", ModelBodyType.Sea);
            AddTypeButton(370, 10, 4, "Human", ModelBodyType.Human);

            AddImage(480, 12, 0x25EA);
            AddImage(497, 12, 0x25E6);

            if (ourList == null)
            {
                AddLabel(15, 40, 0x480, "Choose a body type above.");
            }
            else if (ourList.Count == 0)
            {
                AddLabel(15, 40, 0x480, "The server must have UO:3D installed to use this feature.");
            }
            else
            {
                for (int i = 0, index = (ourPage * 12); i < 12 && index >= 0 && index < ourList.Count; ++i, ++index)
                {
                    InternalEntry entry = (InternalEntry)ourList[index];
                    int itemID = entry.ItemID;

                    Rectangle2D bounds = ItemBounds.Table[itemID & 0x3FFF];

                    int x = 15 + ((i % 4) * 125);
                    int y = 40 + ((i / 4) * 93);

                    AddItem(x + ((120 - bounds.Width) / 2) - bounds.X, y + ((69 - bounds.Height) / 2) - bounds.Y, itemID);
                    AddButton(x + 6, y + 66, 0x98D, 0x98D, 7 + index, GumpButtonType.Reply, 0);

                    x += 6;
                    y += 67;

                    AddHtml(x + 0, y - 1, 108, 21, Center(entry.DisplayName), false, false);
                    AddHtml(x + 0, y + 1, 108, 21, Center(entry.DisplayName), false, false);
                    AddHtml(x - 1, y + 0, 108, 21, Center(entry.DisplayName), false, false);
                    AddHtml(x + 1, y + 0, 108, 21, Center(entry.DisplayName), false, false);
                    AddHtml(x + 0, y + 0, 108, 21, Color(Center(entry.DisplayName), TextColor32), false, false);
                }

                if (ourPage > 0)
                    AddButton(480, 12, 0x15E3, 0x15E7, 5, GumpButtonType.Reply, 0);

                if ((ourPage + 1) * 12 < ourList.Count)
                    AddButton(497, 12, 0x15E1, 0x15E5, 6, GumpButtonType.Reply, 0);
            }
        }

        public string Center(string text)
        {
            return string.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public void AddTypeButton(int x, int y, int buttonID, string text, ModelBodyType type)
        {
            bool isSelection = (m_OurType == type);

            AddButton(x, y - 1, isSelection ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0);
            AddHtml(x + 35, y, 200, 20, Color(text, isSelection ? SelectedColor32 : LabelColor32), false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int index = info.ButtonID - 1;

            if (index == -1)
            {
                m_Mobile.SendGump(new PropertiesGump(m_Mobile, m_Object, m_Stack, m_List, m_Page));
            }
            else if (index >= 0 && index < 4)
            {
                if (m_Monster == null)
                    LoadLists();

                ModelBodyType type;
                ArrayList list;

                switch (index)
                {
                    default:
                    case 0:
                        type = ModelBodyType.Monsters;
                        list = m_Monster;
                        break;
                    case 1:
                        type = ModelBodyType.Animals;
                        list = m_Animal;
                        break;
                    case 2:
                        type = ModelBodyType.Sea;
                        list = m_Sea;
                        break;
                    case 3:
                        type = ModelBodyType.Human;
                        list = m_Human;
                        break;
                }

                m_Mobile.SendGump(new SetBodyGump(m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List, 0, list, type));
            }
            else if (m_OurList != null)
            {
                index -= 4;

                if (index == 0 && m_OurPage > 0)
                {
                    m_Mobile.SendGump(
                        new SetBodyGump(m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List, m_OurPage - 1, m_OurList, m_OurType));
                }
                else if (index == 1 && ((m_OurPage + 1) * 12) < m_OurList.Count)
                {
                    m_Mobile.SendGump(
                        new SetBodyGump(m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List, m_OurPage + 1, m_OurList, m_OurType));
                }
                else
                {
                    index -= 2;

                    if (index >= 0 && index < m_OurList.Count)
                    {
                        try
                        {
                            InternalEntry entry = (InternalEntry)m_OurList[index];

                            CommandLogging.LogChangeProperty(m_Mobile, m_Object, m_Property.Name, entry.Body.ToString());
                            m_Property.SetValue(m_Object, entry.Body, null);
                            PropertiesGump.OnValueChanged(m_Object, m_Property, m_Stack);
                        }
                        catch (Exception e)
                        {
                            Diagnostics.ExceptionLogging.LogException(e);
                        }

                        m_Mobile.SendGump(
                            new SetBodyGump(m_Property, m_Mobile, m_Object, m_Stack, m_Page, m_List, m_OurPage, m_OurList, m_OurType));
                    }
                }
            }
        }

        private static void LoadLists()
        {
            m_Monster = new ArrayList();
            m_Animal = new ArrayList();
            m_Sea = new ArrayList();
            m_Human = new ArrayList();

            System.Collections.Generic.List<BodyEntry> entries = Docs.LoadBodies();

            for (int i = 0; i < entries.Count; ++i)
            {
                BodyEntry oldEntry = entries[i];
                int bodyID = oldEntry.Body.BodyID;

                if (((Body)bodyID).IsEmpty)
                    continue;

                ArrayList list = null;

                switch (oldEntry.BodyType)
                {
                    case ModelBodyType.Monsters:
                        list = m_Monster;
                        break;
                    case ModelBodyType.Animals:
                        list = m_Animal;
                        break;
                    case ModelBodyType.Sea:
                        list = m_Sea;
                        break;
                    case ModelBodyType.Human:
                        list = m_Human;
                        break;
                }

                if (list == null)
                    continue;

                int itemID = ShrinkTable.Lookup(bodyID, -1);

                if (itemID != -1)
                    list.Add(new InternalEntry(bodyID, itemID, oldEntry.Name));
            }

            if (m_Monster != null)
            {
                m_Monster.Sort();
            }

            if (m_Animal != null)
            {
                m_Animal.Sort();
            }

            if (m_Sea != null)
            {
                m_Sea.Sort();
            }

            if (m_Human != null)
            {
                m_Human.Sort();
            }
        }

        private class InternalEntry : IComparable
        {
            private static readonly string[] m_GroupNames =
            {
                "ogres_", "ettins_", "walking_dead_", "gargoyles_", "orcs_", "flails_", "daemons_", "arachnids_", "dragons_",
                "elementals_", "serpents_", "gazers_", "liche_", "spirits_", "harpies_", "headless_", "lizard_race_", "mongbat_",
                "rat_race_", "scorpions_", "trolls_", "slimes_", "skeletons_", "ethereals_", "terathan_", "imps_", "cyclops_",
                "krakens_", "frogs_", "ophidians_", "centaurs_", "mages_", "fey_race_", "genies_", "paladins_", "shadowlords_",
                "succubi_", "lizards_", "rodents_", "birds_", "bovines_", "bruins_", "canines_", "deer_", "equines_", "felines_",
                "fowl_", "gorillas_", "kirin_", "llamas_", "ostards_", "porcines_", "ruminants_", "walrus_", "dolphins_",
                "sea_horse_", "sea_serpents_", "character_", "h_", "titans_"
            };

            private readonly int m_Body;
            private readonly int m_ItemID;
            private readonly string m_Name;
            private readonly string m_DisplayName;

            public InternalEntry(int body, int itemID, string name)
            {
                m_Body = body;
                m_ItemID = itemID;
                m_Name = name;

                m_DisplayName = name.ToLower();

                for (int i = 0; i < m_GroupNames.Length; ++i)
                {
                    if (m_DisplayName.StartsWith(m_GroupNames[i]))
                    {
                        m_DisplayName = m_DisplayName.Substring(m_GroupNames[i].Length);
                        break;
                    }
                }

                m_DisplayName = m_DisplayName.Replace('_', ' ');
            }

            public int Body => m_Body;
            public int ItemID => m_ItemID;
            public string Name => m_Name;
            public string DisplayName => m_DisplayName;

            public int CompareTo(object obj)
            {
                InternalEntry comp = (InternalEntry)obj;

                int v = m_Name.CompareTo(comp.m_Name);

                if (v == 0)
                    return m_Body.CompareTo(comp.m_Body);

                return v;
            }
        }
    }
}
