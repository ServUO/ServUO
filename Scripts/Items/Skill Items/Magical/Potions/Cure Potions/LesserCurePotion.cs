using System;

namespace Server.Items
{
    public class LesserCurePotion : BaseCurePotion
    {
        private static readonly CureLevelInfo[] m_OldLevelInfo = new CureLevelInfo[]
        {
            new CureLevelInfo(Poison.Lesser, 0.75), // 75% chance to cure lesser poison
            new CureLevelInfo(Poison.Regular, 0.50), // 50% chance to cure regular poison
            new CureLevelInfo(Poison.Greater, 0.15)// 15% chance to cure greater poison
        };
        private static readonly CureLevelInfo[] m_AosLevelInfo = new CureLevelInfo[]
        {
            new CureLevelInfo(Poison.Lesser, 0.75),
            new CureLevelInfo(Poison.Regular, 0.50),
            new CureLevelInfo(Poison.Greater, 0.25)
        };
        [Constructable]
        public LesserCurePotion()
            : base(PotionEffect.CureLesser)
        {
        }

        public LesserCurePotion(Serial serial)
            : base(serial)
        {
        }

        public override CureLevelInfo[] LevelInfo
        {
            get
            {
                return Core.AOS ? m_AosLevelInfo : m_OldLevelInfo;
            }
        }
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
    }
}