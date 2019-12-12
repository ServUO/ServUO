using System;
using Server;

namespace Server.Items
{
    public class Ramrod : Item
    {
        public override int LabelNumber { get { return 1095838; } } // ramrod

        [Constructable]
        public Ramrod()
            : base(0x4246)
        {
        }

        public Ramrod(Serial serial)
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
