using System;

namespace Server.Items
{
    public class Amber : Item, ICommodity
    {
        [Constructable]
        public Amber()
            : this(1)
        {
        }

        [Constructable]
        public Amber(int amount)
            : base(0xF25)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Amber(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

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
