using System;
using Server;
using Server.Multis;

namespace Server.Items
{
    public class ShipWheel : Item
    {
        public override int LabelNumber { get { return 1149698; } } // wheel

        private BaseGalleon m_Galleon;

        public Mobile Pilot { get { return m_Galleon != null ? m_Galleon.Pilot : null; } }

        [Constructable]
        public ShipWheel(BaseGalleon galleon)
        {
            m_Galleon = galleon;
            Movable = false;
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            OnDoubleClick(m);
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.RevealingAction();

            BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);
            Item mount = from.FindItemOnLayer(Layer.Mount);

            if (boat == null || m_Galleon == null || boat != m_Galleon)
            {
                from.SendLocalizedMessage(1116724); // You cannot pilot a ship unless you are aboard it!
            }
            else if (m_Galleon.GetSecurityLevel(from) < SecurityLevel.Crewman)
            {
                from.SendLocalizedMessage(1116726); // This is not your ship!
            }
            else if (Pilot != null && Pilot != from && (m_Galleon.GetSecurityLevel(from) < m_Galleon.GetSecurityLevel(Pilot) || Pilot == m_Galleon.Owner))
            {
                from.SendLocalizedMessage(502221); // Someone else is already using this item.
            }
            else if (from.Flying)
            {
                from.SendLocalizedMessage(1116615); // You cannot pilot a ship while flying!
            }
            else if (from.Mounted && !(mount is BoatMountItem))
            {
                from.SendLocalizedMessage(1010097); // You cannot use this while mounted or flying.
            }
            else if (Pilot == null && m_Galleon.Scuttled)
            {
                from.SendLocalizedMessage(1116725); // This ship is too damaged to sail!
            }
            else if (Pilot != null)
            {
                if (from != Pilot) // High authorized player takes control of the ship
                {
                    boat.RemovePilot(from);
                    boat.LockPilot(from);
                }
                else
                {
                    boat.RemovePilot(from);
                }
            }
            else
            {
                boat.LockPilot(from);
            }
        }

        public ShipWheel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Galleon = reader.ReadItem() as BaseGalleon;
        }
    }
}
