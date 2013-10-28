using System;

namespace Server.Factions
{
    public class Silver : Item
    {
        [Constructable]
        public Silver()
            : this(1)
        {
        }

        [Constructable]
        public Silver(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Silver(int amount)
            : base(0xEF0)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Silver(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 0.02;
            }
        }
        public override int GetDropSound()
        {
            if (this.Amount <= 1)
                return 0x2E4;
            else if (this.Amount <= 5)
                return 0x2E5;
            else
                return 0x2E6;
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