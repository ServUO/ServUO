using System;

namespace Server.Items
{
    public class PileofInspectedGoldIngots : Item
    {
        [Constructable]
        public PileofInspectedGoldIngots()
            : base(0x1BEA)
        {
            this.Hue = 2213;
        }

        public PileofInspectedGoldIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113027;
            }
        }//Pile of Inspected Gold Ingots
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