using System;

namespace Server.Items
{
    public class WaterTile : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public WaterTile()
            : base(0x346E)
        {
        }

        public WaterTile(Serial serial)
            : base(serial)
        {
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