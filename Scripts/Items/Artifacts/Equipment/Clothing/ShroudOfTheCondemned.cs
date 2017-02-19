using System;

namespace Server.Items
{
    [Flipable(0x1F03, 0x1F04)]
    public class ShroudOfTheCondemned : BaseOuterTorso
	{
		public override bool IsArtifact { get { return true; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        public override int LabelNumber { get { return 1113703; } } // Shroud of the Condemned

        [Constructable]
        public ShroudOfTheCondemned()
            : base(0x1F04, 0xD6)
        {
            this.Hue = 2075;
            this.Attributes.BonusHits = 3;
            this.Attributes.BonusInt = 5;
        }

        public ShroudOfTheCondemned(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}