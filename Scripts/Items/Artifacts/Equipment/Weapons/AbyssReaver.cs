namespace Server.Items
{
    public class AbyssReaver : Cyclone
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1112694;  // Abyss Reaver

        [Constructable]
        public AbyssReaver()
        {
            SkillBonuses.SetValues(0, SkillName.Throwing, Utility.RandomMinMax(5, 10));
            Attributes.WeaponDamage = Utility.RandomMinMax(25, 35);
            Slayer = SlayerName.Exorcism;
        }

        public AbyssReaver(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = InheritsItem ? 1 : reader.ReadInt();

            if (version == 0)
            {
                Attributes.AttackChance = 0;
                Attributes.WeaponSpeed = 0;
            }
        }
    }
}
