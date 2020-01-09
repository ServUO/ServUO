using System;

namespace Server.Items
{
    public class LanternUnevenScales : BaseLight, IFlipable
    {
        public override int LabelNumber { get { return 1011221; } } // lantern

        public override int LitItemID { get { return ItemID == 0xA479 ? 0xA47A : 0xA47E; } }
        public override int UnlitItemID { get { return ItemID == 0xA47A ? 0xA479 : 0xA47D; } }

        public int NorthID { get { return Burning ? 0xA47A : 0xA479; } }
        public int WestID { get { return Burning ? 0xA47E : 0xA47D; } }

        [Constructable]
        public LanternUnevenScales()
            : base(0xA479)
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

        public LanternUnevenScales(Serial serial)
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
