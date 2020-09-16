namespace Server.Items
{
    [Flipable(0x2D6F, 0x2D70)]
    public class SophisticatedElvenTapestry : Item
    {
        [Constructable]
        public SophisticatedElvenTapestry()
            : base(0x2D70)
        {
            Weight = 1;
        }

        public override int LabelNumber => 1151222;// sophisticated elven tapestry

        public SophisticatedElvenTapestry(Serial serial)
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