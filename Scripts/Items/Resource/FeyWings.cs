using System;

namespace Server.Items
{
    public class FeyWings : Item, ICommodity
    {
        public override int LabelNumber { get { return 1113332; } } // fey wings
        public override double DefaultWeight { get { return 0.1; } }

        [Constructable]
        public FeyWings()
            : this(1)
        {
        }

        [Constructable]
        public FeyWings(int amount)
            : base(0x5726)
        {
            Stackable = true;
            Amount = amount;
        }

        public FeyWings(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

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
