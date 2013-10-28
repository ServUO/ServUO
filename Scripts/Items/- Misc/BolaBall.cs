using System;

namespace Server.Items
{
    public class BolaBall : Item
    {
        [Constructable]
        public BolaBall()
            : this(1)
        {
        }

        [Constructable]
        public BolaBall(int amount)
            : base(0xE73)
        {
            this.Weight = 4.0;
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = 0x8AC;
        }

        public BolaBall(Serial serial)
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