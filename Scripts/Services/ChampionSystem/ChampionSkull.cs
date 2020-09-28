using Server.Engines.CannedEvil;
using System;
using System.Linq;

namespace Server.Items
{
    public class ChampionSkull : Item
    {
        public static readonly ChampionSkullType[] Types = 
            Enum.GetValues(typeof(ChampionSkullType))
                .Cast<ChampionSkullType>()
                .Where(o => o != ChampionSkullType.None)
                .ToArray();

        public static ChampionSkullType RandomType => Types[Utility.Random(Types.Length)];

        private ChampionSkullType m_Type;

        [Constructable]
        public ChampionSkull()
            : this(RandomType)
        { }

        [Constructable]
        public ChampionSkull(ChampionSkullType type)
            : base(0x1AE1)
        {
            m_Type = type;

            LootType = LootType.Cursed;

            switch (type)
            {
                case ChampionSkullType.Power:
                    Hue = 0x159;
                    break;
                case ChampionSkullType.Venom:
                    Hue = 0x172;
                    break;
                case ChampionSkullType.Greed:
                    Hue = 0x1EE;
                    break;
                case ChampionSkullType.Death:
                    Hue = 0x025;
                    break;
                case ChampionSkullType.Pain:
                    Hue = 0x035;
                    break;
            }
        }

        public ChampionSkull(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public ChampionSkullType Type
        {
            get { return m_Type; }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        public override int LabelNumber => 1049479 + (int)m_Type;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();

            m_Type = (ChampionSkullType)reader.ReadInt();
        }
    }
}
