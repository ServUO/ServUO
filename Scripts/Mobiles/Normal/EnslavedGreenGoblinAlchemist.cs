using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGreenGoblinAlchemist : BaseCreature
    {
        [Constructable]
        public EnslavedGreenGoblinAlchemist()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "green goblin alchemist";
            Body = 723;
            BaseSoundID = 0x600;

            SetStr(289, 289);
            SetDex(72, 72);
            SetInt(113, 113);

            SetHits(196, 196);
            SetStam(72, 72);
            SetMana(113, 113);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 49);
            SetResistance(ResistanceType.Fire, 50, 53);
            SetResistance(ResistanceType.Cold, 25, 30);
            SetResistance(ResistanceType.Poison, 40, 42);
            SetResistance(ResistanceType.Energy, 15, 18);

            SetSkill(SkillName.MagicResist, 124.1, 126.2);
            SetSkill(SkillName.Tactics, 75.3, 83.6);
            SetSkill(SkillName.Anatomy, 0.0, 0.0);
            SetSkill(SkillName.Wrestling, 90.4, 94.7);

            Fame = 1500;
            Karma = -1500;
        }

        public EnslavedGreenGoblinAlchemist(Serial serial)
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
