using System;

namespace Server.Items
{
    /* 
    first seen halloween 2009.  subsequently in 2010, 
    2011 and 2012. GM Beggar-only Semi-Rare Treats
    */
    public class MurkyMilk : Pitcher
    {
        [Constructable]
        public MurkyMilk()
            : base(BeverageType.Milk)
        {
            this.Hue = 0x3e5;
            this.Quantity = this.MaxQuantity;
            this.ItemID = (Utility.RandomBool()) ? 0x09F0 : 0x09AD;
        }

        public MurkyMilk(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Murky Milk";
            }
        }
        public override int MaxQuantity
        {
            get
            {
                return 5;
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 1;
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