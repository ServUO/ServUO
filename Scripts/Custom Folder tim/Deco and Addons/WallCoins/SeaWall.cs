using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x6384, 0x639F)]
    public class SeaWall : Item
    {
        [Constructable]
        public SeaWall()
            : base(0x6384)
        {
            Name = "High Tide";
            Weight = 20;
        }


        public SeaWall(Serial serial)
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