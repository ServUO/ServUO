using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public enum MonsterStatuetteType
    {
        Crocodile,
        Daemon,
        Dragon,
        EarthElemental,
        Ettin,
        Gargoyle,
        Gorilla,
        Lich,
        Lizardman,
        Ogre,
        Orc,
        Ratman,
        Skeleton,
        Troll,
        Cow,
        Zombie,
        Llama,
        Ophidian,
        Reaper,
        Mongbat,
        Gazer,
        FireElemental,
        Wolf,
        PhillipsWoodenSteed,
        Seahorse,
        Harrower,
        Efreet,
        Slime,
        PlagueBeast,
        RedDeath,
        Spider,
        OphidianArchMage,
        OphidianWarrior,
        OphidianKnight,
        OphidianMage,
        DreadHorn,
        Minotaur,
        BlackCat,
        HalloweenGhoul,
        SherryTheMouse,
        SlasherOfVeils,
        StygianDragon,
        Medusa,
        PrimevalLich,
        AbyssalInfernal,
        ArchDemon,
        FireAnt,
        Navrey,
        DragonTurtle,
        TigerCub,
        SakkhranBirdOfPrey,
        Exodus,
        TerathanMatriarch,
        FleshRenderer,
        CrystalElemental,
        DarkFather,
        PlatinumDragon,
        TRex,
        Zipactriotal,
        MyrmidexQueen,
        Virtuebane,
        GreyGoblin,
        GreenGoblin
    }

    public class MonsterStatuetteInfo
    {
        public static MonsterStatuetteInfo[] Table { get { return m_Table; } }

        private static readonly MonsterStatuetteInfo[] m_Table = new MonsterStatuetteInfo[]
        {
            /* Crocodile */			new MonsterStatuetteInfo(1041249, 0x20DA, 660),
            /* Daemon */			new MonsterStatuetteInfo(1041250, 0x20D3, 357),
            /* Dragon */			new MonsterStatuetteInfo(1041251, 0x20D6, 362),
            /* EarthElemental */	new MonsterStatuetteInfo(1041252, 0x20D7, 268),
            /* Ettin */			    new MonsterStatuetteInfo(1041253, 0x20D8, 367),
            /* Gargoyle */			new MonsterStatuetteInfo(1041254, 0x20D9, 372),
            /* Gorilla */			new MonsterStatuetteInfo(1041255, 0x20F5, 158),
            /* Lich */			    new MonsterStatuetteInfo(1041256, 0x20F8, 1001),
            /* Lizardman */			new MonsterStatuetteInfo(1041257, 0x20DE, 417),
            /* Ogre */			    new MonsterStatuetteInfo(1041258, 0x20DF, 427),
            /* Orc */			    new MonsterStatuetteInfo(1041259, 0x20E0, 1114),
            /* Ratman */			new MonsterStatuetteInfo(1041260, 0x20E3, 437),
            /* Skeleton */			new MonsterStatuetteInfo(1041261, 0x20E7, 1165),
            /* Troll */		    	new MonsterStatuetteInfo(1041262, 0x20E9, 461),
            /* Cow */			    new MonsterStatuetteInfo(1041263, 0x2103, 120),
            /* Zombie */			new MonsterStatuetteInfo(1041264, 0x20EC, 471),
            /* Llama */			    new MonsterStatuetteInfo(1041265, 0x20F6, 1011),
            /* Ophidian */			new MonsterStatuetteInfo(1049742, 0x2133, 634),
            /* Reaper */			new MonsterStatuetteInfo(1049743, 0x20FA, 442),
            /* Mongbat */			new MonsterStatuetteInfo(1049744, 0x20F9, 422),
            /* Gazer */			    new MonsterStatuetteInfo(1049768, 0x20F4, 377),
            /* FireElemental */		new MonsterStatuetteInfo(1049769, 0x20F3, 838),
            /* Wolf */			    new MonsterStatuetteInfo(1049770, 0x2122, 229),
            /* Phillip's Steed */	new MonsterStatuetteInfo(1063488, 0x3FFE, 168),
            /* Seahorse */			new MonsterStatuetteInfo(1070819, 0x25BA, 138),
            /* Harrower */			new MonsterStatuetteInfo(1080520, 0x25BB, new int[] { 0x289, 0x28A, 0x28B }),
            /* Efreet */			new MonsterStatuetteInfo(1080521, 0x2590, 0x300),
            /* Slime */			    new MonsterStatuetteInfo(1015246, 0x20E8, 456),
            /* PlagueBeast */		new MonsterStatuetteInfo(1029747, 0x2613, 0x1BF),
            /* RedDeath */			new MonsterStatuetteInfo(1094932, 0x2617, new int[] { }),
            /* Spider */			new MonsterStatuetteInfo(1029668, 0x25C4, 1170),
            /* OphidianArchMage */	new MonsterStatuetteInfo(1029641, 0x25A9, 639),
            /* OphidianWarrior */	new MonsterStatuetteInfo(1029645, 0x25AD, 634),
            /* OphidianKnight */	new MonsterStatuetteInfo(1029642, 0x25aa, 634),
            /* OphidianMage */		new MonsterStatuetteInfo(1029643, 0x25ab, 639),
            /* DreadHorn */			new MonsterStatuetteInfo(1031651, 0x2D83, 0xA8),
            /* Minotaur */			new MonsterStatuetteInfo(1031657, 0x2D89, 0x596),
            /* Black Cat */		    new MonsterStatuetteInfo(1096928, 0x4688, 0x69),
            /* HalloweenGhoul */	new MonsterStatuetteInfo(1076782, 0x2109, 0x482),
            /* SherryTheMouse */	new MonsterStatuetteInfo(1080171, 0x20D0, 0x0CE),
            /* Slasher of Veils */  new MonsterStatuetteInfo(1113624, 0x42A0, 0x632),
            /* Stygian Dragon   */  new MonsterStatuetteInfo(1113625, 0x42A6, 0x63E),
            /* Medusa */            new MonsterStatuetteInfo(1113626, 0x4298, 0x612),
            /* Primeval Lich */     new MonsterStatuetteInfo(1113627, 0x429A, 0x61E),
            /* Abyssal Infernal */  new MonsterStatuetteInfo(1113628, 0x4287, 1492), 
            /* ArchDemon */         new MonsterStatuetteInfo(1112411, 0x20D3, 357), 
            /* FireAnt */           new MonsterStatuetteInfo(1113801, 0x42A7, 1006),
            /* Navrey Night-Eyes */ new MonsterStatuetteInfo(1153593, 0x4C07, new int[] { 0x61B, 0x61C, 0x61D, 0x61E }),
            /* Dragon Turtle */     new MonsterStatuetteInfo(1156367, 0x9848, 362),
            /* Tiger Cub     */     new MonsterStatuetteInfo(1156517, 0x9CA7, 0x69),
            /* SakkhranBirdOfPrey */new MonsterStatuetteInfo(1156699, 0x276A, 0x4FE),
            /* Exodus */            new MonsterStatuetteInfo(1153594, 0x4C08, new int[] { 0x301, 0x302, 0x303, 0x304 }),
            /* Terathan Matriarch */new MonsterStatuetteInfo(1113800, 0x212C, 599),
            /* Flesh Renderer */    new MonsterStatuetteInfo(1155746, 0x262F, new int[] { 0x34C, 0x354 }),
            /* Crystal Elemental */ new MonsterStatuetteInfo(1155747, 0x2620, 278),
            /* Dark Father */       new MonsterStatuetteInfo(1155748, 0x2632, 0x165),
            /* Platinum Dragon */   new MonsterStatuetteInfo(1155745, 0x2635, new int[] { 0x2C1, 0x2C3 }),
            /* TRex */              new MonsterStatuetteInfo(1157078, 0x9DED, 278),
            /* Zipactriotl */       new MonsterStatuetteInfo(1157079, 0x9DE4, 609),
            /* Myrmidex Queen */    new MonsterStatuetteInfo(1157080, 0x9DB6, 959),
            /* Virtuebane */        new MonsterStatuetteInfo(1153592, 0x4C06, 357),
            /* Grey Goblin */       new MonsterStatuetteInfo(1125135, 0xA095, 0x45A),
            /* Green Goblin */      new MonsterStatuetteInfo(1125133, 0xA097, 0x45A)
        };

        private readonly int m_LabelNumber;
        private readonly int m_ItemID;
        private readonly int[] m_Sounds;

        public MonsterStatuetteInfo(int labelNumber, int itemID, int baseSoundID)
        {
            m_LabelNumber = labelNumber;
            m_ItemID = itemID;
            m_Sounds = new int[] { baseSoundID, baseSoundID + 1, baseSoundID + 2, baseSoundID + 3, baseSoundID + 4 };
        }

        public MonsterStatuetteInfo(int labelNumber, int itemID, int[] sounds)
        {
            m_LabelNumber = labelNumber;
            m_ItemID = itemID;
            m_Sounds = sounds;
        }

        public int LabelNumber { get { return m_LabelNumber; } }
        public int ItemID { get { return m_ItemID; } }
        public int[] Sounds { get { return m_Sounds; } }

        public static MonsterStatuetteInfo GetInfo(MonsterStatuetteType type)
        {
            int v = (int)type;

            if (v < 0 || v >= m_Table.Length)
                v = 0;

            return m_Table[v];
        }
    }

    public class MonsterStatuette : Item, IRewardItem, IEngravable
    {
        private MonsterStatuetteType m_Type;
        private bool m_TurnedOn;
        private bool m_IsRewardItem;

        [Constructable]
        public MonsterStatuette()
            : this(MonsterStatuetteType.Crocodile)
        {
        }

        [Constructable]
        public MonsterStatuette(MonsterStatuetteType type)
            : base(MonsterStatuetteInfo.GetInfo(type).ItemID)
        {
            LootType = LootType.Blessed;

            m_Type = type;

            if (m_Type == MonsterStatuetteType.Slime)
                Hue = Utility.RandomSlimeHue();
            else if (m_Type == MonsterStatuetteType.RedDeath)
                Hue = 0x21;
            else if (m_Type == MonsterStatuetteType.HalloweenGhoul)
                Hue = 0xF4;
            else if (m_Type == MonsterStatuetteType.ArchDemon)
                Hue = 2021;
            else if (m_Type == MonsterStatuetteType.SakkhranBirdOfPrey)
            {
                double ran = Utility.RandomDouble();
                if (0.01 > ran)
                    Hue = 1907;
                else if (0.1 > ran)
                    Hue = 2562;
                else if (0.25 > ran)
                    Hue = 2525;
                else
                    Hue = 2309;
            }
        }

        public MonsterStatuette(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get { return m_TurnedOn; }
            set { m_TurnedOn = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MonsterStatuetteType Type
        {
            get { return m_Type; }
            set
            {
                MonsterStatuetteType old = m_Type;

                m_Type = value;
                ItemID = MonsterStatuetteInfo.GetInfo(m_Type).ItemID;

                if (m_Type == MonsterStatuetteType.Slime)
                    Hue = Utility.RandomSlimeHue();
                else if (m_Type == MonsterStatuetteType.RedDeath)
                    Hue = 0x21;
                else if (m_Type == MonsterStatuetteType.HalloweenGhoul)
                    Hue = 0xF4;
                else if (m_Type != old && m_Type == MonsterStatuetteType.SakkhranBirdOfPrey)
                {
                    double ran = Utility.RandomDouble();
                    if (0.01 > ran)
                        Hue = 1907;
                    else if (0.1 > ran)
                        Hue = 2562;
                    else if (0.25 > ran)
                        Hue = 2525;
                    else
                        Hue = 2309;
                }
                else
                    Hue = 0;

                InvalidateProperties();
            }
        }

        public override int LabelNumber
        {
            get
            {
                return MonsterStatuetteInfo.GetInfo(m_Type).LabelNumber;
            }
        }

        public override double DefaultWeight { get { return 1.0; } }

        public override bool HandlesOnMovement
        {
            get
            {
                return m_TurnedOn && IsLockedDown;
            }
        }

        #region IEngraveable
        private string m_EngravedText = string.Empty;

        [CommandProperty(AccessLevel.GameMaster)]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set
            {
                if (value != null)
                    m_EngravedText = value;
                else
                    m_EngravedText = string.Empty;

                InvalidateProperties();
            }
        }
        #endregion

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_TurnedOn && IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, Location, 2) && !Utility.InRange(oldLocation, Location, 2))
            {
                int[] sounds = MonsterStatuetteInfo.GetInfo(m_Type).Sounds;

                if (sounds.Length > 0)
                    Effects.PlaySound(Location, Map, sounds[Utility.Random(sounds.Length)]);
            }

            base.OnMovement(m, oldLocation);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);

            if (!String.IsNullOrEmpty(EngravedText))
            {
                list.Add(1072305, Utility.FixHtml(EngravedText)); // Engraved: ~1_INSCRIPTION~
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Core.ML && m_IsRewardItem)
                list.Add(RewardSystem.GetRewardYearLabel(this, new object[] { m_Type })); // X Year Veteran Reward

            if (m_TurnedOn)
                list.Add(502695); // turned on
            else
                list.Add(502696); // turned off
        }

        public bool IsOwner(Mobile mob)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            return (house != null && house.IsOwner(mob));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsOwner(from))
            {
                OnOffGump onOffGump = new OnOffGump(this);
                from.SendGump(onOffGump);
            }
            else
            {
                from.SendLocalizedMessage(502691); // You must be the owner to use this.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write(m_EngravedText);

            writer.WriteEncodedInt((int)m_Type);
            writer.Write((bool)m_TurnedOn);
            writer.Write((bool)m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    m_EngravedText = reader.ReadString();
                    goto case 0;
                case 0:
                    {
                        m_Type = (MonsterStatuetteType)reader.ReadEncodedInt();
                        m_TurnedOn = reader.ReadBool();
                        m_IsRewardItem = reader.ReadBool();
                        break;
                    }
            }
        }

        private class OnOffGump : Gump
        {
            private readonly MonsterStatuette m_Statuette;

            public OnOffGump(MonsterStatuette statuette)
                : base(150, 200)
            {
                m_Statuette = statuette;

                AddBackground(0, 0, 300, 150, 0xA28);

                AddHtmlLocalized(45, 20, 300, 35, statuette.TurnedOn ? 1011035 : 1011034, false, false); // [De]Activate this item

                AddButton(40, 53, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 55, 65, 35, 1011036, false, false); // OKAY

                AddButton(150, 53, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(190, 55, 100, 35, 1011012, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 1)
                {
                    bool newValue = !m_Statuette.TurnedOn;
                    m_Statuette.TurnedOn = newValue;

                    if (newValue && !m_Statuette.IsLockedDown)
                        from.SendLocalizedMessage(502693); // Remember, this only works when locked down.
                }
                else
                {
                    from.SendLocalizedMessage(502694); // Cancelled action.
                }
            }
        }
    }
}