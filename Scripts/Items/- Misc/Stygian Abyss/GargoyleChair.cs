using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x4023, 0x4024)]
    public class GargoyleChair : Item
    {
        [Constructable]
        public GargoyleChair()
            : base(0x4023)
        {
            this.Weight = 6;
        }

        public GargoyleChair(Serial serial)
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