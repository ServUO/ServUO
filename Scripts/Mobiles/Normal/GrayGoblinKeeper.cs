using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a goblin keeper corpse")]
    public class GrayGoblinKeeper : BaseCreature
    {
        [Constructable]
        public GrayGoblinKeeper()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gray goblin keeper";

            Body = 723;
            Hue = 1900;
            BaseSoundID = 0x600;

            SetStr(326);
            SetDex(79);
            SetInt(114);

            SetHits(186);
            SetStam(79);
            SetMana(114);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45);
            SetResistance(ResistanceType.Fire, 33);
            SetResistance(ResistanceType.Cold, 25);
            SetResistance(ResistanceType.Poison, 20);
            SetResistance(ResistanceType.Energy, 10);

            SetSkill(SkillName.MagicResist, 129.9);
            SetSkill(SkillName.Tactics, 86.7);
            SetSkill(SkillName.Anatomy, 86.6);
            SetSkill(SkillName.Wrestling, 103.6);

            Fame = 1500;
            Karma = -1500;
        }

        public GrayGoblinKeeper(Serial serial)
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
