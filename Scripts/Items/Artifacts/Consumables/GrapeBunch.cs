using Server;
using System;

namespace Server.Items
{
    public class GrapeBunch : Food
    {
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1022513; } }

        [Constructable]
        public GrapeBunch() : base(1, 3354)
        {
            Weight = 1.0;
            FillFactor = 1;
            Stackable = false;
        }

        public GrapeBunch(Serial serial)
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