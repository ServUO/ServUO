namespace Server.Items
{
    public class ShimmeringCrystals : Item
    {
        private static readonly int[] m_ItemIDs = new int[]
        {
            0x2206, 0x2207, 0x2208, 0x2209, 0x220A, 0x220B, 0x220C, 0x220D, 0x220E,
            0x2210, 0x2211, 0x2212, 0x2213, 0x2214, 0x2215, 0x2216, 0x2217, 0x2218,
            0x221A, 0x221B, 0x221C, 0x221D, 0x221E, 0x221F, 0x2220, 0x2221, 0x2222,
            0x2224, 0x2225, 0x2226, 0x2227, 0x2228, 0x2229, 0x222A, 0x222B, 0x222C
        };
        [Constructable]
        public ShimmeringCrystals()
            : base(Utility.RandomList(m_ItemIDs))
        {
        }

        public ShimmeringCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075095;// Shimmering Crystals
        public override bool ForceShowProperties => true;
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