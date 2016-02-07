using System;

namespace Server.Items
{
    public class ThornyBriar : PeerlessKey
    { 
        [Constructable]
        public ThornyBriar()
            : base(Utility.RandomList(0x3020, 0x3021, 0x3022, 0x3023, 0x3024))
        {
            this.Weight = 1;
            this.Hue = 0x214;
        }

        public ThornyBriar(Serial serial)
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
                return 1074334;
            }
        }// thorny briar
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