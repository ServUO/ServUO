using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x4A8F)]
    public class DocMaskE : Item
    {
        [Constructable]
        public DocMaskE()
            : base(0x4A8F)
        {
            Name = "Docs Mask East";
            Weight = 20;
        }


        public DocMaskE(Serial serial)
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