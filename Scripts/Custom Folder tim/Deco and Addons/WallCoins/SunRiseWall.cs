using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x6382, 0x639D)]
    public class SunRiseWall : Item
    {
        [Constructable]
        public SunRiseWall()
            : base(0x6382)
        {
            Name = "Sun Rise";
            Weight = 20;
        }


        public SunRiseWall(Serial serial)
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