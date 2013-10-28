using System;

namespace Server.Items
{
    public class Fur : Item
    {
        [Constructable]
        public Fur()
            : base(0x1875)
        {
        }

        public Fur(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}