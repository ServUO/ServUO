using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x6380, 0x639B)]
    public class DragonKnightWall : Item
    {
        [Constructable]
        public DragonKnightWall()
            : base(0x6380)
        {
            Name = "Dragon Knight";
            Weight = 20;
        }


        public DragonKnightWall(Serial serial)
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