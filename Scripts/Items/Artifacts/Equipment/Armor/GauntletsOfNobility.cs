using System;

namespace Server.Items
{
    public class GauntletsOfNobility : RingmailGloves
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GauntletsOfNobility()
        {
            Hue = 0x4FE;
            Attributes.BonusStr = 8;
            Attributes.Luck = 250;
            Attributes.WeaponDamage = 20;
            Attributes.BonusStam = 10;
            Attributes.BonusMana = 10;
            Attributes.RegenMana = 4;
            Attributes.RegenHits = 4;
            Attributes.RegenStam = 4;
            SkillBonuses.SetValues(0, SkillName.Chivalry, 30.0);
        }

        public GauntletsOfNobility(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061092;
            }
        }// Gauntlets of Nobility
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
            {
                if (this.Hue == 0x562)
                    this.Hue = 0x4FE;

                this.PhysicalBonus = 0;
                this.PoisonBonus = 0;
            }
        }
    }
}