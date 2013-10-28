using System;

namespace Server.Items
{
    public class CavalrysFolly : BladedStaff
    {
        [Constructable]
        public CavalrysFolly()
            : base()
        {
            this.Name = ("Cavalry's Folly");
		
            this.Hue = 1165;
		
            this.Weight = 4.0;
            this.Attributes.BonusHits = 2;
            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponDamage = 45;
            this.Attributes.WeaponSpeed = 35;
            this.WeaponAttributes.HitLowerDefend = 40;	
            this.WeaponAttributes.HitFireball = 40;
        }

        public CavalrysFolly(Serial serial)
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