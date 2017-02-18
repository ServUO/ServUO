using System;

namespace Server.Items
{
    public class PileofInspectedAgapiteIngots : Item
    {
        [Constructable]
        public PileofInspectedAgapiteIngots()
            : base(0x1BEA)
        {
            this.Name = "Pile of Inspected Agapite Ingots";

            this.Hue = 2425;
        }

        public PileofInspectedAgapiteIngots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113770;
            }
        }//Essence Box
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