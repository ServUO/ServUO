using System;

namespace Server.Items
{
    [Flipable(0x2FB9, 0x3173)]
    public class CloakOfDeath : BaseOuterTorso
    {
        [Constructable]
        public CloakOfDeath()
            : base(0x2FB9)
        {
            Weight = 2.0;
			Hue = 0x966;
			Attributes.DefendChance = 3;
			Attributes.AttackChance = 3;
			Attributes.SpellDamage = 3;
        }

        public CloakOfDeath(Serial serial)
            : base(serial)
        {
        }
		
		public override int LabelNumber {get {return 1112881;} }// Cloak of Death

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