using System;
using Server;

namespace Server.Items
{
    public class RedHerring : BigFish
    {
        public override int LabelNumber { get { return 1095046; } } // Britain Crown Fish

        [Constructable]
        public RedHerring()
        {
            Hue = 337;
        }

        public RedHerring(Serial serial)
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

            int version = (InheritsItem ? 0 : reader.ReadInt()); // Required for BigFish insertion
        }
    }
}