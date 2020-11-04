namespace Server.Items
{
    [Furniture]
    public class SquirrelMailbox : Mailbox
    {
        public override int LabelNumber => 1113927;  // Mailbox

        public override int DefaultGumpID => 0x6D4;

        public override int SouthMailBoxID => 0xA208;
        public override int SouthEmptyMailBoxID => 0xA209;
        public override int EastMailBoxID => 0xA206;
        public override int EastEmptyMailBoxID => 0xA207;

        [Constructable]
        public SquirrelMailbox()
            : base(0xA207)
        {
        }

        public SquirrelMailbox(Serial serial)
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
