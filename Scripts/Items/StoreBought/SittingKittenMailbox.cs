namespace Server.Items
{
    [Furniture]
    public class SittingKittenMailbox : Mailbox
    {
        public override int LabelNumber => 1113927;  // Mailbox

        public override int DefaultGumpID => 0x9D37;

        public override int SouthMailBoxID => 0xA3EE;
        public override int SouthEmptyMailBoxID => 0xA3ED;
        public override int EastMailBoxID => 0xA3EC;
        public override int EastEmptyMailBoxID => 0xA3EB;

        [Constructable]
        public SittingKittenMailbox()
            : base(0xA3EB)
        {
        }

        public SittingKittenMailbox(Serial serial)
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
