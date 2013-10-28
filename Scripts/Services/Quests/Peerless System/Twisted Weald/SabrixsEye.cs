using System;

namespace Server.Items
{
    public class SabrixsEye : PeerlessKey
    { 
        [Constructable]
        public SabrixsEye()
            : base(0xF87)
        {
            this.Weight = 1;
            this.Hue = 0x480;
        }

        public SabrixsEye(Serial serial)
            : base(serial)
        {
        }

        public override int Lifespan
        {
            get
            {
                return 21600;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074336;
            }
        }// sabrix's eye
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