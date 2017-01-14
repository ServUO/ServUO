using System;

namespace Server.Items
{
    public class PrimalLichDust : Item
    {
        [Constructable]
        public PrimalLichDust()
            : this(1)
        {
        }

        [Constructable]
        public PrimalLichDust(int amount)
            : base(0x2DB5)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public PrimalLichDust(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1031701;
            }
        }// Primeval Lich Dust
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