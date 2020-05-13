namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class Spellbinder : BaseCreature
    {
        [Constructable]
        public Spellbinder()
            : base(AIType.AI_Spellbinder, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a spectral spellbinder";
            Body = 26;
            BaseSoundID = 0x482;

            SetStr(46, 70);
            SetDex(47, 65);
            SetInt(187, 210);

            SetHits(36, 50);

            SetDamage(3, 6);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 10, 20);

            SetSkill(SkillName.MagicResist, 35.1, 45.0);
            SetSkill(SkillName.Tactics, 35.1, 50.0);
            SetSkill(SkillName.Wrestling, 35.1, 50.0);

            Fame = 2500;
            Karma = -2500;
        }

        public Spellbinder(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LootItemCallback(e => Loot.RandomWeapon()));
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
