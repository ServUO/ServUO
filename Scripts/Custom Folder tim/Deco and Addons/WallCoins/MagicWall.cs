using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x6383, 0x639E)]
    public class MagicWall : Item
    {
        [Constructable]
        public MagicWall()
            : base(0x6383)
        {
            Name = "Magician";
            Weight = 20;
        }


        public MagicWall(Serial serial)
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