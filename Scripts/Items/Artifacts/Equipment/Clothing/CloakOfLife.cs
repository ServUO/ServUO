using System;

namespace Server.Items
{
    [Flipable(0x2FB9, 0x3173)]
    public class CloakOfLife : BaseOuterTorso
    {
		public override bool IsArtifact { get { return true; } }
		
        [Constructable]
        public CloakOfLife()
            : base(0x2FB9)
        {
            Weight = 2.0;
			Hue = 0x21;
			Attributes.RegenHits = 6;
			Attributes.BonusHits = 10;
			SAAbsorptionAttributes.EaterDamage = 15;
			Attributes.EnhancePotions = 35;
			Attributes.BonusStr = 10;
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