using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a goblin corpse")]
    public class GreenGoblinAlchemist : BaseCreature
    {
        [Constructable]
        public GreenGoblinAlchemist()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a green goblin alchemist";
            Body = 723;
            BaseSoundID = 0x600;

            SetStr(282, 331);
            SetDex(62, 79);
            SetInt(100, 149);

            SetHits(163, 197);
            SetStam(62, 79);
            SetMana(100, 149);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 11, 20);

            SetSkill(SkillName.MagicResist, 120.3, 129.2);
            SetSkill(SkillName.Tactics, 81.9, 87.1);
            SetSkill(SkillName.Anatomy, 0.0, 0.0);
            SetSkill(SkillName.Wrestling, 94.8, 106.9);

            Fame = 1500;
            Karma = -1500;
        }

        public GreenGoblinAlchemist(Serial serial)
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
        public override TribeType Tribe => TribeType.GreenGoblin;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LootItem<BolaBall>(20.0));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.02)
                c.DropItem(new LuckyCoin());
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
