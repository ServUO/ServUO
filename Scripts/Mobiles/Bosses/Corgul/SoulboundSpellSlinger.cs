namespace Server.Mobiles
{
    public class SoulboundSpellSlinger : EvilMageLord
    {
        [Constructable]
        public SoulboundSpellSlinger()
        {
            Title = "the soulbound spellslinger";

            SetStr(120, 130);
            SetDex(90, 100);
            SetInt(120, 150);

            SetHits(190, 200);
            SetMana(400, 500);

            SetDamage(8, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Wrestling, 90.0, 100.0);
            SetSkill(SkillName.Tactics, 80.0, 90.0);
            SetSkill(SkillName.MagicResist, 90, 100.0);
            SetSkill(SkillName.Anatomy, 1.0, 0.0);
            SetSkill(SkillName.Magery, 100.0, 110.0);
            SetSkill(SkillName.EvalInt, 80.0, 90.0);
            SetSkill(SkillName.Meditation, 20.0, 30.0);

            Fame = 3000;
            Karma = -3000;
        }

        public override int TreasureMapLevel => 3;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override bool OnBeforeDeath()
        {
            if (Region.IsPartOf<Regions.CorgulRegion>())
            {
                CorgulTheSoulBinder.CheckDropSOT(this);
            }

            return base.OnBeforeDeath();
        }

        public SoulboundSpellSlinger(Serial serial)
            : base(serial)
        {
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
