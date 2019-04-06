using System;

namespace Server.Items
{
    public class DraconisWrath : Katana
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1114789; } } // Draconi's Wrath
		
        [Constructable]
        public DraconisWrath() 
        {	
            Hue = 1177;	
            AbsorptionAttributes.EaterFire = 20;
            WeaponAttributes.HitFireball = 60;	
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.UseBestSkill = 1;	
        }

        public DraconisWrath(Serial serial)
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