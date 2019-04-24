using System;

namespace Server.Items
{
    [Furniture]
    public class SquirrelMailbox : Mailbox, IFlipable
    {
        public override int LabelNumber { get { return 1113927; } } // Mailbox

        public override int DefaultGumpID { get { return 0x6D4; } }

        public override int SouthMailBoxID { get { return 0xA208; } }
        public override int SouthEmptyMailBoxID { get { return 0xA209; } }
        public override int EastMailBoxID { get { return 0xA206; } }
        public override int EastEmptyMailBoxID { get { return 0xA207; } }

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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
