using System;

namespace Server.Items
{
    public class SerpentFangKey : PeerlessKey
    {
        [Constructable]
        public SerpentFangKey()
            : base(0x1012)
        {
            this.Weight = 1.0;
            this.Hue = 0x9A; // TODO check
        }

        public SerpentFangKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074341;
            }
        }// serpent fang key
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