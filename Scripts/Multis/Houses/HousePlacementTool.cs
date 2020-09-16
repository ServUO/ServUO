using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Targeting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x194B, 0x194C)]
    public class SurveyorsScope : HousePlacementTool
    {
        public override int LabelNumber => 1026475;  // surveyor's scope

        [Constructable]
        public SurveyorsScope()
            : base(0x194B)
        {
        }

        public SurveyorsScope(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class HousePlacementTool : Item
    {
        public virtual bool UseCustomHousePlots => Misc.TestCenter.Enabled;

        [Constructable]
        public HousePlacementTool()
            : this(0x14F6)
        {
        }

        [Constructable]
        public HousePlacementTool(int itemid)
            : base(itemid)
        {
            Weight = 3.0;
            LootType = LootType.Blessed;
        }

        public HousePlacementTool(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1060651; // a house placement tool

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (from.Map == Map.TerMur && !Engines.Points.PointsSystem.QueensLoyalty.IsNoble(from))
                {
                    from.SendLocalizedMessage(1113713); // You must rise to the rank of noble in the eyes of the Gargoyle Queen before her majesty will allow you to build a house in her lands.
                    return;
                }

                from.SendGump(new HousePlacementCategoryGump(this, from));
            }

            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public virtual void OnPlacement(BaseHouse house)
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
    }

    public class HousePlacementCategoryGump : Gump
    {
        private const int LabelColor = 0x7FFF;
        private const int LabelColorDisabled = 0x4210;

        private readonly Mobile m_From;
        private readonly HousePlacementTool m_Tool;

        public HousePlacementCategoryGump(HousePlacementTool tool, Mobile from)
            : base(50, 50)
        {
            m_From = from;
            m_Tool = tool;

            from.CloseGump(typeof(HousePlacementCategoryGump));
            from.CloseGump(typeof(HousePlacementListGump));
            from.CloseGump(typeof(HouseSwapGump));

            AddPage(0);

            AddBackground(0, 0, 270, 170, 5054);

            AddImageTiled(10, 10, 250, 150, 2624);
            AddAlphaRegion(10, 10, 250, 150);

            AddHtmlLocalized(10, 10, 250, 20, 1060239, LabelColor, false, false); // <CENTER>HOUSE PLACEMENT TOOL</CENTER>

            AddButton(10, 130, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 130, 150, 20, 3000363, LabelColor, false, false); // Close

            AddPage(1);

            AddButton(10, 40, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 40, 200, 20, 1060390, LabelColor, false, false); // Classic Houses

            AddButton(10, 60, 4005, 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 60, 200, 20, 1060391, LabelColor, false, false); // 2-Story Customizable Houses

            AddButton(10, 80, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 80, 200, 20, 1060392, LabelColor, false, false); // 3-Story Customizable Houses

            if (m_Tool.UseCustomHousePlots || from.AccessLevel > AccessLevel.Counselor)
            {
                AddButton(10, 100, 4005, 4007, 4, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 100, 200, 20, 1158540, LabelColor, false, false); // Custom House Contest
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (!m_From.CheckAlive() || m_From.Backpack == null || !m_Tool.IsChildOf(m_From.Backpack))
                return;

            switch (info.ButtonID)
            {
                case 1: // Classic Houses
                    {
                        m_From.SendGump(new HousePlacementListGump(m_Tool, m_From, HousePlacementEntry.PreBuiltHouses, true));
                        break;
                    }
                case 2: // 2-Story Customizable Houses
                    {
                        m_From.SendGump(new HousePlacementListGump(m_Tool, m_From, HousePlacementEntry.TwoStoryFoundations));
                        break;
                    }
                case 3: // 3-Story Customizable Houses
                    {
                        m_From.SendGump(new HousePlacementListGump(m_Tool, m_From, HousePlacementEntry.ThreeStoryFoundations));
                        break;
                    }
                case 4: // Custom House Contest
                    {
                        if (m_Tool.UseCustomHousePlots || m_From.AccessLevel > AccessLevel.Player)
                        {
                            m_From.SendGump(new HousePlacementListGump(m_Tool, m_From, HousePlacementEntry.CustomHouseContest));
                        }
                        break;
                    }
            }
        }
    }

    public class HousePlacementListGump : Gump
    {
        private const int LabelColor = 0x7FFF;
        private const int LabelHue = 0x481;

        private readonly Mobile m_From;
        private readonly HousePlacementEntry[] m_Entries;
        private readonly HousePlacementTool m_Tool;

        private readonly bool m_Classic;

        public HousePlacementListGump(HousePlacementTool tool, Mobile from, HousePlacementEntry[] entries, bool classic = false)
            : base(50, 50)
        {
            m_From = from;
            m_Tool = tool;
            m_Entries = entries;
            m_Classic = classic;

            from.CloseGump(typeof(HousePlacementCategoryGump));
            from.CloseGump(typeof(HousePlacementListGump));
            from.CloseGump(typeof(HouseSwapGump));

            AddPage(0);

            AddBackground(0, 0, 530, 430, 5054);

            AddImageTiled(10, 10, 500, 20, 2624);
            AddAlphaRegion(10, 10, 500, 20);

            AddHtmlLocalized(10, 10, 500, 20, 1060239, LabelColor, false, false); // <CENTER>HOUSE PLACEMENT TOOL</CENTER>

            AddImageTiled(10, 40, 500, 20, 2624);
            AddAlphaRegion(10, 40, 500, 20);

            AddHtmlLocalized(50, 40, 225, 20, 1060235, LabelColor, false, false); // House Description
            AddHtmlLocalized(275, 40, 75, 20, 1060236, LabelColor, false, false); // Storage
            AddHtmlLocalized(350, 40, 75, 20, 1060237, LabelColor, false, false); // Lockdowns
            AddHtmlLocalized(425, 40, 75, 20, 1060034, LabelColor, false, false); // Cost

            AddImageTiled(10, 70, 500, 280, 2624);
            AddAlphaRegion(10, 70, 500, 280);

            AddImageTiled(10, 370, 500, 20, 2624);
            AddAlphaRegion(10, 370, 500, 20);

            AddHtmlLocalized(10, 370, 250, 20, 1060645, LabelColor, false, false); // Bank Balance:
            AddLabel(250, 370, LabelHue, Banker.GetBalance(from).ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")));

            AddImageTiled(10, 400, 500, 20, 2624);
            AddAlphaRegion(10, 400, 500, 20);

            AddButton(10, 400, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 400, 100, 20, 3000363, LabelColor, false, false); // Close

            int page = 1;
            int index = -1;

            for (int i = 0; i < entries.Length; ++i)
            {
                CheckPage(i, ref page, ref index);

                if (index == 0)
                {
                    if (page > 1)
                    {
                        AddButton(450, 400, 4005, 4007, 0, GumpButtonType.Page, page);
                        AddHtmlLocalized(400, 400, 100, 20, 3000406, LabelColor, false, false); // Next
                    }

                    AddPage(page);

                    if (page > 1)
                    {
                        AddButton(200, 400, 4014, 4016, 0, GumpButtonType.Page, page - 1);
                        AddHtmlLocalized(250, 400, 100, 20, 3000405, LabelColor, false, false); // Previous
                    }
                }

                HousePlacementEntry entry = entries[i];

                int y = 70 + (index * 20);
                int storage = (int)(entry.Storage * BaseHouse.GlobalBonusStorageScalar);
                int lockdowns = (int)(entry.Lockdowns * BaseHouse.GlobalBonusStorageScalar);

                AddButton(10, y, 4005, 4007, 1 + i, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, y, 225, 20, entry.Description, LabelColor, false, false);
                AddLabel(275, y, LabelHue, storage.ToString());
                AddLabel(350, y, LabelHue, lockdowns.ToString());
                AddLabel(425, y, LabelHue, entry.Cost.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")));
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (!m_From.CheckAlive() || m_From.Backpack == null || !m_Tool.IsChildOf(m_From.Backpack))
                return;

            int index = info.ButtonID - 1;

            if (index >= 0 && index < m_Entries.Length)
            {
                m_From.Target = new NewHousePlacementTarget(m_Tool, m_Entries, m_Entries[index], m_Classic);
            }
            else if (m_Tool != null && m_Tool.GetType() == typeof(HousePlacementTool))
            {
                m_From.SendGump(new HousePlacementCategoryGump(m_Tool, m_From));
            }
        }

        private void CheckPage(int i, ref int page, ref int index)
        {
            if (m_Classic)
            {
                if (i == 8)
                {
                    page = 2;
                    index = 0;
                }
                else if (i == 20)
                {
                    page = 3;
                    index = 0;
                }
                else if (i == 32)
                {
                    page = 4;
                    index = 0;
                }
                else if (i == 44)
                {
                    page = 5;
                    index = 0;
                }
                else
                {
                    index++;
                }
            }
            else
            {
                page = 1 + (i / 14);
                index = i % 14;
            }
        }
    }

    public class NewHousePlacementTarget : MultiTarget
    {
        private readonly HousePlacementEntry m_Entry;
        private readonly HousePlacementEntry[] m_Entries;

        private bool m_Placed;
        private readonly bool m_Classic;
        private readonly HousePlacementTool m_Tool;

        public NewHousePlacementTarget(HousePlacementTool tool, HousePlacementEntry[] entries, HousePlacementEntry entry, bool classic)
            : base(entry.MultiID, entry.Offset)
        {
            Range = 14;

            m_Tool = tool;
            m_Entries = entries;
            m_Entry = entry;
            m_Classic = classic;
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (!from.CheckAlive() || from.Backpack == null || !m_Tool.IsChildOf(from.Backpack))
                return;

            IPoint3D ip = o as IPoint3D;

            if (ip != null)
            {
                if (ip is Item)
                    ip = ((Item)ip).GetWorldTop();

                Point3D p = new Point3D(ip);

                Region reg = Region.Find(new Point3D(p), from.Map);

                if (from.AccessLevel >= AccessLevel.GameMaster || reg.AllowHousing(from, p))
                    m_Placed = m_Entry.OnPlacement(m_Tool, from, p);
                else if (reg.IsPartOf<TempNoHousingRegion>())
                    from.SendLocalizedMessage(501270); // Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
                else if (reg.IsPartOf<HouseRegion>())
                    from.SendLocalizedMessage(1043287); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
                else if (reg.IsPartOf<HouseRaffleRegion>())
                    from.SendLocalizedMessage(1150493); // You must have a deed for this plot of land in order to build here.
                else
                    from.SendLocalizedMessage(501265); // Housing can not be created in this area.
            }
        }

        protected override void OnTargetFinish(Mobile from)
        {
            if (!from.CheckAlive() || from.Backpack == null || !m_Tool.IsChildOf(from.Backpack))
                return;

            if (!m_Placed)
                from.SendGump(new HousePlacementListGump(m_Tool, from, m_Entries, m_Classic));
        }
    }

    public class HousePlacementEntry
    {
        private static readonly HousePlacementEntry[] m_PreBuiltHouses =
        {
            new HousePlacementEntry(typeof(SmallOldHouse),      1011303,    425,    212,    489,    244,    10, 36750, 0,   4,  0,  0x0064),
            new HousePlacementEntry(typeof(SmallOldHouse),      1011304,    425,    212,    489,    244,    10, 36750, 0,   4,  0,  0x0066),
            new HousePlacementEntry(typeof(SmallOldHouse),      1011305,    425,    212,    489,    244,    10, 36500, 0,   4,  0,  0x0068),
            new HousePlacementEntry(typeof(SmallOldHouse),      1011306,    425,    212,    489,    244,    10, 35000, 0,   4,  0,  0x006A),
            new HousePlacementEntry(typeof(SmallOldHouse),      1011307,    425,    212,    489,    244,    10, 36500, 0,   4,  0,  0x006C),
            new HousePlacementEntry(typeof(SmallOldHouse),      1011308,    425,    212,    489,    244,    10, 36500, 0,   4,  0,  0x006E),
            new HousePlacementEntry(typeof(SmallShop),          1011321,    425,    212,    489,    244,    10, 50250, -1,  4,  0,  0x00A0),
            new HousePlacementEntry(typeof(SmallShop),          1011322,    425,    212,    489,    244,    10, 52250, 0,   4,  0,  0x00A2),
            new HousePlacementEntry(typeof(SmallTower),         1011317,    580,    290,    667,    333,    14, 73250, 3,   4,  0,  0x0098),
            new HousePlacementEntry(typeof(TwoStoryVilla),      1011319,    1100,   550,    1265,   632,    24, 113500, 3,  6,  0,  0x009E),
            new HousePlacementEntry(typeof(SandStonePatio),     1011320,    850,    425,    1265,   632,    24, 76250, -1,  4,  0,  0x009C),
            new HousePlacementEntry(typeof(LogCabin),           1011318,    1100,   550,    1265,   632,    24, 81250, 1,   6,  0,  0x009A),
            new HousePlacementEntry(typeof(GuildHouse),         1011309,    1370,   685,    1576,   788,    28, 131250, -1, 7,  0,  0x0074),
            new HousePlacementEntry(typeof(TwoStoryHouse),      1011310,    1370,   685,    1576,   788,    28, 162500, -3, 7,  0,  0x0076),
            new HousePlacementEntry(typeof(TwoStoryHouse),      1011311,    1370,   685,    1576,   788,    28, 162750, -3, 7,  0,  0x0078),
            new HousePlacementEntry(typeof(LargePatioHouse),    1011315,    1370,   685,    1576,   788,    28, 129000, -4, 7,  0,  0x008C),
            new HousePlacementEntry(typeof(LargeMarbleHouse),   1011316,    1370,   685,    1576,   788,    28, 160250, -4, 7,  0,  0x0096),
            new HousePlacementEntry(typeof(Tower),              1011312,    2119,   1059,   2437,   1218,   42, 366250, 0,  7,  0,  0x007A),
            new HousePlacementEntry(typeof(Keep),               1011313,    2625,   1312,   3019,   1509,   52, 562500, 0, 11,  0,  0x007C),
            new HousePlacementEntry(typeof(Castle),             1011314,    4076,   2038,   4688,   2344,   78, 865000, 0, 16,  0,  0x007E),

            new HousePlacementEntry(typeof(TrinsicKeep),        1158748,    2625,   1312,   3019,   1509,   52, 29643750, 0, 11,    0,  0x147E),
            new HousePlacementEntry(typeof(GothicRoseCastle),   1158749,    4076,   2038,   4688,   2344,   78, 44808750, 0, 16,    0,  0x147F),
            new HousePlacementEntry(typeof(ElsaCastle),         1158750,    4076,   2038,   4688,   2344,   78, 45450000, 0, 16,    0,  0x1480),
            new HousePlacementEntry(typeof(Spires),             1158761,    4076,   2038,   4688,   2344,   78, 47025000, 0, 16,    0,  0x1481),
            new HousePlacementEntry(typeof(CastleOfOceania),    1158760,    4076,   2038,   4688,   2344,   78, 48971250, 0, 16,    0,  0x1482),
            new HousePlacementEntry(typeof(FeudalCastle),       1158762,    4076,   2038,   4688,   2344,   78, 27337500, 0, 16,    0,  0x1483),
            new HousePlacementEntry(typeof(RobinsNest),         1158850,    2625,   1312,   3019,   1509,   52, 25301250, 0, 11,    0,  0x1484),
            new HousePlacementEntry(typeof(TraditionalKeep),    1158851,    2625,   1312,   3019,   1509,   52, 26685000, 0, 11,    0,  0x1485),
            new HousePlacementEntry(typeof(VillaCrowley),       1158852,    2625,   1312,   3019,   1509,   52, 21813750, 0, 11,    0,  0x1486),
            new HousePlacementEntry(typeof(DarkthornKeep),      1158853,    2625,   1312,   3019,   1509,   52, 27990000, 0, 11,    0,  0x1487),
            new HousePlacementEntry(typeof(SandalwoodKeep),     1158854,    2625,   1312,   3019,   1509,   52, 23456250, 0, 11,    0,  0x1488),
            new HousePlacementEntry(typeof(CasaMoga),           1158855,    2625,   1312,   3019,   1509,   52, 26313750, 0, 11,    0,  0x1489),

            new HousePlacementEntry(typeof(RobinsRoost),                1158960,    4076,   2038,   4688,   2344,   78, 43863750, 0, 16,    0,  0x148A),
            new HousePlacementEntry(typeof(Camelot),                    1158961,    4076,   2038,   4688,   2344,   78, 47092500, 0, 16,    0,  0x148B),
            new HousePlacementEntry(typeof(LacrimaeInCaelo),            1158962,    4076,   2038,   4688,   2344,   78, 45315000, 0, 16,    0,  0x148C),
            new HousePlacementEntry(typeof(OkinawaSweetDreamCastle),    1158963,    4076,   2038,   4688,   2344,   78, 40128750, 0, 16,    0,  0x148D),
            new HousePlacementEntry(typeof(TheSandstoneCastle),         1158964,    4076,   2038,   4688,   2344,   78, 48690000, 0, 16,    0,  0x148E),
            new HousePlacementEntry(typeof(GrimswindSisters),           1158965,    4076,   2038,   4688,   2344,   78, 42142500, 0, 16,    0,  0x148F),

            new HousePlacementEntry(typeof(FortressOfLestat),           1159050,    2625,   1312,   3019,   1509,   52, 27405000, 0, 11,    0,  0x1490),
            new HousePlacementEntry(typeof(CitadelOfTheFarEast),        1159051,    2625,   1312,   3019,   1509,   52, 29036250, 0, 11,    0,  0x1491),
            new HousePlacementEntry(typeof(KeepIncarcerated),           1159052,    2625,   1312,   3019,   1509,   52, 26291250, 0, 11,    0,  0x1492),
            new HousePlacementEntry(typeof(DesertRose),                 1159054,    2625,   1312,   3019,   1509,   52, 21206250, 0, 11,    0,  0x1494),
            new HousePlacementEntry(typeof(SallyTreesRefurbishedKeep),  1159053,    2625,   1312,   3019,   1509,   52, 29688750, 0, 11,    0,  0x1493),
            new HousePlacementEntry(typeof(TheCloversKeep),             1159055,    2625,   1312,   3019,   1509,   52, 27360000, 0, 11,    0,  0x1495),

            new HousePlacementEntry(typeof(TheSorceresCastle),          1159264,    4076,   2038,   4688,   2344,   78, 40924500, 0, 16,    0,  0x1496),
            new HousePlacementEntry(typeof(TheCastleCascade),           1159265,    4076,   2038,   4688,   2344,   78, 48217500, 0, 16,    0,  0x1497),
            new HousePlacementEntry(typeof(TheHouseBuiltOnTheRuins),    1159266,    4076,   2038,   4688,   2344,   78, 42255000, 0, 16,    0,  0x1498),
            new HousePlacementEntry(typeof(TheSandstoneFortressOfGrand),1159267,    4076,   2038,   4688,   2344,   78, 48498750, 0, 16,    0,  0x1499),
            new HousePlacementEntry(typeof(TheDragonstoneCastle),       1159268,    4076,   2038,   4688,   2344,   78, 39588750, 0, 16,    0,  0x149A),
            new HousePlacementEntry(typeof(TheTerraceGardens),          1159269,    4076,   2038,   4688,   2344,   78, 46136250, 0, 16,    0,  0x149B),
            new HousePlacementEntry(typeof(TheKeepCalmAndCarryOnKeep),  1159414,    2625,   1312,   3019,   1509,   52, 23006250, 0, 11,    0,  0x149C),
            new HousePlacementEntry(typeof(TheRavenloftKeep),           1159415,    2625,   1312,   3019,   1509,   52, 24457500, 0, 11,    0,  0x149D),
            new HousePlacementEntry(typeof(TheQueensRetreatKeep),       1159416,    2625,   1312,   3019,   1509,   52, 27641250, 0, 11,    0,  0x149E),
        };

        private static readonly HousePlacementEntry[] m_CustomHouseContest = new HousePlacementEntry[]
        {
            new HousePlacementEntry(typeof(HouseFoundation), 1158538,   2625,   1312,   3019,   1509,   78, 525000, 0,  10, 0,  0x147C), // 23x23 3-Story Customizable Keep
            new HousePlacementEntry(typeof(HouseFoundation), 1158539,   4076,   2038,   4688,   2344,   78, 525000, 0,  10, 0,  0x147D),  // 32x32 3-Story Customizable Castle
        };

        private static readonly HousePlacementEntry[] m_TwoStoryFoundations = new HousePlacementEntry[]
        {
            new HousePlacementEntry(typeof(HouseFoundation), 1060241,   425,    212,    489,    244,    10, 33000, 0,   4,  0,  0x13EC), // 7x7 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060242,   580,    290,    667,    333,    14, 37000, 0,   5,  0,  0x13ED), // 7x8 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060243,   650,    325,    748,    374,    16, 41000, 0,   5,  0,  0x13EE), // 7x9 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060244,   700,    350,    805,    402,    16, 45000, 0,   6,  0,  0x13EF), // 7x10 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060245,   750,    375,    863,    431,    16, 49000, 0,   6,  0,  0x13F0), // 7x11 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060246,   800,    400,    920,    460,    18, 53000, 0,   7,  0,  0x13F1), // 7x12 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060253,   580,    290,    667,    333,    14, 37500, 0,   4,  0,  0x13F8), // 8x7 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060254,   650,    325,    748,    374,    16, 42000, 0,   5,  0,  0x13F9), // 8x8 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060255,   700,    350,    805,    402,    16, 46500, 0,   5,  0,  0x13FA), // 8x9 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060256,   750,    375,    863,    431,    16, 51000, 0,   6,  0,  0x13FB), // 8x10 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060257,   800,    400,    920,    460,    18, 55500, 0,   6,  0,  0x13FC), // 8x11 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060258,   850,    425,    1265,   632,    24, 60000, 0,   7,  0,  0x13FD), // 8x12 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060259,   1100,   550,    1265,   632,    24, 64500, 0,   7,  0,  0x13FE), // 8x13 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060265,   650,    325,    748,    374,    16, 42000, 0,   4,  0,  0x1404), // 9x7 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060266,   700,    350,    805,    402,    16, 47000, 0,   5,  0,  0x1405), // 9x8 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060267,   750,    375,    863,    431,    16, 52000, 0,   5,  0,  0x1406), // 9x9 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060268,   800,    400,    920,    460,    18, 57000, 0,   6,  0,  0x1407), // 9x10 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060269,   850,    425,    1265,   632,    24, 62000, 0,   6,  0,  0x1408), // 9x11 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060270,   1100,   550,    1265,   632,    24, 67000, 0,   7,  0,  0x1409), // 9x12 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060271,   1100,   550,    1265,   632,    24, 72000, 0,   7,  0,  0x140A), // 9x13 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060277,   700,    350,    805,    402,    16, 46500, 0,   4,  0,  0x1410), // 10x7 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060278,   750,    375,    863,    431,    16, 52000, 0,   5,  0,  0x1411), // 10x8 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060279,   800,    400,    920,    460,    18, 57500, 0,   5,  0,  0x1412), // 10x9 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060280,   850,    425,    1265,   632,    24, 63000, 0,   6,  0,  0x1413), // 10x10 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060281,   1100,   550,    1265,   632,    24, 68500, 0,   6,  0,  0x1414), // 10x11 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060282,   1100,   550,    1265,   632,    24, 74000, 0,   7,  0,  0x1415), // 10x12 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060283,   1150,   575,    1323,   661,    24, 79500, 0,   7,  0,  0x1416), // 10x13 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060289,   750,    375,    863,    431,    16, 51000, 0,   4,  0,  0x141C), // 11x7 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060290,   800,    400,    920,    460,    18, 57000, 0,   5,  0,  0x141D), // 11x8 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060291,   850,    425,    1265,   632,    24, 63000, 0,   5,  0,  0x141E), // 11x9 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060292,   1100,   550,    1265,   632,    24, 69000, 0,   6,  0,  0x141F), // 11x10 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060293,   1100,   550,    1265,   632,    24, 75000, 0,   6,  0,  0x1420), // 11x11 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060294,   1150,   575,    1323,   661,    24, 81000, 0,   7,  0,  0x1421), // 11x12 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060295,   1200,   600,    1380,   690,    26, 87000, 0,   7,  0,  0x1422), // 11x13 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060301,   800,    400,    920,    460,    18, 55500, 0,   4,  0,  0x1428), // 12x7 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060302,   850,    425,    1265,   632,    24, 62000, 0,   5,  0,  0x1429), // 12x8 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060303,   1100,   550,    1265,   632,    24, 68500, 0,   5,  0,  0x142A), // 12x9 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060304,   1100,   550,    1265,   632,    24, 75000, 0,   6,  0,  0x142B), // 12x10 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060305,   1150,   575,    1323,   661,    24, 81500, 0,   6,  0,  0x142C), // 12x11 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060306,   1200,   600,    1380,   690,    26, 88000, 0,   7,  0,  0x142D), // 12x12 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060307,   1250,   625,    1438,   719,    26, 94500, 0,   7,  0,  0x142E), // 12x13 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060314,   1100,   550,    1265,   632,    24, 67000, 0,   5,  0,  0x1435), // 13x8 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060315,   1100,   550,    1265,   632,    24, 74000, 0,   5,  0,  0x1436), // 13x9 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060316,   1150,   575,    1323,   661,    24, 81000, 0,   6,  0,  0x1437), // 13x10 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060317,   1200,   600,    1380,   690,    26, 88000, 0,   6,  0,  0x1438), // 13x11 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060318,   1250,   625,    1438,   719,    26, 95000, 0,   7,  0,  0x1439), // 13x12 2-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060319,   1300,   650,    1495,   747,    28, 102000, 0,  7,  0,  0x143A)// 13x13 2-Story Customizable House
        };

        private static readonly HousePlacementEntry[] m_ThreeStoryFoundations = new HousePlacementEntry[]
        {
            new HousePlacementEntry(typeof(HouseFoundation), 1060272,   1150,   575,    1323,   661,    24, 77000, 0,   8,  0,  0x140B), // 9x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060284,   1200,   600,    1380,   690,    26, 85000, 0,   8,  0,  0x1417), // 10x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060285,   1250,   625,    1438,   719,    26, 90500, 0,   8,  0,  0x1418), // 10x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060296,   1250,   625,    1438,   719,    26, 93000, 0,   8,  0,  0x1423), // 11x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060297,   1300,   650,    1495,   747,    28, 99000, 0,   8,  0,  0x1424), // 11x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060298,   1350,   675,    1553,   776,    28, 105000, 0,  9,  0,  0x1425), // 11x16 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060308,   1300,   650,    1495,   747,    28, 101000, 0,  8,  0,  0x142F), // 12x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060309,   1350,   675,    1553,   776,    28, 107500, 0,  8,  0,  0x1430), // 12x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060310,   1370,   685,    1576,   788,    28, 114000, 0,  9,  0,  0x1431), // 12x16 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060311,   1370,   685,    1576,   788,    28, 120500, 0,  9,  0,  0x1432), // 12x17 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060320,   1350,   675,    1553,   776,    28, 109000, 0,  8,  0,  0x143B), // 13x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060321,   1370,   685,    1576,   788,    28, 116000, 0,  8,  0,  0x143C), // 13x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060322,   1370,   685,    1576,   788,    28, 123000, 0,  9,  0,  0x143D), // 13x16 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060323,   2119,   1059,   2437,   1218,   42, 130000, 0,  9,  0,  0x143E), // 13x17 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060324,   2119,   1059,   2437,   1218,   42, 137000, 0,  10, 0,  0x143F), // 13x18 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060327,   1150,   575,    1323,   661,    24, 79000, 0,   5,  0,  0x1442), // 14x9 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060328,   1200,   600,    1380,   690,    26, 87000, 0,   6,  0,  0x1443), // 14x10 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060329,   1250,   625,    1438,   719,    26, 94500, 0,   6,  0,  0x1444), // 14x11 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060330,   1300,   650,    1495,   747,    28, 102000, 0,  7,  0,  0x1445), // 14x12 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060331,   1350,   675,    1553,   776,    28, 109500, 0,  7,  0,  0x1446), // 14x13 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060332,   1370,   685,    1576,   788,    28, 117000, 0,  8,  0,  0x1447), // 14x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060333,   1370,   685,    1576,   788,    28, 124500, 0,  8,  0,  0x1448), // 14x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060334,   2119,   1059,   2437,   1218,   42, 132000, 0,  9,  0,  0x1449), // 14x16 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060335,   2119,   1059,   2437,   1218,   42, 139500, 0,  9,  0,  0x144A), // 14x17 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060336,   2119,   1059,   2437,   1218,   42, 147000, 0,  10, 0,  0x144B), // 14x18 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060340,   1250,   625,    1438,   719,    26, 93000, 0,   6,  0,  0x144F), // 15x10 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060341,   1300,   650,    1495,   747,    28, 101000, 0,  6,  0,  0x1450), // 15x11 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060342,   1350,   675,    1553,   776,    28, 109000, 0,  7,  0,  0x1451), // 15x12 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060343,   1370,   685,    1576,   788,    28, 117000, 0,  7,  0,  0x1452), // 15x13 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060344,   1370,   685,    1576,   788,    28, 125000, 0,  8,  0,  0x1453), // 15x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060345,   2119,   1059,   2437,   1218,   42, 133000, 0,  8,  0,  0x1454), // 15x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060346,   2119,   1059,   2437,   1218,   42, 141000, 0,  9,  0,  0x1455), // 15x16 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060347,   2119,   1059,   2437,   1218,   42, 149000, 0,  9,  0,  0x1456), // 15x17 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060348,   2119,   1059,   2437,   1218,   42, 157000, 0,  10, 0,  0x1457), // 15x18 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060353,   1350,   675,    1553,   776,    28, 107500, 0,  6,  0,  0x145C), // 16x11 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060354,   1370,   685,    1576,   788,    28, 116000, 0,  7,  0,  0x145D), // 16x12 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060355,   1370,   685,    1576,   788,    28, 124500, 0,  7,  0,  0x145E), // 16x13 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060356,   2119,   1059,   2437,   1218,   42, 133000, 0,  8,  0,  0x145F), // 16x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060357,   2119,   1059,   2437,   1218,   42, 141500, 0,  8,  0,  0x1460), // 16x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060358,   2119,   1059,   2437,   1218,   42, 150000, 0,  9,  0,  0x1461), // 16x16 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060359,   2119,   1059,   2437,   1218,   42, 158500, 0,  9,  0,  0x1462), // 16x17 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060360,   2119,   1059,   2437,   1218,   42, 167000, 0,  10, 0,  0x1463), // 16x18 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060366,   1370,   685,    1576,   788,    28, 123000, 0,  7,  0,  0x1469), // 17x12 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060367,   2119,   1059,   2437,   1218,   42, 132000, 0,  7,  0,  0x146A), // 17x13 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060368,   2119,   1059,   2437,   1218,   42, 141000, 0,  8,  0,  0x146B), // 17x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060369,   2119,   1059,   2437,   1218,   42, 150000, 0,  8,  0,  0x146C), // 17x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060370,   2119,   1059,   2437,   1218,   42, 159000, 0,  9,  0,  0x146D), // 17x16 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060371,   2119,   1059,   2437,   1218,   42, 168000, 0,  9,  0,  0x146E), // 17x17 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060372,   2119,   1059,   2437,   1218,   42, 177000, 0,  10, 0,  0x146F), // 17x18 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060379,   2119,   1059,   2437,   1218,   42, 139500, 0,  7,  0,  0x1476), // 18x13 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060380,   2119,   1059,   2437,   1218,   42, 149000, 0,  8,  0,  0x1477), // 18x14 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060381,   2119,   1059,   2437,   1218,   42, 158500, 0,  8,  0,  0x1478), // 18x15 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060382,   2119,   1059,   2437,   1218,   42, 168000, 0,  9,  0,  0x1479), // 18x16 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060383,   2119,   1059,   2437,   1218,   42, 177500, 0,  9,  0,  0x147A), // 18x17 3-Story Customizable House
            new HousePlacementEntry(typeof(HouseFoundation), 1060384,   2119,   1059,   2437,   1218,   42, 187000, 0,  10, 0,  0x147B)// 18x18 3-Story Customizable House
        };

        private static readonly Hashtable m_Table;
        private readonly Type m_Type;
        private readonly int m_Description;
        private readonly int m_Storage;
        private readonly int m_Lockdowns;
        private readonly int m_NewStorage;
        private readonly int m_NewLockdowns;
        private readonly int m_Vendors;
        private readonly int m_Cost;
        private readonly int m_MultiID;
        private readonly Point3D m_Offset;

        public HousePlacementEntry(Type type, int description, int storage, int lockdowns, int newStorage, int newLockdowns, int vendors, int cost, int xOffset, int yOffset, int zOffset, int multiID)
        {
            m_Type = type;
            m_Description = description;
            m_Storage = storage;
            m_Lockdowns = lockdowns;
            m_NewStorage = newStorage;
            m_NewLockdowns = newLockdowns;
            m_Vendors = vendors;

            m_Cost = Siege.SiegeShard ? cost * 2 : cost;

            m_Offset = new Point3D(xOffset, yOffset, zOffset);

            m_MultiID = multiID;
        }

        static HousePlacementEntry()
        {
            m_Table = new Hashtable();

            FillTable(m_PreBuiltHouses);

            FillTable(m_TwoStoryFoundations);
            FillTable(m_ThreeStoryFoundations);
            FillTable(m_CustomHouseContest);
        }

        public static HousePlacementEntry[] PreBuiltHouses => m_PreBuiltHouses;
        public static HousePlacementEntry[] TwoStoryFoundations => m_TwoStoryFoundations;
        public static HousePlacementEntry[] ThreeStoryFoundations => m_ThreeStoryFoundations;
        public static HousePlacementEntry[] CustomHouseContest => m_CustomHouseContest;

        public Type Type => m_Type;
        public int Description => m_Description;
        public int Storage => m_NewStorage;
        public int Lockdowns => m_NewLockdowns;
        public int Vendors => m_Vendors;
        public int Cost => m_Cost;
        public int MultiID => m_MultiID;
        public Point3D Offset => m_Offset;
        public static HousePlacementEntry Find(BaseHouse house)
        {
            object obj = m_Table[house.GetType()];

            if (obj is HousePlacementEntry)
            {
                return ((HousePlacementEntry)obj);
            }
            else if (obj is ArrayList)
            {
                ArrayList list = (ArrayList)obj;

                for (int i = 0; i < list.Count; ++i)
                {
                    HousePlacementEntry e = (HousePlacementEntry)list[i];

                    if (e.m_MultiID == house.ItemID)
                        return e;
                }
            }
            else if (obj is Hashtable)
            {
                Hashtable table = (Hashtable)obj;

                obj = table[house.ItemID];

                if (obj is HousePlacementEntry)
                    return (HousePlacementEntry)obj;
            }

            return null;
        }

        public BaseHouse ConstructHouse(Mobile from)
        {
            try
            {
                object[] args;

                if (m_Type == typeof(HouseFoundation))
                    args = new object[4] { from, m_MultiID, m_Storage, m_Lockdowns };
                else if (m_Type == typeof(SmallOldHouse) || m_Type == typeof(SmallShop) || m_Type == typeof(TwoStoryHouse))
                    args = new object[2] { from, m_MultiID };
                else
                    args = new object[1] { from };

                return Activator.CreateInstance(m_Type, args) as BaseHouse;
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }

            return null;
        }

        public void PlacementWarning_Callback(Mobile from, bool okay, object state)
        {
            object[] objs = (object[])state;

            PreviewHouse prevHouse = (PreviewHouse)objs[0];
            HousePlacementTool tool = objs[1] as HousePlacementTool;

            if (!from.CheckAlive() || from.Backpack == null || tool == null || !tool.IsChildOf(from.Backpack))
                return;

            if (!okay)
            {
                prevHouse.Delete();
                return;
            }

            if (prevHouse.Deleted)
            {
                /* Too much time has passed and the test house you created has been deleted.
                * Please try again!
                */
                from.SendGump(new NoticeGump(1060637, 30720, 1060647, 32512, 320, 180, null, null));

                return;
            }

            Point3D center = prevHouse.Location;
            Map map = prevHouse.Map;

            prevHouse.Delete();

            ArrayList toMove;
            //Point3D center = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );
            HousePlacementResult res = HousePlacement.Check(from, m_MultiID, center, out toMove);

            switch (res)
            {
                case HousePlacementResult.Valid:
                    {
                        if (from.AccessLevel > AccessLevel.Player || BaseHouse.CheckAccountHouseLimit(from))
                        {
                            BaseHouse house = ConstructHouse(from);

                            if (house == null)
                                return;

                            house.Price = m_Cost;

                            if (from.AccessLevel >= AccessLevel.GameMaster)
                            {
                                from.SendMessage("{0} gold would have been withdrawn from your bank if you were not a GM.", m_Cost.ToString());
                            }
                            else
                            {
                                if (!Banker.Withdraw(from, m_Cost, true))
                                {
                                    house.Delete();
                                    from.SendLocalizedMessage(1060646); // You do not have the funds available in your bank box to purchase this house.  Try placing a smaller house, or adding gold or checks to your bank box.
                                    return;
                                }
                            }

                            house.MoveToWorld(center, from.Map);

                            if (house is HouseFoundation)
                                ((HouseFoundation)house).OnPlacement();

                            for (int i = 0; i < toMove.Count; ++i)
                            {
                                object o = toMove[i];

                                if (o is Mobile)
                                    ((Mobile)o).Location = house.BanLocation;
                                else if (o is Item)
                                    ((Item)o).Location = house.BanLocation;
                            }

                            if (tool != null)
                            {
                                tool.OnPlacement(house);
                            }
                        }

                        break;
                    }
                case HousePlacementResult.BadItem:
                case HousePlacementResult.BadLand:
                case HousePlacementResult.BadStatic:
                case HousePlacementResult.BadRegionHidden:
                case HousePlacementResult.NoSurface:
                    {
                        from.SendLocalizedMessage(1043287); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
                        break;
                    }
                case HousePlacementResult.BadRegion:
                    {
                        from.SendLocalizedMessage(501265); // Housing cannot be created in this area.
                        break;
                    }
                case HousePlacementResult.BadRegionTemp:
                    {
                        from.SendLocalizedMessage(501270); // Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
                        break;
                    }
                case HousePlacementResult.BadRegionRaffle:
                    {
                        from.SendLocalizedMessage(1150493); // You must have a deed for this plot of land in order to build here.
                        break;
                    }
                case HousePlacementResult.InvalidCastleKeep:
                    {
                        from.SendLocalizedMessage(1061122); // Castles and keeps cannot be created here.
                        break;
                    }
                case HousePlacementResult.NoQueenLoyalty:
                    {
                        from.SendLocalizedMessage(1113707, "10000"); // You must have at lease ~1_MIN~ loyalty to the Gargoyle Queen to place a house in Ter Mur.
                        break;
                    }
            }
        }

        public bool OnPlacement(HousePlacementTool tool, Mobile from, Point3D p)
        {
            if (!from.CheckAlive() || from.Backpack == null || !tool.IsChildOf(from.Backpack))
                return false;

            ArrayList toMove;
            Point3D center = new Point3D(p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z);
            HousePlacementResult res = HousePlacement.Check(from, m_MultiID, center, out toMove);

            switch (res)
            {
                case HousePlacementResult.Valid:
                    {
                        from.SendLocalizedMessage(1011576); // This is a valid location.

                        PreviewHouse prev = new PreviewHouse(m_MultiID);

                        MultiComponentList mcl = prev.Components;

                        Point3D banLoc = new Point3D(center.X + mcl.Min.X, center.Y + mcl.Max.Y + 1, center.Z);

                        for (int i = 0; i < mcl.List.Length; ++i)
                        {
                            MultiTileEntry entry = mcl.List[i];

                            int itemID = entry.m_ItemID;

                            if (itemID >= 0xBA3 && itemID <= 0xC0E)
                            {
                                banLoc = new Point3D(center.X + entry.m_OffsetX, center.Y + entry.m_OffsetY, center.Z);
                                break;
                            }
                        }

                        for (int i = 0; i < toMove.Count; ++i)
                        {
                            object o = toMove[i];

                            if (o is Mobile)
                                ((Mobile)o).Location = banLoc;
                            else if (o is Item)
                                ((Item)o).Location = banLoc;
                        }

                        prev.MoveToWorld(center, from.Map);

                        /* You are about to place a new house.
                        * Placing this house will condemn any and all of your other houses that you may have.
                        * All of your houses on all shards will be affected.
                        * 
                        * In addition, you will not be able to place another house or have one transferred to you for one (1) real-life week.
                        * 
                        * Once you accept these terms, these effects cannot be reversed.
                        * Re-deeding or transferring your new house will not uncondemn your other house(s) nor will the one week timer be removed.
                        * 
                        * If you are absolutely certain you wish to proceed, click the button next to OKAY below.
                        * If you do not wish to trade for this house, click CANCEL.
                        */
                        from.SendGump(new WarningGump(1060635, 30720, 1049583, 32512, 420, 280, PlacementWarning_Callback, new object[] { prev, tool }));

                        return true;
                    }
                case HousePlacementResult.BadItem:
                case HousePlacementResult.BadLand:
                case HousePlacementResult.BadStatic:
                case HousePlacementResult.BadRegionHidden:
                case HousePlacementResult.NoSurface:
                    {
                        from.SendLocalizedMessage(1043287); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
                        break;
                    }
                case HousePlacementResult.BadRegion:
                    {
                        from.SendLocalizedMessage(501265); // Housing cannot be created in this area.
                        break;
                    }
                case HousePlacementResult.BadRegionTemp:
                    {
                        from.SendLocalizedMessage(501270); //Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
                        break;
                    }
                case HousePlacementResult.BadRegionRaffle:
                    {
                        from.SendLocalizedMessage(1150493); // You must have a deed for this plot of land in order to build here.
                        break;
                    }
                case HousePlacementResult.InvalidCastleKeep:
                    {
                        from.SendLocalizedMessage(1061122); // Castles and keeps cannot be created here.
                        break;
                    }
                case HousePlacementResult.NoQueenLoyalty:
                    {
                        from.SendLocalizedMessage(1113707, "10000"); // You must have at lease ~1_MIN~ loyalty to the Gargoyle Queen to place a house in Ter Mur.
                        break;
                    }
            }

            return false;
        }

        private static void FillTable(HousePlacementEntry[] entries)
        {
            for (int i = 0; i < entries.Length; ++i)
            {
                HousePlacementEntry e = entries[i];

                object obj = m_Table[e.m_Type];

                if (obj == null)
                {
                    m_Table[e.m_Type] = e;
                }
                else if (obj is HousePlacementEntry)
                {
                    ArrayList list = new ArrayList();

                    list.Add(obj);
                    list.Add(e);

                    m_Table[e.m_Type] = list;
                }
                else if (obj is ArrayList)
                {
                    ArrayList list = (ArrayList)obj;

                    if (list.Count == 8)
                    {
                        Hashtable table = new Hashtable();

                        for (int j = 0; j < list.Count; ++j)
                            table[((HousePlacementEntry)list[j]).m_MultiID] = list[j];

                        table[e.m_MultiID] = e;

                        m_Table[e.m_Type] = table;
                    }
                    else
                    {
                        list.Add(e);
                    }
                }
                else if (obj is Hashtable)
                {
                    ((Hashtable)obj)[e.m_MultiID] = e;
                }
            }
        }
    }

    public class HouseSwapGump : BaseGump
    {
        private const int LabelColor = 0x7FFF;
        private const int LabelHue = 0x481;

        private readonly Mobile m_From;
        private readonly HousePlacementEntry[] m_Entries;
        private readonly BaseHouse m_House;

        public HouseSwapGump(Mobile from, BaseHouse house, HousePlacementEntry[] entries)
            : base((PlayerMobile)from, 50, 50)
        {
            m_From = from;
            m_Entries = entries;
            m_House = house;

            from.CloseGump(typeof(HousePlacementCategoryGump));
            from.CloseGump(typeof(HousePlacementListGump));
            from.CloseGump(typeof(HouseSwapGump));
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 530, 430, 5054);

            AddImageTiled(10, 10, 500, 20, 2624);
            AddAlphaRegion(10, 10, 500, 20);

            AddHtmlLocalized(10, 10, 500, 20, 1158759, LabelColor, false, false); // <CENTER>SECURE HOUSE REPLACEMENT</CENTER>

            AddImageTiled(10, 40, 500, 20, 2624);
            AddAlphaRegion(10, 40, 500, 20);

            AddHtmlLocalized(50, 40, 225, 20, 1060235, LabelColor, false, false); // House Description
            AddHtmlLocalized(275, 40, 75, 20, 1060236, LabelColor, false, false); // Storage
            AddHtmlLocalized(350, 40, 75, 20, 1060237, LabelColor, false, false); // Lockdowns
            AddHtmlLocalized(425, 40, 75, 20, 1060034, LabelColor, false, false); // Cost

            AddImageTiled(10, 70, 500, 280, 2624);
            AddAlphaRegion(10, 70, 500, 280);

            AddImageTiled(10, 370, 500, 20, 2624);
            AddAlphaRegion(10, 370, 500, 20);

            AddHtmlLocalized(10, 370, 250, 20, 1060645, LabelColor, false, false); // Bank Balance:
            AddLabel(250, 370, LabelHue, Banker.GetBalance(m_From).ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")));

            AddImageTiled(10, 400, 500, 20, 2624);
            AddAlphaRegion(10, 400, 500, 20);

            AddButton(10, 400, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 400, 100, 20, 3000363, LabelColor, false, false); // Close

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                int page = 1 + (i / 14);
                int index = i % 14;

                if (index == 0)
                {
                    if (page > 1)
                    {
                        AddButton(450, 400, 4005, 4007, 0, GumpButtonType.Page, page);
                        AddHtmlLocalized(400, 400, 100, 20, 3000406, LabelColor, false, false); // Next
                    }

                    AddPage(page);

                    if (page > 1)
                    {
                        AddButton(200, 400, 4014, 4016, 0, GumpButtonType.Page, page - 1);
                        AddHtmlLocalized(250, 400, 100, 20, 3000405, LabelColor, false, false); // Previous
                    }
                }

                HousePlacementEntry entry = m_Entries[i];

                int y = 70 + (index * 20);
                int storage = (int)(entry.Storage * BaseHouse.GlobalBonusStorageScalar);
                int lockdowns = (int)(entry.Lockdowns * BaseHouse.GlobalBonusStorageScalar);

                AddButton(10, y, 4005, 4007, 1 + i, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, y, 225, 20, entry.Description, LabelColor, false, false);
                AddLabel(275, y, LabelHue, storage.ToString());
                AddLabel(350, y, LabelHue, lockdowns.ToString());
                AddLabel(425, y, LabelHue, entry.Cost.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("en-US")));
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (!m_From.CheckAlive() || m_From.Backpack == null || m_From.Backpack.FindItemByType(typeof(HousePlacementTool)) == null)
                return;

            int index = info.ButtonID - 1;

            if (index >= 0 && index < m_Entries.Length)
            {
                HousePlacementEntry e = m_Entries[index];

                if (e != null)
                {
                    if (e != null)
                    {
                        int cost = e.Cost - m_House.Price;

                        if (cost > 0)
                        {
                            if (!Banker.Withdraw(m_From, cost, true))
                            {
                                m_From.SendLocalizedMessage(1061624); // You do not have enough funds in your bank to cover the difference between your old house and your new one.
                                return;
                            }
                        }
                        else if (cost < 0)
                        {
                            Banker.Deposit(m_From, -cost, true);
                        }

                        BaseHouse newHouse = e.ConstructHouse(m_From);

                        if (newHouse != null)
                        {
                            newHouse.Price = e.Cost;

                            m_House.MoveAllToCrate();

                            newHouse.Friends = new List<Mobile>(m_House.Friends);
                            newHouse.CoOwners = new List<Mobile>(m_House.CoOwners);
                            newHouse.Bans = new List<Mobile>(m_House.Bans);
                            newHouse.Access = new List<Mobile>(m_House.Access);
                            newHouse.BuiltOn = m_House.BuiltOn;
                            newHouse.LastTraded = m_House.LastTraded;
                            newHouse.Public = m_House.Public;

                            newHouse.VendorInventories.AddRange(m_House.VendorInventories);
                            m_House.VendorInventories.Clear();

                            foreach (VendorInventory inventory in newHouse.VendorInventories)
                            {
                                inventory.House = newHouse;
                            }

                            newHouse.InternalizedVendors.AddRange(m_House.InternalizedVendors);
                            m_House.InternalizedVendors.Clear();

                            foreach (Mobile mobile in newHouse.InternalizedVendors)
                            {
                                if (mobile is PlayerVendor)
                                    ((PlayerVendor)mobile).House = newHouse;
                                else if (mobile is PlayerBarkeeper)
                                    ((PlayerBarkeeper)mobile).House = newHouse;
                            }

                            if (m_House.MovingCrate != null)
                            {
                                newHouse.MovingCrate = m_House.MovingCrate;
                                newHouse.MovingCrate.House = newHouse;
                                m_House.MovingCrate = null;
                            }

                            List<Item> items = m_House.GetItems();
                            List<Mobile> mobiles = m_House.GetMobiles();

                            newHouse.MoveToWorld(new Point3D(m_House.X + m_House.ConvertOffsetX, m_House.Y + m_House.ConvertOffsetY, m_House.Z + m_House.ConvertOffsetZ), m_House.Map);
                            m_House.Delete();

                            foreach (Item item in items)
                            {
                                item.Location = newHouse.BanLocation;
                            }

                            foreach (Mobile mobile in mobiles)
                            {
                                mobile.Location = newHouse.BanLocation;
                            }

                            /* You have successfully replaced your original house with a new house.
                            * The value of the replaced house has been deposited into your bank box.
                            * All of the items in your original house have been relocated to a Moving Crate in the new house.
                            * Any deed-based house add-ons have been converted back into deeds.
                            * Vendors and barkeeps in the house, if any, have been stored in the Moving Crate as well.
                            * Use the <B>Get Vendor</B> context-sensitive menu option on your character to retrieve them.
                            * These containers can be used to re-create the vendor in a new location.
                            * Any barkeepers have been converted into deeds.
                            */
                            m_From.SendGump(new NoticeGump(1060637, 30720, 1060012, 32512, 420, 280, null, null));
                            return;
                        }
                    }
                }
            }
            else
            {
                Refresh();
            }
        }
    }
}
