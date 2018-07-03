using System;

namespace Server.Items
{
    public class DragonFlameKey : PeerlessKey
    {
        [Constructable]
        public DragonFlameKey()
            : base(0x1012)
        {
            Weight = 1.0;
            Hue = 0x8F; // TODO check
            LootType = LootType.Blessed;
        }

        public DragonFlameKey(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074343;
            }
        }// dragon flame key
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
