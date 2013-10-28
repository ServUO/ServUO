using System;

namespace Server.Items
{
    public class Emerald : Item, IGem
    {
        [Constructable]
        public Emerald()
            : this(1)
        {
        }

        [Constructable]
        public Emerald(int amount)
            : base(0xF10)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Emerald(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
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