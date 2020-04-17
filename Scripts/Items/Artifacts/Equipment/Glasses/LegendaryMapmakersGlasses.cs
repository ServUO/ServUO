namespace Server.Items
{
    public class LegendaryMapmakersGlasses : ElvenGlasses
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1159023;  //Legendary Mapmaker's Glasses

        [Constructable]
        public LegendaryMapmakersGlasses()
        {
            SkillBonuses.SetValues(0, SkillName.Cartography, Utility.RandomMinMax(1, 5));
            Quality = ItemQuality.Exceptional;
        }

        public LegendaryMapmakersGlasses(Serial serial)
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
