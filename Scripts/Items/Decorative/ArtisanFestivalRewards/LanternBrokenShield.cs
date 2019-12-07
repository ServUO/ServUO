using System;

namespace Server.Items
{
    public class LanternBrokenShield : BaseLight, IFlipable
    {
        public override int LabelNumber { get { return 1011221; } } // lantern

        public override int LitItemID { get { return ItemID == 0xA469 ? 0xA46A : 0xA46E; } }
        public override int UnlitItemID { get { return ItemID == 0xA46A ? 0xA469 : 0xA46D; } }

        public int NorthID { get { return Burning ? 0xA46A : 0xA469; } }
        public int WestID { get { return Burning ? 0xA46E : 0xA46D; } }

        [Constructable]
        public LanternBrokenShield()
            : base(0xA469)
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

        public LanternBrokenShield(Serial serial)
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
