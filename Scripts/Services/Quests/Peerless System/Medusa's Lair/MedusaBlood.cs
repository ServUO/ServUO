using System;

namespace Server.Items
{
    public class MedusaBlood : Item
    {
        [Constructable]
        public MedusaBlood()
            : this(1)
        {
        }

        [Constructable]
        public MedusaBlood(int amount)
            : base(0x2DB6)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public MedusaBlood(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1031702;
            }
        }// Medusa Blood
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