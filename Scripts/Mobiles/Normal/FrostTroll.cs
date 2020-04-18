namespace Server.Mobiles
{
    [CorpseName("a frost troll corpse")]
    public class FrostTroll : BaseCreature
    {
        [Constructable]
        public FrostTroll()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a frost troll";
            Body = 55;
            BaseSoundID = 461;

            SetStr(227, 265);
            SetDex(66, 85);
            SetInt(46, 70);

            SetHits(140, 156);

            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Cold, 25);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.MagicResist, 65.1, 80.0);
            SetSkill(SkillName.Tactics, 80.1, 100.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 4000;
            Karma = -4000;
        }

        public FrostTroll(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 2;
        public override int TreasureMapLevel => 1;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems);
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