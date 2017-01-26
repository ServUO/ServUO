using System;

namespace Server.Items
{
    public class IceCrystals : Item
    {
        public override int LabelNumber { get { return 1028710; } } // crystal

        private static readonly int[] m_ItemIDs = new int[]
        {
            0x2208, 0x221D, 0x2FDC, 0x2228, 0x2209, 0x2FDD, 0x2FDC
        };

        [Constructable]
        public IceCrystals()
            : base(Utility.RandomList(m_ItemIDs))
        {
            this.Hue = 2729;
            this.Movable = false;
        }

        public IceCrystals(Serial serial)
            : base(serial)
        {
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
