using System;

namespace Server.Items
{
    public class AxesOfFury : DualShortAxes
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113517; } } // Axes Of Fury
		
        [Constructable]
        public AxesOfFury() 
        {	
            Hue = 33;			
            WeaponAttributes.HitFireball = 45;
            WeaponAttributes.HitLowerDefend = 40;			
            Attributes.BonusDex = 5;			
            Attributes.DefendChance = -15;			
            Attributes.AttackChance = 20;	
            Attributes.WeaponDamage = 45;
            Attributes.WeaponSpeed = 30;	
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
