using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGoblinScout : BaseCreature
    {
        [Constructable]
        public EnslavedGoblinScout()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "enslaved goblin scout";
            Body = 334;
            BaseSoundID = 0x600;

            SetStr(320, 320);
            SetDex(74, 74);
            SetInt(112, 112);

            SetHits(182, 182);
            SetStam(74, 74);
            SetMana(112, 112);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 42, 42);
            SetResistance(ResistanceType.Fire, 33, 33);
            SetResistance(ResistanceType.Cold, 30, 30);
            SetResistance(ResistanceType.Poison, 14, 14);
            SetResistance(ResistanceType.Energy, 18, 18);

            SetSkill(SkillName.MagicResist, 95.0, 95.0);
            SetSkill(SkillName.Tactics, 80.0, 86.9);
            SetSkill(SkillName.Anatomy, 82.0, 89.3);
            SetSkill(SkillName.Wrestling, 99.2, 113.7);

            Fame = 1500;
            Karma = -1500;
        }

        public EnslavedGoblinScout(Serial serial)
            : base(serial)
        {
        }

        public override int GetAngerSound() { return 0x600; }
        public override int GetIdleSound() { return 0x600; }
        public override int GetAttackSound() { return 0x5FD; }
        public override int GetHurtSound() { return 0x5FF; }
        public override int GetDeathSound() { return 0x5FE; }

        public override bool CanRummageCorpses => true;
        public override int TreasureMapLevel => 1;
        public override int Meat => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LootItem<BolaBall>(20.0));
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
