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
    }

    public class MonsterStatuetteInfo
    {
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
            /* Slasher of Veils */  new MonsterStatuetteInfo( 1113624, 0x42A0, 0x632 ),
            /* Stygian Dragon   */  new MonsterStatuetteInfo( 1113625, 0x42A6, 0x63E ),
            /* Medusa */            new MonsterStatuetteInfo( 1113626, 0x4298, 0x612 ),
            /* Primeval Lich */     new MonsterStatuetteInfo( 1113627, 0x429A, 0x61E ),
            /* Abyssal Infernal */  new MonsterStatuetteInfo( 1113628, 0x4287, 1492 ), 
            /* ArchDemon */         new MonsterStatuetteInfo( 1112411, 0x20D3, 357 ), 
            /* FireAnt */           new MonsterStatuetteInfo( 1113801, 0x42A7, 849 ),
            /* Navrey Night-Eyes */ new MonsterStatuetteInfo( 1153593, 0x4C07, 0x61A ),
        };
        private readonly int m_LabelNumber;
        private readonly int m_ItemID;
        private readonly int[] m_Sounds;
        public MonsterStatuetteInfo(int labelNumber, int itemID, int baseSoundID)
        {
            this.m_LabelNumber = labelNumber;
            this.m_ItemID = itemID;
            this.m_Sounds = new int[] { baseSoundID, baseSoundID + 1, baseSoundID + 2, baseSoundID + 3, baseSoundID + 4 };
        }

        public MonsterStatuetteInfo(int labelNumber, int itemID, int[] sounds)
        {
            this.m_LabelNumber = labelNumber;
            this.m_ItemID = itemID;
            this.m_Sounds = sounds;
        }

        public int LabelNumber
        {
            get
            {
                return this.m_LabelNumber;
            }
        }
        public int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
        }
        public int[] Sounds
        {
            get
            {
                return this.m_Sounds;
            }
        }
        public static MonsterStatuetteInfo GetInfo(MonsterStatuetteType type)
        {
            int v = (int)type;

            if (v < 0 || v >= m_Table.Length)
                v = 0;

            return m_Table[v];
        }
    }

    public class MonsterStatuette : Item, IRewardItem
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
            this.LootType = LootType.Blessed;

            this.m_Type = type;

            if (this.m_Type == MonsterStatuetteType.Slime)
                this.Hue = Utility.RandomSlimeHue();
            else if (this.m_Type == MonsterStatuetteType.RedDeath)
                this.Hue = 0x21;
            else if (this.m_Type == MonsterStatuetteType.HalloweenGhoul)
                this.Hue = 0xF4;
            else if (m_Type == MonsterStatuetteType.ArchDemon)
                Hue = 2021;
        }

        public MonsterStatuette(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get
            {
                return this.m_TurnedOn;
            }
            set
            {
                this.m_TurnedOn = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public MonsterStatuetteType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.ItemID = MonsterStatuetteInfo.GetInfo(this.m_Type).ItemID;

                if (this.m_Type == MonsterStatuetteType.Slime)
                    this.Hue = Utility.RandomSlimeHue();
                else if (this.m_Type == MonsterStatuetteType.RedDeath)
                    this.Hue = 0x21;
                else if (this.m_Type == MonsterStatuetteType.HalloweenGhoul)
                    this.Hue = 0xF4;
                else
                    this.Hue = 0;

                this.InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return MonsterStatuetteInfo.GetInfo(this.m_Type).LabelNumber;
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return this.m_TurnedOn && this.IsLockedDown;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.m_TurnedOn && this.IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, this.Location, 2) && !Utility.InRange(oldLocation, this.Location, 2))
            {
                int[] sounds = MonsterStatuetteInfo.GetInfo(this.m_Type).Sounds;

                if (sounds.Length > 0)
                    Effects.PlaySound(this.Location, this.Map, sounds[Utility.Random(sounds.Length)]);
            }

            base.OnMovement(m, oldLocation);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Core.ML && this.m_IsRewardItem)
                list.Add(RewardSystem.GetRewardYearLabel(this, new object[] { this.m_Type })); // X Year Veteran Reward

            if (this.m_TurnedOn)
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
            if (this.IsOwner(from))
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

            writer.Write((int)0); // version

            writer.WriteEncodedInt((int)this.m_Type);
            writer.Write((bool)this.m_TurnedOn);
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Type = (MonsterStatuetteType)reader.ReadEncodedInt();
                        this.m_TurnedOn = reader.ReadBool();
                        this.m_IsRewardItem = reader.ReadBool();
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
                this.m_Statuette = statuette;

                this.AddBackground(0, 0, 300, 150, 0xA28);

                this.AddHtmlLocalized(45, 20, 300, 35, statuette.TurnedOn ? 1011035 : 1011034, false, false); // [De]Activate this item

                this.AddButton(40, 53, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(80, 55, 65, 35, 1011036, false, false); // OKAY

                this.AddButton(150, 53, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(190, 55, 100, 35, 1011012, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 1)
                {
                    bool newValue = !this.m_Statuette.TurnedOn;
                    this.m_Statuette.TurnedOn = newValue;

                    if (newValue && !this.m_Statuette.IsLockedDown)
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