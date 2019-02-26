using System;

using Server.Mobiles;

namespace Server.Engines.SorcerersDungeon
{
    [CorpseName("a nightmare fairys corpse")]
    public class NightmareFairy : BaseCreature
    {
        [Constructable]
        public NightmareFairy()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a nightmare fairy";

            Body = 176;
            BaseSoundID = 0x467;
            Hue = 1910;

            SetStr(400);
            SetDex(150);
            SetInt(1200);

            SetHits(8000);

            SetDamage(21, 27);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 100);

            SetSkill(SkillName.EvalInt, 120);
            SetSkill(SkillName.Magery, 120);
            SetSkill(SkillName.Meditation, 100);
            SetSkill(SkillName.MagicResist, 200);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 120);

            Fame = 12000;
            Karma = -12000;

            SetSpecialAbility(SpecialAbility.LifeLeech);
        }

        public NightmareFairy(Serial serial)
            : base(serial)
        {
        }

        public override bool DrainsLife { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }

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
