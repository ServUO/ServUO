using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a tangle corpse")]
    public class Tangle : BogThing
    {
        [Constructable]
        public Tangle()
            : base()
        {
            Name = "Tangle";
            Hue = 0x21;

            SetStr(843, 943);
            SetDex(58, 74);
            SetInt(46, 58);

            SetHits(2468, 2733);

            SetDamage(15, 28);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 50, 57);
            SetResistance(ResistanceType.Fire, 40, 43);
            SetResistance(ResistanceType.Cold, 30, 35);
            SetResistance(ResistanceType.Poison, 61, 69);
            SetResistance(ResistanceType.Energy, 41, 45);

            SetSkill(SkillName.Wrestling, 80.8, 94.6);
            SetSkill(SkillName.Tactics, 90.6, 100.4);
            SetSkill(SkillName.MagicResist, 108.4, 114.0);

            Fame = 16000;
            Karma = -16000;
        }

        public Tangle(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.ArcanistScrolls, 1, 3);
            AddLoot(LootPack.LootItem<TaintedSeeds>(30.0));
        }

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
