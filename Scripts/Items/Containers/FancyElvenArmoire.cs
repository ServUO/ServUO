namespace Server.Items
{
    [Furniture]
    [Flipable(0x2D07, 0x2D08)]
    public class FancyElvenArmoire : BaseContainer
    {
        [Constructable]
        public FancyElvenArmoire()
            : base(0x2D07)
        {
            Weight = 1.0;
        }

        public FancyElvenArmoire(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x4E;
        public override int DefaultDropSound => 0x42;
        public override int LabelNumber => 1031527;// fancy elven armoire
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