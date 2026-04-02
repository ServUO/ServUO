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
            SkillBonuses.SetValues(0, SkillName.Cartography, 15.0);
            Attributes.Luck = 250;
            Attributes.LowerRegCost = 15;
            Attributes.LowerManaCost = 8;
            Quality = ItemQuality.Exceptional;
        }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }

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
