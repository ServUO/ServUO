using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x4A93)]
    public class DevilMaskE : Item
    {
        [Constructable]
        public DevilMaskE()
            : base(0x4A93)
        {
            Name = "Devil Mask East";
            Weight = 20;
        }


        public DevilMaskE(Serial serial)
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