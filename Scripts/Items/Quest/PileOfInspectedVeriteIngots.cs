using System;

namespace Server.Items
{
    public class PileOfInspectedVeriteIngots : PileOfInspectedIngots
    {
        public override int LabelNumber { get { return 1113029; } } // Pile of Inspected Verite Ingots

        [Constructable]
        public PileOfInspectedVeriteIngots()
            : base(0x89F)
        {
        }

        public PileOfInspectedVeriteIngots(Serial serial)
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