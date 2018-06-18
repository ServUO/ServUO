using System;
using System.Linq;

namespace Server.Items
{
    public enum HaochisPigmentType
    {
        None,
        HeartwoodSienna,
        CampionWhite,
        YewishPine,
        MinocianFire,
        NinjaBlack,
        Olive,
        DarkReddishBrown,
        Yellow,
        PrettyPink,
        MidnightBlue,
        Emerald,
        SmokyGold,
        GhostsGrey,
        OceanBlue,
        CelticLime
    }

    public class HaochisPigment : BasePigmentsOfTokuno
    {
		public override bool IsArtifact { get { return true; } }

        private HaochisPigmentType m_Type;

        [Constructable]
        public HaochisPigment()
            : this(HaochisPigmentType.None, 50)
        {
        }

        [Constructable]
        public HaochisPigment(HaochisPigmentType type)
            : this(type, 50)
        {
        }

        [Constructable]
        public HaochisPigment(HaochisPigmentType type, int uses)
            : base(uses)
        {
            Weight = 1.0;
            Type = type;
        }

        public HaochisPigment(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public HaochisPigmentType Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;

                var info = m_Table.FirstOrDefault(x => x.Type == m_Type);

                if (info != null)
                {
                    Hue = info.Hue;
                    Label = info.Localization;
                }
                else
                {
                    Hue = 0;
                    Label = -1;
                }
            }
        }

        public override int LabelNumber { get { return 1071249; } } // Haochi's Pigments

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Type = (HaochisPigmentType)reader.ReadInt();
        }

        public static HoachisPigmentInfo[] Table { get { return m_Table; } }
        private static HoachisPigmentInfo[] m_Table =
        {
            new HoachisPigmentInfo( HaochisPigmentType.None, 0, -1 ),
            new HoachisPigmentInfo( HaochisPigmentType.HeartwoodSienna, 2739, 1157275 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.CampionWhite, 2738, 1157274 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.YewishPine, 2737, 1157273 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.MinocianFire, 2736, 1157272 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.NinjaBlack, 1175, 1071246 ), // NOT Verified
            new HoachisPigmentInfo( HaochisPigmentType.Olive, 2730, 1018352 ), // Not Verified
            new HoachisPigmentInfo( HaochisPigmentType.DarkReddishBrown, 1148, 1071247 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.Yellow, 1169, 1071245 ), // NOT Verified
            new HoachisPigmentInfo( HaochisPigmentType.PrettyPink, 1168, 1071244 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.MidnightBlue, 1176, 1071248 ), // NOT Verified
            new HoachisPigmentInfo( HaochisPigmentType.Emerald, 1173, 1023856 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.SmokyGold, 1801, 1115467 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.GhostsGrey, 1000, 1115468 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.OceanBlue, 1195, 1115471 ), // Verified
            new HoachisPigmentInfo( HaochisPigmentType.CelticLime, 2733, 1157269 ), // Verified
        };

        public class HoachisPigmentInfo
        {
            public HaochisPigmentType Type { get; private set; }
            public int Hue { get; private set; }
            public int Localization { get; private set; }

            public HoachisPigmentInfo(HaochisPigmentType type, int hue, int loc)
            {
                Type = type;
                Hue = hue;
                Localization = loc;
            }
        }
    }
}
