using System;

namespace Server.Items
{
    public class DeathsHead : DiscMace
    {
        [Constructable]
        public DeathsHead() 
        {
            this.Name = ("Death's Head");
		
            this.Hue = 1154;	
		
            this.WeaponAttributes.HitFatigue = 10;
            this.WeaponAttributes.HitLightning = 45;	
            this.WeaponAttributes.HitLowerDefend = 30;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 45;
        }

        public DeathsHead(Serial serial)
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