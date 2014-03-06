using System;

namespace Server.Items
{
    public class ResonantStaffofEnlightenment : QuarterStaff
    {
        [Constructable]
        public ResonantStaffofEnlightenment()
        {
            this.Name = ("Resonant Staff of Enlightenment");
		
            this.Hue = 2401;

            this.WeaponAttributes.HitMagicArrow = 40;
            this.WeaponAttributes.MageWeapon = 20;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.DefendChance = 10;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = -40;
            this.Attributes.LowerManaCost = 5;			
            this.AbsorptionAttributes.ResonanceCold = 20;	
            this.AosElementDamages.Cold = 100;		
			this.Attributes.BonusInt = 5;
				
        }

        public ResonantStaffofEnlightenment(Serial serial)
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}