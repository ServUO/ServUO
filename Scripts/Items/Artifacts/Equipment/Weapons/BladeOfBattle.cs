using System;

namespace Server.Items
{
    public class BladeOfBattle : Shortblade
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BladeOfBattle() 
        {
            Name = ("Blade Of Battle");
		
            Hue = 2045;		
            WeaponAttributes.HitLowerDefend = 40;
            WeaponAttributes.BattleLust = 1;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 50;
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
