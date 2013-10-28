using System;

namespace Server.Items
{
    public class AxesOfFury : DualShortAxes
    {
        [Constructable]
        public AxesOfFury() 
        {
            this.Name = ("Axes Of Fury");
		
            this.Hue = 33;	
			
            this.WeaponAttributes.HitFireball = 45;
            this.WeaponAttributes.HitLowerDefend = 40;			
            this.Attributes.BonusDex = 5;			
            this.Attributes.DefendChance = -15;			
            this.Attributes.AttackChance = 20;	
            this.Attributes.WeaponDamage = 45;
            this.Attributes.WeaponSpeed = 30;	
        }

        public AxesOfFury(Serial serial)
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
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
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