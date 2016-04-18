using System;

namespace Server.Items
{
    public class Bottle : Item, ICommodity
    {
        [Constructable]
        public Bottle()
            : this(1)
        {
        }

        [Constructable]
        public Bottle(int amount)
            : base(0xF0E)
        {
            this.Stackable = true;
            this.Weight = 1.0;
            this.Amount = amount;
        }

        public Bottle(Serial serial)
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