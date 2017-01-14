using System;
using Server;

namespace Server.Items
{
    public class Ramrod : Item
    {
        [Constructable]
        public Ramrod()
            : base(Utility.RandomMinMax(16966, 16967))
        {
        }

        public Ramrod(Serial serial) : base(serial) { }

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