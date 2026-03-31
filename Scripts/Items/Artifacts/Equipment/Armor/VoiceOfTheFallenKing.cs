using System;

namespace Server.Items
{
    public class VoiceOfTheFallenKing : LeatherGorget
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public VoiceOfTheFallenKing()
        {
            Hue = 0x76D;
            Attributes.BonusStr = 8;
            Attributes.BonusStam = 12;
            Attributes.BonusMana = 12;
            Attributes.RegenHits = 5;
            Attributes.RegenStam = 3;
            Attributes.LowerManaCost = 8;
            AbsorptionAttributes.EaterDamage = 15;
            SkillBonuses.SetValues(0, SkillName.Healing, 20.0);
        }

        public VoiceOfTheFallenKing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061094;
            }
        }// Voice of the Fallen King
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
                if (this.Hue == 0x551)
                    this.Hue = 0x76D;

                this.ColdBonus = 0;
                this.EnergyBonus = 0;
            }
        }
    }
}