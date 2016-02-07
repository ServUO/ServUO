using System;

namespace Server.Items
{
    public class BrokenCrystals : PeerlessKey
    {
        [Constructable]
        public BrokenCrystals()
            : base(0x2247)
        {
            this.Weight = 1;
            this.Hue = 0x2B2;
        }

        public BrokenCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074261;
            }
        }// broken crystal
        public override int Lifespan
        {
            get
            {
                return 21600;
            }
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