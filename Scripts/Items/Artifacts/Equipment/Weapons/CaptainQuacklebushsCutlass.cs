using System;

namespace Server.Items
{
    public class CaptainQuacklebushsCutlass : Cutlass
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public CaptainQuacklebushsCutlass()
        {
            this.Hue = 0x66C;
            this.Attributes.BonusDex = 5;
            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 50;
            this.WeaponAttributes.UseBestSkill = 1;
        }

        public CaptainQuacklebushsCutlass(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063474;
            }
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