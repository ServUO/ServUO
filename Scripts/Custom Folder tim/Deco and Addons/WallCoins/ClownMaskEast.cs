using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x4A91)]
    public class CLownMaskE : Item
    {
        [Constructable]
        public CLownMaskE()
            : base(0x4A91)
        {
            Name = "Clown Mask East";
            Weight = 20;
        }


        public CLownMaskE(Serial serial)
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