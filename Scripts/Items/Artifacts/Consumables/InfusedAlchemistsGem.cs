namespace Server.Items
{
    public class InfusedAlchemistsGem : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public InfusedAlchemistsGem()
            : base(0x1EA7)
        {
            Weight = 1.0;
        }

        public InfusedAlchemistsGem(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113006;
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1070722, "Alchemy Skill Increaser + 1");
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.Skills[SkillName.Alchemy].Base += 1;
            from.SendMessage("You have increased your Alchemy Skill by 1 Point !.");
            Delete();
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