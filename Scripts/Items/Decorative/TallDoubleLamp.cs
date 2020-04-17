using System;

namespace Server.Items
{
    public class TallDoubleLamp : BaseLight, IFlipable
    {
        public override int LitItemID => ItemID == 0x4C56 ? 0x4C57 : 0x4C59;
        public override int UnlitItemID => ItemID == 0x4C57 ? 0x4C56 : 0x4C58;

        public int NorthID => Burning ? 0x4C57 : 0x4C56;
        public int WestID => Burning ? 0x4C59 : 0x4C58;

        [Constructable]
        public TallDoubleLamp()
            : base(0x4C56)
        {
            Duration = Burnout ? TimeSpan.FromMinutes(60) : TimeSpan.Zero;
            Burning = false;
            Light = LightType.Circle225;
            Weight = 1.0;
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == NorthID)
                ItemID = WestID;
            else if (ItemID == WestID)
                ItemID = NorthID;
        }

        public TallDoubleLamp(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}