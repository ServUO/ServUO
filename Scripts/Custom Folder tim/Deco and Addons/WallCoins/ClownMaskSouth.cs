using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x4A90)]
    public class CLownMaskS : Item
    {
        [Constructable]
        public CLownMaskS()
            : base(0x4A90)
        {
            Name = "Clown Mask South";
            Weight = 20;
        }


        public CLownMaskS(Serial serial)
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