using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0xB4A, 0xB49, 0xB4B, 0xB4C)]
    public class WritingTable : Item
    {
        [Constructable]
        public WritingTable()
            : base(0xB4A)
        {
            this.Weight = 1.0;
        }

        public WritingTable(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 4.0)
                this.Weight = 1.0;
        }
    }
}