namespace Server.Items
{
    [Flipable(0x2D71, 0x2D72)]
    public class OrnateElvenTapestry : Item
    {
        [Constructable]
        public OrnateElvenTapestry()
            : base(0x2D72)
        {
            Weight = 1;
        }

        public override int LabelNumber => 1031633;// ornate elven tapestry

        public OrnateElvenTapestry(Serial serial)
            : base(serial)
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