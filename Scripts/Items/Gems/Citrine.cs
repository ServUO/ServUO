using System;

namespace Server.Items
{
    public class Citrine : Item, IGem
    {
        [Constructable]
        public Citrine()
            : this(1)
        {
        }

        [Constructable]
        public Citrine(int amount)
            : base(0xF15)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Citrine(Serial serial)
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