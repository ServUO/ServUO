using System;

namespace Server.Items
{
    public class HolyKnightsBreastplate : PlateChest
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public HolyKnightsBreastplate()
        {
            Hue = 0x47E;
            Attributes.BonusHits = 10;
            Attributes.ReflectPhysical = 15;
            Attributes.AttackChance = 15;
            Attributes.CastRecovery = 1;
            Attributes.LowerManaCost = 8;
            Attributes.RegenMana = 4;
            SkillBonuses.SetValues(0, SkillName.Chivalry, 20.0);
            AbsorptionAttributes.EaterDamage = 10;
        }

        public HolyKnightsBreastplate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061097;
            }
        }// Holy Knight's Breastplate
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
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
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
                this.PhysicalBonus = 0;
        }
    }
}