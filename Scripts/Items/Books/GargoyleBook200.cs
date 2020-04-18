namespace Server.Items
{
    public class GargoyleBook200 : BaseBook
    {
        [Constructable]
        public GargoyleBook200()
            : base(0x42B8, 200, true)
        {
        }

        [Constructable]
        public GargoyleBook200(int pageCount, bool writable)
            : base(0x42B8, pageCount, writable)
        {
        }

        [Constructable]
        public GargoyleBook200(string title, string author, int pageCount, bool writable)
            : base(0x42B8, title, author, pageCount, writable)
        {
        }

        // Intended for defined books only
        public GargoyleBook200(bool writable)
            : base(0x42B8, writable)
        {
        }

        public GargoyleBook200(Serial serial)
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