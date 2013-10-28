using System;

namespace Server.Items
{
    public class Mangler : Broadsword
    {
        [Constructable]
        public Mangler()
            : base()
        {
            this.Hue = 2001;
			
            this.Name = ("Mangler");
		
            this.WeaponAttributes.HitLeechMana = 50;
            this.Attributes.WeaponDamage = 50;
            this.Attributes.WeaponSpeed = 25;
            this.WeaponAttributes.HitHarm = 50;
            this.WeaponAttributes.UseBestSkill = 1;			
            this.WeaponAttributes.HitLowerDefend = 30;		
        }

        public Mangler(Serial serial)
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
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }
    }
}