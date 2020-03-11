using System;

namespace Server.Items
{
    public class LegendaryMapmakersGlasses : ElvenGlasses
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1159023; } } //Legendary Mapmaker's Glasses

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
