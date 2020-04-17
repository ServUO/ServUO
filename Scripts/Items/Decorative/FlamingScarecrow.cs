using System;

namespace Server.Items
{
    public class FlamingScarecrow : BaseLight, IFlipable
    {
        public override int LabelNumber => 1027732;  // scarecrow

        [Constructable]
        public FlamingScarecrow()
            : base(0x9F33)
        {
            Duration = TimeSpan.Zero;
            Burning = false;
        }

        public void OnFlip(Mobile from)
        {
            if (ItemID == 0x9F33 || ItemID == 0x9F34)
                ItemID = ItemID + 6;
            else
                ItemID = ItemID - 6;
        }

        public override int LitItemID => ItemID == 0x9F33 ? 0x9F34 : 0x9F3A;
        public override int UnlitItemID => ItemID == 0x9F34 ? 0x9F33 : 0x9F39;

        public FlamingScarecrow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
