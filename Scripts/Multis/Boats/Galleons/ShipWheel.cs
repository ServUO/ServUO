using Server.Multis;

namespace Server.Items
{
    public class ShipWheel : Item, IGalleonFixture
    {
        public override int LabelNumber => 1149698;  // wheel

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get; set; }

        public Mobile Pilot => Galleon != null ? Galleon.Pilot : null;

        public ShipWheel(BaseGalleon galleon, int id)
            : base(id)
        {
            Galleon = galleon;
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

            if (boat == null || Galleon == null || boat != Galleon)
            {
                from.SendLocalizedMessage(1116724); // You cannot pilot a ship unless you are aboard it!
            }
            else if (Galleon.GetSecurityLevel(from) < SecurityLevel.Crewman)
            {
                from.SendLocalizedMessage(1116726); // This is not your ship!
            }
            else if (Pilot != null && Pilot != from && (Galleon.GetSecurityLevel(from) < Galleon.GetSecurityLevel(Pilot) || Pilot == Galleon.Owner))
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
            else if (Pilot == null && Galleon.Scuttled)
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
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                reader.ReadItem();
            }
        }
    }
}
