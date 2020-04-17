namespace Server.Items
{
    public class HygieiasAmulet : GoldNecklace
    {
        public override bool IsArtifact => true;

        [Constructable]
        public HygieiasAmulet()
        {
            SkillBonuses.SetValues(0, SkillName.Alchemy, 10);
        }

        public HygieiasAmulet(Serial serial) : base(serial)
        {
        }

        public override bool CanFortify => false;

        public override int LabelNumber => 1153524; // Hygieia's Amulet [Replica]

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
