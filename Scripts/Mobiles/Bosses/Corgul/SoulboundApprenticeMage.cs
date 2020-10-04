namespace Server.Mobiles
{
    public class SoulboundApprenticeMage : EvilMage
    {
        [Constructable]
        public SoulboundApprenticeMage()
        {
            Title = "the soulbound apprentice mage";

            SetStr(115);
            SetDex(97);
            SetInt(106);

            SetHits(128);
            SetMana(210);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20);
            SetResistance(ResistanceType.Fire, 21);
            SetResistance(ResistanceType.Cold, 22);
            SetResistance(ResistanceType.Poison, 20);
            SetResistance(ResistanceType.Energy, 25);

            SetSkill(SkillName.Wrestling, 40.0, 50.0);
            SetSkill(SkillName.MagicResist, 40.0, 50.0);
            SetSkill(SkillName.Magery, 60.2, 72.4);
            SetSkill(SkillName.EvalInt, 60.1, 73.4);
            SetSkill(SkillName.Meditation, 40.0, 50.0);

            Fame = 1000;
            Karma = -1000;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
        }

        public override bool OnBeforeDeath()
        {
            if (Region.IsPartOf<Regions.CorgulRegion>())
            {
                CorgulTheSoulBinder.CheckDropSOT(this);
            }

            return base.OnBeforeDeath();
        }

        public SoulboundApprenticeMage(Serial serial)
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
