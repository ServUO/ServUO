using Server;
using System;

namespace Server.Items
{
    public class AntiqueWeddingDress : PlainDress
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1149958; } }

        [Constructable]
        public AntiqueWeddingDress()
        {
            Hue = 2953;
        }

        public AntiqueWeddingDress(Serial serial)
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