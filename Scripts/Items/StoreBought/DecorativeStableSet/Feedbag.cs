namespace Server.Items
{
    [Furniture]
    [Flipable(0xA4E3, 0xA4E4)]
    public class Feedbag : BaseContainer
    {
        public override int LabelNumber => 1126235;  // feedbag

		[Constructable]
        public Feedbag()
            : base(0xA4E3)
        {
        }

        public Feedbag(Serial serial)
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
