using Server.Accounting;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public class MasterThinkerBooks : Item
    {
        public override int LabelNumber { get { return 1123597; } } // Book

        [Constructable]
        public MasterThinkerBooks() : base(0x42BF)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154222); // *You thumb through the pages of the book, it seems to describe the anatomy of a variety of frost creatures*            
        }

        public MasterThinkerBooks(Serial serial) : base( serial )
		{
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
