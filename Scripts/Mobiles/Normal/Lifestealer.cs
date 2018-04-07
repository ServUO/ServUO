using System;

namespace Server.Mobiles
{
    [CorpseName("a lifestealer corpse")]
    public class Lifestealer : BaseCreature
    {
        [Constructable]
        public Lifestealer()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lifestealer";
            Body = 303;
            Hue = 2606;
            BaseSoundID = 357;

            SetStr(200, 240);
            SetDex(130, 150);
            SetInt(200, 250);

            SetHits(4600, 4650);

            SetDamage(22, 26);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 10);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Necromancy, 90.1, 100.0);
            SetSkill(SkillName.SpiritSpeak, 90.1, 105.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 90.1, 105.0);
            SetSkill(SkillName.Tactics, 75.1, 85.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);
            SetSkill(SkillName.DetectHidden, 84.3);

            Fame = 9500;
            Karma = -9500;

            VirtualArmor = 44;

            PackNecroReg(24, 45);
        }

        public Lifestealer(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int Meat { get { return 3; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
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
