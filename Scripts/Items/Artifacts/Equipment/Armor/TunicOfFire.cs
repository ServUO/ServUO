using System;

namespace Server.Items
{
    public class TunicOfFire : ChainChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TunicOfFire()
        {
            Hue = 0x54F;
            ArmorAttributes.SelfRepair = 5;
            Attributes.NightSight = 1;
            Attributes.ReflectPhysical = 15;
            Attributes.WeaponSpeed = 5;
            Attributes.LowerRegCost = 15;
            Attributes.LowerManaCost = 8;
            AbsorptionAttributes.EaterFire = 15;
            AbsorptionAttributes.ResonanceFire = 15;
            SkillBonuses.SetValues(0, SkillName.Anatomy, 20.0);
        }

        public TunicOfFire(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061099;
            }
        }// Tunic of Fire
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 24;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 34;
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

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                if (this.Hue == 0x54E)
                    this.Hue = 0x54F;

                if (this.Attributes.NightSight == 0)
                    this.Attributes.NightSight = 1;

                this.PhysicalBonus = 0;
                this.FireBonus = 0;
            }
        }
    }
}