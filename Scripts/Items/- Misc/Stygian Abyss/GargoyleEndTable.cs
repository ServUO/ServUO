using System;

namespace Server.Items
{
    [Furniture]
    public class GargoyleEndTable : Item
    {
        [Constructable]
        public GargoyleEndTable()
            : base(0x4041)
        {
            this.Weight = 1.0;
        }

        public GargoyleEndTable(Serial serial)
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