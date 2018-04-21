using System;

namespace Server.Items
{
    [Flipable(0x2FB9, 0x3173)]
    public class CloakOfPower : BaseOuterTorso
    {
        [Constructable]
        public CloakOfPower()
            : base(0x2FB9)
        {
            Weight = 2.0;
			Hue = 0xFE;
			Attributes.BonusStr = 2;
			Attributes.BonusDex = 2;
			Attributes.BonusInt = 2;
        }

        public CloakOfPower(Serial serial)
            : base(serial)
        {
        }
		
		public override int LabelNumber {get {return 1112882;} }// Cloak of Power

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