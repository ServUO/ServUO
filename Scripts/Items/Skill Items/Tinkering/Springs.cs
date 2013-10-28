using System;

namespace Server.Items
{
    [Flipable(0x105D, 0x105E)]
    public class Springs : Item
    {
        [Constructable]
        public Springs()
            : this(1)
        {
        }

        [Constructable]
        public Springs(int amount)
            : base(0x105D)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public Springs(Serial serial)
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