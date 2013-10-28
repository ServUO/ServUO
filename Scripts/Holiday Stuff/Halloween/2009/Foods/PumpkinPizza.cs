using System;

namespace Server.Items
{
    /* 
    first seen halloween 2009.  subsequently in 2010, 
    2011 and 2012. GM Beggar-only Semi-Rare Treats
    */
    public class PumpkinPizza : CheesePizza
    {
        [Constructable]
        public PumpkinPizza()
            : base()
        {
            this.Hue = 0xF3;
        }

        public PumpkinPizza(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Pumpkin Pizza";
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