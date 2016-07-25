using System;
using Server;

namespace Server.Items
{
    public class FloorTrapComponents : Item
    {
        public override int LabelNumber { get { return 1095001; } } // Floor Trap Components

        [Constructable]
        public FloorTrapComponents()
            : base(0xC2F)
        {
            Weight = 1.0;
            Stackable = false;
        }

        public FloorTrapComponents(Serial serial)
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