using System;

namespace Server.Mobiles
{
    [CorpseName("an oni corpse")]
    public class Oni : BaseCreature
    {
        //private DateTime m_NextAbilityTime;

        [Constructable]
        public Oni()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an oni";
            Body = 241;

            SetStr(801, 910);
            SetDex(151, 300);
            SetInt(171, 195);

            SetHits(401, 530);

            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Fire, 10);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 65, 80);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 35, 50);
            SetResistance(ResistanceType.Poison, 45, 70);
            SetResistance(ResistanceType.Energy, 45, 65);

            SetSkill(SkillName.EvalInt, 100.1, 125.0);
            SetSkill(SkillName.Magery, 96.1, 106.0);
            SetSkill(SkillName.Anatomy, 85.1, 95.0);
            SetSkill(SkillName.MagicResist, 85.1, 100.0);
            SetSkill(SkillName.Tactics, 86.1, 101.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 15000;
            Karma = -15000;

            if (Utility.RandomDouble() < .33)
                PackItem(Engines.Plants.Seed.RandomBonsaiSeed());

            SetSpecialAbility(SpecialAbility.AngryFire);
        }

        public Oni(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override int GetAngerSound()
        {
            return 0x4E3;
        }

        public override int GetIdleSound()
        {
            return 0x4E2;
        }

        public override int GetAttackSound()
        {
            return 0x4E1;
        }

        public override int GetHurtSound()
        {
            return 0x4E4;
        }

        public override int GetDeathSound()
        {
            return 0x4E0;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
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
        }
    }
}
