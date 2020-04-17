namespace Server.Items
{
    public class BlankMap : MapItem
    {
        [Constructable]
        public BlankMap()
        {
        }

        public BlankMap(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            SendLocalizedMessageTo(from, 500208); // It appears to be blank.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}