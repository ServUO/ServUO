namespace Server.Items
{
    public class GargoyleBook100 : BaseBook
    {
        [Constructable]
        public GargoyleBook100()
            : base(0x42B7, 100, true)
        {
        }

        [Constructable]
        public GargoyleBook100(int pageCount, bool writable)
            : base(0x42B7, pageCount, writable)
        {
        }

        [Constructable]
        public GargoyleBook100(string title, string author, int pageCount, bool writable)
            : base(0x42B7, title, author, pageCount, writable)
        {
        }

        // Intended for defined books only
        public GargoyleBook100(bool writable)
            : base(0x42B7, writable)
        {
        }

        public GargoyleBook100(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }
    }
}