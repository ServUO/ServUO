using System;

namespace Server.Items
{
    public class AdmiralJacksPumpkinSpiceAle : BaseShield
    {
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1159230; } } // Admiral Jack's Pumpkin Spice Ale
        
        [Constructable]
        public AdmiralJacksPumpkinSpiceAle()
            : base(0xA40B)
        {
            Hue = 1922;
            Weight = 3.0;
            Attributes.SpellChanneling = 1;
        }

        public AdmiralJacksPumpkinSpiceAle(Serial serial)
            : base(serial)
        {
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
