using System;

namespace Server.Items
{
    public class ObsidianEarrings : GargishEarrings
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113820; } } // Obsidian Earrings

        public override int BasePhysicalResistance { get { return 4; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 10; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 13; } }

        [Constructable]
        public ObsidianEarrings()
        {	
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 2;
            Attributes.RegenStam = 2;
            Attributes.SpellDamage = 8;
            AbsorptionAttributes.CastingFocus = 4;
        }

        public ObsidianEarrings(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
