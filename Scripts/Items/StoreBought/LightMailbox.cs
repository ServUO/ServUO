namespace Server.Items
{
    [Furniture]
    public class LightMailbox : Mailbox
    {
        public override int LabelNumber => 1113927;  // Mailbox

        public override int DefaultGumpID => 0x9D37;

        public override int SouthMailBoxID => 0xA26D;
        public override int SouthEmptyMailBoxID => 0xA26C;
        public override int EastMailBoxID => 0xA269;
        public override int EastEmptyMailBoxID => 0xA268;

        [Constructable]
        public LightMailbox()
            : base(0xA268)
        {
        }

        public LightMailbox(Serial serial)
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
