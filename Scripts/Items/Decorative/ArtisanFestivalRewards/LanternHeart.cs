using System;

namespace Server.Items
{
    public class LanternHeart : BaseLight, IFlipable
    {
        public override int LabelNumber { get { return 1011221; } } // lantern

        public override int LitItemID { get { return ItemID == 0xA481 ? 0xA482 : 0xA486; } }
        public override int UnlitItemID { get { return ItemID == 0xA482 ? 0xA481 : 0xA485; } }

        public int NorthID { get { return Burning ? 0xA482 : 0xA481; } }
        public int WestID { get { return Burning ? 0xA486 : 0xA485; } }

        [Constructable]
        public LanternHeart()
            : base(0xA481)
        {
            Weight = 1;
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == NorthID)
                ItemID = WestID;
            else if (ItemID == WestID)
                ItemID = NorthID;
        }

        public LanternHeart(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
