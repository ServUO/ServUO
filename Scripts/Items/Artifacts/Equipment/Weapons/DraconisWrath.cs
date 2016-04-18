using System;

namespace Server.Items
{
    public class DraconisWrath : Katana
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DraconisWrath() 
        {
            this.Name = ("Draconi's Wrath");
		
            this.Hue = 1177;
		
            this.AbsorptionAttributes.EaterFire = 20;
            this.WeaponAttributes.HitFireball = 60;	
            this.Attributes.AttackChance = 15;
            this.Attributes.WeaponDamage = 50;
            this.WeaponAttributes.UseBestSkill = 1;	
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