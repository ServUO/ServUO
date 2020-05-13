using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an goblin corpse")]
    public class EnslavedGoblinMage : BaseCreature
    {
        [Constructable]
        public EnslavedGoblinMage()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "enslaved goblin mage";
            Body = 334;
            BaseSoundID = 0x600;

            SetStr(297, 297);
            SetDex(94, 94);
            SetInt(510, 510);

            SetHits(174, 174);
            SetStam(94, 94);
            SetMana(510, 510);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 22, 22);
            SetResistance(ResistanceType.Fire, 36, 37);
            SetResistance(ResistanceType.Cold, 39, 39);
            SetResistance(ResistanceType.Poison, 43, 43);
            SetResistance(ResistanceType.Energy, 14, 14);

            SetSkill(SkillName.MagicResist, 121.6, 149.7);
            SetSkill(SkillName.Tactics, 80.0, 85.2);
            SetSkill(SkillName.Anatomy, 82.0, 86.6);
            SetSkill(SkillName.Wrestling, 99.2, 106.4);

            Fame = 1500;
            Karma = -1500;
        }

        public EnslavedGoblinMage(Serial serial)
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
