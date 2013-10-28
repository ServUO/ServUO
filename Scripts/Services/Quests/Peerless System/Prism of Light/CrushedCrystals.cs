using System;

namespace Server.Items
{
    public class CrushedCrystals : PeerlessKey
    {
        [Constructable]
        public CrushedCrystals()
            : base(0x223C)
        {
            this.Weight = 1;
            this.Hue = 0x47E;
        }

        public CrushedCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074262;
            }
        }// crushed crystal pieces
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