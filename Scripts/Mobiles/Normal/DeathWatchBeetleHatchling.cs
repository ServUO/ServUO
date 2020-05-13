namespace Server.Mobiles
{
    [CorpseName("a deathwatchbeetle hatchling corpse")]
    public class DeathwatchBeetleHatchling : BaseCreature
    {
        [Constructable]
        public DeathwatchBeetleHatchling()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a deathwatch beetle hatchling";
            Body = 242;

            SetStr(26, 50);
            SetDex(41, 52);
            SetInt(21, 30);

            SetHits(51, 60);
            SetMana(20);

            SetDamage(2, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 15, 30);
            SetResistance(ResistanceType.Cold, 15, 30);
            SetResistance(ResistanceType.Poison, 20, 40);
            SetResistance(ResistanceType.Energy, 20, 35);

            SetSkill(SkillName.Wrestling, 30.1, 40.0);
            SetSkill(SkillName.Tactics, 47.1, 57.0);
            SetSkill(SkillName.MagicResist, 30.1, 38.0);
            SetSkill(SkillName.Anatomy, 20.1, 24.0);

            Fame = 700;
            Karma = -700;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LowScrolls, 1);
            AddLoot(LootPack.Potions, 1);

            if (Utility.RandomBool())
            {
                AddLoot(LootPack.MageryRegs, 3);
            }
        }

        public DeathwatchBeetleHatchling(Serial serial)
            : base(serial)
        {
        }

        public override int Hides => 8;
        public override int GetAngerSound()
        {
            return 0x4F3;
        }

        public override int GetIdleSound()
        {
            return 0x4F2;
        }

        public override int GetAttackSound()
        {
            return 0x4F1;
        }

        public override int GetHurtSound()
        {
            return 0x4F4;
        }

        public override int GetDeathSound()
        {
            return 0x4F0;
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
