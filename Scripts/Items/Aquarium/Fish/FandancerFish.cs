using System;

namespace Server.Items
{
    public class FandancerFish : BaseFish
    { 
        [Constructable]
        public FandancerFish()
            : base(0x3B02)
        {
        }

        public FandancerFish(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1074591;
            }
        }// Fandancer Fish
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