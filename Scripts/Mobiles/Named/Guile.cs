namespace Server.Mobiles
{
    [CorpseName("a Guile corpse")]
    public class Guile : Changeling
    {
        [Constructable]
        public Guile()
        {
            Hue = DefaultHue;

            SetStr(53, 214);
            SetDex(243, 367);
            SetInt(369, 586);

            SetHits(1013, 1058);
            SetStam(243, 367);
            SetMana(369, 586);

            SetDamage(14, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 43, 46);
            SetResistance(ResistanceType.Cold, 42, 44);
            SetResistance(ResistanceType.Poison, 42, 50);
            SetResistance(ResistanceType.Energy, 47, 50);

            SetSkill(SkillName.Wrestling, 12.8, 16.7);
            SetSkill(SkillName.Tactics, 102.6, 131.0);
            SetSkill(SkillName.MagicResist, 141.2, 161.6);
            SetSkill(SkillName.Magery, 108.4, 120.0);
            SetSkill(SkillName.EvalInt, 108.4, 120.0);
            SetSkill(SkillName.Meditation, 109.2, 120.0);

            Fame = 21000;
            Karma = -21000;

            SetSpecialAbility(SpecialAbility.StealLife);
        }

        public Guile(Serial serial)
            : base(serial)
        {
        }
        public override bool CanBeParagon => false;
        public override string DefaultName => "Guile";
        public override int DefaultHue => 0x3F;
        public override bool GivesMLMinorArtifact => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.ArcanistScrolls);
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
