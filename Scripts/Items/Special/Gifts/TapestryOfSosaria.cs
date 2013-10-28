using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    [FlipableAttribute(0x234E, 0x234F)]
    public class TapestryOfSosaria : Item, ISecurable
    {
        private SecureLevel m_Level;
        [Constructable]
        public TapestryOfSosaria()
            : base(0x234E)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public TapestryOfSosaria(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1062917;
            }
        }// The Tapestry of Sosaria
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 2))
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

            writer.WriteEncodedInt((int)0); // version

            writer.WriteEncodedInt((int)this.m_Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Level = (SecureLevel)reader.ReadEncodedInt();
        }

        private class InternalGump : Gump
        {
            public InternalGump()
                : base(50, 50)
            {
                this.AddImage(0, 0, 0x2C95);
            }
        }
    }
}