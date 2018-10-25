using System;

namespace Server.Items
{
    public class GoldDust : Item, ICommodity
    {
        [Constructable]
        public GoldDust() : this(1)
        {
        }

        [Constructable]
        public GoldDust(int amount) : base(0x4C09)
        {
            this.Stackable = true;
            this.Hue = 1177;
            this.Weight = 1.0;
            this.Amount = amount;
        }

        public GoldDust(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }

        public override int LabelNumber { get { return 1153504; } } // gold dust

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
