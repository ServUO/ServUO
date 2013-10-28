using System;

namespace Server.Items
{
    public class Bone : Item, ICommodity
    {
        [Constructable]
        public Bone()
            : this(1)
        {
        }

        [Constructable]
        public Bone(int amount)
            : base(0xf7e)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public Bone(Serial serial)
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
                return true;
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