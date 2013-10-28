using System;

namespace Server.Items
{
    public class StrippedFlakeFish : BaseFish
    { 
        [Constructable]
        public StrippedFlakeFish()
            : base(0x3B0A)
        {
        }

        public StrippedFlakeFish(Serial serial)
            : base(serial)
        { 
        }

        public override int LabelNumber
        {
            get
            {
                return 1074595;
            }
        }// Stripped Flake Fish
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