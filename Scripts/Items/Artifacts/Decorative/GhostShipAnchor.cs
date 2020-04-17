namespace Server.Items
{
    public class GhostShipAnchor : Item
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GhostShipAnchor()
            : base(0x14F7)
        {
            Hue = 0x47E;
            Weight = 2;
        }

        public GhostShipAnchor(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1070816;// Ghost Ship Anchor
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    Weight = 2;
                    break;
            }

            if (ItemID == 0x1F47)
                ItemID = 0x14F7;
        }
    }
}