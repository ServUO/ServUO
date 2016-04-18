using System;

namespace Server.Items
{
    public class CorporealBrumeStatuette : ShimmeringEffusionStatuette
    {
        [Constructable]
        public CorporealBrumeStatuette()
            : base(0x2D94)
        {
            this.Weight = 1.0;			
        }

        public CorporealBrumeStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074506;
            }
        }// Corporeal Brume Statuette
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