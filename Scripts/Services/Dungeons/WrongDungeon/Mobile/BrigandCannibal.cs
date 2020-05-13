namespace Server.Mobiles
{
    [CorpseName("brigand cannibal corpse")]
    public class BrigandCannibal : Brigand
    {
        [Constructable]
        public BrigandCannibal()
            : base()
        {
            Title = "the brigand cannibal";
            Hue = 33782;

            SetStr(68, 95);
            SetDex(81, 95);
            SetInt(110, 115);

            SetHits(2058, 2126);
            SetMana(552, 553);

            SetDamage(10, 23);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 68);
            SetResistance(ResistanceType.Fire, 65, 66);
            SetResistance(ResistanceType.Cold, 62, 69);
            SetResistance(ResistanceType.Poison, 62, 67);
            SetResistance(ResistanceType.Energy, 64, 68);

            SetSkill(SkillName.MagicResist, 96.9, 96.9);
            SetSkill(SkillName.Tactics, 94.0, 94.0);
            SetSkill(SkillName.Swords, 54.3, 54.3);

            Fame = 14500;
            Karma = -14500;
        }

        public BrigandCannibal(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
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