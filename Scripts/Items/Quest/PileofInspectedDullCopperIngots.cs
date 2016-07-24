using System;

namespace Server.Items
{
    public class PileOfInspectedDullCopperIngots : PileOfInspectedIngots
    {
        public override int LabelNumber { get { return 1113021; } } // Pile of Inspected Dull Copper Ingots

        [Constructable]
        public PileOfInspectedDullCopperIngots()
            : base(0x973)
        {
        }

        public PileOfInspectedDullCopperIngots(Serial serial)
            : base(serial)
        {
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