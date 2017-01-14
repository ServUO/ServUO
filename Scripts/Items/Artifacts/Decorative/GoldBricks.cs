using System;

namespace Server.Items
{
    public class GoldBricks : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GoldBricks()
            : base(0x1BEB)
        {
        }

        public GoldBricks(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063489;
            }
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