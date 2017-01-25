using System;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x1fa1, 0x1fa2)]
    public class MasterThinkerTunic : BaseMiddleTorso
    {
        public override int LabelNumber { get { return 1011361; } } // Tunic

        [Constructable]
        public MasterThinkerTunic()
            : this(0)
        {
        }

        [Constructable]
        public MasterThinkerTunic(int hue)
            : base(0x1FA2, hue)
        {
            this.Hue = 398;
        }

        public MasterThinkerTunic(Serial serial)
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
