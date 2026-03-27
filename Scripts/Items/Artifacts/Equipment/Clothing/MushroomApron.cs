using System;

namespace Server.Items
{
    public class MushroomApron : HalfApron
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MushroomApron()
        {
            Name = "Mushroom Apron";
            Hue = 1167;

            SkillBonuses.SetValues(0, SkillName.Alchemy, 10.0);
            SkillBonuses.SetValues(1, GetRandomSkill(), 10.0);
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;
            Attributes.EnhancePotions = 15;
            ApplyRandomStat(Attributes);
        }

        public static SkillName GetRandomSkill()
        {
            SkillName[] skills = new SkillName[]
            {
                SkillName.Swords, SkillName.Macing, SkillName.Fencing, SkillName.Archery,
                SkillName.Wrestling, SkillName.Throwing, SkillName.Magery, SkillName.Necromancy,
                SkillName.Mysticism, SkillName.Chivalry, SkillName.Bushido, SkillName.Ninjitsu
            };
            return skills[Utility.Random(skills.Length)];
        }

        public static void ApplyRandomStat(AosAttributes attributes)
        {
            switch (Utility.Random(3))
            {
                case 0: attributes.BonusStr = 10; break;
                case 1: attributes.BonusDex = 10; break;
                case 2: attributes.BonusInt = 10; break;
            }
        }

        public MushroomApron(Serial serial)
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

    public class GargishMushroomApron : GargoyleHalfApron
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishMushroomApron()
        {
            Name = "Gargish Mushroom Apron";
            Hue = 1167;

            SkillBonuses.SetValues(0, SkillName.Alchemy, 10.0);
            SkillBonuses.SetValues(1, MushroomApron.GetRandomSkill(), 10.0);
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;
            Attributes.EnhancePotions = 15;
            MushroomApron.ApplyRandomStat(Attributes);
        }

        public GargishMushroomApron(Serial serial)
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
