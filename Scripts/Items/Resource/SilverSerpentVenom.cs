using System;

namespace Server.Items
{
    public class SilverSerpentVenom : Item
    {
        public override int LabelNumber { get { return 1112173; } } // silver serpent venom

        [Constructable]
        public SilverSerpentVenom()
            : this(1)
        {
        }

        [Constructable]
        public SilverSerpentVenom(int amount)
            : base(0xE24)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
            this.Hue = 0x483;
        }

        public SilverSerpentVenom(Serial serial)
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