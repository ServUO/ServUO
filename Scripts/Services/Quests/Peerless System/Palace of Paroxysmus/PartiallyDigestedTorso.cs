using System;

namespace Server.Items
{
    public class PartiallyDigestedTorso : PeerlessKey
    {
        [Constructable]
        public PartiallyDigestedTorso()
            : base(0x1D9F)
        {
            this.Weight = 1.0;
        }

        public PartiallyDigestedTorso(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074326;
            }
        }// partially digested torso
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