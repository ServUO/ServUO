using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGoblinKeeper : BaseCreature
    {
        [Constructable]
        public EnslavedGoblinKeeper()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "enslaved goblin keeper";
            Body = 334;
            BaseSoundID = 0x600;

            SetStr(297, 297);
            SetDex(80, 80);
            SetInt(118, 118);

            SetHits(174, 174);
            SetStam(80, 80);
            SetMana(118, 118);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 47, 47);
            SetResistance(ResistanceType.Fire, 37, 37);
            SetResistance(ResistanceType.Cold, 29, 29);
            SetResistance(ResistanceType.Poison, 10, 11);
            SetResistance(ResistanceType.Energy, 19, 19);

            SetSkill(SkillName.MagicResist, 121.6, 122.2);
            SetSkill(SkillName.Tactics, 80.0, 82.8);
            SetSkill(SkillName.Anatomy, 82.0, 84.8);
            SetSkill(SkillName.Wrestling, 99.2, 100.7);

            Fame = 1500;
            Karma = -1500;
        }

        public EnslavedGoblinKeeper(Serial serial)
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
