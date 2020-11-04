namespace Server.Items
{
    [Furniture]
    public class LionMailbox : Mailbox
    {
        public override int LabelNumber => 1113927;  // Mailbox

        public override int DefaultGumpID => 0x9D3A;

        public override int SouthMailBoxID => 0xA3FA;
        public override int SouthEmptyMailBoxID => 0xA3F9;
        public override int EastMailBoxID => 0xA3F8;
        public override int EastEmptyMailBoxID => 0xA3F7;

        [Constructable]
        public LionMailbox()
            : base(0xA3F7)
        {
        }

        public LionMailbox(Serial serial)
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
            reader.ReadInt();
        }
    }
}
