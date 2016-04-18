using System;

namespace Server.Items
{
    public class VampiricEssence : Cutlass
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public VampiricEssence()
        {
            this.Name = ("Vampiric Essence");
		
            this.Hue = 39;
            this.WeaponAttributes.HitLeechHits = 100;			
            this.WeaponAttributes.HitHarm = 50;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 50;
            this.AosElementDamages.Cold = 100;
            this.WeaponAttributes.BloodDrinker = 1;
        }

        public VampiricEssence(Serial serial)
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

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Attributes.AttackChance == 50)
                this.Attributes.AttackChance = 10;
        }
    }
}