using Server;
using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Multis;
using Server.Gumps;

namespace Server.Items
{
    public class ShipCrate : SmallCrate
    {
        public static readonly int DT = 30;

        private Mobile m_Owner;
        private BaseBoat m_Boat;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat Boat { get { return m_Boat; } }

        public override TimeSpan DecayTime
        {
            get
            {
                return TimeSpan.FromMinutes(DT);
            }
        }

        public override bool Decays
        {
            get
            {
                return true;
            }
        }

        public ShipCrate(Mobile owner, BaseBoat boat)
        {
            LiftOverride = true;
            m_Owner = owner;
            m_Boat = boat;
            Movable = false;
        }

        public override void Delete()
        {
            Server.Mobiles.DockMaster.RemoveCrate(this);
            base.Delete();
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_Owner != null)
                list.Add(1116515, m_Owner.Name);
            else
                list.Add("a shipping crate");
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            list.Add(new DestroyCrate(from, this));

            if(m_Boat != null && this.Items.Count > 0)
                list.Add(new LoadShip(from, this));
        }

        private class DestroyCrate : ContextMenuEntry
        {
            private Mobile m_From;
            private ShipCrate m_Crate;

            public DestroyCrate(Mobile from, ShipCrate crate) : base(1116522, 3)
            {
                m_From = from;
                m_Crate = crate;
            }

            public override void OnClick()
            {
                m_From.SendGump(new InternalGump(m_Crate)); 
            }
        }

        private class LoadShip : ContextMenuEntry
        {
            private Mobile m_From;
            private ShipCrate m_Crate;

            public LoadShip(Mobile from, ShipCrate crate)
                : base(1116521, 3) //Load Ship from Crate
            {
                m_From = from;
                m_Crate = crate;
            }

            public override void OnClick()
            {
                if (m_Crate == null || m_Crate.Boat == null)
                    return;

                Container hold;

                if(m_Crate.Boat is BaseGalleon)
                    hold = ((BaseGalleon)m_Crate.Boat).GalleonHold;
                else
                    hold = m_Crate.Boat.Hold;

                if(hold == null)
                    return;

                if (m_From.InRange(m_Crate.Boat.Location, Server.Mobiles.DockMaster.DryDockDistance))
                {
                    List<Item> items = new List<Item>(m_Crate.Items);
                    foreach (Item item in items)
                        hold.DropItem(item);

                    m_From.SendMessage("You hold has been loaded from the shipping crate.");
                }
                else
                    m_From.SendLocalizedMessage(1116519); //I can't find your ship! You need to bring it in closer.
            }
        }

        private class InternalGump : BaseConfirmGump
        {
            private ShipCrate m_Crate;

            public override int LabelNumber { get { return 1116523; } } // Are you sure you want to destroy your shipping crate and its contents?

            public InternalGump(ShipCrate crate)
            {
                m_Crate = crate;
            }

            public override void Confirm(Mobile from)
            {
                if(m_Crate != null)
                    m_Crate.Delete();
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from != m_Owner)
            {
                from.SendLocalizedMessage(1112589); //This does not belong to you! Find your own!
                return;
            }

            base.OnDoubleClick(from);
        }   

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (this.TotalItems == 0)
                Delete();
        }

        public ShipCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Owner);
            writer.Write(m_Boat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Owner = reader.ReadMobile();
            m_Boat = reader.ReadItem() as BaseBoat;
        }
    }
}