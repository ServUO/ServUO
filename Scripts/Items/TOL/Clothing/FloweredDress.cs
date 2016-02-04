using System;

namespace Server.Items
{
    public class FloweredDress : BaseOuterTorso
    {
        [Constructable]
        public FloweredDress()
            : this(0)
        {
        }

        [Constructable]
        public FloweredDress(int hue)
            : base(0x781E, hue)
        {
            Weight = 3.0;
        }

        public FloweredDress(Serial serial)
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