using System;

namespace Server.Mobiles
{
    [CorpseName("a rock mite corpse")]
    public class RockMite : BaseCreature
    {
        [Constructable]
        public RockMite()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rock mite";
            Body = 787;
            BaseSoundID = 1006;
            Hue = 2500;

            SetStr(733, 754);
            SetDex(126, 144);
            SetInt(75, 94);

            SetHits(803, 817);
            SetStam(126, 144);
            SetMana(75, 94);

            SetDamage(12, 19);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 50, 59);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 52, 59);
            SetResistance(ResistanceType.Poison, 80, 85);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.MagicResist, 88.5, 119.6);
            SetSkill(SkillName.Tactics, 84.9, 112.9);
            SetSkill(SkillName.Wrestling, 82.7, 119.8);
			SetSkill(SkillName.Parry, 90.0, 100.0);
			SetSkill(SkillName.DetectHidden, 42.9);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 40;

            SetSpecialAbility(SpecialAbility.TailSwipe);
        }

        public RockMite(Serial serial)
            : base(serial)
        {
        }

        public override int GetAngerSound()
        {
            return 0x5A;
        }

        public override int GetIdleSound()
        {
            return 0x5A;
        }

        public override int GetAttackSound()
        {
            return 0x164;
        }

        public override int GetHurtSound()
        {
            return 0x187;
        }

        public override int GetDeathSound()
        {
            return 0x1BA;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, Utility.Random(1, 5));
        }

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
