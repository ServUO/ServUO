using System;
using Server.Engines.Harvest;

namespace Server.Items
{
    public class JadeWarAxe : WarAxe
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public JadeWarAxe()
        {
            this.Name = ("Jade War Axe");
		
            this.Hue = 1162;
			
            this.AbsorptionAttributes.EaterFire = 10;
            this.Slayer = SlayerName.ReptilianDeath;
            this.WeaponAttributes.HitFireball = 30;	
            this.WeaponAttributes.HitLowerDefend = 60;		
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 50;
        }

        public JadeWarAxe(Serial serial)
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
        public override HarvestSystem HarvestSystem
        {
            get
            {
                return null;
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