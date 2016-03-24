using System;

namespace Server.Items
{
    public class FeyWings : Item
    {
        [Constructable]
        public FeyWings()
            : this(1)
        {
        }

        [Constructable]
        public FeyWings(int amount)
            : base(0x5726)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public FeyWings(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113332;
            }
        }// fey wings
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