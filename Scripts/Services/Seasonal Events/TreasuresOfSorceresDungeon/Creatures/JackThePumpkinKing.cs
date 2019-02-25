using System;

using Server.Mobiles;
using Server.Items;

namespace Server.Engines.SorcerersDungeon
{
    [CorpseName("the corpse of jack the pumpkin king")]
    public class JackThePumpkinKing : BaseCreature
    {
        [Constructable]
        public JackThePumpkinKing()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "jack";
            Title = "the pumpkin king";
            Body = 0x190;
            Hue = Race.RandomSkinHue();

            SetStr(500);
            SetDex(200);
            SetInt(1200);

            SetHits(26000);

            SetDamage(22, 26);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 70, 80);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Anatomy, 25);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Tactics, 100, 110);
            SetSkill(SkillName.Wrestling, 130, 140);
            SetSkill(SkillName.Parry, 20, 30);

            SetSkill(SkillName.Magery, 120);
            SetSkill(SkillName.EvalInt, 120);
            SetSkill(SkillName.Necromancy, 120);
            SetSkill(SkillName.SpiritSpeak, 120);
            SetSkill(SkillName.Meditation, 120);
            SetSkill(SkillName.Focus, 70, 80);

            Fame = 12000;
            Karma = -12000;

            SetWearable(new ClothNinjaHood(), 1281);
            SetWearable(new BoneChest(), 1175);
            SetWearable(new BoneArms(), 1175);
            SetWearable(new BoneGloves(), 1175);
            SetWearable(new ThighBoots());
            SetWearable(new Scepter());
        }

        public JackThePumpkinKing(Serial serial)
            : base(serial)
        {
        }

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
