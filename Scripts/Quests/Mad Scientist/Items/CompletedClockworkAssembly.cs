using System;
using Server;

namespace Server.Items
{
    public class CompletedClockworkAssembly : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1112879; } } // completed clockwork assembly
        public override int Lifespan { get { return 600; } }

        [Constructable]
        public CompletedClockworkAssembly()
            : base(0x1EAE)
        {
            Weight = 1.0;
        }

        public CompletedClockworkAssembly(Serial serial)
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
