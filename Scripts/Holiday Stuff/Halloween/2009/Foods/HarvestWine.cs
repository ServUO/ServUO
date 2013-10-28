using System;

namespace Server.Items
{
    /* 
    first seen halloween 2009.  subsequently in 2010, 
    2011 and 2012. GM Beggar-only Semi-Rare Treats
    */
    public class HarvestWine : BeverageBottle
    {
        [Constructable]
        public HarvestWine()
            : base(BeverageType.Wine)
        {
            this.Hue = 0xe0;
        }

        public HarvestWine(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Harvest Wine";
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