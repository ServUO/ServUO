using Server;
using System;

namespace Server.Items
{
    public class CreepingVine : Item
    {
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1112401; } }

        [Constructable]
        public CreepingVine()
            : base(Utility.Random(18322, 4))
        {
        }

        public CreepingVine(Serial serial)
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