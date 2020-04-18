namespace Server.Items
{
    public class LanternHeart : BaseLight, IFlipable
    {
        public override int LabelNumber => 1011221;  // lantern

        public override int LitItemID => ItemID == 0xA481 ? 0xA482 : 0xA486;
        public override int UnlitItemID => ItemID == 0xA482 ? 0xA481 : 0xA485;

        public int NorthID => Burning ? 0xA482 : 0xA481;
        public int WestID => Burning ? 0xA486 : 0xA485;

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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
