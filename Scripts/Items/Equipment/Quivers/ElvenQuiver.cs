namespace Server.Items
{
    [Flipable(0x2FB7, 0x3171)]
    public class ElvenQuiver : BaseQuiver
    {
        public override int LabelNumber => 1032657; // elven quiver

        [Constructable]
        public ElvenQuiver()
            : base()
        {
            WeightReduction = 30;
        }

        public ElvenQuiver(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}