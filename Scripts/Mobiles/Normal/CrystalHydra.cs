using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal hydra corpse")]
    public class CrystalHydra : BaseCreature
    {
        [Constructable]
        public CrystalHydra()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a crystal hydra";
            Body = 0x109;
            Hue = 0x47E;
            BaseSoundID = 0x16A;

            SetStr(800, 830);
            SetDex(100, 120);
            SetInt(100, 120);

            SetHits(1450, 1500);

            SetDamage(21, 26);

            SetDamageType(ResistanceType.Physical, 5);
            SetDamageType(ResistanceType.Fire, 5);
            SetDamageType(ResistanceType.Cold, 80);
            SetDamageType(ResistanceType.Poison, 5);
            SetDamageType(ResistanceType.Energy, 5);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 80, 100);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 80, 100);

            SetSkill(SkillName.Wrestling, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0, 110.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Anatomy, 70.0, 80.0);

            Fame = 17000;
            Karma = -17000;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public CrystalHydra(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.HighScrolls);
            AddLoot(LootPack.Parrot);
            AddLoot(LootPack.ArcanistScrolls);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.25)
                c.DropItem(new ShatteredCrystals());
        }

        public override int Hides => 40;
        public override int Meat => 19;
        public override int TreasureMapLevel => 5;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
