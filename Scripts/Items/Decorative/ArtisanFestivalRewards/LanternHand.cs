namespace Server.Items
{
    public class LanternHand : BaseLight, IFlipable
    {
        public override int LabelNumber => 1011221;  // lantern

        public override int LitItemID => ItemID == 0xA471 ? 0xA472 : 0xA476;
        public override int UnlitItemID => ItemID == 0xA472 ? 0xA471 : 0xA475;

        public int NorthID => Burning ? 0xA472 : 0xA471;
        public int WestID => Burning ? 0xA476 : 0xA475;

        [Constructable]
        public LanternHand()
            : base(0xA471)
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

        public LanternHand(Serial serial)
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
