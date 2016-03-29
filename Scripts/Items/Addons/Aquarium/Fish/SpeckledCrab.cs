using System;

namespace Server.Items
{
    public class SpeckledCrab : BaseFish
    { 
        [Constructable]
        public SpeckledCrab()
            : base(0x3AFC)
        {
        }

        public SpeckledCrab(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1073826;
            }
        }// A Speckled Crab 
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