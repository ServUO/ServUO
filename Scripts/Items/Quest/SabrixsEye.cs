namespace Server.Items
{
    public class SabrixsEye : PeerlessKey
    {
        [Constructable]
        public SabrixsEye()
            : base(0xF87)
        {
            Weight = 1;
            Hue = 0x480;
            LootType = LootType.Blessed;
        }

        public SabrixsEye(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074336;// sabrix's eye
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
