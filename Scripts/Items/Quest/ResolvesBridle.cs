namespace Server.Items
{
    public class ResolvesBridle : Item
    {
        [Constructable]
        public ResolvesBridle()
            : base(0x1374)
        {
        }

        public ResolvesBridle(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074761;// Resolve's Bridle
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