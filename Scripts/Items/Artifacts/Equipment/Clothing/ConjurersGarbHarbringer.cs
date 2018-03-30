using System;

namespace Server.Items
{
    [Flipable(0x1F03, 0x1F04)]
    public class ConjureresGarbHarbringer : BaseOuterTorso, ITokunoDyable
	{
        public override bool CanBeWornByGargoyles { get { return true; } }
		public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ConjureresGarbHarbringer()
            : base(0x1F03, 0x486)
        {
			this.Hue = 0x4AA;
			this.Name = "Conjurer's Garb";
            this.Weight = 3.0;
			this.Attributes.DefendChance = 5;
			this.Attributes.RegenMana = 2;
			// TODO: Supports arcane?
        }

        public ConjureresGarbHarbringer(Serial serial)
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