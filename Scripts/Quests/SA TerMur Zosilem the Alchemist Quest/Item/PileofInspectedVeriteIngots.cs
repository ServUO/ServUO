using System;

namespace Server.Items
{
    public class PileofInspectedVeriteIngots : Item
    {
        [Constructable]
        public PileofInspectedVeriteIngots()
            : base(0x1BEA)
        {
            this.Name = "Pile of Inspected Agapite Ingots";

            this.Hue = 2207;
        }

        public PileofInspectedVeriteIngots(Serial serial)
            : base(serial)
        {
        }

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