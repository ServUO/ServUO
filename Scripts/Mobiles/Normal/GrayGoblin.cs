using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a goblin corpse")]
    public class GrayGoblin : BaseCreature
    {
        [Constructable]
        public GrayGoblin()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gray goblin";

            Body = 723;
            Hue = 1900;
            BaseSoundID = 0x600;

            SetStr(258, 327);
            SetDex(62, 80);
            SetInt(103, 150);

            SetHits(159, 194);
            SetStam(62, 80);
            SetMana(103, 150);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 25, 32);
            SetResistance(ResistanceType.Poison, 10, 19);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 120.9, 129.1);
            SetSkill(SkillName.Tactics, 80.6, 89.4);
            SetSkill(SkillName.Anatomy, 80.3, 89.4);
            SetSkill(SkillName.Wrestling, 96.1, 105.5);

            Fame = 1500;
            Karma = -1500;
        }

        public GrayGoblin(Serial serial)
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
        public override TribeType Tribe => TribeType.GrayGoblin;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LootItem<BolaBall>(20.0));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
