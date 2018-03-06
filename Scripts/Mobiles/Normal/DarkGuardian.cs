using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a dark guardians' corpse")]
    public class DarkGuardian : BaseCreature
    {
        [Constructable]
        public DarkGuardian()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dark guardian";
            Body = 78;
            BaseSoundID = 0x3E9;

            SetStr(125, 150);
            SetDex(100, 120);
            SetInt(200, 235);

            SetHits(150, 180);
            SetDamage(24, 26);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 20, 45);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 45);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 40.1, 50.0);
            SetSkill(SkillName.Magery, 50.1, 60.0);
            SetSkill(SkillName.Meditation, 85.0, 95.0);
            SetSkill(SkillName.MagicResist, 50.1, 70.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Necromancy, 100.0, 110.0);
            SetSkill(SkillName.SpiritSpeak, 95.0, 105.0);
            SetSkill(SkillName.DetectHidden, 55.0, 60.0);

            Fame = 5000;
            Karma = -5000;

            VirtualArmor = 50;
            PackNecroReg(15, 25);
        }

        public DarkGuardian(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);
        }

        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.FeyAndUndead; }
        }

        public override int TreasureMapLevel { get { return Utility.RandomMinMax(1, 3); } }
        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool Unprovokable { get { return true; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}