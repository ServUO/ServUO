using Server.Mobiles;
using System;

namespace Server.Items
{
    public class TreatiseonAlchemyTalisman : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073353; // Library Talisman - Treatise on Alchemy
        public override bool ForceShowName => true;

        [Constructable]
        public TreatiseonAlchemyTalisman()
            : base(0x2F58)
        {
            Skill = TalismanSkill.Alchemy;
            SuccessBonus = GetRandomSuccessful();
            Blessed = GetRandomBlessed();
            Attributes.EnhancePotions = 15;
            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
        }

        public TreatiseonAlchemyTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PrimerOnArmsTalisman : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073354; // Library Talisman - A Primer on Arms
        public override bool ForceShowName => true;

        [Constructable]
        public PrimerOnArmsTalisman()
            : base(0x2F59)
        {
            Blessed = GetRandomBlessed();
            Attributes.BonusStr = 1;
            Attributes.RegenHits = 2;
            Attributes.WeaponDamage = 20;
            Removal = TalismanRemoval.Damage;
            MaxChargeTime = 1200;
        }

        public PrimerOnArmsTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class MyBookTalisman : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073355; // Library Talisman - My Book
        public override bool ForceShowName => true;

        [Constructable]
        public MyBookTalisman()
            : base(0x2F5A)
        {
            Blessed = GetRandomBlessed();
            Skill = TalismanSkill.Inscription;
            SuccessBonus = GetRandomSuccessful();
            ExceptionalBonus = GetRandomExceptional();
            Attributes.BonusInt = 5;
            Attributes.BonusMana = 2;
        }

        public MyBookTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class TalkingtoWispsTalisman : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073356; // Library Talisman - Talking to Wisps
        public override bool ForceShowName => true;

        [Constructable]
        public TalkingtoWispsTalisman()
            : base(0x2F5B)
        {
            Blessed = GetRandomBlessed();
            SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 3.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
            Removal = TalismanRemoval.Ward;
            MaxChargeTime = 1200;
        }

        public TalkingtoWispsTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GrammarOfOrchishTalisman : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073358; // Library Talisman - a Grammar of Orchish (Summoner)
        public override bool ForceShowName => true;

        [Constructable]
        public GrammarOfOrchishTalisman()
            : base(0x2F59)
        {
            Blessed = GetRandomBlessed();
            Protection = GetRandomProtection();
            Summoner = new TalismanAttribute(typeof(SummonedOrcBrute), 0, 1072414);
            SkillBonuses.SetValues(0, SkillName.MagicResist, 5.0);
            SkillBonuses.SetValues(1, SkillName.Anatomy, 7.0);
            MaxChargeTime = 1800;
        }

        public GrammarOfOrchishTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BirdsofBritanniaTalisman : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1074892; // Library Talisman - Birds of Britannia Random Summoner
        public override bool ForceShowName => true;
        public override Type GetSummoner() { return GetRandomSummonType(); }

        [Constructable]
        public BirdsofBritanniaTalisman()
            : base(0x2F5A)
        {
            Blessed = GetRandomBlessed();
            Slayer = TalismanSlayerName.Bird;
            SkillBonuses.SetValues(0, SkillName.AnimalTaming, 5.0);
            SkillBonuses.SetValues(1, SkillName.AnimalLore, 5.0);
            MaxChargeTime = 1800;
        }

        public BirdsofBritanniaTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class TheLifeOfTravelingMinstrelTalisman : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1073360; // Library Talisman - The Life of a Traveling Minstrel
        public override bool ForceShowName => true;

        [Constructable]
        public TheLifeOfTravelingMinstrelTalisman()
            : base(0x2F5B)
        {
            Blessed = GetRandomBlessed();
            Protection = GetRandomProtection();
            SkillBonuses.SetValues(0, SkillName.Provocation, 5.0);
            SkillBonuses.SetValues(1, SkillName.Musicianship, 5.0);
            Removal = TalismanRemoval.Curse;
            MaxChargeTime = 1200;
        }

        public TheLifeOfTravelingMinstrelTalisman(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}