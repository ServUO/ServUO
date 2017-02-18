using System;

namespace Server.Items
{
    public class PileofInspectedShadowIronIngots : Item
    {
        [Constructable]
        public PileofInspectedShadowIronIngots()
            : base(0x1BEA)
        {
            this.Hue = 2406;
        }

        public PileofInspectedShadowIronIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113022;
            }
        }//Pile of Inspected Shadow Iron Ingots
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}