using System;

namespace Server.Items
{
    public class JaggedCrystals : PeerlessKey
    {
        [Constructable]
        public JaggedCrystals()
            : base(0x223E)
        {
            this.Weight = 1;
            this.Hue = 0x2B2;
        }

        public JaggedCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074265;
            }
        }// jagged crystal shards
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