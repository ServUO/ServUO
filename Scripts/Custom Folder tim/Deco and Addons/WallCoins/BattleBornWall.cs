using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x6381, 0x639C)]
    public class BattleBornWall : Item
    {
        [Constructable]
        public BattleBornWall()
            : base(0x6381)
        {
            Name = "Battle Born";
            Weight = 20;
        }


        public BattleBornWall(Serial serial)
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