using System;

namespace Server.Items
{
    [Flipable(0x1051, 0x1052)]
    public class AxleGears : Item
    {
        [Constructable]
        public AxleGears()
            : this(1)
        {
        }

        [Constructable]
        public AxleGears(int amount)
            : base(0x1051)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public AxleGears(Serial serial)
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