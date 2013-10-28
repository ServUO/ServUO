using System;

namespace Server.Items
{
    public class BrownBook : BaseBook
    {
        [Constructable]
        public BrownBook()
            : base(0xFEF)
        {
        }

        [Constructable]
        public BrownBook(int pageCount, bool writable)
            : base(0xFEF, pageCount, writable)
        {
        }

        [Constructable]
        public BrownBook(string title, string author, int pageCount, bool writable)
            : base(0xFEF, title, author, pageCount, writable)
        {
        }

        // Intended for defined books only
        public BrownBook(bool writable)
            : base(0xFEF, writable)
        {
        }

        public BrownBook(Serial serial)
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