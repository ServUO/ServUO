namespace Server.Items
{
    [Furniture]
    public class DolphinMailbox : Mailbox
    {
        public override int LabelNumber => 1113927;  // Mailbox
        public override int DefaultGumpID => 0x6D3;	
        public override int SouthMailBoxID => 0xA204;
        public override int SouthEmptyMailBoxID => 0xA205;
        public override int EastMailBoxID => 0xA202;	
        public override int EastEmptyMailBoxID => 0xA203;

        [Constructable]
        public DolphinMailbox()
            : base(0xA203)
        {
        }

        public DolphinMailbox(Serial serial)
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
