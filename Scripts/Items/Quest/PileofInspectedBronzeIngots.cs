using System;

namespace Server.Items
{
    public class PileofInspectedBronzeIngots : Item
    {
        [Constructable]
        public PileofInspectedBronzeIngots()
            : base(0x1BEA)
        {
            this.Hue = 2418;
        }

        public PileofInspectedBronzeIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113024;
            }
        }//Pile of Inspected Bronze Ingots
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