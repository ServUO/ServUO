using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x63B0, 0x6395)]
    public class YinYangWall : Item
    {
        [Constructable]
        public YinYangWall()
            : base(0x63B0)
        {
            Name = "Yin & Yang";
            Weight = 20;
        }


        public YinYangWall(Serial serial)
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