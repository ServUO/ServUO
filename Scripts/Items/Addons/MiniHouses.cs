using System;

namespace Server.Items
{
    public enum MiniHouseType
    {
        StoneAndPlaster,
        FieldStone,
        SmallBrick,
        Wooden,
        WoodAndPlaster,
        ThatchedRoof,
        Brick,
        TwoStoryWoodAndPlaster,
        TwoStoryStoneAndPlaster,
        Tower,
        SmallStoneKeep,
        Castle,
        LargeHouseWithPatio,
        MarbleHouseWithPatio,
        SmallStoneTower,
        TwoStoryLogCabin,
        TwoStoryVilla,
        SandstoneHouseWithPatio,
        SmallStoneWorkshop, 
        SmallMarbleWorkshop,
        MalasMountainPass,	//Veteran reward house
        ChurchAtNight		//Veteran reward house
    }

    public class MiniHouseAddon : BaseAddon
    {
        private MiniHouseType m_Type;
        [Constructable]
        public MiniHouseAddon()
            : this(MiniHouseType.StoneAndPlaster)
        {
        }

        [Constructable]
        public MiniHouseAddon(MiniHouseType type)
        {
            this.m_Type = type;

            this.Construct();
        }

        public MiniHouseAddon(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MiniHouseType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.Construct();
            }
        }
        public override BaseAddonDeed Deed
        {
            get
            {
                return new MiniHouseDeed(this.m_Type);
            }
        }
        public void Construct()
        {
            foreach (AddonComponent c in this.Components)
            {
                c.Addon = null;
                c.Delete();
            }

            this.Components.Clear();

            MiniHouseInfo info = MiniHouseInfo.GetInfo(this.m_Type);

            int size = (int)Math.Sqrt(info.Graphics.Length);
            int num = 0;

            for (int y = 0; y < size; ++y)
                for (int x = 0; x < size; ++x)
                    if (info.Graphics[num] != 0x1) // Veteran Rewards Mod
                        this.AddComponent(new AddonComponent(info.Graphics[num++]), size - x - 1, size - y - 1, 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Type = (MiniHouseType)reader.ReadInt();
                        break;
                    }
            }
        }
    }

    public class MiniHouseDeed : BaseAddonDeed
    {
        private MiniHouseType m_Type;
        [Constructable]
        public MiniHouseDeed()
            : this(MiniHouseType.StoneAndPlaster)
        {
        }

        [Constructable]
        public MiniHouseDeed(MiniHouseType type)
        {
            this.m_Type = type;

            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public MiniHouseDeed(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MiniHouseType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateProperties();
            }
        }
        public override BaseAddon Addon
        {
            get
            {
                return new MiniHouseAddon(this.m_Type);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1062096;
            }
        }// a mini house deed
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(MiniHouseInfo.GetInfo(this.m_Type).LabelNumber);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Type = (MiniHouseType)reader.ReadInt();
                        break;
                    }
            }

            if (this.Weight == 0.0)
                this.Weight = 1.0;
        }
    }

    public class MiniHouseInfo
    {
        private static readonly MiniHouseInfo[] m_Info = new MiniHouseInfo[]
        {
            /* Stone and plaster house           */ new MiniHouseInfo(0x22C4, 1, 1011303),
            /* Field stone house                 */ new MiniHouseInfo(0x22DE, 1, 1011304),
            /* Small brick house                 */ new MiniHouseInfo(0x22DF, 1, 1011305),
            /* Wooden house                      */ new MiniHouseInfo(0x22C9, 1, 1011306),
            /* Wood and plaster house            */ new MiniHouseInfo(0x22E0, 1, 1011307),
            /* Thatched-roof cottage             */ new MiniHouseInfo(0x22E1, 1, 1011308),
            /* Brick house                       */ new MiniHouseInfo(1011309, 0x22CD, 0x22CB, 0x22CC, 0x22CA),
            /* Two-story wood and plaster house  */ new MiniHouseInfo(1011310, 0x2301, 0x2302, 0x2304, 0x2303),
            /* Two-story stone and plaster house */ new MiniHouseInfo(1011311, 0x22FC, 0x22FD, 0x22FF, 0x22FE),
            /* Tower                             */ new MiniHouseInfo(1011312, 0x22F7, 0x22F8, 0x22FA, 0x22F9),
            /* Small stone keep                  */ new MiniHouseInfo(0x22E6, 9, 1011313),
            /* Castle                            */ new MiniHouseInfo(1011314, 0x22CE, 0x22D0, 0x22D2, 0x22D7, 0x22CF, 0x22D1, 0x22D4, 0x22D9, 0x22D3, 0x22D5, 0x22D6, 0x22DB, 0x22D8, 0x22DA, 0x22DC, 0x22DD),
            /* Large house with patio            */ new MiniHouseInfo(0x22E2, 4, 1011315),
            /* Marble house with patio           */ new MiniHouseInfo(0x22EF, 4, 1011316),
            /* Small stone tower                 */ new MiniHouseInfo(0x22F5, 1, 1011317),
            /* Two-story log cabin               */ new MiniHouseInfo(0x22FB, 1, 1011318),
            /* Two-story villa                   */ new MiniHouseInfo(0x2300, 1, 1011319),
            /* Sandstone house with patio        */ new MiniHouseInfo(0x22F3, 1, 1011320),
            /* Small stone workshop              */ new MiniHouseInfo(0x22F6, 1, 1011321),
            /* Small marble workshop             */ new MiniHouseInfo(0x22F4, 1, 1011322),
            /* Malas Mountain Pass               */ new MiniHouseInfo(1062692, 0x2316, 0x2315, 0x2314, 0x2313),
            /* Church At Night                   */ new MiniHouseInfo(1072215, 0x2318, 0x2317, 0x2319, 0x1)
        };
        private readonly int[] m_Graphics;
        private readonly int m_LabelNumber;
        public MiniHouseInfo(int start, int count, int labelNumber)
        {
            this.m_Graphics = new int[count];

            for (int i = 0; i < count; ++i)
                this.m_Graphics[i] = start + i;

            this.m_LabelNumber = labelNumber;
        }

        public MiniHouseInfo(int labelNumber, params int[] graphics)
        {
            this.m_LabelNumber = labelNumber;
            this.m_Graphics = graphics;
        }

        public int[] Graphics
        {
            get
            {
                return this.m_Graphics;
            }
        }
        public int LabelNumber
        {
            get
            {
                return this.m_LabelNumber;
            }
        }
        public static MiniHouseInfo GetInfo(MiniHouseType type)
        {
            int v = (int)type;

            if (v < 0 || v >= m_Info.Length)
                v = 0;

            return m_Info[v];
        }
    }
}