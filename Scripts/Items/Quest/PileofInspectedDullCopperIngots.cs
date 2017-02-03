using System;

namespace Server.Items
{
    public class PileofInspectedDullCopperIngots : Item
    {
        [Constructable]
        public PileofInspectedDullCopperIngots()
            : base(0x1BEA)
        {
            this.Hue = 2419;
        }

        public PileofInspectedDullCopperIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113021;
            }
        }//Pile of Inspected Dull Copper Ingots
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