using System;

namespace Server.Items
{
    public class IrksBrain : PeerlessKey
    { 
        [Constructable]
        public IrksBrain()
            : base(0x1CF0)
        {
            this.Weight = 1;
            this.Hue = 0x453; // TODO check
        }

        public IrksBrain(Serial serial)
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
                return 1074335;
            }
        }// irk's brain
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