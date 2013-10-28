using System;

namespace Server.Items
{
    /* 
    first seen halloween 2009.  subsequently in 2010, 
    2011 and 2012. GM Beggar-only Semi-Rare Treats
    */
    public class GrimWarning : Item
    {
        [Constructable]
        public GrimWarning()
            : base(0x42BD)
        {
        }

        public GrimWarning(Serial serial)
            : base(serial)
        {
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