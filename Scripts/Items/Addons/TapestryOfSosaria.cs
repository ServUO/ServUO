using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x234E, 0x234F)]
    public class TapestryOfSosaria : Item, ISecurable
    {
        private SecureLevel m_Level;
        [Constructable]
        public TapestryOfSosaria()
            : base(0x234E)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public TapestryOfSosaria(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1062917;// The Tapestry of Sosaria
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump());
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt((int)m_Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Level = (SecureLevel)reader.ReadEncodedInt();
        }

        private class InternalGump : Gump
        {
            public InternalGump()
                : base(50, 50)
            {
                AddImage(0, 0, 0x2C95);
            }
        }
    }
}