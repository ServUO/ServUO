namespace Server.Items
{
    public class LesserCurePotion : BaseCurePotion
    {
        private static readonly CureLevelInfo[] m_AosLevelInfo = new CureLevelInfo[]
        {
            new CureLevelInfo(Poison.Lesser, 1.00),
            new CureLevelInfo(Poison.Regular, 0.35),
            new CureLevelInfo(Poison.Greater, 0.15),
            new CureLevelInfo(Poison.Deadly, 0.10),
            new CureLevelInfo(Poison.Lethal, 0.05)
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

        public override CureLevelInfo[] LevelInfo => m_AosLevelInfo;
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
}
