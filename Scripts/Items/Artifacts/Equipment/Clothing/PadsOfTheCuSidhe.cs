using System;

namespace Server.Items
{
    public class PadsOfTheCuSidhe : FurBoots
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public PadsOfTheCuSidhe()
            : base(0x47E)
        {
        }

        public PadsOfTheCuSidhe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075048;
            }
        }// Pads of the Cu Sidhe
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