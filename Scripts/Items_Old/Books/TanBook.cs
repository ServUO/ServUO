using System;

namespace Server.Items
{
    public class TanBook : BaseBook
    {
        [Constructable]
        public TanBook()
            : base(0xFF0)
        {
        }

        [Constructable]
        public TanBook(int pageCount, bool writable)
            : base(0xFF0, pageCount, writable)
        {
        }

        [Constructable]
        public TanBook(string title, string author, int pageCount, bool writable)
            : base(0xFF0, title, author, pageCount, writable)
        {
        }

        // Intended for defined books only
        public TanBook(bool writable)
            : base(0xFF0, writable)
        {
        }

        public TanBook(Serial serial)
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

            writer.Write((int)0); // version
        }
    }
}