using System;

namespace Server.Items
{
    public class LifeSyphon : BloodBlade
    {
        [Constructable]
        public LifeSyphon()
            : base()
        {
            this.Name = ("Life Syphon");
		
            this.Hue = 1172;
			
            this.WeaponAttributes.BloodDrinker = 1;	
            this.WeaponAttributes.HitHarm = 30;			
            this.WeaponAttributes.HitLeechHits = 100;	
            this.Attributes.BonusHits = 10;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 50;	
        }

        public LifeSyphon(Serial serial)
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
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
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