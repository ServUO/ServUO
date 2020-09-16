namespace Server.Items
{
    [Flipable(0x497D, 0x497E)]
    public class OstardTopiary : Item
    {
        public override int LabelNumber => 1070878;  // a decorative topiary

        [Constructable]
        public OstardTopiary() : base(0x497D)
        {
            Weight = 1.0;
            Name = ("an ostard topiary");
        }

        public OstardTopiary(Serial serial) : base(serial)
        {
        }

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