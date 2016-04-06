using System;

namespace Server.Items
{
    public class WallBlood : Item
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public WallBlood()
            : base(Utility.RandomBool() ? 0x1D95 : 0x1D94)
        {
        }

        public WallBlood(Serial serial)
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