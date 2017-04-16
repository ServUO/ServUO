using System;
using Server;
using Server.Items;
using Server.Engines.MyrmidexInvasion;

namespace Server.Mobiles
{
    [CorpseName("a kotl warrior corpse")]
    public class SpectralKotlWarrior : BaseCreature
    {
        [Constructable]
        public SpectralKotlWarrior()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a spectral kotl warrior";

            Body = 0x24;
            BaseSoundID = 417;
            Hue = 2951;

            SetStr(500, 600);
            SetDex(82, 95);
            SetInt(130, 140);

            SetDamage(18, 22);

            SetHits(2800, 3000);
            SetMana(40, 50);

            SetResistance(ResistanceType.Physical, 1, 10);
            SetResistance(ResistanceType.Fire, 1, 10);
            SetResistance(ResistanceType.Cold, 1, 10);
            SetResistance(ResistanceType.Poison, 1, 10);
            SetResistance(ResistanceType.Energy, 1, 10);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetSkill(SkillName.Wrestling, 90, 100);
            SetSkill(SkillName.Tactics, 90, 100);
            SetSkill(SkillName.MagicResist, 70, 80);
            SetSkill(SkillName.Poisoning, 70, 80);
            SetSkill(SkillName.Magery, 80, 90);
            SetSkill(SkillName.EvalInt, 70, 80);

            Fame = 16000;
            Karma = -16000;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 2);
        }

        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

        public override bool IsEnemy(Mobile m)
        {
            if (m is SpectralMyrmidexWarrior)
                return true;

            return base.IsEnemy(m);
        }

        public SpectralKotlWarrior(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}