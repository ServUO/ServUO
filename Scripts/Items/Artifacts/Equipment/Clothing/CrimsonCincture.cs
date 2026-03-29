using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishCrimsonCincture))]
    public class CrimsonCincture : HalfApron
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public CrimsonCincture()
            : base()
        {
            Hue = 0x485;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.BonusStam = 8;
            Attributes.RegenHits = 2;
            Attributes.RegenMana = 3;

            SkillBonuses.SetValues(0, GetRandomSkill(), 20.0);
        }

        public static SkillName GetRandomSkill()
        {
            SkillName[] skills = new SkillName[]
            {
                SkillName.MagicResist, SkillName.Tactics, SkillName.Anatomy, SkillName.Focus,
                SkillName.SpiritSpeak, SkillName.Ninjitsu, SkillName.Bushido, SkillName.Healing
            };
            return skills[Utility.Random(skills.Length)];
        }

        public CrimsonCincture(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075043;
            }
        }// Crimson Cincture
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

    public class GargishCrimsonCincture : GargoyleHalfApron
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishCrimsonCincture()
            : base()
        {
            Hue = 0x485;
            Attributes.BonusDex = 5;
            Attributes.BonusHits = 10;
            Attributes.BonusStam = 8;
            Attributes.RegenHits = 2;
            Attributes.RegenMana = 3;

            SkillBonuses.SetValues(0, CrimsonCincture.GetRandomSkill(), 20.0);
        }

        public GargishCrimsonCincture(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075043;
            }
        }// Crimson Cincture
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