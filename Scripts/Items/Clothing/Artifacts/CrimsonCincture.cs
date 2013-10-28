using System;

namespace Server.Items
{
    public class CrimsonCincture : HalfApron, ITokunoDyable
    {
        [Constructable]
        public CrimsonCincture()
            : base()
        {
            this.Hue = 0x485;
			
            this.Attributes.BonusDex = 5;
            this.Attributes.BonusHits = 10;
            this.Attributes.RegenHits = 2;
        }

        public CrimsonCincture(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075043;
            }
        }// Crimson Cincture
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