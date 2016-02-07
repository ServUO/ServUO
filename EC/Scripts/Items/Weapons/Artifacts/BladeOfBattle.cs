using System;

namespace Server.Items
{
    public class BladeOfBattle : Shortblade
    {
        [Constructable]
        public BladeOfBattle() 
        {
            this.Name = ("Blade Of Battle");
		
            this.Hue = 2045;	
		
            this.WeaponAttributes.HitLowerDefend = 40;
            this.WeaponAttributes.BattleLust = 1;
            this.Attributes.AttackChance = 15;
            this.Attributes.DefendChance = 10;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 50;
			this.StrRequirement = 10;
        }

        public BladeOfBattle(Serial serial)
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
		
        public override int AosMinDamage
        {
            get
            {
                return 9;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 13;
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