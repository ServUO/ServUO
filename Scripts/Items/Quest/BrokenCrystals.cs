namespace Server.Items
{
    public class BrokenCrystals : PeerlessKey
    {
        [Constructable]
        public BrokenCrystals()
            : base(0x2247)
        {
            Weight = 1;
            Hue = 0x2B2;
        }

        public BrokenCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074261;// broken crystal
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