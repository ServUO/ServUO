using System;

namespace Server.Items
{
    public class LegacyOfDespair : DreadSword
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113519; } } // Legacy of Despair
		
        [Constructable]
        public LegacyOfDespair()
        {	
            Hue = 48;		
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 60;
            WeaponAttributes.HitLowerDefend = 50;
            WeaponAttributes.HitLowerAttack = 50;
            WeaponAttributes.HitCurse = 10;		
            AosElementDamages.Cold = 75;
            AosElementDamages.Poison = 25;			
        }

        public LegacyOfDespair(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
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