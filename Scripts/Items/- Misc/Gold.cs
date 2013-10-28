using System;

namespace Server.Items
{
    public class Gold : Item
    {
        [Constructable]
        public Gold()
            : this(1)
        {
        }

        [Constructable]
        public Gold(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Gold(int amount)
            : base(0xEED)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Gold(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return (Core.ML ? (0.02 / 3) : 0.02);
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

        public override int GetTotal(TotalType type)
        {
            int baseTotal = base.GetTotal(type);

            if (type == TotalType.Gold)
                baseTotal += this.Amount;

            return baseTotal;
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

        protected override void OnAmountChange(int oldValue)
        {
            int newValue = this.Amount;

            this.UpdateTotal(this, TotalType.Gold, newValue - oldValue);
        }
    }
}