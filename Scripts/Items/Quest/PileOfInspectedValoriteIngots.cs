using System;

namespace Server.Items
{
    public class PileOfInspectedValoriteIngots : PileOfInspectedIngots
    {
        public override int LabelNumber { get { return 1113030; } } // Pile of Inspected Valorite Ingots

        [Constructable]
        public PileOfInspectedValoriteIngots()
            : base(0x8AB)
        {
        }

        public PileOfInspectedValoriteIngots(Serial serial)
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