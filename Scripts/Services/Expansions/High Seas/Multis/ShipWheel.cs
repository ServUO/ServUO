using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Multis;
using System.Collections.Generic;
using Server.Commands;
using Server.Targeting;

namespace Server.Items
{
    public class ShipWheel : Item
    {
        public static void Initialize()
        {
            //CommandSystem.Register("FreePilot", AccessLevel.GameMaster, new CommandEventHandler(FreePilot_OnCommand));

            //EventSink.Disconnected += new DisconnectedEventHandler(EventSink_Disconnected);
            //EventSink.PlayerDeath += new PlayerDeathEventHandler(EventSink_PlayerDeath);
        }

        //private static readonly bool UseBoatMovementRequest = false;

        public override int LabelNumber { get { return 1150109; } }

        private BaseGalleon m_Galleon;

        public Mobile Pilot { get { return m_Galleon != null ? m_Galleon.Pilot : null; } }

        [Constructable]
        public ShipWheel(BaseGalleon galleon)
        {
            m_Galleon = galleon;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);
            Item mount = from.FindItemOnLayer(Layer.Mount);

            if (boat != null && Pilot == from)
                boat.RemovePilot(from);
            else if (!from.InRange(this.Location, 3))
                from.SendLocalizedMessage(500295); //You are too far away to do that.
            else if (boat == null && boat != m_Galleon)
                from.SendLocalizedMessage(1116724); //You cannot pilot a ship unless you are aboard it!
            else if (m_Galleon.GetSecurityLevel(from) < SecurityLevel.Crewman)
                from.SendLocalizedMessage(1116726); //This is not your ship!
            else if (Pilot != null && Pilot != from)
                from.SendMessage("Someone is already piloting this vessle!");
            else if (from.Flying)
                from.SendLocalizedMessage(1116615); // You cannot pilot a ship while flying!
            else if (from.Mounted && !(mount is BoatMountItem))
                from.SendLocalizedMessage(1010097); //You cannot use this while mounted or flying. 
            else if (Pilot == null && m_Galleon.Scuttled)
                from.SendLocalizedMessage(1116725); //This ship is too damaged to sail!
            else
                boat.LockPilot(from);
        }

        public static bool IsDriving(Mobile from)
        {
            return BaseBoat.IsDriving(from);
        }

        public ShipWheel(Serial serial) : base(serial) { }

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