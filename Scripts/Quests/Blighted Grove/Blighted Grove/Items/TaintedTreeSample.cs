using System;

namespace Server.Items
{
    public class TaintedTreeSample : Item
    {
        [Constructable]
        public TaintedTreeSample()
            : base(0xDE2)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 5;
            this.Hue = 0x9D;
        }

        public TaintedTreeSample(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074997;
            }
        }// tainted tree sample
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