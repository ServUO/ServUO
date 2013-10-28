using System;

namespace Server.Items
{
    public class ZoogiFungus : Item, ICommodity
    {
        [Constructable]
        public ZoogiFungus()
            : this(1)
        {
        }

        [Constructable]
        public ZoogiFungus(int amount)
            : base(0x26B7)
        {
            this.Stackable = true;
            this.Weight = 0.1;
            this.Amount = amount;
        }

        public ZoogiFungus(Serial serial)
            : base(serial)
        {
        }

        int ICommodity.DescriptionNumber
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return (Core.ML);
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}