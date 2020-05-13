using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGrayGoblin : BaseCreature
    {
        [Constructable]
        public EnslavedGrayGoblin()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "enslaved gray goblin";
            Body = 334;
            BaseSoundID = 0x600;

            SetStr(321, 321);
            SetDex(64, 64);
            SetInt(147, 147);

            SetHits(179, 179);
            SetStam(64, 64);
            SetMana(147, 147);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 50);
            SetResistance(ResistanceType.Fire, 38, 38);
            SetResistance(ResistanceType.Cold, 32, 32);
            SetResistance(ResistanceType.Poison, 12, 12);
            SetResistance(ResistanceType.Energy, 11, 11);

            SetSkill(SkillName.MagicResist, 121.6, 121.6);
            SetSkill(SkillName.Tactics, 90.0, 90.0);
            SetSkill(SkillName.Anatomy, 82.0, 82.0);
            SetSkill(SkillName.Wrestling, 99.2, 99.2);

            Fame = 1500;
            Karma = -1500;
        }

        public EnslavedGrayGoblin(Serial serial)
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
