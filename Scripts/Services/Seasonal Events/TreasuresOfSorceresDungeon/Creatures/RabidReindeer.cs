using System;

using Server.Mobiles;

namespace Server.Engines.SorcerersDungeon
{
    [CorpseName("a rabid reindeers corpse")]
    public class RabidReindeer : BaseCreature
    {
        [Constructable]
        public RabidReindeer()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rabid reindeer";

            Body = 0xEA;
            Hue = 2707;

            SetStr(800);
            SetDex(150);
            SetInt(1200);

            SetHits(8000);

            SetDamage(22, 29);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 100);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Anatomy, 115, 120);
            SetSkill(SkillName.Poisoning, 120);
            SetSkill(SkillName.MagicResist, 115, 120);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 120, 130);

            Fame = 12000;
            Karma = -12000;

            SetSpecialAbility(SpecialAbility.VenomousBite);
        }

        public override int Meat { get { return 6; } }
        public override int Hides { get { return 15; } }

        public override int GetAttackSound()
        {
            return 0x82;
        }

        public override int GetHurtSound()
        {
            return 0x83;
        }

        public override int GetDeathSound()
        {
            return 0x84;
        }

        public RabidReindeer(Serial serial)
            : base(serial)
        {
        }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
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
