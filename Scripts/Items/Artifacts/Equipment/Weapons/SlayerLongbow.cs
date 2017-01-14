using System;

namespace Server.Items
{
    public class SlayerLongbow : ElvenCompositeLongbow
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SlayerLongbow()
        {
            this.Slayer2 = BaseRunicTool.GetRandomSlayer();
        }

        public SlayerLongbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073506;
            }
        }// slayer longbow
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