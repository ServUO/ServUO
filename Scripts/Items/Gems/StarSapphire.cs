using System;

namespace Server.Items
{
    public class StarSapphire : Item, IGem
    {
        [Constructable]
        public StarSapphire()
            : this(1)
        {
        }

        [Constructable]
        public StarSapphire(int amount)
            : base(0xF21)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public StarSapphire(Serial serial)
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