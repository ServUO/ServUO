using System;

namespace Server.Items
{
    public class MagicalRope : PeerlessKey
    { 
        [Constructable]
        public MagicalRope()
            : base(0x20D)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 5.0;
        }

        public MagicalRope(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074338;
            }
        }// Magical Rope	
        public override int Lifespan
        {
            get
            {
                return 600;
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