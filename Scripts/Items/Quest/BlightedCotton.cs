using System;

namespace Server.Items
{
    public class BlightedCotton : PeerlessKey
    { 
        [Constructable]
        public BlightedCotton()
            : base(0x2DB)
        {
            Weight = 1;
            Hue = 0x35; // TODO check
            LootType = LootType.Blessed;
        }

        public BlightedCotton(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074331;
            }
        }// blighted cotton
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
