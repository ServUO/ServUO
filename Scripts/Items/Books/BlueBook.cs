namespace Server.Items
{
    public class BlueBook : BaseBook
    {
        [Constructable]
        public BlueBook()
            : base(0xFF2, 40, true)
        {
        }

        [Constructable]
        public BlueBook(int pageCount, bool writable)
            : base(0xFF2, pageCount, writable)
        {
        }

        [Constructable]
        public BlueBook(string title, string author, int pageCount, bool writable)
            : base(0xFF2, title, author, pageCount, writable)
        {
        }

        // Intended for defined books only
        public BlueBook(bool writable)
            : base(0xFF2, writable)
        {
        }

        public BlueBook(Serial serial)
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