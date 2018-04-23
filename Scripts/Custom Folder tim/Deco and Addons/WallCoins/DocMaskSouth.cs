using System;
using Server.Network;

namespace Server.Items
{

    [FlipableAttribute(0x4A8E)]
    public class DocMaskS : Item
    {
        [Constructable]
        public DocMaskS()
            : base(0x4A8E)
        {
            Name = "Docs Mask South";
            Weight = 20;
        }


        public DocMaskS(Serial serial)
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