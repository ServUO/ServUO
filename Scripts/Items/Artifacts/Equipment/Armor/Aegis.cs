using System;

namespace Server.Items
{
    public class Aegis : HeaterShield
    {
		public override bool IsArtifact { get { return true; } }

        private static readonly SkillName[] m_PossibleBonusSkills = new SkillName[]
        {
            SkillName.Parry,
            SkillName.Tactics,
            SkillName.Anatomy,
            SkillName.Healing,
            SkillName.MagicResist
        };

        [Constructable]
        public Aegis()
        {
            Hue = 0x47E;
            ArmorAttributes.SelfRepair = 5;
            ArmorAttributes.ReactiveParalyze = 1;
            Attributes.ReflectPhysical = 15;
            Attributes.DefendChance = 15;
            Attributes.LowerManaCost = 8;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponDamage = 15;
            Attributes.RegenStam = 4;
            Attributes.BonusStam = 8;
            SkillBonuses.SetValues(0, m_PossibleBonusSkills[Utility.Random(m_PossibleBonusSkills.Length)], 20.0);
        }

        public Aegis(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061602;
            }
        }// �gis
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