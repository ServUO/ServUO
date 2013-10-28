using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class SpecialBeardDye : Item
    {
        [Constructable]
        public SpecialBeardDye()
            : base(0xE26)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Newbied;
        }

        public SpecialBeardDye(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041087;
            }
        }// Special Beard Dye
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.CloseGump(typeof(SpecialBeardDyeGump));
                from.SendGump(new SpecialBeardDyeGump(this));
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 906, 1019045); // I can't reach that.
            }
        }
    }

    public class SpecialBeardDyeGump : Gump
    {
        private static readonly SpecialBeardDyeEntry[] m_Entries = new SpecialBeardDyeEntry[]
        {
            new SpecialBeardDyeEntry("*****", 12, 10),
            new SpecialBeardDyeEntry("*****", 32, 5),
            new SpecialBeardDyeEntry("*****", 38, 8),
            new SpecialBeardDyeEntry("*****", 54, 3),
            new SpecialBeardDyeEntry("*****", 62, 10),
            new SpecialBeardDyeEntry("*****", 81, 2),
            new SpecialBeardDyeEntry("*****", 89, 2),
            new SpecialBeardDyeEntry("*****", 1153, 2)
        };
        private readonly SpecialBeardDye m_SpecialBeardDye;
        public SpecialBeardDyeGump(SpecialBeardDye dye)
            : base(0, 0)
        {
            this.m_SpecialBeardDye = dye;

            this.AddPage(0);
            this.AddBackground(150, 60, 350, 358, 2600);
            this.AddBackground(170, 104, 110, 270, 5100);
            this.AddHtmlLocalized(230, 75, 200, 20, 1011013, false, false);		// Hair Color Selection Menu
            this.AddHtmlLocalized(235, 380, 300, 20, 1013007, false, false);		// Dye my beard this color!
            this.AddButton(200, 380, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);        // DYE HAIR

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                this.AddLabel(180, 109 + (i * 22), m_Entries[i].HueStart - 1, m_Entries[i].Name);
                this.AddButton(257, 110 + (i * 22), 5224, 5224, 0, GumpButtonType.Page, i + 1);
            }

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                SpecialBeardDyeEntry e = m_Entries[i];

                this.AddPage(i + 1);

                for (int j = 0; j < e.HueCount; ++j)
                {
                    this.AddLabel(328 + ((j / 16) * 80), 102 + ((j % 16) * 17), e.HueStart + j - 1, "*****");
                    this.AddRadio(310 + ((j / 16) * 80), 102 + ((j % 16) * 17), 210, 211, false, (i * 100) + j);
                }
            }
        }

        public override void OnResponse(NetState from, RelayInfo info)
        {
            if (this.m_SpecialBeardDye.Deleted)
                return;

            Mobile m = from.Mobile;
            int[] switches = info.Switches;

            if (!this.m_SpecialBeardDye.IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1042010); //You must have the objectin your backpack to use it.
                return;
            }

            if (info.ButtonID != 0 && switches.Length > 0)
            {
                if (m.FacialHairItemID == 0)
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
                        SpecialBeardDyeEntry e = m_Entries[entryIndex];

                        if (hueOffset >= 0 && hueOffset < e.HueCount)
                        {
                            int hue = e.HueStart + hueOffset;

                            m.FacialHairHue = hue;

                            m.SendLocalizedMessage(501199);  // You dye your hair
                            this.m_SpecialBeardDye.Delete();
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

        private class SpecialBeardDyeEntry
        {
            private readonly string m_Name;
            private readonly int m_HueStart;
            private readonly int m_HueCount;
            public SpecialBeardDyeEntry(string name, int hueStart, int hueCount)
            {
                this.m_Name = name;
                this.m_HueStart = hueStart;
                this.m_HueCount = hueCount;
            }

            public string Name
            {
                get
                {
                    return this.m_Name;
                }
            }
            public int HueStart
            {
                get
                {
                    return this.m_HueStart;
                }
            }
            public int HueCount
            {
                get
                {
                    return this.m_HueCount;
                }
            }
        }
    }
}