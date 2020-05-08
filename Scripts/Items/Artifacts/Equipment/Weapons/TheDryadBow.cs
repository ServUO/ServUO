namespace Server.Items
{
    public class TheDryadBow : Bow
    {
        public override bool IsArtifact => true;
		public override int LabelNumber => 1061090;// The Dryad Bow
        public override int ArtifactRarity => 11;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
		
        private static readonly SkillName[] m_PossibleBonusSkills = new SkillName[]
        {
            SkillName.Archery,
            SkillName.Healing,
            SkillName.MagicResist,
            SkillName.Peacemaking,
            SkillName.Chivalry,
            SkillName.Ninjitsu
        };
        [Constructable]
        public TheDryadBow()
        {
            Hue = 0x48F;
            SkillBonuses.SetValues(0, m_PossibleBonusSkills[Utility.Random(m_PossibleBonusSkills.Length)], (Utility.Random(4) == 0 ? 10.0 : 5.0));
            WeaponAttributes.SelfRepair = 5;
            Attributes.WeaponSpeed = 50;
            Attributes.WeaponDamage = 35;
            WeaponAttributes.ResistPoisonBonus = 15;
        }

        public TheDryadBow(Serial serial)
            : base(serial)
        {
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