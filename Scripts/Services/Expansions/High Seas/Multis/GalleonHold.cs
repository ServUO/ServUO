using Server;
using System;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

namespace Server.Items
{
    public class GalleonHold : Container
    {
        private BaseGalleon m_Galleon;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get { return m_Galleon; } }

        public override int DefaultMaxWeight
        {
            get
            {
                if (m_Galleon is BritannianShip)
                    return 28000;
                else if (m_Galleon is GargishGalleon)
                    return 12000;
                else if (m_Galleon is OrcishGalleon)
                    return 14000;
                else
                    return 16000;
            }
        }

        public override bool IsDecoContainer { get { return false; } }

        public override Rectangle2D Bounds { get { return new Rectangle2D(46, 74, 150, 110); } }
        public override int DefaultGumpID { get { return 0x4C; } }
        public override int DefaultDropSound { get { return 0x42; } }

        public GalleonHold(BaseGalleon galleon, int itemID) : base(itemID)
        {
            m_Galleon = galleon;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Galleon == null || from.AccessLevel > AccessLevel.Player)
                base.OnDoubleClick(from);
            else if (!m_Galleon.Contains(from))
            {
                if(m_Galleon.TillerMan != null)
                    m_Galleon.TillerManSay(502490); // You must be on the ship to open the hold.
            }
            else if (m_Galleon.Owner is PlayerMobile && !m_Galleon.Scuttled && m_Galleon.GetSecurityLevel(from) < SecurityLevel.Officer)
                from.SendMessage("You must be at least an officer to access the cargo hold.");
            else
                base.OnDoubleClick(from);
        }

        public GalleonHold(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Galleon = reader.ReadItem() as BaseGalleon;

            if (ItemID == 33648)
                ItemID = 23648;
        }
    }

    public class HoldItem : Item
    {
        public override int LabelNumber { get { return 1149699; } }
        public override bool ForceShowProperties { get { return true; } }

        private GalleonHold m_Hold;

        [CommandProperty(AccessLevel.GameMaster)]
        public GalleonHold Hold { get { return m_Hold; } }

        public HoldItem(GalleonHold hold, int itemid) : base(itemid)
        {
            m_Hold = hold;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Hold == null || m_Hold.Galleon == null || !from.InRange(this.Location, 2))
                return;

            m_Hold.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072241, String.Format("{0}\t{1}\t{2}\t{3}", Hold.TotalItems, Hold.MaxItems, Hold.TotalWeight, Hold.MaxWeight)); // Contents: ~1_COUNT~/~2_MAXCOUNT~ items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones
        }

        public HoldItem(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Hold);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Hold = reader.ReadItem() as GalleonHold;
        }
    }
}
