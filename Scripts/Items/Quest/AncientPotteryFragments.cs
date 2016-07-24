using System;

namespace Server.Items
{
    public class AncientPotteryFragments : Item
    {
        public override int LabelNumber { get { return 1112990; } } // Ancient Pottery fragments

        [Constructable]
        public AncientPotteryFragments()
            : base(0x2243)
        {
            this.Weight = 1.0;
            this.Hue = 0x971;
        }

        public AncientPotteryFragments(Serial serial)
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