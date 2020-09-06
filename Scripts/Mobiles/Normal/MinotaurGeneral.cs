namespace Server.Mobiles
{
    [CorpseName("a minotaur general corpse")]
    public class MinotaurGeneral : MinotaurCaptain
    {
        [Constructable]
        public MinotaurGeneral()
        {
            Name = "a minotaur general";
            Body = 0x118;
            BaseSoundID = 1270;

            SetStr(602, 606);
            SetDex(137, 139);
            SetInt(83, 96);

            SetHits(1006, 1041);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 77, 80);
            SetResistance(ResistanceType.Fire, 59, 64);
            SetResistance(ResistanceType.Cold, 63, 68);
            SetResistance(ResistanceType.Poison, 61, 66);
            SetResistance(ResistanceType.Energy, 63, 66);

            SetSkill(SkillName.Wrestling, 101.0, 129.7);
            SetSkill(SkillName.Tactics, 92.0, 104.8);
            SetSkill(SkillName.MagicResist, 86.0, 87.0);

            Fame = 18000;
            Karma = -18000;
        }

        public MinotaurGeneral(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.ArcanistScrolls, 0, 1);
        }

        public override int TreasureMapLevel => 4;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
