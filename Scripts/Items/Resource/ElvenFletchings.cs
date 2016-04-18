using System;

namespace Server.Items
{
    public class ElvenFletchings : Item
    {
        [Constructable]
        public ElvenFletchings()
            : this(1)
        {
        }

        [Constructable]
        public ElvenFletchings(int amount)
            : base(0x5737)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public ElvenFletchings(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113346;
            }
        }// elven fletching
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