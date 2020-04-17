namespace Server.Items
{
    [Furniture]
    [Flipable(0x2D05, 0x2D06)]
    public class SimpleElvenArmoire : BaseContainer
    {
        [Constructable]
        public SimpleElvenArmoire()
            : base(0x2D05)
        {
            Weight = 1.0;
        }

        public SimpleElvenArmoire(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x4F;
        public override int DefaultDropSound => 0x42;
        public override int LabelNumber => 1031525;// simple elven armoire
        public override Rectangle2D Bounds => new Rectangle2D(30, 30, 90, 150);
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