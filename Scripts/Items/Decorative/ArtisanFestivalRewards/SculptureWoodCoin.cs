namespace Server.Items
{
    public class SculptureWoodCoin : BaseLight
    {
        public override int LabelNumber => 1029241;  // sculpture

        public override int LitItemID => 0xA4A4;
        public override int UnlitItemID => 0xA493;

        public override int LitSound => 480;
        public override int UnlitSound => 482;

        [Constructable]
        public SculptureWoodCoin()
            : base(0xA493)
        {
            Weight = 1;
        }

        public SculptureWoodCoin(Serial serial)
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
