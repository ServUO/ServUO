using System;

namespace Server.Items
{
    public class BlankScroll : Item, ICommodity
    {
        [Constructable]
        public BlankScroll()
            : this(1)
        {
        }

        [Constructable]
        public BlankScroll(int amount)
            : base(0xEF3)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;
        }

        public BlankScroll(Serial serial)
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}