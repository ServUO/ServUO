using Server.Accounting;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public class MasterThinkerGarment : LongPants
    {
        public override int LabelNumber { get { return 1015280; } } // long pants

        [Constructable]
        public MasterThinkerGarment()
            : this(0)
        {
        }

        [Constructable]
        public MasterThinkerGarment(int hue)
            : base(hue)
        {
            this.Hue = 2017;
            this.Movable = false;
        }

        public MasterThinkerGarment(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1154221); // *You carefully examine the garment and take note of it's superior quality. You surmise it would be useful in keeping you warm in a cold environment*           
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
