using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a saurosaurus corpse")]
    public class Saurosaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return !Controlled; } }

        [Constructable]
        public Saurosaurus() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a saurosaurus";
            Body = 1291;
            BaseSoundID = 362;

            SetStr(802, 824);
            SetDex(201, 220);
            SetInt(403, 440);

            SetDamage(21, 28);

            SetHits(1321, 1468);

            SetResistance(ResistanceType.Physical, 75, 85);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 110.0, 120.0);
            SetSkill(SkillName.Wrestling, 110.0, 130.0);
            SetSkill(SkillName.Anatomy, 50.0, 60.0);
            SetSkill(SkillName.DetectHidden, 80.0);
            SetSkill(SkillName.Parry, 80.0, 90);
            SetSkill(SkillName.Focus, 115.0, 125.0);

            Fame = 11000;
            Karma = -11000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 102.0;

            SetWeaponAbility(WeaponAbility.ConcussionBlow);
            SetSpecialAbility(SpecialAbility.TailSwipe);
            SetSpecialAbility(SpecialAbility.LifeLeech);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
        }

        // Missing: Life Leech, Tail Swipe ability

        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }
        public override int DragonBlood { get { return 8; } }
        public override int Meat { get { return 5; } }
        public override int Hides { get { return 11; } }

        public Saurosaurus(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }
}