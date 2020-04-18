namespace Server.Items
{
    public class TaintedTreeSample : Item
    {
        [Constructable]
        public TaintedTreeSample()
            : base(0xDE2)
        {
            LootType = LootType.Blessed;
            Weight = 5;
            Hue = 0x9D;
        }

        public TaintedTreeSample(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074997;// tainted tree sample
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