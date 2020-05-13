using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lava elemental corpse")]
    public class LavaElemental : BaseCreature
    {
        [Constructable]
        public LavaElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a lava elemental";
            Body = 720;

            SetStr(446, 510);
            SetDex(160, 190);
            SetInt(360, 430);

            SetHits(270, 290);

            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Fire, 90);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 84.8, 92.6);
            SetSkill(SkillName.Magery, 80.0, 92.7);
            SetSkill(SkillName.Meditation, 97.8, 120.0);
            SetSkill(SkillName.MagicResist, 101.9, 106.2);
            SetSkill(SkillName.Tactics, 80.3, 94.0);
            SetSkill(SkillName.Wrestling, 71.7, 85.4);
            SetSkill(SkillName.Poisoning, 90.0, 100.0);
            SetSkill(SkillName.DetectHidden, 75.1);
        }

        public LavaElemental(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.MedScrolls);

            AddLoot(LootPack.LootItem<Nightshade>(4, true));
            AddLoot(LootPack.LootItem<SulfurousAsh>(5, true));
            AddLoot(LootPack.LootItem<LesserPoisonPotion>(true));
        }

        public override int GetAttackSound() { return 0x60A; }
        public override int GetDeathSound() { return 0x60B; }
        public override int GetHurtSound() { return 0x60C; }
        public override int GetIdleSound() { return 0x60D; }

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
