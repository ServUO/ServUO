using System;

namespace Server.Items
{
    public class Sand : Item, ICommodity
    {
        public override int LabelNumber { get { return 1044626; } } // sand
        public override double DefaultWeight { get { return 0.1; } }

        [Constructable]
        public Sand()
            : this(1)
        {
        }

        [Constructable]
        public Sand(int amount)
            : base(0x423A)
        {
            Hue = 2413;
            Stackable = true;
            Amount = amount;
        }

        public Sand(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (ItemID != 0x423A)
            {
                ItemID = 0x423A;
                Hue = 2413;
            }
        }
    }
}
