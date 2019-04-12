using System;

namespace Server.Items
{
    public class GoldBranch : Item
    {
        public override int LabelNumber { get { return 1158835; } } // branch

        [Constructable]
        public GoldBranch()
            : base(0x234) // YFF, LGF
        {
            Hue = 2721;
        }
        public GoldBranch(Serial serial)
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

    public class SilverBranch : Item
    {
        public override int LabelNumber { get { return 1158835; } } // branch

        [Constructable]
        public SilverBranch()
            : base(0x234) // YFF, LGF
        {
            Hue = 2500;
        }
        public SilverBranch(Serial serial)
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
