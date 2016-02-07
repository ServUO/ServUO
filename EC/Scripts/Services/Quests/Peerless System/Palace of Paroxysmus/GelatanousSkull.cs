using System;

namespace Server.Items
{
    public class GelatanousSkull : PeerlessKey
    {
        [Constructable]
        public GelatanousSkull()
            : base(0x1AE0)
        {
            this.Weight = 1.0;
        }

        public GelatanousSkull(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074328;
            }
        }// gelatanous skull
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