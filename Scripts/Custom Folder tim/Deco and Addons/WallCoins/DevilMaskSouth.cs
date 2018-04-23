using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x4A92)]
    public class DevilMaskS : Item
    {
        [Constructable]
        public DevilMaskS()
            : base(0x4A92)
        {
            Name = "Devil Mask South";
            Weight = 20;
        }


        public DevilMaskS(Serial serial)
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