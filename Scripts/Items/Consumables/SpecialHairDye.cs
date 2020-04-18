using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class SpecialHairDye : Item
    {
        public override int LabelNumber => 1074402;  // Special Hair Dye

        [Constructable]
        public SpecialHairDye()
            : base(0xE26)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public SpecialHairDye(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 1))
            {
                from.CloseGump(typeof(SpecialHairDyeGump));
                from.SendGump(new SpecialHairDyeGump(this));
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 906, 1019045); // I can't reach that.
            }
        }
    }

    public class SpecialHairDyeGump : Gump
    {
        private static readonly SpecialHairDyeEntry[] m_Entries = new SpecialHairDyeEntry[]
        {
            new SpecialHairDyeEntry("*****", 12, 10),
            new SpecialHairDyeEntry("*****", 32, 5),
            new SpecialHairDyeEntry("*****", 38, 8),
            new SpecialHairDyeEntry("*****", 54, 3),
            new SpecialHairDyeEntry("*****", 62, 10),
            new SpecialHairDyeEntry("*****", 81, 2),
            new SpecialHairDyeEntry("*****", 89, 2),
            new SpecialHairDyeEntry("*****", 1153, 2)
        };
        private readonly SpecialHairDye m_SpecialHairDye;
        public SpecialHairDyeGump(SpecialHairDye dye)
            : base(0, 0)
        {
            m_SpecialHairDye = dye;

            AddPage(0);
            AddBackground(150, 60, 350, 358, 2600);
            AddBackground(170, 104, 110, 270, 5100);
            AddHtmlLocalized(230, 75, 200, 20, 1011013, false, false);		// Hair Color Selection Menu
            AddHtmlLocalized(235, 380, 300, 20, 1011014, false, false);		// Dye my hair this color!
            AddButton(200, 380, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);        // DYE HAIR

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                AddLabel(180, 109 + (i * 22), m_Entries[i].HueStart - 1, m_Entries[i].Name);
                AddButton(257, 110 + (i * 22), 5224, 5224, 0, GumpButtonType.Page, i + 1);
            }

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                SpecialHairDyeEntry e = m_Entries[i];

                AddPage(i + 1);

                for (int j = 0; j < e.HueCount; ++j)
                {
                    AddLabel(328 + ((j / 16) * 80), 102 + ((j % 16) * 17), e.HueStart + j - 1, "*****");
                    AddRadio(310 + ((j / 16) * 80), 102 + ((j % 16) * 17), 210, 211, false, (i * 100) + j);
                }
            }
        }

        public override void OnResponse(NetState from, RelayInfo info)
        {
            if (m_SpecialHairDye.Deleted)
                return;

            Mobile m = from.Mobile;
            int[] switches = info.Switches;

            if (!m_SpecialHairDye.IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1042010); //You must have the objectin your backpack to use it.
                return;
            }

            if (info.ButtonID != 0 && switches.Length > 0)
            {
                if (m.HairItemID == 0)
                {
                    m.SendLocalizedMessage(502623);	// You have no hair to dye and cannot use this
                }
                else
                {
                    // To prevent this from being exploited, the hue is abstracted into an internal list
                    int entryIndex = switches[0] / 100;
                    int hueOffset = switches[0] % 100;

                    if (entryIndex >= 0 && entryIndex < m_Entries.Length)
                    {
                        SpecialHairDyeEntry e = m_Entries[entryIndex];

                        if (hueOffset >= 0 && hueOffset < e.HueCount)
                        {
                            m_SpecialHairDye.Delete();

                            int hue = e.HueStart + hueOffset;

                            m.HairHue = hue;

                            m.SendLocalizedMessage(501199);  // You dye your hair
                            m.PlaySound(0x4E);
                        }
                    }
                }
            }
            else
            {
                m.SendLocalizedMessage(501200); // You decide not to dye your hair
            }
        }

        private class SpecialHairDyeEntry
        {
            private readonly string m_Name;
            private readonly int m_HueStart;
            private readonly int m_HueCount;
            public SpecialHairDyeEntry(string name, int hueStart, int hueCount)
            {
                m_Name = name;
                m_HueStart = hueStart;
                m_HueCount = hueCount;
            }

            public string Name => m_Name;
            public int HueStart => m_HueStart;
            public int HueCount => m_HueCount;
        }
    }
}