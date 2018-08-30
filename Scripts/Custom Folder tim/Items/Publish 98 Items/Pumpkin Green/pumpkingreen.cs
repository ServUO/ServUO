using System;

namespace Server.Items
{
    public class PumpkinGreen : Item
    {
        [Constructable]
        public PumpkinGreen()
            : base(0x9D22)
        {
            //this.Name = "Carveable Pumpkin";
        }

        public PumpkinGreen(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1123239;
            }
        }// carvable pumpkin

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