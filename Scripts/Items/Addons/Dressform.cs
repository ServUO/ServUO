using System;

namespace Server.Items
{
    [FlipableAttribute(0xec6, 0xec7)]
    public class Dressform : Item
    {
        [Constructable]
        public Dressform()
            : base(0xec6)
        {
            this.Weight = 10;
        }

        public Dressform(Serial serial)
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
        }
    }
}