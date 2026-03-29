using System;

namespace Server.Items
{
    public class GeneralLethesEpaulettes : Epaulette
    {
        public override bool IsArtifact { get { return true; } }

        public static SkillName GetRandomCasterSkill()
        {
            SkillName[] skills = new SkillName[] { SkillName.EvalInt, SkillName.Magery, SkillName.Mysticism, SkillName.Spellweaving };
            return skills[Utility.Random(skills.Length)];
        }

        [Constructable]
        public GeneralLethesEpaulettes()
        {
            Name = "General Lethe's Epaulettes";
            Hue = 1157;

            SkillBonuses.SetValues(0, GeneralLethesEpaulettes.GetRandomCasterSkill(), 20.0);
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 1;
            Attributes.CastRecovery = 1;
            Attributes.LowerRegCost = 10;
        }

        public GeneralLethesEpaulettes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GargishGeneralLethesEpaulettes : GargishEpaulette
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishGeneralLethesEpaulettes()
        {
            Name = "Gargish General Lethe's Epaulettes";
            Hue = 1157;

            SkillBonuses.SetValues(0, GeneralLethesEpaulettes.GetRandomCasterSkill(), 20.0);
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 1;
            Attributes.CastRecovery = 1;
            Attributes.LowerRegCost = 10;
        }

        public GargishGeneralLethesEpaulettes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class LordMorphiusEpaulettes : Epaulette
    {
        public override bool IsArtifact { get { return true; } }

        public static SkillName GetRandomWarriorSkill()
        {
            SkillName[] skills = new SkillName[] { SkillName.Anatomy, SkillName.Tactics, SkillName.Healing, SkillName.MagicResist, SkillName.Bushido, SkillName.Chivalry, SkillName.Necromancy };
            return skills[Utility.Random(skills.Length)];
        }

        [Constructable]
        public LordMorphiusEpaulettes()
        {
            Name = "Lord Morphius' Epaulettes";
            Hue = 1161;

            SkillBonuses.SetValues(0, LordMorphiusEpaulettes.GetRandomWarriorSkill(), 20.0);
            Attributes.BonusStam = 8;
            Attributes.RegenStam = 2;
            Attributes.LowerManaCost = 5;
            Attributes.WeaponSpeed = 10;
        }

        public LordMorphiusEpaulettes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GargishLordMorphiusEpaulettes : GargishEpaulette
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishLordMorphiusEpaulettes()
        {
            Name = "Gargish Lord Morphius' Epaulettes";
            Hue = 1161;

            SkillBonuses.SetValues(0, LordMorphiusEpaulettes.GetRandomWarriorSkill(), 20.0);
            Attributes.BonusStam = 8;
            Attributes.RegenStam = 2;
            Attributes.LowerManaCost = 5;
            Attributes.WeaponSpeed = 10;
        }

        public GargishLordMorphiusEpaulettes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
