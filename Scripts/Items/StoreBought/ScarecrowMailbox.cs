namespace Server.Items
{
    [Furniture]
    public class ScarecrowMailbox : Mailbox
    {
        public override int LabelNumber => 1113927;  // Mailbox
        public override int DefaultGumpID => 0x9D39;
        public override int SouthMailBoxID => 0xA3F5;
        public override int SouthEmptyMailBoxID => 0xA3F6;	
        public override int EastMailBoxID => 0xA3F3;	
        public override int EastEmptyMailBoxID => 0xA3F4;

        [Constructable]
        public ScarecrowMailbox()
            : base(0xA3F4)
        {
        }

        public ScarecrowMailbox(Serial serial)
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
