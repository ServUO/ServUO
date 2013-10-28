using System;

namespace Server.Items
{
    public class BansheesCall : Cyclone
    {
        [Constructable]
        public BansheesCall() 
        {
            this.Name = ("Banshee's Call");
		
            this.Hue = 1266;
			
            this.WeaponAttributes.HitHarm = 40;
            this.Attributes.BonusStr = 5;
            this.WeaponAttributes.HitLeechHits = 45;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 50;
            this.Velocity = 35;		
            this.AosElementDamages.Cold = 100;
        }

        public BansheesCall(Serial serial)
            : base(serial)
        {
        }

        public override int MinThrowRange
        {
            get
            {
                return 4;
            }
        }// MaxRange 8
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