using System;

namespace Server.Items
{
    [Flipable(0x2FB9, 0x3173)]
    public class CloakOfLife : BaseOuterTorso
    {

        [Constructable]
        public CloakOfLife()
            : base(0x2FB9)
        {
            Weight = 2.0;
			Hue = 0x21;
			Attributes.RegenHits = 1;
			Attributes.BonusHits = 3;
        }

        public CloakOfLife(Serial serial)
            : base(serial)
        {
        }
		
		public override int LabelNumber {get {return 1112880;} }// Cloak of Life

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