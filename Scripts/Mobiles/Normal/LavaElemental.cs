using System;
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
            this.Name = "a lava elemental";
            this.Body = 720; 

            this.SetStr(446, 510);
            this.SetDex(160, 190);
            this.SetInt(360, 430);

            this.SetHits(270, 290);

            this.SetDamage(12, 18);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Fire, 90);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 84.8, 92.6);
            this.SetSkill(SkillName.Magery, 80.0, 92.7);
            this.SetSkill(SkillName.Meditation, 97.8, 120.0);
            this.SetSkill(SkillName.MagicResist, 101.9, 106.2);
            this.SetSkill(SkillName.Tactics, 80.3, 94.0);
            this.SetSkill(SkillName.Wrestling, 71.7, 85.4);
            this.SetSkill(SkillName.Poisoning, 90.0, 100.0);
            this.SetSkill(SkillName.DetectHidden, 75.1);

            AddItem(new Nightshade(4));
            AddItem(new SulfurousAsh(5));
            AddItem(new LesserPoisonPotion());
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
        }

        public override int GetAttackSound() { return 0x60A; }
        public override int GetDeathSound() { return 0x60B; }
        public override int GetHurtSound() { return 0x60C; }
        public override int GetIdleSound() { return 0x60D; }

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