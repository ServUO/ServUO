using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGreenGoblin : BaseCreature
    {
        [Constructable]
        public EnslavedGreenGoblin()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "enslaved green goblin";
            Body = 334;
            BaseSoundID = 0x600;

            SetStr(326, 326);
            SetDex(71, 71);
            SetInt(126, 126);

            SetHits(184, 184);
            SetStam(71, 71);
            SetMana(126, 126);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 40);
            SetResistance(ResistanceType.Fire, 38, 39);
            SetResistance(ResistanceType.Cold, 31, 32);
            SetResistance(ResistanceType.Poison, 12, 12);
            SetResistance(ResistanceType.Energy, 10, 11);

            SetSkill(SkillName.MagicResist, 121.6, 122.9);
            SetSkill(SkillName.Tactics, 80.0, 81.2);
            SetSkill(SkillName.Anatomy, 82.0, 83.4);
            SetSkill(SkillName.Wrestling, 99.2, 99.4);

            Fame = 1500;
            Karma = -1500;
        }

        public EnslavedGreenGoblin(Serial serial)
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
