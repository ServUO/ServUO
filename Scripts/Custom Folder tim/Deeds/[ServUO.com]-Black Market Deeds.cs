using System;

namespace Server.Items
{
    public class BlackMarketDeed : Item
    {
        [Constructable]
        public BlackMarketDeed()
            : this(1)
        {
        }

        [Constructable]
        public BlackMarketDeed(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public BlackMarketDeed(int amount)
            : base(0x46af) // get the itemid for this.  we are using the calculation scroll item id 0x46af
		{
			this.Name = "Black Market Deed";  // Add this
			this.Stackable = true;
            this.Amount = amount;
        }

        public BlackMarketDeed(Serial serial)
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