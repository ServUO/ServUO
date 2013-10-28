using System;

namespace Server.Items
{
    public class SpinedScratcherFish : BaseFish
    { 
        [Constructable]
        public SpinedScratcherFish()
            : base(0x3B05)
        {
        }

        public SpinedScratcherFish(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1073832;
            }
        }// A Spined Scratcher Fish 
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